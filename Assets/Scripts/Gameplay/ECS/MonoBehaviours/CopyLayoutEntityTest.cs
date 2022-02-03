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
        if (!LayoutEntity.Exist)
            return;

        var t = LayoutEntity.GetTranslation();
        var b = LayoutEntity.GetRectBoundsData();
        transform.position = new float3(t.Value.xy, transform.position.z);
        transform.localScale = new float3(b.Bounds, 1);
    }
}
