using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

/// <summary>
/// 处理单位移动逻辑
/// </summary>
public partial struct UnitMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // 工人
        UnitMovementJob unitMovementJob = new UnitMovementJob
        {
            deltaTime = deltaTime
        };

        // 并行调度 Job
        unitMovementJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct UnitMovementJob : IJobEntity
{
    public float deltaTime;

    // ref 表示可修改，in 只读
    private void Execute(
        ref LocalTransform localTransform,
        ref PhysicsVelocity physicsVelocity,
        in UnitMovement unitMovement)
    {
        float3 targetPosition = unitMovement.targetPosition;
        float moveSpeed = unitMovement.moveSpeed;
        float rotateSpeed = unitMovement.rotateSpeed;

        // 移动方向
        float3 moveDirection =  targetPosition - localTransform.Position;
        float minReachDistance = 2f;
        
        // 距离目的地够近
        if (math.lengthsq(moveDirection) < minReachDistance)
        {
            // 物理速度
            physicsVelocity.Linear =  float3.zero;
            physicsVelocity.Angular =  float3.zero;
            return;
        }
        
        moveDirection = math.normalizesafe(moveDirection);
            

        // 旋转
        localTransform.Rotation =
            math.slerp(
                localTransform.Rotation,
                quaternion.LookRotationSafe(
                    moveDirection,
                    math.up()),
                deltaTime * rotateSpeed);

        // 物理速度
        physicsVelocity.Linear =  moveDirection * moveSpeed;
        physicsVelocity.Angular =  float3.zero;
           
    }
}