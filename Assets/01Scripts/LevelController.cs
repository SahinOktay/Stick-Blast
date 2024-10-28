using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Garawell
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private InventoryController inventoryController;

        private bool _hasPlacable = false;
        private PlacableBlock _currentPlacable;

        private void Start()
        {
            gridManager.GenerateGrid();
            inventoryController.PlacableBlockChosen += OnPlacableChosen;
            inventoryController.Initialize();
        }

        private void Update()
        {
            if (!_hasPlacable) return;

            _currentPlacable.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 5);
        }

        private void OnPlacableChosen(PlacableBlock placableBlock)
        {
            _hasPlacable = true;
            _currentPlacable = placableBlock;
        }
    }
}