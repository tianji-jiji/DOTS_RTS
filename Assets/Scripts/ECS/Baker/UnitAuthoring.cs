using Unity.Entities;
using UnityEngine;

class UnitAuthoring : MonoBehaviour
{
    public Faction faction;
}

class UnitAuthoringBaker : Baker<UnitAuthoring>
{
    public override void Bake(UnitAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new UnitTag
        {
            faction = authoring.faction
        });
    }
}
