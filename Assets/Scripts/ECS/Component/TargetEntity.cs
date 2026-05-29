using Unity.Entities;

/// <summary>
/// 存储目标实体的组件
/// </summary>
public struct TargetEntity : IComponentData
{
    public Entity value;
}
