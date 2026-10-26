using UnityEngine;

public class UICursorFollow : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void LateUpdate()
    {
        // 游戏运行时强制隐藏系统鼠标
        Cursor.visible = false;

        Vector2 localPoint;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        // 让UI图片跟着鼠标坐标走
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            cam,
            out localPoint
        );

        rectTransform.localPosition = localPoint;
    }

    void OnDisable()
    {
        Cursor.visible = true;
    }
}