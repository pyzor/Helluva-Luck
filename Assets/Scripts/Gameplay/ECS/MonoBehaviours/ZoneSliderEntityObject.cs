using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using Random = UnityEngine.Random;
using Helpers;

public struct ZoneSliderEntityObjectRef {
    public int I;
}

public class ZoneSliderEntityObject : MonoBehaviour {

    [SerializeField] private SpriteRenderer _spriteRenderer;

    private bool _isActive = false;
    public bool IsActive { get { return _isActive; } }

    private Entity _entity = Entity.Null;
    public bool Exist { get { return _entity != Entity.Null; } }
    public int EntityKey {
        get { return _entity.Index; }
    }

    private ZoneSliderHandler _handler = null;


    public void ZoneSliderEntityObjectSetup(ZoneSliderHandler handler) {
        if (_handler != null)
            return;

        _handler = handler;
        _entity = _handler.EntityManager.CreateEntity(
            // Tags
            typeof(ZoneSliderTag),
            // CreatureData
            typeof(Translation),
            typeof(TeamMemberData),
            typeof(VelocityData),
            typeof(ActiveStatusData),
            typeof(RectBoundsData),
            typeof(ZoneData)
            );

        _handler.EntityManager.SetComponentData(_entity, new ActiveStatusData { IsActive = false, InPool = true });
        gameObject.SetActive(false);
    }

    public void InitSlider(float3 pos, float speed, float2 bounds, int team, ZoneData zoneData) {
        if (!Exist)
            return;

        _handler.EntityManager.SetComponentData(_entity, new Translation { Value = pos });
        _handler.EntityManager.SetComponentData(_entity, new TeamMemberData { TeamID = team });
        _handler.EntityManager.SetComponentData(_entity, new VelocityData {
            direction = new float3(0, 1f, 0),
            speed = speed,
            multiplier = 1
        });
        _handler.EntityManager.SetComponentData(_entity, new ActiveStatusData { IsActive = true, InPool = false });
        _handler.EntityManager.SetComponentData(_entity, new RectBoundsData { Bounds = bounds });
        _handler.EntityManager.SetComponentData(_entity, zoneData);

        transform.position = pos;
        transform.localScale = new float3(bounds.x * zoneData.RectBounds.x, bounds.y * zoneData.RectBounds.y, 1);

        _spriteRenderer.color = (team == (int)ZoneSliderHandler.Team.Player) ? new Color(0, 1, 0, 0.4f) : new Color(1, 0, 0, 0.4f);

        _isActive = true;
        gameObject.SetActive(true);
    }

    public void SyncEntity(SliderObjectSyncData syncData) {
        if (!syncData.ActiveStatus.IsActive) {
            _isActive = false;
            OnDeactivation();
            return;
        }

        if (!NaNCheck.IsNaN(syncData.Pos))
            transform.position = syncData.Pos;
    }

    public void Deactivate() {
        _handler.EntityManager.SetComponentData(_entity, new ActiveStatusData { IsActive = false, InPool = true });
        gameObject.SetActive(false);
    }

    public void DestroyEntity() {
        _handler.EntityManager.DestroyEntity(_entity);
        _entity = Entity.Null;
        _isActive = false;
        _handler = null;
    }

    private void OnDeactivation() {
        _handler.EntityManager.SetComponentData(_entity, new ActiveStatusData { IsActive = false, InPool = true });
        gameObject.SetActive(false);
        _handler.DespawnSlider(EntityKey);
    }
}