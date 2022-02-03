using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public class DeathCheckSystem : SystemBase {

    protected override void OnUpdate() {

        Entities.ForEach((
            ref HealthStateData healthState,
            ref ActiveStatusData activeStatus) => {
                if (activeStatus.IsActive) {
                    if(healthState.CurrentHealthPoints <= 0) {
                        activeStatus.IsActive = false;
                    }
                }

            }).Run();
    }
}