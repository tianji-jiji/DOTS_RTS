using Unity.Entities;
using UnityEngine;

class TargetEntityAuthoring : MonoBehaviour
{
}

class TargetEntityAuthoringBaker : Baker<TargetEntityAuthoring>
{
    public override void Bake(TargetEntityAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new TargetEntity());
    }
}
