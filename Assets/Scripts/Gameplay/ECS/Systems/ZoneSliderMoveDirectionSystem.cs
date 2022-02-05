using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using Helpers;


[BurstCompile]
[UpdateBefore(typeof(ZoneSliderHitCreatureSystem))]
public class ZoneSliderMoveDirectionSystem : SystemBase {

    protected override void OnUpdate() {

        float deltaTime = Time.DeltaTime;

        Entities.WithAll<ZoneSliderTag>().ForEach((
            ref VelocityData velocity,
            in Translation t,
            in RectBoundsData rectBounds,
            in ZoneData zone,
            in ActiveStatusData activeStatus
            ) => {
                if (activeStatus.IsActive) {
                    float3 nextPos = t.Value + (velocity.direction * velocity.speed * velocity.multiplier * deltaTime);
                    if (!MathHelper.IsInsideRectBounds(nextPos.xy, zone.Position.xy, zone.RectBounds - new float2(0, rectBounds.Bounds.y))) {
                        velocity.direction = -velocity.direction;
                    }
                }
            }).ScheduleParallel(Dependency).Complete();

    }


}