using Unity.Entities;
using UnityEngine;

class SelectionAuthoring : MonoBehaviour
{
    public GameObject selectedVisual;
    public float showScale;
}

class SelectedAuthoringBaker : Baker<SelectionAuthoring>
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
        
        // 初始化 ECS 状态:
        // SelectedData 组件默认未启用
        // 类似 Awake() -> gameObject.SetActive(false);
        
        /*为什么不放 System 里，能不能运行时再统一关闭？
        OnCreate()
        {
            全部设置 false
        }
        理论上可以。但这不符合 ECS 思维，Bake 阶段就应该准备好运行时初始数据
        */
        SetComponentEnabled<Selection>(entity,false);
    }
}
