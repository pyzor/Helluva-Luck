using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct QuadrantSystemData {
    public Entity Entity;
    public float3 Position;
    public int TeamID;
    public bool IsTargetable;
}

[BurstCompile]
[UpdateBefore(typeof(FindClosestOppositeTeamMemberCreatureSystem))]
public class QuadrantSystem : SystemBase {

    private const float _quadrantCellSize = 0.225f;
    private const float _quadrantCellSizeMulti = 1f / _quadrantCellSize;
    public static NativeMultiHashMap<int, QuadrantSystemData> QuadrantHashMap;
    public static readonly int HashYScale = 10;

    protected override void OnCreate() {
        QuadrantHashMap = new NativeMultiHashMap<int, QuadrantSystemData>(0, Allocator.Persistent);
    }

    protected override void OnDestroy() {
        QuadrantHashMap.Dispose();
    }

    protected override void OnUpdate() {
        EntityQuery entityQuery = GetEntityQuery(typeof(Translation), typeof(TargetableData), typeof(TeamMemberData));
        int enityCount = entityQuery.CalculateEntityCount();

        QuadrantHashMap.Clear();
        if (enityCount > QuadrantHashMap.Capacity) {
            QuadrantHashMap.Capacity = enityCount;
        }

        NativeMultiHashMap<int, QuadrantSystemData>.ParallelWriter hashMapWriter = QuadrantHashMap.AsParallelWriter();

        Entities.ForEach((
            Entity e,
            in TeamMemberData teamMember,
            in Translation translation,
            in TargetableData targetable,
            in ActiveStatusData activeStatus
            ) => {
                if (activeStatus.IsActive) {
                    hashMapWriter.Add(PositionHash(translation.Value), new QuadrantSystemData {
                        Entity = e,
                        Position = translation.Value,
                        TeamID = teamMember.TeamID,
                        IsTargetable = targetable.IsTargetable
                    });
                }
            }).ScheduleParallel(Dependency).Complete();

    }

    public static int PositionHash(float3 pos) {
        return (int)(math.floor(pos.x * _quadrantCellSizeMulti) + math.floor(pos.y * _quadrantCellSizeMulti) * HashYScale);
    }



    /*  USEFULL FOR DEBUG
     * 
     *  var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     *  DebugDraw(pos);
     *  DebugEntityCount(pos, QuadrantHashMap);
     * /
    private static void DebugDraw(float3 pos) {
        float3 indexed = new float3(math.floor(pos.x / _quadrantCellSize), math.floor(pos.y / _quadrantCellSize), 0);
        float3 origin = indexed * _quadrantCellSize;
        Debug.DrawLine(origin, origin + new float3(_quadrantCellSize, 0, 0));
        Debug.DrawLine(origin, origin + new float3(0, _quadrantCellSize, 0));
        Debug.DrawLine(origin + new float3(_quadrantCellSize, _quadrantCellSize, 0), origin + new float3(_quadrantCellSize, 0, 0));
        Debug.DrawLine(origin + new float3(_quadrantCellSize, _quadrantCellSize, 0), origin + new float3(0, _quadrantCellSize, 0));
    }
    private static void DebugEntityCount(float3 pos, NativeMultiHashMap<int, QuadrantSystemData> hashMap) {
        int key = PositionHash(pos);
        int count = 0;
        if (hashMap.TryGetFirstValue(key, out QuadrantSystemData q, out NativeMultiHashMapIterator<int> iterator)) {
            do {
                count += 1;
            } while (hashMap.TryGetNextValue(out q, ref iterator));
        }   
        Debug.Log("Quadrant EntityCount [" + count + "]");
    }
    /**/
}