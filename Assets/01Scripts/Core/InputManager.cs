using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Action TouchRelease;
    public Action<Vector3> TouchMoved;

    private void Update()
    {
        if (Input.GetMouseButton(0)) TouchMoved?.Invoke(Input.mousePosition);
        if (Input.GetMouseButtonUp(0)) TouchRelease?.Invoke();
    }
}
