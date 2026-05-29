using Unity.Entities;

/// <summary>
/// 目标搜索配置组件
/// </summary>
public struct TargetSearchConfig : IComponentData
{
    // 范围
    public float radius;
    // 要寻找的目标派系
    public Faction factionToSearch;
    // 搜索频率
    public float searchInterval;
    public float timer;
}
