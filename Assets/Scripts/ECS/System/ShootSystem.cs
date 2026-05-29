using Unity.Burst;
using Unity.Entities;
using UnityEngine;

partial struct ShootSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 查询士兵实体
        foreach (var (attackTarget, shooter) 
                 in SystemAPI.Query<RefRO<AttackTarget>, RefRW<Shooter>>())
        {
            if (attackTarget.ValueRO.targetEntity == Entity.Null)
                continue;

            shooter.ValueRW.timer += SystemAPI.Time.DeltaTime;
            // 士兵发现目标
            if (shooter.ValueRO.timer >= shooter.ValueRO.shootInterval)
            {
                // 存在性检查
                if (!SystemAPI.HasComponent<Health>(attackTarget.ValueRO.targetEntity))
                    continue;
                // 获取目标身上的健康组件
                var health = SystemAPI.GetComponentRW<Health>(attackTarget.ValueRO.targetEntity);
                health.ValueRW.hp -= 5;
                shooter.ValueRW.timer = 0f;
            }
        }
    }
}