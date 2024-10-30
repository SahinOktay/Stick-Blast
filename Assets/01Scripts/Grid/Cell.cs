using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Garawell
{
    public class Cell
    {
        private bool _isFilled = false;
        private GameObject _cellCenter;
        private List<GridConnection> _connections;

        public bool IsFilled => !_connections.Any(item => !item.IsConnected);

        public Cell(List<GridConnection> connections, GameObject center)
        {
            _cellCenter = center;
            _connections = connections;

            _cellCenter.SetActive(false);
        }

        public void ClearCell()
        {
            Vector3 centerPos = _cellCenter.transform.position;
            _cellCenter.SetActive(false);
            _isFilled = false;
            for (int i = 0; i < _connections.Count; i++)
            {
                _connections[i].Reset(Vector3.Normalize(_connections[i].transform.position - centerPos));
            }
        }

        public bool CheckFillStatus()
        {
            bool isFilledBefore = _isFilled;
            _isFilled = IsFilled;

            if (isFilledBefore && !_isFilled)
            {
                _cellCenter.SetActive(false);
                return false;
            }

            if (!isFilledBefore && IsFilled)
            {
                _cellCenter.SetActive(true);
                _cellCenter.transform.localScale = Vector2.zero;
                _cellCenter.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutCubic);

                return true;
            }

            return false;
        }
    }
}