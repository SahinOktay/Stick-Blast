using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Garawell
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private InventoryController inventoryController;

        private PlacableBlock _currentPlacable;

        private void Start()
        {
            gridManager.GenerateGrid();
            inventoryController.PlacableBlockChosen += OnPlacableChosen;
            inventoryController.Initialize();
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

        private void OnTouchRelease() 
        {
            inputManager.TouchMoved -= OnTouchMove;
            inputManager.TouchRelease -= OnTouchRelease;
            if (!gridManager.PlaceBlock(_currentPlacable))
                _currentPlacable.GetReleased();
        }

        private void OnPlacableChosen(PlacableBlock placableBlock)
        {
            _currentPlacable = placableBlock;
            inputManager.TouchMoved += OnTouchMove;
            inputManager.TouchRelease += OnTouchRelease;
        }
    }
}