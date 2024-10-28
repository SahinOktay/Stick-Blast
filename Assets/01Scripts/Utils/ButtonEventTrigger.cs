using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEventTrigger : MonoBehaviour, IPointerDownHandler
{
    public Action<ButtonEventTrigger> Click;

    public void OnPointerDown(PointerEventData eventData)
    {
        Click?.Invoke(this);
    }
}
