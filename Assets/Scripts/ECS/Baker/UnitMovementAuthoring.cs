using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class UnitMovementAuthoring : MonoBehaviour
{
    public float rotateSpeed;
    public float moveSpeed;
}

class MoveAuthoringBaker : Baker<UnitMovementAuthoring>
{
    public override void Bake(UnitMovementAuthoring authoring)
    {
        // 获取当前 authoring 依附的 GameObject 转化后的 entity
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        // 添加组件
        AddComponent(entity, new UnitMovement
        {
            moveSpeed = authoring.moveSpeed,
            rotateSpeed = authoring.rotateSpeed,
        });
    }
}