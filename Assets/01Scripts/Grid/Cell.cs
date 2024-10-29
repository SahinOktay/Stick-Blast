using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Garawell
{
    public class Cell
    {
        private GameObject _cellCenter;
        private int _connectedCount = 0;
        private List<GridConnection> _connections;
        private Vector2Int _pos;

        public Action<Vector2Int> CellFilled;

        public bool IsFilled => _connectedCount == _connections.Count;

        public Cell(List<GridConnection> connections, GameObject center, Vector2Int pos)
        {
            _cellCenter = center;
            _connections = connections;
            _pos = pos;

            _cellCenter.SetActive(false);
            for (int i = 0; i < _connections.Count; i++) 
            {
                _connections[i].Connected += OnConnected;
            }
        }

        private void OnConnected(GridConnection connection)
        {
            _connectedCount++;
            connection.Connected -= OnConnected;

            if (_connectedCount == _connections.Count)
            {
                _cellCenter.SetActive(true);
                _cellCenter.transform.localScale = Vector2.zero;
                _cellCenter.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    CellFilled?.Invoke(_pos);
                });
            }
        }

        public void ClearCell()
        {
            if (_connectedCount == 0) return;

            Vector3 centerPos = _cellCenter.transform.position;
            _connectedCount = 0;
            _cellCenter.SetActive(false);
            for (int i = 0; i < _connections.Count; i++)
            {
                _connections[i].Connected += OnConnected;
                _connections[i].Reset(Vector3.Normalize(_connections[i].transform.position - centerPos));
            }
        }

        public void CheckExistingConnections()
        {
            bool isFilledBefore = IsFilled;

            _connectedCount = 0;
            for (int i = 0; i < _connections.Count; i++) 
            {
                if (_connections[i].IsConnected)
                    _connectedCount++;
                else
                {
                    _connections[i].Connected -= OnConnected;
                    _connections[i].Connected += OnConnected;
                }
                    
            }

            if (isFilledBefore && !IsFilled)
            {
                _cellCenter.SetActive(false);
            }
        }
    }
}