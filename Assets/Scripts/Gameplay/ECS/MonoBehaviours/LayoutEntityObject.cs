using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using System;
using UnityEngine.UI;

public class LayoutEntityObject : MonoBehaviour {

    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private CanvasScaler CanvasScaler;

    private float3 _position;
    private float2 _rectBounds;

    public float3 Position { get { return _position; } }
    public float2 RectBounds { get { return _rectBounds; } }

    private void Awake() {
        OnLayoutUpdate();
    }

    private void OnLayoutUpdate() {
        _position = RectTransform.position;
        _rectBounds = new float2(RectTransform.rect.width * RectTransform.lossyScale.x, RectTransform.rect.height * RectTransform.lossyScale.y);
    }

    private void Update() {
        if (!Compare(_position, RectTransform.position))
            OnLayoutUpdate();
    }

    private static bool Compare(float3 a, float3 b) {
        return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
    }
}
