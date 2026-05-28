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
    
    // 图片缩放
    public float scale;
}