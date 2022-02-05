using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(QuadrantSystem))]
public class MovementSystem : SystemBase {

    protected override void OnUpdate() {

        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Translation translation, in VelocityData velocity, in ActiveStatusData activeStatus) => {
            if(activeStatus.IsActive)
                translation.Value += velocity.direction * velocity.speed * velocity.multiplier * deltaTime;
        }).ScheduleParallel();

    }
}