using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(BoxCollider2D))]
public class ColliderToRectTransform : MonoBehaviour {
    [Header("设置")] [Tooltip("是否在Awake时初始化")]
    public bool initializeOnAwake = true;

    [Tooltip("是否在每帧更新")] public bool updateEveryFrame = false;
    [Tooltip("边缘缩进")] public Vector2 padding = Vector2.zero;

    private RectTransform _rectTransform;
    private BoxCollider2D _boxCollider;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _boxCollider = GetComponent<BoxCollider2D>();

        if (initializeOnAwake) {
            UpdateColliderSize();
        }
    }

    private void Update() {
        if (updateEveryFrame) {
            UpdateColliderSize();
        }
    }

    [ContextMenu("更新碰撞体大小")]
    public void UpdateColliderSize() {
        if (_rectTransform == null || _boxCollider == null)
            return;

        // 获取RectTransform的实际大小（考虑缩放）
        Vector2 size = _rectTransform.rect.size;
        Vector2 scaledSize = new Vector2(
            size.x * _rectTransform.lossyScale.x,
            size.y * _rectTransform.lossyScale.y
        );

        // 应用缩进
        scaledSize -= padding * 2f;

        // 设置BoxCollider2D大小
        _boxCollider.size = scaledSize;

        // 调整偏移量确保中心对齐
        _boxCollider.offset = Vector2.zero;
    }

    // 当RectTransform尺寸变化时自动更新（需要UI组件）
    private void OnRectTransformDimensionsChange() {
        if (!updateEveryFrame) {
            UpdateColliderSize();
        }
    }
}