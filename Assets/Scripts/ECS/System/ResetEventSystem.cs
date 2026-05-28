using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup))] 
// LateSimulationSystemGroup 类比 MonoBehaviour 的 LateUpdate()，在同帧所有主要逻辑执行完之后才运行
// 适合放视觉效果处理、事件标志重置
partial struct ResetEventSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        using var entities = SystemAPI.QueryBuilder()
            .WithPresent<Selection>()
            .Build()
            .ToEntityArray(Unity.Collections.Allocator.Temp);
        
        foreach (var entity in entities)
        {
            var selection = SystemAPI.GetComponentRW<Selection>(entity);
            selection.ValueRW.onSelected = false;
            selection.ValueRW.onDeSelected = false;
        }
    }
}
