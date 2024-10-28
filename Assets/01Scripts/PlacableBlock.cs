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

    public class PlacableBlock : MonoBehaviour
    {
        public ConnectionPoints[] nodePoints;
        
    }
}