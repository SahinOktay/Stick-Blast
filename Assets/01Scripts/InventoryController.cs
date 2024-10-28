using Garawell;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Garawell
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private ButtonEventTrigger[] buttons;
        [SerializeField] private GameObject[] blockPrefabs;

        private Dictionary<ButtonEventTrigger, PlacableBlock> pairs;

        public Action<PlacableBlock> PlacableBlockChosen;

        public void Initialize()
        {
            StartCoroutine(GenerateBlocks());
        }

        private IEnumerator GenerateBlocks()
        {
            yield return null;

            pairs = new Dictionary<ButtonEventTrigger, PlacableBlock>();

            for (int i = 0; i < buttons.Length; i++)
            {
                PlacableBlock nextBlock = Instantiate(
                    blockPrefabs[UnityEngine.Random.Range(0, blockPrefabs.Length)]
                ).GetComponent<PlacableBlock>();

                nextBlock.transform.position = buttons[i].transform.position;
                pairs.Add(buttons[i], nextBlock);
                buttons[i].Click += OnButtonClick;
            }
        }

        private void OnButtonClick(ButtonEventTrigger button)
        {
            PlacableBlockChosen?.Invoke(pairs[button]);
        }
    }
}