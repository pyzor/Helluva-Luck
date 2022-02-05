using Unity.Entities;
using Unity.Mathematics;

public struct ZoneData : IComponentData {
    public int TeamID;
    public float3 Position;
    public float2 RectBounds;
}
