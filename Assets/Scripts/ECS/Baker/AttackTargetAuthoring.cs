using Unity.Entities;
using UnityEngine;

class AttackTargetAuthoring : MonoBehaviour
{
}

class AttackTargetAuthoringBaker : Baker<AttackTargetAuthoring>
{
    public override void Bake(AttackTargetAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new AttackTarget());
    }
}
