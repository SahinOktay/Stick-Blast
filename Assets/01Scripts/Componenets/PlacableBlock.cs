using DG.Tweening;
using PoolSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Garawell
{
    [Serializable] 
    public class ConnectionPoints
    {
        public Transform point1, point2;
    }

    public class PlacableBlock : MonoBehaviour, IPoolable
    {
        public ConnectionPoints[] nodePoints;
        public SingleConnection[] connections;

        [NonSerialized] public ButtonEventTrigger mButton;

        public Action<ButtonEventTrigger, PlacableBlock> Released, Placed, PlacedLogicly;

        public void Place(HashSet<GridConnection> connections, Vector3 offset)
        {
            PlacedLogicly?.Invoke(mButton, this);
            transform.DOMove(transform.position - offset, .15f).OnComplete(() =>
            {
                foreach (GridConnection con in connections)
                {
                    con.EnableConnectionVisuals();
                }
                Placed?.Invoke(mButton, this);
            });
        }

        public void GetReleased()
        {
            Released?.Invoke(mButton, this);
        }

        public void Reset()
        {
            
        }
    }
}