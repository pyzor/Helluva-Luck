using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public class KillTargetCreatureSystem : SystemBase {

    //private EndSimulationEntityCommandBufferSystem _ecbs;
    //protected override void OnCreate() {
    //    _ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    //}

    protected override void OnUpdate() {

        //var ecb = _ecbs.CreateCommandBuffer().AsParallelWriter();

        Entities.WithAll<CreatureTag>().ForEach((
            //int entityInQueryIndex,
            ref VelocityData velocity,
            ref TargetEntityData target,
            in Translation translation,
            in CurrentObjectiveData objective,
            in ActiveStatusData activeStatus) => {
                if (activeStatus.IsActive)
                    if ((!objective.IgnoreTargets) && target.TargetEntity != Entity.Null) {
                        var targetPos = GetComponent<Translation>(target.TargetEntity).Value;

                        if (math.distance(translation.Value, targetPos) < 0.01f) {

                            var targetHealth = GetComponent<HealthStateData>(target.TargetEntity);
                            float currentTargetHealth = targetHealth.CurrentHealthPoints - 100;
                            SetComponent(target.TargetEntity, new HealthStateData {
                                CurrentHealthPoints = currentTargetHealth,
                                MaxHealthPoints = targetHealth.MaxHealthPoints
                            });

                            if (currentTargetHealth <= 0) {
                                SetComponent(target.TargetEntity, new TargetableData { IsTargetable = false });
                                target.TargetEntity = Entity.Null;
                                velocity.direction = float3.zero;
                            }
                        }
                    }

            }).Run();
        //_ecbs.AddJobHandleForProducer(Dependency);
    }
}