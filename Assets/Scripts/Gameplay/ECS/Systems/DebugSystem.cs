using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[DisableAutoCreation]
public class DebugSystem : SystemBase {

    protected override void OnUpdate() {

        Entities.ForEach((
            in ViewRangeData viewRange,
            in Translation translation,
            in ActiveStatusData activeStatus) => {
                if (activeStatus.IsActive) {
                    var t = translation.Value;
                    var r = viewRange.Value;
                    Debug.DrawLine(new float3(t.x - r, t.y + r, t.z), new float3(t.x + r, t.y + r, t.z));
                    Debug.DrawLine(new float3(t.x + r, t.y + r, t.z), new float3(t.x + r, t.y - r, t.z));
                    Debug.DrawLine(new float3(t.x + r, t.y - r, t.z), new float3(t.x - r, t.y - r, t.z));
                    Debug.DrawLine(new float3(t.x - r, t.y - r, t.z), new float3(t.x - r, t.y + r, t.z));
                }

            }).Run();
    }
}