using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class CreatureHandler : MonoBehaviour {

    public static CreatureHandler Instance;
    
    public enum Team {
        Player,
        Enemy
    }

    [SerializeField] private LayoutEntityObject EnemyZoneLayoutEntity;
    [SerializeField] private LayoutEntityObject PlayerZoneLayoutEntity;

    [SerializeField] private LayoutEntityObject EnemyBaseLayoutEntity;
    [SerializeField] private LayoutEntityObject PlayerBaseLayoutEntity;

    [SerializeField] private GameObject UniversalCreaturePrefab;
    [SerializeField] private float2 CreatureSize;

    private List<CreatureEntityObject> _creatures;

    private NativeQueue<CreatureEntityObjectRef> _creaturePoolNative;

    private NativeHashMap<int, CreatureEntityObjectRef> _activeCreaturesMap;

    private EntityManager _entityManager;
    public EntityManager EntityManager { get { return _entityManager; } }

    public CreatureEntityObject ByRef(CreatureEntityObjectRef cRef) {
        return _creatures[cRef.I];
    }
    private CreatureEntityObject ByRef(CreatureObjectSyncData syncData) {
        return _creatures[_activeCreaturesMap[syncData.HashKey].I];
    }

    [SerializeField] private float TestCreatureViewRange = 0.15f;
    [SerializeField] private int TestAmount = 1;
    [SerializeField] private float TestSpawnSpeed = 0.25f;
    [SerializeField] private float TestArenaSpeed = 0.05f;
    [SerializeField] private float TestFrequency = 0.25f;

    private float _testDelta = 0;
    private bool _testToggle = false;
    public void RestartTest() {
        _testDelta = 0;
        CreatureObjectSyncSystem.SyncData.Clear();

        KillCreatures();
        DebugCreatureAmount.CreatureAmount = 0;
    }
    public void Test() {
        _testToggle = !_testToggle;
        if (_testToggle) {
            _testDelta = 0;
        }
    }

    private void Update() {
        if (_testToggle) {
            _testDelta += Time.deltaTime;
            if (_testDelta >= TestFrequency) {
                _testDelta -= TestFrequency;
                for (int i = 0; i < TestAmount; i++) {
                    SpawnEnemy(
                        new ClassTypeData { 
                            ClassType = Random.Range(0, 2), 
                            SubClassType = (int)CreatureClass.CreatureSubClassType.Melee 
                        },
                        TestSpawnSpeed,
                        1, // max hp 
                        TestCreatureViewRange);
                    SpawnPlayer(
                        new ClassTypeData {
                            ClassType = Random.Range(0, 2),
                            SubClassType = (int)CreatureClass.CreatureSubClassType.Melee
                        },
                        TestSpawnSpeed,
                        1, // max hp 
                        TestCreatureViewRange);
                }
            }
        }

        SyncEntities();
    }

    public void Init() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        ClearCreatures();
    }

    public void SpawnEnemy(ClassTypeData classType, float speed, float maxHealth, float viewRange) {
        var p = EnemyZoneLayoutEntity.Position;
        var rb = EnemyZoneLayoutEntity.RectBounds;

        float2 halfBounds = rb * 0.5f;
        float2 halfSize = CreatureSize * 0.5f;
        var pos = new float3(p.x - halfBounds.x - halfSize.x, Random.Range(p.y - halfBounds.y, p.y + halfBounds.y), transform.position.z);
        var objective = new CurrentObjectiveData {
            Status = 0,
            ObjectiveID = 0,
            IgnoreTargets = true,
            ObjectiveLocation = new float3(p.x + halfBounds.x - halfSize.x, p.y, transform.position.z),
            ObjectiveBounds = new float3(0, rb.y, 0)
        };
        var attackData = new AttackData {   // TODO get enemy stats
            Damage = 1, 
            AttackRange = 0.045f,  
            AttacksPerSecond = 2f,
            AttackDelay = 2f / 4f,
            Attacking = false,
            HitTarget = false,
            AttackDelta = 0
        };
        SpawnCreature(classType, pos, speed * rb.x, maxHealth, viewRange, (int)Team.Enemy, objective, attackData);
    }
    public void SpawnPlayer(ClassTypeData classType, float speed, float maxHealth, float viewRange) {
        var p = PlayerZoneLayoutEntity.Position;
        var rb = PlayerZoneLayoutEntity.RectBounds;

        float2 halfBounds = rb * 0.5f;
        float2 halfSize = CreatureSize * 0.5f;
        var pos = new float3(p.x + halfBounds.x + halfSize.x, Random.Range(p.y - halfBounds.y, p.y + halfBounds.y), transform.position.z);
        var objective = new CurrentObjectiveData {
            Status = 0,
            ObjectiveID = 0,
            IgnoreTargets = true,
            ObjectiveLocation = new float3(p.x - halfBounds.x + halfSize.x, p.y, transform.position.z),
            ObjectiveBounds = new float3(0, rb.y, 0)
        };
        var attackData = new AttackData {   // TODO get player stats
            Damage = 1,
            AttackRange = 0.045f,
            AttacksPerSecond = 2f,
            AttackDelay = 2f / 4f,
            Attacking = false,
            HitTarget = false,
            AttackDelta = 0
        };
        SpawnCreature(classType, pos, speed * rb.x, maxHealth, viewRange, (int)Team.Player, objective, attackData);
    }
    private void SpawnCreature(ClassTypeData classType, float3 pos, float speed, float maxHealth, float viewRange, int team, CurrentObjectiveData objective, AttackData attackData) {
        var cRef = InstantiateCreature();
        var creature = ByRef(cRef);
        _activeCreaturesMap.Add(creature.EntityKey, cRef);
        creature.InitCreature(classType, pos, speed, maxHealth, viewRange, team, objective, attackData);
    }
    public void DespawnCreature(int EntityKey) {
        if (!_activeCreaturesMap.ContainsKey(EntityKey))
            return;

        var objectRef = _activeCreaturesMap[EntityKey];
        _activeCreaturesMap.Remove(EntityKey);

        _creaturePoolNative.Enqueue(objectRef);
    }

    public void OnObjectiveComplete(CreatureEntityObject creature, CurrentObjectiveData objective, TeamMemberData team) {
        if (objective.ObjectiveID == 0) {

            var playerBase_p = PlayerBaseLayoutEntity.Position;
            var playerBase_rb = PlayerBaseLayoutEntity.RectBounds;
            var enemyBase_p = EnemyBaseLayoutEntity.Position;
            var enemyBase_rb = EnemyBaseLayoutEntity.RectBounds;

            CurrentObjectiveData newObjective = new CurrentObjectiveData {
                Status = 0,
                ObjectiveID = 1,
                IgnoreTargets = false
            };
            float3 position = new float3(0, 0, transform.position.z);

            float2 halfSize = CreatureSize * 0.5f;
            if (team.TeamID == (int)Team.Player) {
                newObjective.ObjectiveLocation = new float3(enemyBase_p.xy, transform.position.z);
                newObjective.ObjectiveBounds = new float3(enemyBase_rb, 0);
                float halfXbounds = playerBase_rb.x * 0.5f;
                position.x = Random.Range(playerBase_p.x - halfXbounds + halfSize.x,
                                          playerBase_p.x + halfXbounds - halfSize.x);
                position.y = playerBase_p.y;
            } else {
                newObjective.ObjectiveLocation = new float3(playerBase_p.xy, transform.position.z);
                newObjective.ObjectiveBounds = new float3(playerBase_rb, 0);
                float halfXbounds = enemyBase_rb.x * 0.5f;
                position.x = Random.Range(enemyBase_p.x - halfXbounds + halfSize.x,
                                          enemyBase_p.x + halfXbounds - halfSize.x);
                position.y = enemyBase_p.y;
            }

            creature.UpdateObjective(
                newObjective,
                position,
                math.distance(playerBase_p.y, enemyBase_p.y) * TestArenaSpeed, // TEMP speed
                3 // health                
                );
        } else if(objective.ObjectiveID == 1){
            creature.Kill();
        }

    }

    private void SyncEntities() {
        for (int i = 0; i < CreatureObjectSyncSystem.SyncData.Length; i++) {
            SyncEntity(CreatureObjectSyncSystem.SyncData[i]);
        }
    }
    private void SyncEntity(CreatureObjectSyncData syncData) {
        var creature = ByRef(syncData);
        creature.SyncEntity(syncData);
    }

    private CreatureEntityObjectRef InstantiateCreature() {
        if (_creaturePoolNative.Count == 0) {
            CreatureEntityObject creature = Instantiate(UniversalCreaturePrefab, transform).GetComponent<CreatureEntityObject>();
            creature.CreatureEntityObjectSetup(this);
            _creatures.Add(creature);
            if (_activeCreaturesMap.Capacity <= _creatures.Count) {
                _activeCreaturesMap.Capacity += 100;
            }
            return new CreatureEntityObjectRef { I = _creatures.Count -1 };
        } else {
            return _creaturePoolNative.Dequeue();
        }
    }

    private void Awake() {
        if (Instance != null && Instance != this) { 
            Destroy(gameObject);
        } else {
            Instance = this;

            _creatures = new List<CreatureEntityObject>();
            _creaturePoolNative = new NativeQueue<CreatureEntityObjectRef>(Allocator.Persistent);
            _activeCreaturesMap = new NativeHashMap<int, CreatureEntityObjectRef>(500, Allocator.Persistent);

            // Temp
            Init();
        }
    }



    private void OnDestroy() {
        DisposeCreatures();
    }

    private void KillCreatures() {
        if (_creatures == null)
            return;

        var enumerator = _activeCreaturesMap.GetEnumerator();
        while (enumerator.MoveNext()) {
            var cRef = enumerator.Current.Value;
            var c = ByRef(cRef);
            if (c != null) {
                c.Kill();
                _creaturePoolNative.Enqueue(cRef);
            }
        }
        _activeCreaturesMap.Clear();
    }

    private void ClearCreatures() {
        if (_creatures == null)
            return;

        var enumerator = _activeCreaturesMap.GetEnumerator();
        while (enumerator.MoveNext()) {
            var c = ByRef(enumerator.Current.Value);
            if (c != null) {
                c.DestroyEntity();
            }
        }
        _activeCreaturesMap.Clear();

        if (_creaturePoolNative.TryDequeue(out CreatureEntityObjectRef item)) {
            do {
                var c = ByRef(item);
                if (c != null) {
                    c.DestroyEntity();
                }
            } while (_creaturePoolNative.TryDequeue(out item));
        }
        _creaturePoolNative.Clear();

        for(int i = 0; i < _creatures.Count; i++) {
            var c = _creatures[i];
            if (c != null) {
                Destroy(c.gameObject);
            }
        }
        _creatures.Clear();
    }

    private void DisposeCreatures() {
        if (_creatures == null)
            return;

        for (int i = 0; i < _creatures.Count; i++) {
            var c = _creatures[i];
            if (c != null) {
                Destroy(c.gameObject);
            }
        }
        _creatures.Clear();

        _creaturePoolNative.Dispose();
        _activeCreaturesMap.Dispose();
    }
}
