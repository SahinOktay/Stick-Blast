using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Garawell
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private InventoryController inventoryController;
        [SerializeField] private UIController uiController;
        [SerializeField] private PoolManager poolManager;

        private int _comboCount = 0, _score = 0;
        private PlacableBlock _currentPlacable;

        private void Start()
        {
            gridManager.GenerateGrid();
            inventoryController.PlacableBlockChosen += OnPlacableChosen;
            poolManager.Initialize();
            inventoryController.Initialize(poolManager);
        }

        private void OnTouchMove(Vector3 pos)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 5);
            _currentPlacable.transform.position = Vector3.Lerp(
                _currentPlacable.transform.position,
                new Vector3(newPos.x, newPos.y, 0),
                .5f
            );
            gridManager.CheckPlacement(_currentPlacable, true, out Vector3 offset);
        }

        private void OnFillFeedbackComplete(FillFeedback fillFeedback)
        {
            fillFeedback.FeedbackComplete -= OnFillFeedbackComplete;
            poolManager.Recycle(fillFeedback);
        }

        private void OnTouchRelease() 
        {
            inputManager.TouchMoved -= OnTouchMove;
            inputManager.TouchRelease -= OnTouchRelease;
            if (!gridManager.PlaceBlock(_currentPlacable, out int fillCount))
            {
                _currentPlacable.GetReleased();
                return;
            }

            if (fillCount > 0) _comboCount += fillCount;
            else _comboCount = 0;

            int scoreGain = Constants.Numbers.PLACEMENT_BONUS + Constants.Numbers.FILL_BONUS * _comboCount;
            _score += scoreGain;

            uiController.SetScore(_score);

            PlacableBlock[] unplacedBlocks = inventoryController.GetUnplacedBlocks();

            bool hasAvailableSpot = false;
            for (int i = 0; i < unplacedBlocks.Length; i++)
            {
                if (gridManager.HasAnyAvailableSpot(unplacedBlocks[i]))
                {
                    hasAvailableSpot = true;
                    break;
                }
            }

            if (!hasAvailableSpot)
            {
                uiController.ShowEndGamePanel(_score);
                uiController.RetryClick += OnRetryButtonClick;
            }

            if (_comboCount > 0)
            {
                FillFeedback fillFeedback = poolManager.GetFillFeedback();
                fillFeedback.gameObject.SetActive(true);
                fillFeedback.FeedbackComplete += OnFillFeedbackComplete;
                fillFeedback.PlayScoreFeedback(scoreGain);
                fillFeedback.transform.position = new Vector3(
                    _currentPlacable.transform.position.x, 
                    _currentPlacable.transform.position.y, 
                    0
                );

                if (_comboCount > 1) uiController.PlayComboFeedbacks(_comboCount);
            }
        }

        private void OnPlacableChosen(PlacableBlock placableBlock)
        {
            _currentPlacable = placableBlock;
            inputManager.TouchMoved += OnTouchMove;
            inputManager.TouchRelease += OnTouchRelease;
        }

        private void OnRetryButtonClick()
        {
            uiController.RetryClick -= OnRetryButtonClick;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}