using Unity.Entities;
using UnityEngine;

class HealthAuthoring : MonoBehaviour
{
    public int hp;
}

class HealthAuthoringBaker : Baker<HealthAuthoring>
{
    public override void Bake(HealthAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Health
        {
            hp = authoring.hp,
        });
    }
}
