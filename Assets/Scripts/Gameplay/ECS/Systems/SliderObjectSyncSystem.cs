using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using System;

[Serializable]
public struct SliderObjectSyncData {
    public int HashKey;
    public float3 Pos;
    public ActiveStatusData ActiveStatus;
    public TeamMemberData Team;
    public ZoneData Zone;
}

[BurstCompile]
[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderFirst = true)]
public class SliderObjectSyncSystem : SystemBase {

    public static NativeList<SliderObjectSyncData> SyncData;

    protected override void OnCreate() {
        SyncData = new NativeList<SliderObjectSyncData>(0, Allocator.Persistent);
    }

    protected override void OnDestroy() {
        SyncData.Dispose();
    }

    protected override void OnUpdate() {
        EntityQuery entityQuery = GetEntityQuery(typeof(ZoneSliderTag), typeof(ActiveStatusData));
        int enityCount = entityQuery.CalculateEntityCount();

        SyncData.Clear();
        if (enityCount > SyncData.Capacity) {
            SyncData.Capacity = enityCount;
        }

        NativeList<SliderObjectSyncData>.ParallelWriter writer = SyncData.AsParallelWriter();

        Entities.WithAll<ZoneSliderTag>().ForEach((
            Entity e,
            in Translation translation,
            in TeamMemberData team,
            in ZoneData zone,
            in ActiveStatusData activeStatus
            ) => {
                if (!activeStatus.InPool)
                    writer.AddNoResize((new SliderObjectSyncData {
                        HashKey = e.Index,
                        Pos = translation.Value,
                        ActiveStatus = activeStatus,
                        Zone = zone,
                        Team = team
                    }));
            }).ScheduleParallel(Dependency).Complete();
    }
}