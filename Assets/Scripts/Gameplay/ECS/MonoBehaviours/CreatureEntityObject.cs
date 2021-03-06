using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using Random = UnityEngine.Random;
using Helpers;

public struct CreatureEntityObjectRef {
    public int I;
}

public class CreatureEntityObject : MonoBehaviour {
    [SerializeField] private CreatureAnimator CreatureAnimator;
    [SerializeField] private CreatureClass CreatureClass;

    private bool _isActive = false;
    public bool IsActive { get { return _isActive; } }

    private Entity _entity = Entity.Null;
    public bool Exist { get { return _entity != Entity.Null; } }
    public int EntityKey {
        get { return _entity.Index; }
    }

    private CreatureHandler _handler = null;

    private float2 _lastState;
    private bool _idleState;
    private bool _ApplyMaterialBlock;
    private bool _lookLeft;

    public void CreatureEntityObjectSetup(CreatureHandler handler) {
        if (_handler != null)
            return;

        _handler = handler;
        _entity = _handler.EntityManager.CreateEntity(
            // Tags
            typeof(CreatureTag),
            // CreatureData
            typeof(Translation),
            typeof(ClassTypeData),
            typeof(HealthStateData),
            typeof(TeamMemberData),
            typeof(VelocityData),
            typeof(TargetableData),
            typeof(ActiveStatusData),
            typeof(CurrentObjectiveData),
            typeof(ViewRangeData),
            typeof(AttackData),
            // SystemDependent
            typeof(TargetPointData),
            typeof(TargetEntityData)
            );

        _handler.EntityManager.SetComponentData(_entity, new ActiveStatusData { IsActive = false, InPool = true });
        gameObject.SetActive(false);
    }

    public void InitCreature(ClassTypeData classType, float3 pos, float speed, float maxHealth, float viewRange, int team, CurrentObjectiveData objective, AttackData attackData) {
        if (!Exist)
            return;

        _handler.EntityManager.SetComponentData(_entity, classType);
        transform.position = pos;
        _handler.EntityManager.SetComponentData(_entity, new Translation { Value = pos });
        _handler.EntityManager.SetComponentData(_entity, new VelocityData {
            speed = speed,
            multiplier = 1
        });
        _handler.EntityManager.SetComponentData(_entity, new HealthStateData {
            CurrentHealthPoints = maxHealth,
            MaxHealthPoints = maxHealth
        });
        _handler.EntityManager.SetComponentData(_entity, new ViewRangeData { Value = viewRange });
        _handler.EntityManager.SetComponentData(_entity, new TeamMemberData { TeamID = team });
        _handler.EntityManager.SetComponentData(_entity, objective);
        _handler.EntityManager.SetComponentData(_entity, new TargetableData { IsTargetable = !objective.IgnoreTargets });
        _handler.EntityManager.SetComponentData(_entity, new ActiveStatusData { IsActive = true, InPool = false });
        _handler.EntityManager.SetComponentData(_entity, attackData);

        _isActive = true;
        gameObject.SetActive(true);

        CreatureClass.UpdateClass(classType);
        CreatureAnimator.SetColor((team == 0) ? Color.white : (
            (classType.ClassType == (int)CreatureClass.CreatureClassType.Imp) ? new Color(0.5f, 0, 1, 1) : new Color(0.4f,0,0.4f,1))); // Temp Coloring

        _idleState = true;
        _lastState = CreatureAnimator.STATE_IDLE;
        CreatureAnimator.ChangeAnimationState(_lastState, 0.5f);
        CreatureAnimator.ApplyMaterialBlock();

        DebugCreatureAmount.CreatureAmount += 1; // TODO REMOVE_DEBUG
    }

    public void UpdateObjective(CurrentObjectiveData objective, float3 pos, float speed, float maxHealth) {
        if (!Exist)
            return;

        transform.position = pos;
        _handler.EntityManager.SetComponentData(_entity, new Translation { Value = pos });
        _handler.EntityManager.SetComponentData(_entity, new VelocityData {
            speed = speed,
            multiplier = 1
        });
        _handler.EntityManager.SetComponentData(_entity, new HealthStateData {
            CurrentHealthPoints = maxHealth,
            MaxHealthPoints = maxHealth
        });
        _handler.EntityManager.SetComponentData(_entity, objective);
        _handler.EntityManager.SetComponentData(_entity, new TargetableData { IsTargetable = !objective.IgnoreTargets });
        _handler.EntityManager.SetComponentData(_entity, new TargetEntityData { TargetEntity = Entity.Null });
    }

    public void SyncEntity(CreatureObjectSyncData syncData) {
        if (!syncData.ActiveStatus.IsActive) {
            _isActive = false;
            OnDeath();
            return;
        }

        if (syncData.Objective.Status == 1) {
            _handler.OnObjectiveComplete(this, syncData.Objective, syncData.Team);
            return;
        }

        if (!NaNCheck.IsNaN(syncData.Pos))
            transform.position = syncData.Pos;

        if (syncData.AttackData.Attacking) {
            if (_lastState.x != CreatureAnimator.STATE_ATTACK.x) {
                _ApplyMaterialBlock = true;
                _lastState = CreatureAnimator.STATE_ATTACK;
                CreatureAnimator.ChangeAnimationState(_lastState);
            }
        } else { 
            if ((syncData.Velocity.speed == 0) || (syncData.Velocity.direction.x == 0 && syncData.Velocity.direction.y == 0)) {
                if (_idleState) {
                    if (_lastState.x != CreatureAnimator.STATE_IDLE.x) {
                        _ApplyMaterialBlock = true;
                        _lastState = CreatureAnimator.STATE_IDLE;
                        CreatureAnimator.ChangeAnimationState(_lastState, 0.5f);
                    }
                } else {
                    _idleState = true;
                }
            } else {
                if (_lastState.x != CreatureAnimator.STATE_WALK.x) {
                    _ApplyMaterialBlock = true;
                    _lastState = CreatureAnimator.STATE_WALK;
                    CreatureAnimator.ChangeAnimationState(_lastState);
                }
            }
        }

        if (syncData.Velocity.direction.x > 0 && _lookLeft) {
            _lookLeft = false;
            _ApplyMaterialBlock = true;
            CreatureAnimator.LookLeft(_lookLeft);
        } else if (syncData.Velocity.direction.x < 0 && !_lookLeft) {
            _lookLeft = true;
            _ApplyMaterialBlock = true;
            CreatureAnimator.LookLeft(_lookLeft);
        }

        if (_ApplyMaterialBlock) {
            _ApplyMaterialBlock = false;
            CreatureAnimator.ApplyMaterialBlock();
        }
    }

    public void Kill() {
        _handler.EntityManager.SetComponentData(_entity, new ActiveStatusData { IsActive = false, InPool = true });
        gameObject.SetActive(false);
        DebugCreatureAmount.CreatureAmount -= 1; // TODO REMOVE_DEBUG
    }

    public void DestroyEntity() {
        _handler.EntityManager.DestroyEntity(_entity);
        _entity = Entity.Null;
        _isActive = false;
        _handler = null;
    }

    private void OnDeath() {
        _handler.EntityManager.SetComponentData(_entity, new ActiveStatusData { IsActive = false, InPool = true });
        gameObject.SetActive(false);
        _handler.DespawnCreature(EntityKey);
        DebugCreatureAmount.CreatureAmount -= 1; // TODO REMOVE_DEBUG
    }
}