using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

class TargetSearchConfigAuthoring : MonoBehaviour
{
    [FormerlySerializedAs("range")] public float radius;
    [FormerlySerializedAs("targetFaction")] public Faction factionToSearch;
    public float searchInterval;
}

class FindTargetAuthoringBaker : Baker<TargetSearchConfigAuthoring>
{
    public override void Bake(TargetSearchConfigAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity,new TargetSearchConfig
        {
            radius = authoring.radius,
            factionToSearch = authoring.factionToSearch,
            searchInterval = authoring.searchInterval,
        });
    }
}
