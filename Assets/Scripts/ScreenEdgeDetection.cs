using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScreenEdgeDetection : MonoBehaviour {
    public float edgeThreshold = 0.05f; // 屏幕边缘阈值(百分比)
    public List<UnityEvent> TopEdgeEnterEvents;
    public List<UnityEvent> TopEdgeExitEvents;
    public static ScreenEdgeDetection Instance;
    public bool IsTopEdge;
    private bool IsTopEdgeLock;
    public List<UnityEvent> TopLeftEdgeEnterEvents;
    public List<UnityEvent> TopLeftEdgeExitEvents;
    public bool IsTopLeftEdge;
    private bool IsTopLeftEdgeLock;
    public bool IsLeftEdge;
    private void Awake() {
        Instance = this;
    }

    void Update() {
        // 获取鼠标在屏幕空间的位置(0-1)
        Vector2 mousePosition = Input.mousePosition;
        float normalizedX = mousePosition.x / Screen.width;
        float normalizedY = mousePosition.y / Screen.height;

        IsLeftEdge = normalizedX < edgeThreshold;
        IsTopEdge = normalizedY > 1 - edgeThreshold;
        IsTopLeftEdge = IsTopEdge && IsLeftEdge;
        
        // 检测左边缘
        if (normalizedX < edgeThreshold) {
            Debug.Log("鼠标移动到左边缘");
            // 触发左边缘事件
            OnLeftEdge();
        }

        // 检测右边缘
        if (normalizedX > 1 - edgeThreshold) {
            Debug.Log("鼠标移动到右边缘");
            // 触发右边缘事件
            OnRightEdge();
        }

        // 检测上边缘
        if (IsTopEdge) {
            OnEnterTopEdge();
        }

        if (IsTopLeftEdge && !IsTopLeftEdgeLock) {
            IsTopLeftEdgeLock = true;
            OnEnterTopLeftEdge();
        }

        // 检测下边缘
        if (normalizedY < edgeThreshold) {
            Debug.Log("鼠标移动到下边缘");
            // 触发下边缘事件
            OnBottomEdge();
        }
    }

    void OnLeftEdge() {
        // 左边缘触发逻辑
    }

    void OnRightEdge() {
        // 右边缘触发逻辑
    }

    public void OnEnterTopEdge() {
        if (!IsTopEdgeLock) {
            IsTopEdgeLock = true;
            // 上边缘触发逻辑
            if (TopEdgeEnterEvents != null && TopEdgeEnterEvents.Count > 0) {
                foreach (var tmp in TopEdgeEnterEvents) {
                    tmp.Invoke();
                }
            }
            
            Invoke("OnExitTopEdge", 1);
        }
    }
    
    public void OnExitTopEdge() {
        if (IsTopEdgeLock) {
            IsTopEdgeLock = false;
            // 上边缘触发逻辑
            if (TopEdgeExitEvents != null && TopEdgeExitEvents.Count > 0) {
                foreach (var tmp in TopEdgeExitEvents) {
                    tmp.Invoke();
                }
            }
        }
    }

    void OnEnterTopLeftEdge() {
        // 上边缘触发逻辑
        if (TopLeftEdgeEnterEvents != null && TopLeftEdgeEnterEvents.Count > 0) {
            foreach (var tmp in TopLeftEdgeEnterEvents) {
                tmp.Invoke();
            }
        }
    }

    public void OnExitTopLeftEdge() {
        IsTopLeftEdgeLock = false;
    }

    void OnBottomEdge() {
        // 下边缘触发逻辑
    }
}