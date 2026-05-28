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
        foreach (var selection in SystemAPI.Query<RefRW<Selection>>().WithPresent<Selection>())
        {
            selection.ValueRW.onSelected = false;
            selection.ValueRW.onDeSelected = false;
        }
    }
}
