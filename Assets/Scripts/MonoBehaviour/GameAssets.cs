using System;
using UnityEngine;

/// <summary>
/// 存储游戏通用配置
/// </summary>
public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance { get; private set; }

    public const int UNIT_LAYER = 6;

    private void Awake()
    {
        Instance = this;
    }
}