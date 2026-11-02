using UnityEngine;

public class UICursorFollow : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void OnEnable()
    {
        // 隐藏系统自带的鼠标光标
        Cursor.visible = false;
    }

    // 优化：从 Update 改为 LateUpdate，确保光标跟随丝滑、不卡顿、无撕裂
    void LateUpdate()
    {
        // 如果找不到 Canvas 或 RectTransform，则跳过
        if (canvas == null || rectTransform == null)
            return;

        // 动态获取当前画布（以防切换场景时画布发生变化）
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            return;
        }

        Vector2 mousePosition = Input.mousePosition;

        // 根据 Canvas 的渲染模式计算光标位置
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rectTransform.position = mousePosition;
        }
        else
        {
            Camera cam = canvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mousePosition,
                cam,
                out Vector2 localPoint
            );

            rectTransform.localPosition = localPoint;
        }
    }

    void OnDisable()
    {
        // 当光标组件关闭或切换场景销毁时，恢复显示系统光标
        Cursor.visible = true;
    }
}