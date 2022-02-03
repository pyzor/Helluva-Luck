using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct VelocityData : IComponentData {

    public float3 direction;
    public float currentSpeed;
    public float speed;
    public float multiplier;
}
