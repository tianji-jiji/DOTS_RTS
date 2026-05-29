using Unity.Entities;
using UnityEngine;

class ShooterAuthoring : MonoBehaviour
{
    public float shootInterval;
}

class ShooterAuthoringBaker : Baker<ShooterAuthoring>
{
    public override void Bake(ShooterAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Shooter
        {
            shootInterval = authoring.shootInterval,
        });
    }
}
