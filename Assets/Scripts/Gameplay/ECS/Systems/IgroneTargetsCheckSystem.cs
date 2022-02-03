using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(ValidTargetCheckSystem))]
public class IgroneTargetsCheckSystem : SystemBase {

    protected override void OnUpdate() {

        Entities.ForEach((
            ref TargetEntityData target,
            in CurrentObjectiveData objective,
            in ActiveStatusData activeStatus) => {
                if (activeStatus.IsActive)
                    if (objective.IgnoreTargets && target.TargetEntity != Entity.Null) {
                        target.TargetEntity = Entity.Null;
                    }
            }).ScheduleParallel();

    }
}