using Unity.Entities;

public struct ActiveStatusData : IComponentData {
    public bool IsActive;
    public bool InPool;
}
