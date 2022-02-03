using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System;




[BurstCompile]
[UpdateBefore(typeof(FindClosestObjectivePointCreatureSystem))]
public class FindClosestOppositeTeamMemberCreatureSystem : SystemBase {


    protected override void OnUpdate() {

        var quadrantHashMap = QuadrantSystem.QuadrantHashMap;

        Entities.WithReadOnly(quadrantHashMap).WithAll<CreatureTag>().ForEach((
            ref TargetEntityData target,
            in Translation translation,
            in TeamMemberData team,
            in ViewRangeData viewRange,
            in CurrentObjectiveData objective,
            in ActiveStatusData activeStatus) => {
                if (activeStatus.IsActive)
                    if (!objective.IgnoreTargets) {

                        Entity closestEntity = Entity.Null;
                        float squaredDistance = float.MaxValue;

                        NativeArray<int> hashes = new NativeArray<int>(9, Allocator.Temp);

                        hashes[0]=QuadrantSystem.PositionHash(translation.Value);
                        hashes[1]=QuadrantSystem.PositionHash(translation.Value + new float3(0, +viewRange.Value, 0));
                        hashes[2]=QuadrantSystem.PositionHash(translation.Value + new float3(0, -viewRange.Value, 0));
                        hashes[3]=QuadrantSystem.PositionHash(translation.Value + new float3(+viewRange.Value, 0, 0));
                        hashes[4]=QuadrantSystem.PositionHash(translation.Value + new float3(-viewRange.Value, 0, 0));
                        hashes[5]=QuadrantSystem.PositionHash(translation.Value + new float3(+viewRange.Value, +viewRange.Value, 0));
                        hashes[6]=QuadrantSystem.PositionHash(translation.Value + new float3(-viewRange.Value, +viewRange.Value, 0));
                        hashes[7]=QuadrantSystem.PositionHash(translation.Value + new float3(+viewRange.Value, -viewRange.Value, 0));
                        hashes[8]=QuadrantSystem.PositionHash(translation.Value + new float3(-viewRange.Value, -viewRange.Value, 0));

                        hashes.Sort();

                        CheckTargets(hashes[0], quadrantHashMap, team.TeamID, translation.Value, ref closestEntity, ref squaredDistance);
                        for (int i = 1; i < 9; i++) {
                            if (hashes[i] != hashes[i - 1])
                                CheckTargets(hashes[i], quadrantHashMap, team.TeamID, translation.Value, ref closestEntity, ref squaredDistance);
                        }
                        hashes.Dispose();

                        target.TargetEntity = (squaredDistance > (viewRange.Value * viewRange.Value)) ? Entity.Null : closestEntity;
                    }
            })
        .ScheduleParallel(Dependency).Complete();
    }

    private static void CheckTargets(int hashKey, NativeMultiHashMap<int, QuadrantSystemData> quadrantHashMap, int TeamID, float3 pos, ref Entity closestEntity, ref float squaredDistance) {
        if (quadrantHashMap.TryGetFirstValue(hashKey, out QuadrantSystemData item, out NativeMultiHashMapIterator<int> it)) {
            do {
                if (TeamID != item.TeamID) {
                    if (closestEntity == Entity.Null) {
                        closestEntity = item.Entity;
                    } else {
                        float newDist = math.distancesq(pos, item.Position);
                        if (newDist < squaredDistance) {
                            squaredDistance = newDist;
                            closestEntity = item.Entity;
                        }
                    }
                }
            } while (quadrantHashMap.TryGetNextValue(out item, ref it));
        }
    }
}






/* OLD VERSION


[BurstCompile]
[UpdateBefore(typeof(FindClosestObjectivePointCreatureSystem))]
public class FindClosestOppositeTeamMemberCreatureSystem : SystemBase {

    private EntityQueryDesc _entityQueryDesc;

    protected override void OnCreate() {
        _entityQueryDesc = new EntityQueryDesc {
            All = new ComponentType[] {
                ComponentType.ReadOnly<CreatureTag>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<TeamMemberData>(),
                ComponentType.ReadOnly<TargetableData>(),
                ComponentType.ReadOnly<ActiveStatusData>(),
            }
        };
    }

    protected override void OnUpdate() {

        var entities = GetEntityQuery(_entityQueryDesc).ToEntityArrayAsync(Allocator.TempJob, out JobHandle queryJobHandle);

        Entities.WithAll<CreatureTag>().ForEach((
            ref TargetEntityData target,
            in Translation translation,
            in TeamMemberData team,
            in ViewRangeData viewRange,
            in CurrentObjectiveData objective,
            in ActiveStatusData activeStatus) => {
                if (activeStatus.IsActive)
                    if (!objective.IgnoreTargets) {
                        //if (target.TargetEntity == Entity.Null) {

                        int closestEntity = -1;
                        float distance = float.MaxValue;
                        float3 closestPos = float3.zero;

                        for (int i = 0; i < entities.Length; i++) {
                            var status = GetComponent<ActiveStatusData>(entities[i]);
                            if (!status.IsActive)
                                continue;

                            var tar = GetComponent<TargetableData>(entities[i]);
                            if (!tar.IsTargetable)
                                continue;

                            var pos = GetComponent<Translation>(entities[i]);
                            var t = GetComponent<TeamMemberData>(entities[i]);

                            if (team.TeamID != t.TeamID) {
                                if (closestEntity == -1) {
                                    closestEntity = i;
                                    closestPos = pos.Value;
                                } else {
                                    float newDist = math.distance(translation.Value, pos.Value);
                                    if (newDist < distance) {
                                        distance = newDist;
                                        closestEntity = i;
                                        closestPos = pos.Value;
                                    }
                                }
                            }
                        }   

                        target.TargetEntity = (closestEntity == -1 || distance > viewRange.Value) ? Entity.Null : entities[closestEntity];
                    }
            })
        .WithReadOnly(entities)
        .WithDisposeOnCompletion(entities)
        .ScheduleParallel(queryJobHandle).Complete();

    }
}

  
  */