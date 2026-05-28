using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// 处理单位选中与否的视觉效果逻辑
/// </summary>
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateBefore(typeof(ResetEventSystem))]
partial struct SelectionVisualSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 查询拥有 Selection 组件的单位
        foreach (var selection in SystemAPI.Query<RefRO<Selection>>().WithPresent<Selection>())
        {
            // 根据 Selection 的事件标志做不同选择
            if (selection.ValueRO.onSelected)
            {
                var visualTrans = SystemAPI.GetComponentRW<LocalTransform>(selection.ValueRO.selectedVisual);
                visualTrans.ValueRW.Scale = selection.ValueRO.scale;
                Debug.Log("onSelected");
            }

            if (selection.ValueRO.onDeSelected)
            {
                var visualTrans = SystemAPI.GetComponentRW<LocalTransform>(selection.ValueRO.selectedVisual);
                visualTrans.ValueRW.Scale = 0f;
                Debug.Log("onDeSelected");
            }
        }
    }
}