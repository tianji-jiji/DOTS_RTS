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
        var query = SystemAPI.QueryBuilder()
            .WithPresent<Selection>()
            .WithAll<LocalTransform>()
            .Build();

        var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
    
        foreach (var entity in entities)
        {
            var selection = SystemAPI.GetComponent<Selection>(entity);
        
            if (selection.onSelected)
            {
                var visualTrans = SystemAPI.GetComponentRW<LocalTransform>(selection.selectedVisual);
                visualTrans.ValueRW.Scale = selection.scale;
            }
            else if (selection.onDeSelected)
            {
                var visualTrans = SystemAPI.GetComponentRW<LocalTransform>(selection.selectedVisual);
                visualTrans.ValueRW.Scale = 0f;
            }
        }
    
        entities.Dispose();
    }
}