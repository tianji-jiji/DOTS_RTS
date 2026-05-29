using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

/// <summary>
/// 处理 Dots 世界中的单位选择逻辑
/// </summary>
public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; private set; }
    private Vector2 _selectionStartMousePosition;
    public event Action OnSelectionAreaStart;
    public event Action OnSelectionAreaEnd;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
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

            // 1.先把已选中的单位设置为未选中
            var entityQuery =
                new EntityQueryBuilder(Allocator.Temp).WithAll<Selection>(). // 拥有且启用状态
                    Build(entityManager);

            using var entities = entityQuery.ToEntityArray(Allocator.Temp);

            foreach (var entity in entities)
            {
                entityManager.SetComponentEnabled<Selection>(entity, false);
                Selection selection = entityManager.GetComponentData<Selection>(entity);
                
                // onDeSelected 事件触发
                selection.onDeSelected = true;
                selection.onSelected = false;
                entityManager.SetComponentData(entity, selection);
            }


            Rect selectionArea = CalculateSelectionArea();
            float selectionAreaSize = selectionArea.width + selectionArea.height;
            float multipleSelectionAreaSizeMin = 40f;
            bool isMultipleSelection = selectionAreaSize > multipleSelectionAreaSizeMin;

            // 多选逻辑
            if (isMultipleSelection)
            {
                // 2.再把单位设置为已选中
                entityQuery =
                    new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, UnitTag>(). // 拥有且启用状态
                        WithPresent<Selection>(). // 存在即可，不管启用还是禁用
                        Build(entityManager);

                using var entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                using var transformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

                for (int i = 0; i < entityArray.Length; i++)
                {
                    Vector2 unitScreenPoint = _mainCamera.WorldToScreenPoint(transformArray[i].Position);
                    // 单位世界位置转化为屏幕位置
                    // 如果单位屏幕位置在框选区域内
                    if (selectionArea.Contains(unitScreenPoint))
                    {
                        entityManager.SetComponentEnabled<Selection>(entityArray[i], true);
                        Selection selection = entityManager.GetComponentData<Selection>(entityArray[i]);
                        // onSelected 事件触发
                        selection.onSelected = true;
                        selection.onDeSelected = false;
                        entityManager.SetComponentData(entityArray[i], selection);
                    }
                }
            }
            // 单选逻辑
            else
            {
                // 获取物理世界
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                var physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                var collisionWorld = physicsWorldSingleton.CollisionWorld;

                // 从摄像机发出的射线
                Ray rayFromCamera = _mainCamera.ScreenPointToRay(Input.mousePosition);

                // 配置碰撞过滤器
              
                RaycastInput raycastInput = new RaycastInput
                {
                    Start = rayFromCamera.origin, //射线起点
                    End = rayFromCamera.GetPoint(9999f), //射线终点（沿方向延伸 9999 米）
                    Filter = new CollisionFilter //碰撞过滤器，决定射线能打到什么
                    {
                        BelongsTo = ~0u, //射线自身属于所有层
                        CollidesWith = 1 << GameAssets.UNIT_LAYER, //只与 Unit 层碰撞
                        GroupIndex = 0, //不使用组过滤
                    }
                };

                // 执行射线检测
                if (collisionWorld.CastRay(raycastInput, out RaycastHit raycastHit))
                {
                    // 这个 Entity 是单位实体并且可以选择
                    if (entityManager.HasComponent<UnitTag>(raycastHit.Entity) &&
                        entityManager.HasComponent<Selection>(raycastHit.Entity))
                    {
                        entityManager.SetComponentEnabled<Selection>(raycastHit.Entity, true);
                        var selection = entityManager.GetComponentData<Selection>(raycastHit.Entity);

                        // onSelected 事件触发
                        selection.onSelected = true;
                        selection.onDeSelected = false;
                        entityManager.SetComponentData(raycastHit.Entity, selection);
                    }
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
        using var entityQuery =
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
        using var movePositionArray = GenerateMovePositionArray(worldPosition, entities.Length);

        // 4.遍历 Entity 句柄
        for (int i = 0; i < entities.Length; i++)
        {
            var movement = entityManager.GetComponentData<UnitMovement>(entities[i]);
            movement.targetPosition = movePositionArray[i];
            entityManager.SetComponentData(entities[i], movement);
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

    // 生成移动位置数组，让单位处于不同的位置
    private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
        if (positionCount == 0)
        {
            return positionArray;
        }

        positionArray[0] = targetPosition;
        if (positionCount == 1)
        {
            return positionArray;
        }

        float ringSize = 2.2f;
        int ring = 0;
        int positionIndex = 1;

        while (positionIndex < positionCount)
        {
            int ringPositionCount = 3 + ring * 2;

            for (int i = 0; i < ringPositionCount; i++)
            {
                float angle = i * (math.PI2 / ringPositionCount);
                float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1), 0, 0));
                float3 ringPosition = targetPosition + ringVector;

                positionArray[positionIndex] = ringPosition;
                positionIndex++;

                if (positionIndex >= positionCount)
                {
                    break;
                }
            }

            ring++;
        }

        return positionArray;
    }
}