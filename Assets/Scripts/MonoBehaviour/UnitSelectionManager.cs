using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// 处理 Dots 世界中的单位选择逻辑
/// </summary>
public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; private set; }
    private Vector2 _selectionStartMousePosition;
    public event Action OnSelectionAreaStart;
    public event Action OnSelectionAreaEnd;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // 按下鼠标左键
        if (Input.GetMouseButtonDown(0))
        {
            _selectionStartMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke();
        }

        //松开鼠标左键
        if (Input.GetMouseButtonUp(0))
        {
            OnSelectionAreaEnd?.Invoke();
            
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            // 先查询已选中的单位，设置为未选中
            var entityQuery =
                new EntityQueryBuilder(Allocator.Temp). 
                    WithAll<Selection>().   // 拥有且启用状态
                    Build(entityManager); 
            
            using var entities = entityQuery.ToEntityArray(Allocator.Temp);

            foreach (var entity in entities)
            {
                entityManager.SetComponentEnabled<Selection>(entity,false);
            }
            
            // 再查询未选中的单位，设置为已选中
            entityQuery =
                new EntityQueryBuilder(Allocator.Temp). 
                    WithAll<LocalTransform, UnitTag>().   // 拥有且启用状态
                    WithPresent<Selection>().             // 存在即可，不管启用还是禁用
                    Build(entityManager); 
            
            using var entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            
            Rect selectionArea = CalculateSelectionArea();
            
            foreach (var entity in entityArray)
            {
                var localTransform = entityManager.GetComponentData<LocalTransform>(entity);
                
                // 单位世界位置转化为屏幕位置
                Vector2 unitScreenPoint = Camera.main.WorldToScreenPoint(localTransform.Position);
                
                // 如果单位屏幕位置在框选区域内
                if (selectionArea.Contains(unitScreenPoint))
                {
                    entityManager.SetComponentEnabled<Selection>(entity,true);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            var worldPosition = MouseWorldPosition.Instance.GetWorldPosition();
            SetUnitsTargetPosition(worldPosition);
        }
    }

    /// <summary>
    /// 给所有单位设置目标位置
    /// </summary>
    /// <param name="worldPosition"></param>
    private void SetUnitsTargetPosition(Vector3 worldPosition)
    {
        // 1.获取 EntityManager
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // 2.建立查询 sql
        var entityQuery =
            new EntityQueryBuilder(Allocator.Temp). // 创建一个查询构建器，用临时内存
                WithAll<UnitMovement, Selection>(). // 条件：必须同时拥有这两个组件
                Build(entityManager); // 交给 EntityManager，生成 EntityQuery

        // 3. 执行查询，返回 Entity 句柄的副本数组（只是 ID，不含组件数据）
        // using 保证作用域结束时自动 Dispose() 这块 Allocator.Temp 内存
        using var entities = entityQuery.ToEntityArray(Allocator.Temp);

        //================================================================
        /*ECS 内部 Archetype 内存
            ┌─────────────────────────────┐
            │ Entity(1,v1) │ Entity(3,v2) │  ← 真正存在 ECS 世界里的
            │ UnitMovement │ UnitMovement │
            │ Selection    │ Selection    │
            └─────────────────────────────┘
                    │ ToEntityArray() 复制句柄
                    ↓
                NativeArray<Entity>
            ┌─────────────────────────────┐
            │ Entity(1,v1) │ Entity(3,v2) │  ← 副本，只是 ID，不含组件数据
            └─────────────────────────────┘*/
        //==================================================================

        // 4.遍历 Entity 句柄
        foreach (var entity in entities)
        {
            // 去 ECS 内存里找到它对应的 UnitMovement 组件，复制一份
            var movement = entityManager.GetComponentData<UnitMovement>(entity);

            //修改副本
            movement.targetPosition = worldPosition;

            // 把修改好的副本写回 ECS 内存，覆盖掉原来的数据
            entityManager.SetComponentData(entity, movement);
        }
    }

    /// <summary>
    /// 计算选择区域数据
    /// </summary>
    /// <returns></returns>
    public Rect CalculateSelectionArea()
    {
        // 不断获取当前鼠标位置
        Vector2 currentMousePosition = Input.mousePosition;
        
        Vector2 lowerLeftCorner = new Vector2(Mathf.Min(_selectionStartMousePosition.x, currentMousePosition.x),
            Mathf.Min(_selectionStartMousePosition.y, currentMousePosition.y));

        Vector2 upperRightCorner = new Vector2(Mathf.Max(_selectionStartMousePosition.x, currentMousePosition.x),
            Mathf.Max(_selectionStartMousePosition.y, currentMousePosition.y));

        return new Rect(lowerLeftCorner.x, lowerLeftCorner.y, upperRightCorner.x - lowerLeftCorner.x,
            upperRightCorner.y - lowerLeftCorner.y);
    }
}