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


    private Entity _entity = Entity.Null;
    private EntityManager _entityManager;

    public bool Exist {
        get { return _entity != Entity.Null; }
    }

    private Translation translation;
    private RectBoundsData rectBoundsData;

    public Translation GetTranslation() {
        return translation;
    }
    public RectBoundsData GetRectBoundsData() {
        return rectBoundsData;
    }

    private void Awake() {
        FirstSetup();
    }

    private void FirstSetup() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _entity = _entityManager.CreateEntity(
            typeof(LayoutTag),
            typeof(Translation),
            typeof(RectBoundsData)
            );
        //_entityManager.SetName(_entity, gameObject.name + "'s LayoutEntity");


        //print(gameObject.name + "'s FirstSetup()");
        OnLayoutUpdate();
    }

    private void OnLayoutUpdate() {
        try {
            translation = new Translation {
                Value = RectTransform.position
            };
            rectBoundsData = new RectBoundsData {
                Bounds = new float2(RectTransform.rect.width * RectTransform.lossyScale.x, RectTransform.rect.height * RectTransform.lossyScale.y)
            };

            _entityManager.SetComponentData(_entity, translation);
            _entityManager.SetComponentData(_entity, rectBoundsData);
            //print(gameObject.name + "'s OnLayoutUpdate() - _entity updated!");
        } catch (ObjectDisposedException) {
            //print(gameObject.name+ "'s OnLayoutUpdate() - ObjectDisposedException Catch");
            gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (!Exist)
            return;

        if (!Compare(translation.Value, RectTransform.position))
            OnLayoutUpdate();
    }

    private bool Compare(float3 a, float3 b) {
        return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
    }
}
