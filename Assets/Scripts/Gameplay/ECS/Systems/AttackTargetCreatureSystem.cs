using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public class AttackTargetCreatureSystem : SystemBase {

    private EndSimulationEntityCommandBufferSystem _ecbs;
    protected override void OnCreate() {
        _ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {

        var ecb = _ecbs.CreateCommandBuffer().AsParallelWriter();
        float deltaTime = Time.DeltaTime;


        Entities.WithAll<CreatureTag>().ForEach((
            int entityInQueryIndex,
            ref VelocityData velocity,
            ref TargetEntityData target,
            ref AttackData attackData,
            in Translation translation,
            in CurrentObjectiveData objective,
            in ActiveStatusData activeStatus) => {
                if (activeStatus.IsActive) {
                    if ((!objective.IgnoreTargets) && target.TargetEntity != Entity.Null) {
                        var targetPos = GetComponent<Translation>(target.TargetEntity).Value;

                        if (math.distancesq(translation.Value, targetPos) <= attackData.AttackRange * attackData.AttackRange) {
                            velocity.multiplier = 0;
                            attackData.Attacking = true;
                            attackData.AttackDelta += deltaTime * attackData.AttacksPerSecond;
                            if(attackData.AttackDelta >= attackData.AttackDelay) {
                                if (!attackData.HitTarget) {
                                    attackData.HitTarget = true;

                                    var targetHealth = GetComponent<HealthStateData>(target.TargetEntity);
                                    float currentTargetHealth = targetHealth.CurrentHealthPoints - attackData.Damage;
                                    ecb.SetComponent(entityInQueryIndex, target.TargetEntity, new HealthStateData {
                                        CurrentHealthPoints = currentTargetHealth,
                                        MaxHealthPoints = targetHealth.MaxHealthPoints
                                    });

                                    if (currentTargetHealth <= 0) {
                                        target.TargetEntity = Entity.Null;
                                        velocity.direction = float3.zero;
                                        velocity.multiplier = 1;
                                    }
                                }

                                if(attackData.AttackDelta >= 1.0f) {
                                    attackData.AttackDelta -= 1.0f;
                                    attackData.HitTarget = false;
                                }
                            }

                        } else {
                            velocity.multiplier = 1;
                            attackData.AttackDelta = 0;
                            attackData.HitTarget = false;
                            attackData.Attacking = false;
                        }
                    } else if((!objective.IgnoreTargets) && target.TargetEntity == Entity.Null) {
                        velocity.multiplier = 1;
                        attackData.AttackDelta = 0;
                        attackData.HitTarget = false;
                        attackData.Attacking = false;
                    }
                }
            }).ScheduleParallel(Dependency).Complete();
        _ecbs.AddJobHandleForProducer(Dependency);
    }
}