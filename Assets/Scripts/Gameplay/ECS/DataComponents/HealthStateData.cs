using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct HealthStateData : IComponentData {
    public float MaxHealthPoints;
    public float CurrentHealthPoints;
}
