using Unity.Entities;
using UnityEngine;

class SelectionAuthoring : MonoBehaviour
{
    public GameObject selectedVisual;
    public float showScale;
}

class SelectionAuthoringBaker : Baker<SelectionAuthoring>
{
    public override void Bake(SelectionAuthoring authoring)
    {
        // 获取当前 authoring 依附的 GameObject 转化后的 entity
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        // 为 entity 添加组件
        AddComponent(entity, new Selection
        {
            // 选中圈是谁
            selectedVisual = GetEntity(authoring.selectedVisual, TransformUsageFlags.Dynamic),
            // 选中圈显示多大
            scale =  authoring.showScale,
        });
        
        SetComponentEnabled<Selection>(entity,false);
    }
}
