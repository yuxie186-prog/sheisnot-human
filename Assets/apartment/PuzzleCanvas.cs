using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false; // 拖拽时穿透射线，避免阻挡判定
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;

        // 限制坐标不超出屏幕边缘（防止拖丢）
        float clampedX = Mathf.Clamp(rectTransform.anchoredPosition.x, -750f, 750f);
        float clampedY = Mathf.Clamp(rectTransform.anchoredPosition.y, -450f, 450f);
        rectTransform.anchoredPosition = new Vector2(clampedX, clampedY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        // 后续可在此处添加放置到目标槽位的判定逻辑
    }
}