using Unity.Burst;
using Unity.Entities;
using UnityEngine;

partial struct ShooterSystem : ISystem
{
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 查询实体
        foreach (var (targetEntity,
                     shooter) 
                 in SystemAPI.Query
                 <RefRO<TargetEntity>,
                     RefRW<Shooter>>())
        {
            if (targetEntity.ValueRO.value == Entity.Null)
                continue;
            
            shooter.ValueRW.timer += SystemAPI.Time.DeltaTime;
            if (shooter.ValueRO.timer  >= shooter.ValueRO.shootInterval)
            {
                Debug.Log("射击！！");
                shooter.ValueRW.timer = 0f;
            }
        }
    }
}
