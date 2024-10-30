using Garawell;
using PoolSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Garawell
{
    public class PoolManager : MonoBehaviour
    {
        [SerializeField] private GameObject fillFeedback;
        [SerializeField] private GameObject[] blocks;

        private Dictionary<string, Pool<PlacableBlock>> _blockPools;
        private Pool<FillFeedback> _fillFeedbackPool;

        public void Initialize()
        {
            _blockPools = new();

            for (int i = 0; i < blocks.Length; i++)
                _blockPools.Add(
                    blocks[i].name,
                    new Pool<PlacableBlock>(new PrefabFactory<PlacableBlock>(blocks[i]), 3)
                );

            _fillFeedbackPool = new Pool<FillFeedback>(new PrefabFactory<FillFeedback>(fillFeedback), 5);
        }

        public PlacableBlock GetRandomBlock() => _blockPools[
            blocks[UnityEngine.Random.Range(0, blocks.Length)].name
        ].GetItem();

        public FillFeedback GetFillFeedback() => _fillFeedbackPool.GetItem();

        public void Recycle(PlacableBlock block) =>
            _blockPools[block.gameObject.name].Recycle(block);

        public void Recycle(FillFeedback fillFeedback) =>
            _fillFeedbackPool.Recycle(fillFeedback);
    }
}