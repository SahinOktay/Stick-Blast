using Garawell;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Garawell
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private ButtonEventTrigger[] buttons;
        [SerializeField] private GameObject[] blockPrefabs;

        private Dictionary<ButtonEventTrigger, PlacableBlock> _pairs;
        private int _unplacedBlocks;

        public Action<PlacableBlock> PlacableBlockChosen;

        public void Initialize()
        {
            StartCoroutine(InitNextFrame());
        }

        private void GenerateBlocks()
        {
            _pairs = new Dictionary<ButtonEventTrigger, PlacableBlock>();

            _unplacedBlocks = buttons.Length;
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].enabled = true;
                PlacableBlock nextBlock = Instantiate(
                    blockPrefabs[UnityEngine.Random.Range(0, blockPrefabs.Length)]
                ).GetComponent<PlacableBlock>();

                nextBlock.mButton = buttons[i];
                nextBlock.transform.position = buttons[i].transform.position;
                nextBlock.transform.localScale = Vector3.one * .8f;
                _pairs.Add(buttons[i], nextBlock);
                buttons[i].Click += OnButtonClick;
            }
        }

        private IEnumerator InitNextFrame()
        {
            yield return null;

            GenerateBlocks();
        }

        private void OnBlockReleased(ButtonEventTrigger button)
        {
            _pairs[button].Placed -= OnBlockPlaced;
            _pairs[button].Released -= OnBlockReleased;

            _pairs[button].transform.DOMove(button.transform.position, .2f).SetEase(Ease.OutCubic);
            _pairs[button].transform.DOScale(Vector3.one * .8f, .3f).SetEase(Ease.InOutCubic);
        }

        private void OnBlockPlaced(ButtonEventTrigger button)
        {
            button.Click -= OnButtonClick;
            _pairs[button].Placed -= OnBlockPlaced;
            _pairs[button].Released -= OnBlockReleased;
            _pairs[button] = null;
            if (--_unplacedBlocks == 0) GenerateBlocks();
        }

        private void OnButtonClick(ButtonEventTrigger button)
        {
            _pairs[button].transform.DOScale(Vector3.one, .2f).SetEase(Ease.OutCubic);
            _pairs[button].Placed += OnBlockPlaced;
            _pairs[button].Released += OnBlockReleased;
            PlacableBlockChosen?.Invoke(_pairs[button]);
        }
    }
}