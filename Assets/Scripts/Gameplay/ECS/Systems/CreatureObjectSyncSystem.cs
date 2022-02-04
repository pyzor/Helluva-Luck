using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using System;

[Serializable]
public struct CreatureObjectSyncData {
    public int HashKey;
    public float3 Pos;
    public ActiveStatusData ActiveStatus;
    public VelocityData Velocity;
    public CurrentObjectiveData Objective;
    public TeamMemberData Team;
    public AttackData AttackData;
}

[BurstCompile]
public class CreatureObjectSyncSystem : SystemBase {

    public static NativeList<CreatureObjectSyncData> SyncData;

    protected override void OnCreate() {
        SyncData = new NativeList<CreatureObjectSyncData>(0, Allocator.Persistent);
    }

    protected override void OnDestroy() {
        SyncData.Dispose();
    }

    protected override void OnUpdate() {
        EntityQuery entityQuery = GetEntityQuery(typeof(CreatureTag), typeof(ActiveStatusData));
        int enityCount = entityQuery.CalculateEntityCount();

        SyncData.Clear();
        if (enityCount > SyncData.Capacity) {
            SyncData.Capacity = enityCount;
        }

        NativeList<CreatureObjectSyncData>.ParallelWriter writer = SyncData.AsParallelWriter();

        Entities.WithoutBurst().WithAll<CreatureTag>().ForEach((
            Entity e,
            in Translation translation,
            in VelocityData velocity,
            in CurrentObjectiveData objective,
            in TeamMemberData team,
            in AttackData attackData,
            in ActiveStatusData activeStatus
            ) => {
                if (!activeStatus.InPool)
                    writer.AddNoResize((new CreatureObjectSyncData {
                        HashKey = e.Index,
                        Pos = translation.Value,
                        ActiveStatus = activeStatus,
                        Velocity = velocity,
                        Objective = objective,
                        Team = team,
                        AttackData = attackData
                    }));
            }).ScheduleParallel(Dependency).Complete();
    }
}