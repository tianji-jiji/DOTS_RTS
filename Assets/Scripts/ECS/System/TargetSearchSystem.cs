using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct TargetSearchSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var collisionWorld = physicsWorldSingleton.CollisionWorld;
        NativeList<DistanceHit> outHits = new NativeList<DistanceHit>(16, Allocator.Temp);
        // 设置碰撞过滤器
        CollisionFilter filter = new CollisionFilter //碰撞过滤器，决定射线能打到什么
        {
            BelongsTo = ~0u,
            CollidesWith = 1 << GameAssets.UNIT_LAYER, //只与 Unit 层碰撞
            GroupIndex = 0,
        };

        // 查询士兵和僵尸
        foreach (var (localTransform, 
                     targetSearchConfig, 
                     targetEntity)
                 in SystemAPI.Query<RefRO<LocalTransform>, RefRW<TargetSearchConfig>, RefRW<AttackTarget>>())
        {
            targetSearchConfig.ValueRW.timer += SystemAPI.Time.DeltaTime;
            
            // 每隔一段时间进行范围检测
            if (targetSearchConfig.ValueRO.timer >= targetSearchConfig.ValueRO.searchInterval)
            {
                targetSearchConfig.ValueRW.timer = 0f;
                outHits.Clear();
                Debug.Log("范围检测");
                bool hasUnit = collisionWorld.OverlapSphere(
                    localTransform.ValueRO.Position,
                    targetSearchConfig.ValueRO.radius,
                    ref outHits, // 结果写入这里
                    filter);

                if (hasUnit)
                {
                    foreach (var hit in outHits)
                    {
                        var unitTag = SystemAPI.GetComponent<UnitTag>(hit.Entity);
                        // 要找的目标和检测出来的目标一致
                        if (targetSearchConfig.ValueRO.factionToSearch == unitTag.faction)
                        {
                            targetEntity.ValueRW.targetEntity = hit.Entity;
                            break;
                        }
                    }
                }
            }
            
            
        }
    }
}