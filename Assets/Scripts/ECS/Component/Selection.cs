using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// 选中状态组件
/// </summary>
public struct Selection : IComponentData, IEnableableComponent
{
    // 选中的视觉效果实体
    // Entity 本身就是纯数据，所以 Entity 放进组件没问题
    public Entity selectedVisual;
    
    // 图片缩放-外部设置
    public float scale;
    
    // 模拟 c# 事件的标志-外部设置
    public bool onSelected;
    public bool onDeSelected;
    
}