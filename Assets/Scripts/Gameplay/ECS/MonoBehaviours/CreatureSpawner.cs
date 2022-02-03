/*using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;

public class CreatureSpawner : MonoBehaviour {

    [SerializeField] private GameObject CreaturePrefab;

    [SerializeField] private float2 CreateBounds;
    [SerializeField] private int Amount;

    private World _world;
    private EntityManager _entityManager;

    private Entity _entityCreaturePrefab;

    private void Awake() {
        _world = World.DefaultGameObjectInjectionWorld;
        _entityManager = _world.EntityManager;

        list = new List<CreatureEntityObject>();
        //ConvertPrefabs();


        //InstantiateCreatures(Amount);
    }

    private void ConvertPrefabs() {
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(_world, null);
        _entityCreaturePrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(CreaturePrefab, settings);
    }

    private List<CreatureEntityObject> list;


    public void InstantiateCreatures(int n) {

        for (int i = 0; i < n; i++) {
            float x = UnityEngine.Random.Range(-CreateBounds.x, CreateBounds.x);
            float y = UnityEngine.Random.Range(-CreateBounds.y, CreateBounds.y);

            var c = Instantiate(CreaturePrefab, new Vector3(x, y, 0), Quaternion.identity, transform).GetComponent<CreatureEntityObject>();
            c.spawner = this;
            list.Add(c);
        }
    }

    public void RemoveFromList(CreatureEntityObject entityObject) {
        list.Remove(entityObject);
    }

    public void KillAll() {
        for(int i = 0; i < list.Count; i++) {
            list[i].Kill();
        }
        list.Clear();
    }

    public void InstantiateCreaturesECS(int n) {
        var entities = _entityManager.Instantiate(_entityCreaturePrefab, n, Allocator.Temp);

        float3 t = new float3(transform.position);


        for(int i = 0; i < entities.Length; i++) {
            float x = UnityEngine.Random.Range(-CreateBounds.x, CreateBounds.x); 
            float y = UnityEngine.Random.Range(-CreateBounds.y, CreateBounds.y); 

            _entityManager.SetComponentData(entities[i], new Translation { 
                Value = new float3(t.x + x, t.y + y, t.z) 
            });
        }

        entities.Dispose();
    }

    private void InstantiateCreature(float3 pos) {
        Entity entity = _entityManager.Instantiate(_entityCreaturePrefab);
        _entityManager.SetComponentData(entity, new Translation { Value = pos });

    }



}
*/