using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;
using System;

public class ZoneSliderHandler : MonoBehaviour {

    public static ZoneSliderHandler Instance;
    
    public enum Team {
        Player,
        Enemy
    }

    [SerializeField] private LayoutEntityObject EnemyZoneLayoutEntity;
    [SerializeField] private LayoutEntityObject PlayerZoneLayoutEntity;

    [SerializeField] private GameObject ZoneSliderPrefab;
    [SerializeField] private float2 ZoneSliderBounds;
    [SerializeField] private float ZoneSliderSpeed;

    private List<ZoneSliderEntityObject> _sliders;

    private NativeQueue<ZoneSliderEntityObjectRef> _sliderPoolNative;

    private NativeHashMap<int, ZoneSliderEntityObjectRef> _activeSlidersMap;

    private EntityManager _entityManager;
    public EntityManager EntityManager { get { return _entityManager; } }

    public ZoneSliderEntityObject ByRef(ZoneSliderEntityObjectRef cRef) {
        return _sliders[cRef.I];
    }
    private ZoneSliderEntityObject ByRef(SliderObjectSyncData syncData) {
        return _sliders[_activeSlidersMap[syncData.HashKey].I];
    }

    private void Update() {
        SyncEntities();
        if (!isTest)
            Test();
    }

    private bool isTest = false;
    private void Test() {
        var EnemyZone_p = EnemyZoneLayoutEntity.Position;
        EnemyZone_p.z = transform.position.z;
        var EnemyZone_rb = EnemyZoneLayoutEntity.RectBounds;

        var PlayerZone_p = PlayerZoneLayoutEntity.Position;
        PlayerZone_p.z = transform.position.z;
        var PlayerZone_rb = PlayerZoneLayoutEntity.RectBounds;

        SpawnSlider(EnemyZone_p, ZoneSliderSpeed, ZoneSliderBounds, (int)Team.Player, new ZoneData { Position = EnemyZone_p, RectBounds = EnemyZone_rb, TeamID = (int)Team.Enemy });
        
        SpawnSlider(PlayerZone_p, -ZoneSliderSpeed, ZoneSliderBounds, (int)Team.Enemy, new ZoneData { Position = PlayerZone_p, RectBounds = PlayerZone_rb, TeamID = (int)Team.Player });
        isTest = true;
    }


    public void Init() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        ClearSliders();
    }

    public void SpawnSlider(float3 pos, float speed, float2 bounds, int teamID, ZoneData zoneData) {
        var sRef = InstantiateSlider();
        var slider = ByRef(sRef);
        _activeSlidersMap.Add(slider.EntityKey, sRef);
        slider.InitSlider(pos, speed, bounds, teamID, zoneData);
    }

    public void DespawnSlider(int EntityKey) {
        if (!_activeSlidersMap.ContainsKey(EntityKey))
            return;

        var objectRef = _activeSlidersMap[EntityKey];
        _activeSlidersMap.Remove(EntityKey);

        _sliderPoolNative.Enqueue(objectRef);
    }

    private void SyncEntities() {
        for (int i = 0; i < SliderObjectSyncSystem.SyncData.Length; i++) {
            SyncEntity(SliderObjectSyncSystem.SyncData[i]);
        }
    }

    private void SyncEntity(SliderObjectSyncData syncData) {
        var slider = ByRef(syncData);
        slider.SyncEntity(syncData);
    }

    private ZoneSliderEntityObjectRef InstantiateSlider() {
        if (_sliderPoolNative.Count == 0) {
            ZoneSliderEntityObject slider = Instantiate(ZoneSliderPrefab, transform).GetComponent<ZoneSliderEntityObject>();
            slider.ZoneSliderEntityObjectSetup(this);
            _sliders.Add(slider);
            if (_activeSlidersMap.Capacity <= _sliders.Count) {
                _activeSlidersMap.Capacity += 100;
            }
            return new ZoneSliderEntityObjectRef { I = _sliders.Count -1 };
        } else {
            return _sliderPoolNative.Dequeue();
        }
    }

    private void Awake() {
        if (Instance != null && Instance != this) { 
            Destroy(gameObject);
        } else {
            Instance = this;

            _sliders = new List<ZoneSliderEntityObject>();
            _sliderPoolNative = new NativeQueue<ZoneSliderEntityObjectRef>(Allocator.Persistent);
            _activeSlidersMap = new NativeHashMap<int, ZoneSliderEntityObjectRef>(500, Allocator.Persistent);

            // Temp
            Init();
        }
    }

    private void OnDestroy() {
        DisposeSliders();
    }

    private void DestroySliders() {
        if (_sliders == null)
            return;

        var enumerator = _activeSlidersMap.GetEnumerator();
        while (enumerator.MoveNext()) {
            var sRef = enumerator.Current.Value;
            var obj = ByRef(sRef);
            if (obj != null) {
                obj.Deactivate();
                _sliderPoolNative.Enqueue(sRef);
            }
        }
        _activeSlidersMap.Clear();
    }

    private void ClearSliders() {
        if (_sliders == null)
            return;

        var enumerator = _activeSlidersMap.GetEnumerator();
        while (enumerator.MoveNext()) {
            var obj = ByRef(enumerator.Current.Value);
            if (obj != null) {
                obj.DestroyEntity();
            }
        }
        _activeSlidersMap.Clear();

        if (_sliderPoolNative.TryDequeue(out ZoneSliderEntityObjectRef item)) {
            do {
                var obj = ByRef(item);
                if (obj != null) {
                    obj.DestroyEntity();
                }
            } while (_sliderPoolNative.TryDequeue(out item));
        }
        _sliderPoolNative.Clear();

        for(int i = 0; i < _sliders.Count; i++) {
            var obj = _sliders[i];
            if (obj != null) {
                Destroy(obj.gameObject);
            }
        }
        _sliders.Clear();
    }

    private void DisposeSliders() {
        if (_sliders == null)
            return;

        for (int i = 0; i < _sliders.Count; i++) {
            var obj = _sliders[i];
            if (obj != null) {
                Destroy(obj.gameObject);
            }
        }
        _sliders.Clear();

        _sliderPoolNative.Dispose();
        _activeSlidersMap.Dispose();
    }
}
