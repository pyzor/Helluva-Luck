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

    //private Queue<CreatureEntityObject> _creaturePool;


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

    private float testDelta = 0;
    private bool TestToggle = false;
    public void RestartTest() {
        testDelta = 0;
        CreatureObjectSyncSystem.SyncData.Clear();

        KillCreatures();
        DebugCreatureAmount.CreatureAmount = 0;
    }
    public void Test() {
        TestToggle = !TestToggle;
        if (TestToggle) {
            testDelta = 0;
        }
    }

    private void Update() {
        if (TestToggle) {
            testDelta += Time.deltaTime;
            if (testDelta >= TestFrequency) {
                testDelta -= TestFrequency;
                for (int i = 0; i < TestAmount; i++) {
                    SpawnEnemy(
                        new ClassTypeData { ClassType = Random.Range(0, 2) },
                        TestSpawnSpeed,
                        1, // max hp 
                        TestCreatureViewRange);
                    SpawnPlayer(
                        new ClassTypeData { ClassType = Random.Range(0, 2) },
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
        var t = EnemyZoneLayoutEntity.GetTranslation();
        var rb = EnemyZoneLayoutEntity.GetRectBoundsData();

        float2 halfBounds = rb.Bounds * 0.5f;
        float2 halfSize = CreatureSize * 0.5f;
        var pos = new float3(t.Value.x - halfBounds.x - halfSize.x, Random.Range(t.Value.y - halfBounds.y, t.Value.y + halfBounds.y), transform.position.z);
        var objective = new CurrentObjectiveData {
            Status = 0,
            ObjectiveID = 0,
            IgnoreTargets = true,
            ObjectiveLocation = new float3(t.Value.x + halfBounds.x - halfSize.x, t.Value.y, transform.position.z),
            ObjectiveBounds = new float3(0, rb.Bounds.y, 0)
        };
        SpawnCreature(classType, pos, speed * rb.Bounds.x, maxHealth, viewRange, (int)Team.Enemy, objective);
    }
    public void SpawnPlayer(ClassTypeData classType, float speed, float maxHealth, float viewRange) {
        var t = PlayerZoneLayoutEntity.GetTranslation();
        var rb = PlayerZoneLayoutEntity.GetRectBoundsData();

        float2 halfBounds = rb.Bounds * 0.5f;
        float2 halfSize = CreatureSize * 0.5f;
        var pos = new float3(t.Value.x + halfBounds.x + halfSize.x, Random.Range(t.Value.y - halfBounds.y, t.Value.y + halfBounds.y), transform.position.z);
        var objective = new CurrentObjectiveData {
            Status = 0,
            ObjectiveID = 0,
            IgnoreTargets = true,
            ObjectiveLocation = new float3(t.Value.x - halfBounds.x + halfSize.x, t.Value.y, transform.position.z),
            ObjectiveBounds = new float3(0, rb.Bounds.y, 0)
        };
        SpawnCreature(classType, pos, speed * rb.Bounds.x, maxHealth, viewRange, (int)Team.Player, objective);
    }
    private void SpawnCreature(ClassTypeData classType, float3 pos, float speed, float maxHealth, float viewRange, int team, CurrentObjectiveData objective) {
        var cRef = InstantiateCreature();
        var creature = ByRef(cRef);
        _activeCreaturesMap.Add(creature.EntityKey, cRef);
        creature.InitCreature(classType, pos, speed, maxHealth, viewRange, team, objective);
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

            Translation playerBase_t = PlayerBaseLayoutEntity.GetTranslation();
            RectBoundsData playerBase_rb = PlayerBaseLayoutEntity.GetRectBoundsData();
            Translation enemyBase_t = EnemyBaseLayoutEntity.GetTranslation();
            RectBoundsData enemyBase_rb = EnemyBaseLayoutEntity.GetRectBoundsData();

            CurrentObjectiveData newObjective = new CurrentObjectiveData {
                Status = 0,
                ObjectiveID = 1,
                IgnoreTargets = false
            };
            float3 position = new float3(0, 0, transform.position.z);

            float2 halfSize = CreatureSize * 0.5f;
            if (team.TeamID == (int)Team.Player) {
                newObjective.ObjectiveLocation = new float3(enemyBase_t.Value.xy, transform.position.z);
                newObjective.ObjectiveBounds = new float3(enemyBase_rb.Bounds, 0);
                float halfXbounds = playerBase_rb.Bounds.x * 0.5f;
                position.x = Random.Range(playerBase_t.Value.x - halfXbounds + halfSize.x,
                                          playerBase_t.Value.x + halfXbounds - halfSize.x);
                position.y = playerBase_t.Value.y;
            } else {
                newObjective.ObjectiveLocation = new float3(playerBase_t.Value.xy, transform.position.z);
                newObjective.ObjectiveBounds = new float3(playerBase_rb.Bounds, 0);
                float halfXbounds = enemyBase_rb.Bounds.x * 0.5f;
                position.x = Random.Range(enemyBase_t.Value.x - halfXbounds + halfSize.x,
                                          enemyBase_t.Value.x + halfXbounds - halfSize.x);
                position.y = enemyBase_t.Value.y;
            }

            creature.UpdateObjective(
                newObjective,
                position,
                math.distance(playerBase_t.Value.y, enemyBase_t.Value.y) * TestArenaSpeed, // TEMP speed
                1000 // health                
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
            //_creaturePool = new Queue<CreatureEntityObject>();
            //_activeCreatures = new List<CreatureEntityObject>();

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
                    print("[Pool] DestroyedEntity " + c);
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

    /*
    private void ClearCreaturePool() {
        if (_creaturePoolNative.TryDequeue(out CreatureEntityObjectRef item)) {
            do {
                if(item.Creature != null) {
                    Destroy(item.Creature.gameObject);
                }
            } while (_creaturePoolNative.TryDequeue(out item));
        }
    }

    private void ClearActiveCreatures() {
        if (_activeCreatures != null) {
            for (int i = 0; i < _activeCreatures.Count; i++) {
                var creature = _activeCreatures[i];
                if (creature != null)
                    Destroy(creature.gameObject);
            }
            _activeCreatures.Clear();
        }
    }
    private void ClearCreaturePool() {
        if (_creaturePool != null) {
            for (int i = 0; i < _activeCreatures.Count; i++) {
                var creature = _activeCreatures[i];
                if (creature != null)
                    Destroy(creature.gameObject);
            }
            _activeCreatures.Clear();
        }
    }

    private void KillActiveCreatures() {
        if (_activeCreatures != null) {
            for (int i = 0; i < _activeCreatures.Count; i++) {
                var creature = _activeCreatures[i];
                if (creature != null) {
                    creature.Kill();
                    i--;
                }
            }
        }
    }
    */
}
