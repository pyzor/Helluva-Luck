using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public class FindClosestObjectivePointCreatureSystem : SystemBase {

    protected override void OnUpdate() {

        Entities.WithAll<CreatureTag>().ForEach((
            ref TargetPointData target,
            in Translation translation, 
            in CurrentObjectiveData objective, 
            in ActiveStatusData activeStatus) => {
                if (activeStatus.IsActive && objective.Status == 0) {
                    float3 halfBounds = objective.ObjectiveBounds * 0.5f;
                    target.Point = math.clamp(translation.Value, objective.ObjectiveLocation - halfBounds, objective.ObjectiveLocation + halfBounds);
                    //Debug.DrawLine(translation.Value, target.Point);
                }
            }).ScheduleParallel();

    }
}
