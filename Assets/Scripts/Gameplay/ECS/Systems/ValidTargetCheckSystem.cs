using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(FollowTargetEntitySystem))]
public class ValidTargetCheckSystem : SystemBase {

    protected override void OnUpdate() {

        Entities.ForEach((
            ref TargetEntityData target,
            ref AttackData attackData,
            in ActiveStatusData activeStatus) => {
                if (activeStatus.IsActive)
                    if (target.TargetEntity != Entity.Null) {
                        if (HasComponent<ActiveStatusData>(target.TargetEntity)) { // Check if target Entity is destroyed
                            if (!GetComponent<ActiveStatusData>(target.TargetEntity).IsActive ||
                                !GetComponent<TargetableData>(target.TargetEntity).IsTargetable) {
                                target.TargetEntity = Entity.Null;
                                attackData.AttackDelta = 0;
                            }
                        } else {
                            target.TargetEntity = Entity.Null;
                            attackData.AttackDelta = 0;
                        }
                    }
                    
            }).ScheduleParallel();

    }
}