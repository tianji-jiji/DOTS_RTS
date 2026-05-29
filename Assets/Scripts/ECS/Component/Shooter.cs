using Unity.Entities;

/// <summary>
/// 射击组件
/// </summary>
public struct Shooter : IComponentData
{
    // 射击间隔
    public float shootInterval;
    public float timer;
}
