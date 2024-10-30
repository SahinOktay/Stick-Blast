using Garawell;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

namespace Garawell
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private ButtonEventTrigger[] buttons;

        private Dictionary<ButtonEventTrigger, PlacableBlock> _pairs;
        private int _unplacedBlocks;
        private PoolManager _poolManager;

        public Action<PlacableBlock> PlacableBlockChosen;

        public void Initialize(PoolManager poolManager)
        {
            _poolManager = poolManager;
            StartCoroutine(InitNextFrame());
        }

        private void GenerateBlocks()
        {
            _pairs = new Dictionary<ButtonEventTrigger, PlacableBlock>();

            _unplacedBlocks = buttons.Length;
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].enabled = true;
                PlacableBlock nextBlock = _poolManager.GetRandomBlock();

                nextBlock.gameObject.SetActive(true);
                nextBlock.mButton = buttons[i];
                nextBlock.transform.position = buttons[i].transform.position;
                nextBlock.transform.localScale = Vector3.zero;
                nextBlock.transform.DOScale(Vector3.one * .7f, .25f).SetEase(Ease.OutCubic);

                _pairs.Add(buttons[i], nextBlock);
                buttons[i].Click += OnButtonClick;
            }
        }

        private IEnumerator InitNextFrame()
        {
            yield return null;

            GenerateBlocks();
        }

        private void OnBlockReleased(ButtonEventTrigger button, PlacableBlock block)
        {
            _pairs[button].PlacedLogicly -= OnBlockPlacedLogicly;
            _pairs[button].Released -= OnBlockReleased;

            _pairs[button].transform.DOMove(button.transform.position, .2f).SetEase(Ease.OutCubic);
            _pairs[button].transform.DOScale(Vector3.one * .7f, .3f).SetEase(Ease.InOutCubic);
        }

        private void OnBlockPlacedLogicly(ButtonEventTrigger button, PlacableBlock block)
        {
            button.Click -= OnButtonClick;
            _pairs[button].PlacedLogicly -= OnBlockPlacedLogicly;
            _pairs[button].Released -= OnBlockReleased;
            _pairs[button].Placed += OnBlockPlacedVisually;
            _pairs[button] = null;
            if (--_unplacedBlocks == 0) GenerateBlocks();
        }

        private void OnBlockPlacedVisually(ButtonEventTrigger button, PlacableBlock block)
        {
            block.Placed -= OnBlockPlacedVisually;
            _poolManager.Recycle(block);
        }

        private void OnButtonClick(ButtonEventTrigger button)
        {
            _pairs[button].transform.DOScale(Vector3.one, .2f).SetEase(Ease.OutCubic);
            _pairs[button].PlacedLogicly += OnBlockPlacedLogicly;
            _pairs[button].Released += OnBlockReleased;
            PlacableBlockChosen?.Invoke(_pairs[button]);
        }

        public PlacableBlock[] GetUnplacedBlocks() => _pairs.Values.Where(item => item != null).ToArray();
    }
}