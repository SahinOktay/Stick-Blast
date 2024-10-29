using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;

namespace Garawell
{
    [Serializable] 
    public class ConnectionPoints
    {
        public Transform point1, point2;
    }

    public class PlacableBlock : MonoBehaviour
    {
        public ConnectionPoints[] nodePoints;

        [NonSerialized] public ButtonEventTrigger mButton;

        public Action<ButtonEventTrigger> Released, Placed;

        public void Place(HashSet<GridConnection> connections, Vector3 offset)
        {
            transform.DOMove(transform.position - offset, .15f).OnComplete(() =>
            {
                foreach (GridConnection con in connections)
                {
                    con.EnableConnectionVisuals();
                }
                gameObject.SetActive(false);
                Placed?.Invoke(mButton);
            });
        }

        public void GetReleased()
        {
            Released?.Invoke(mButton);
        }
        
    }
}