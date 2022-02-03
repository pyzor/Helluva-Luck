using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(KillTargetCreatureSystem))]
public class FollowTargetPointSystem : SystemBase {

    protected override void OnUpdate() {

        Entities.ForEach((
            ref VelocityData velocity,
            in Translation translation,
            in TargetPointData targetPoint,
            in TargetEntityData targetEntity,
            in ActiveStatusData activeStatus) => {
                if (activeStatus.IsActive && targetEntity.TargetEntity == Entity.Null)
                    velocity.direction = math.normalize(targetPoint.Point - translation.Value);
            }).ScheduleParallel();

    }
}