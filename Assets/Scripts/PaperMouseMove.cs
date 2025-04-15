using UnityEngine;
using UnityEngine.UI;

public class PaperMouseMove : MonoBehaviour {
    [Header("鼠标移动参数")]
    public float offsetFactor = 0.1f;      // 位置偏移系数
    public float smoothSpeed = 5f;         // 移动平滑度
    public float maxOffsetDistance = 500f; // 最大有效偏移距离（用于计算缩放）
    public float minScale = 0.95f;         // 最小缩放比例（中心位置）
    public float maxScale = 1.05f;         // 最大缩放比例（边缘位置）

    private Image targetImage;
    private Vector2 initialImagePosition;
    private Vector3 initialImageScale;

    void Start() {
        targetImage = GetComponent<Image>();
        if (targetImage != null) {
            initialImagePosition = targetImage.rectTransform.anchoredPosition;
            initialImageScale = targetImage.rectTransform.localScale;
        }
    }

    void Update() {
        if (targetImage == null || targetImage.sprite == null) return;

        // // 计算鼠标偏移
        // Vector2 mousePos = Input.mousePosition;
        // Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        // Vector2 offset = (mousePos - screenCenter) * offsetFactor;
        //
        // // 计算动态缩放（基于偏移距离）
        // float offsetDistance = Vector2.Distance(mousePos, screenCenter);
        // float scaleRatio = Mathf.Clamp01(offsetDistance / maxOffsetDistance);
        // float targetScale = Mathf.Lerp(minScale, maxScale, scaleRatio);
        //
        // // 平滑应用位置和缩放
        // Vector2 targetPos = initialImagePosition + offset;
        // targetImage.rectTransform.anchoredPosition = Vector2.Lerp(
        //     targetImage.rectTransform.anchoredPosition,
        //     targetPos,
        //     smoothSpeed * Time.deltaTime
        // );
        //
        // targetImage.rectTransform.localScale = Vector3.Lerp(
        //     targetImage.rectTransform.localScale,
        //     initialImageScale * targetScale,
        //     smoothSpeed * Time.deltaTime
        // );
    }
}