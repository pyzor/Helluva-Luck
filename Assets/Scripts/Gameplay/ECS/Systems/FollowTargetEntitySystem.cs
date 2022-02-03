using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(KillTargetCreatureSystem))]
[UpdateBefore(typeof(FollowTargetPointSystem))]
public class FollowTargetEntitySystem : SystemBase {

    protected override void OnUpdate() {

        Entities.ForEach((
            ref VelocityData velocity,
            in TargetEntityData target,
            in Translation translation,
            in TargetPointData targetPoint,
            in CurrentObjectiveData objective,
            in ActiveStatusData activeStatus) => {
                if (activeStatus.IsActive && !objective.IgnoreTargets)
                    //velocity.direction = math.normalize(targetPoint.Point - translation.Value);

                    if (target.TargetEntity != Entity.Null) {
                        velocity.direction = math.normalize(GetComponent<Translation>(target.TargetEntity).Value - translation.Value);
                        //Debug.DrawLine(translation.Value, GetComponent<Translation>(target.TargetEntity).Value, Color.white);

                    } else {
                        velocity.direction = float3.zero;
                    }
            }).ScheduleParallel();

    }
}