using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public class ObjectiveCompletionCreatureSystem : SystemBase {

    protected override void OnUpdate() {

        Entities.WithAll<CreatureTag>().ForEach((
            ref CurrentObjectiveData objective,
            in TargetPointData target,
            in Translation translation,
            in ActiveStatusData activeStatus) => {
                if (activeStatus.IsActive) {
                    float dist = math.distance(translation.Value, target.Point);
                    if (dist <= 0.01f) {
                        objective.Status = 1;
                    }
                }
            }).ScheduleParallel();

    }
}
