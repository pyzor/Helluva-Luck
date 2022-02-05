using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(FindClosestObjectivePointCreatureSystem))]
public class FindClosestOppositeTeamMemberCreatureSystem : SystemBase {


    protected override void OnUpdate() {

        var quadrantHashMap = QuadrantSystem.QuadrantHashMap;

        Entities.WithReadOnly(quadrantHashMap).WithAll<CreatureTag>().ForEach((
            ref TargetEntityData target,
            in Translation t,
            in TeamMemberData team,
            in ViewRangeData viewRange,
            in CurrentObjectiveData objective,
            in ActiveStatusData activeStatus) => {
                if (activeStatus.IsActive)
                    if (!objective.IgnoreTargets) {

                        Entity closestEntity = Entity.Null;
                        float squaredDistance = float.MaxValue;

                        var topL = QuadrantSystem.PositionHash(t.Value + new float3(-viewRange.Value, +viewRange.Value, 0));
                        var topR = QuadrantSystem.PositionHash(t.Value + new float3(+viewRange.Value, +viewRange.Value, 0));
                        var botL = QuadrantSystem.PositionHash(t.Value + new float3(-viewRange.Value, -viewRange.Value, 0));

                        float horizontalCount = (topR - topL) + 1;
                        float verticalCount = ((topL - botL) / (float)QuadrantSystem.HashYScale) + 1;

                        for (int i = 0; i < horizontalCount; i++) {
                            for (int j = 0; j < verticalCount; j++) {
                                CheckForTargets(topL + i - j * QuadrantSystem.HashYScale, quadrantHashMap, team.TeamID, t.Value, ref closestEntity, ref squaredDistance);
                            }
                        }

                        target.TargetEntity = (squaredDistance > (viewRange.Value * viewRange.Value)) ? Entity.Null : closestEntity;
                    }
            }).ScheduleParallel(Dependency).Complete();
    }

    private static void CheckForTargets(int hashKey, NativeMultiHashMap<int, QuadrantSystemData> quadrantHashMap, int TeamID, float3 pos, ref Entity closestEntity, ref float squaredDistance) {
        if (quadrantHashMap.TryGetFirstValue(hashKey, out QuadrantSystemData item, out NativeMultiHashMapIterator<int> it)) {
            do {
                if (TeamID != item.TeamID && item.IsTargetable) {
                    if (closestEntity == Entity.Null) {
                        closestEntity = item.Entity;
                        squaredDistance = math.distancesq(pos, item.Position);
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