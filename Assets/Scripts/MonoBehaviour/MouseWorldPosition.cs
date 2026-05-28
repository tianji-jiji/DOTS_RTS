using System;
using UnityEngine;

/// <summary>
/// 将鼠标点击位置转化为世界坐标
/// </summary>
public class MouseWorldPosition : MonoBehaviour
{
    public static MouseWorldPosition Instance { get; private set; }
    private Camera _camera;
    private void Awake()
    {
        _camera = Camera.main;
        Instance = this;
    }

    public Vector3 GetWorldPosition()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        
        return Physics.Raycast(ray, out RaycastHit hit) ? hit.point : Vector3.zero;
    }
}