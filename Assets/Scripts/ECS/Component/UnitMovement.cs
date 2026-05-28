using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// 单位移动状态组件
/// </summary>
public struct UnitMovement : IComponentData
{
    public float moveSpeed;
    public float rotateSpeed;
    // 目标位置，不需要同步到 authoring，由其他 mono 脚本设置
    public float3 targetPosition;
}
