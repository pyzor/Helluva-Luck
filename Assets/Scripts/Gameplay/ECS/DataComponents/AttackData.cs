using Unity.Entities;

public struct AttackData : IComponentData {
    public float Damage;
    public float AttackRange;
    public float AttacksPerSecond;
    // Delay Before Attack Counts As A Hit
    // (ex. first 2 animation frames to prepare, then attack frame + 1 frame = 4 frames total, with attack on 3rd frame)
    // (AttackDelay = 2f / 4f = 0,5f)
    public float AttackDelay;
    public float AttackDelta;
    public bool Attacking;
    public bool HitTarget;
}
