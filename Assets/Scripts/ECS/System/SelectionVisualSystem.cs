using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

/// <summary>
/// 处理单位选中与否的视觉效果逻辑
/// </summary>
partial struct SelectionVisualSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 查询 SelectedData 被禁用的单位
        foreach (var selectedData in SystemAPI.Query<RefRO<Selection>>().WithDisabled<Selection>())
        {
            var visualTrans = SystemAPI.GetComponentRW<LocalTransform>(selectedData.ValueRO.selectedVisual);
            visualTrans.ValueRW.Scale = 0f;
        }
        
        foreach (var selectedData in SystemAPI.Query<RefRO<Selection>>())
        {
            var visualTrans = SystemAPI.GetComponentRW<LocalTransform>(selectedData.ValueRO.selectedVisual);
            visualTrans.ValueRW.Scale = selectedData.ValueRO.scale;
        }
    }
}