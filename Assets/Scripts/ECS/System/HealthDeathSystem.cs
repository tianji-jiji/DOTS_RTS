using Unity.Burst;
using Unity.Entities;

partial struct HealthDeathSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (health, entity) in SystemAPI.Query<RefRW<Health>>().WithEntityAccess())
        {
            if (health.ValueRO.hp > 0)
                continue;

            ecb.DestroyEntity(entity);
        }
    }
}
