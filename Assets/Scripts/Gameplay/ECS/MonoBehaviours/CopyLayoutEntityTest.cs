using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using System;
using UnityEngine.UI;

public class CopyLayoutEntityTest : MonoBehaviour {

    [SerializeField] private LayoutEntityObject LayoutEntity;

    private void Update() {

        var p = LayoutEntity.Position;
        var rb = LayoutEntity.RectBounds;
        transform.position = new float3(p.xy, transform.position.z);
        transform.localScale = new float3(rb, 1);
    }
}
