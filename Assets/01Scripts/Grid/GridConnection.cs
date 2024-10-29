using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridConnection : MonoBehaviour
{
    [SerializeField] private GameObject notConnected, preview, connected;

    private bool _isConnected = false;

    public Action<GridConnection> Connected;

    public bool IsConnected => _isConnected;

    public void SetPreview(bool enabled)
    {
        if (_isConnected) return;

        notConnected.SetActive(!enabled);
        preview.SetActive(enabled);
    }

    public void Connect()
    {
        _isConnected = true;
        preview.SetActive(false);
    }

    public void EnableConnectionVisuals()
    {
        connected.transform.DOKill();
        connected.transform.localScale = Vector3.one;
        connected.transform.localPosition = Vector3.zero;
        connected.transform.localEulerAngles = Vector3.zero;

        notConnected.SetActive(false);
        connected.SetActive(true);
        Connected?.Invoke(this);
    }

    public void Reset(Vector3 direction)
    {
        _isConnected = false;
        preview.SetActive(false);
        notConnected.SetActive(true);

        float throwAnimDuraiton = 1f;
        connected.transform.DOMove(
            connected.transform.position + direction * 2f,
            throwAnimDuraiton
        ).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            connected.SetActive(false);
        });
        connected.transform.DOScale(Vector3.zero, throwAnimDuraiton);
        connected.transform.DORotate(
            (UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1) *
            Vector3.forward * 
            UnityEngine.Random.Range(720f, 1080f), 
            throwAnimDuraiton
        );
    }
}
