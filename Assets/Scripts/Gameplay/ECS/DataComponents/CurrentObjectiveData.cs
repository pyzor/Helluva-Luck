using Unity.Entities;
using Unity.Mathematics;

public struct CurrentObjectiveData : IComponentData {
    //       -1 (Failed)
    // Status 0 (In progress)
    //        1 (Completed)
    public int Status;
    public int ObjectiveID;
    public bool IgnoreTargets;
    public float3 ObjectiveLocation;
    public float3 ObjectiveBounds;
}
