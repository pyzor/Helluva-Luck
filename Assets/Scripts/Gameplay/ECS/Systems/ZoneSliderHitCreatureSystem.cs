using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using Helpers;


[BurstCompile]
public class ZoneSliderHitCreatureSystem : SystemBase {

    private EndSimulationEntityCommandBufferSystem _ecbs;
    protected override void OnCreate() {
        _ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {

        var ecb = _ecbs.CreateCommandBuffer().AsParallelWriter();
        var quadrantHashMap = QuadrantSystem.QuadrantHashMap;

        Entities.WithReadOnly(quadrantHashMap).WithAll<ZoneSliderTag>().ForEach((
            int entityInQueryIndex,
            in Translation t,
            in RectBoundsData rb,
            in TeamMemberData team,
            in ZoneData zone,
            in ActiveStatusData activeStatus
            ) => {
                if (activeStatus.IsActive && (team.TeamID != zone.TeamID)) {
                    float2 RB = (rb.Bounds * zone.RectBounds) - new float2(0.001f, 0);
                    float2 halfRB = RB * 0.5f;

                    var topL = QuadrantSystem.PositionHash(t.Value + new float3(-halfRB.x, +halfRB.y, 0));
                    var topR = QuadrantSystem.PositionHash(t.Value + new float3(+halfRB.x, +halfRB.y, 0));
                    var botL = QuadrantSystem.PositionHash(t.Value + new float3(-halfRB.x, -halfRB.y, 0));

                    float horizontalCount = (topR - topL) + 1;
                    float verticalCount = ((topL - botL) / (float)QuadrantSystem.HashYScale) + 1;

                    for (int i = 0; i < horizontalCount; i++) {
                        for (int j = 0; j < verticalCount; j++) {
                            if (quadrantHashMap.TryGetFirstValue(topL + i - j * QuadrantSystem.HashYScale, out QuadrantSystemData item, out NativeMultiHashMapIterator<int> it)) {
                                do {
                                    if (MathHelper.IsInsideRectBounds(item.Position.xy, t.Value.xy, RB)) {
                                        var targetHealth = GetComponent<HealthStateData>(item.Entity);
                                        targetHealth.CurrentHealthPoints = 0;
                                        ecb.SetComponent(entityInQueryIndex, item.Entity, targetHealth);
                                    }
                                } while (quadrantHashMap.TryGetNextValue(out item, ref it));
                            }

                        }
                    }

                }
            }).ScheduleParallel(Dependency).Complete();
        _ecbs.AddJobHandleForProducer(Dependency);
    }
}