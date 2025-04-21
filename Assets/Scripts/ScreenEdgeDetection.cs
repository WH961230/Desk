using UnityEngine;
using UnityEngine.Events;

public class ScreenEdgeDetection : MonoBehaviour {
    public float edgeThreshold = 0.05f; // 屏幕边缘阈值(百分比)
    public UnityEvent TopEdgeEnterEvent;
    public UnityEvent TopEdgeExitEvent;
    public static ScreenEdgeDetection Instance;
    public bool IsTopEdge;
    private bool IsTopEdgeLock;
    private float delayTime;
    private void Awake() {
        Instance = this;
    }

    void Update() {
        Vector2 mousePosition = Input.mousePosition;
        float normalizedY = mousePosition.y / Screen.height;
        IsTopEdge = normalizedY > 1 - edgeThreshold;
        //顶部触发
        if (IsTopEdge) {
            //计时
            if (delayTime > 0) {
                delayTime -= Time.deltaTime;
            } else {
                //计时结束触发
                if (delayTime != 0) {
                    OnEnterTopEdge();
                    delayTime = 1;
                }
            }
        } else {
            OnExitTopEdge();
            delayTime = 1;
        }
    }

    public void OnEnterTopEdge() {
        if (!IsTopEdgeLock) {
            IsTopEdgeLock = true;
            TopEdgeEnterEvent?.Invoke();
        }
    }

    public void OnExitTopEdge() {
        if (IsTopEdgeLock) {
            IsTopEdgeLock = false;
            TopEdgeExitEvent?.Invoke();
        }
    }
}