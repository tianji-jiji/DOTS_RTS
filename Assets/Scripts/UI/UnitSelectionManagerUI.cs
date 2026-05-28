using System;
using UnityEngine;

public class UnitSelectionManagerUI : MonoBehaviour
{
    [SerializeField] private RectTransform selectionAreaRectTrans;
    [SerializeField] private Canvas canvas;
    private void Start()
    {
        UnitSelectionManager.Instance.OnSelectionAreaStart += OnSelectionAreaStart;
        UnitSelectionManager.Instance.OnSelectionAreaEnd += OnSelectionAreaEnd;
        
        selectionAreaRectTrans.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (selectionAreaRectTrans.gameObject.activeSelf)
        {
            UpdateVisual();
        }
    }

    private void OnSelectionAreaStart()
    {
        selectionAreaRectTrans.gameObject.SetActive(true);
        UpdateVisual();
    }
    
    private void OnSelectionAreaEnd()
    {
        selectionAreaRectTrans.gameObject.SetActive(false);
    }

    private void UpdateVisual()
    {
        Rect selectionArea = UnitSelectionManager.Instance.CalculateSelectionArea();
        float canScale = canvas.transform.localScale.x; 
        selectionAreaRectTrans.anchoredPosition = new Vector2(selectionArea.x, selectionArea.y) / canScale;
        selectionAreaRectTrans.sizeDelta = new Vector2(selectionArea.width, selectionArea.height) / canScale;
    }
}

