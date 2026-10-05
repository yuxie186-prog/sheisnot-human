using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    [Header("这块拼图正确的目标位置（拖入对应的槽位UI）")]
    public RectTransform targetSlot;

    [Header("吸附允许的最大距离误差（像素）")]
    public float snapThreshold = 50f;

    [HideInInspector]
    public bool isCorrectlyPlaced = false;

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
        if (isCorrectlyPlaced) return;

        Debug.Log(gameObject.name + " 开始被拖拽");
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isCorrectlyPlaced) return;

        rectTransform.anchoredPosition += eventData.delta;

        // 限制坐标不超出屏幕边缘
        float clampedX = Mathf.Clamp(rectTransform.anchoredPosition.x, -750f, 750f);
        float clampedY = Mathf.Clamp(rectTransform.anchoredPosition.y, -450f, 450f);
        rectTransform.anchoredPosition = new Vector2(clampedX, clampedY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isCorrectlyPlaced) return;

        Debug.Log(gameObject.name + " 松开鼠标了");
        canvasGroup.blocksRaycasts = true;

        if (targetSlot != null)
        {
            float distance = Vector2.Distance(rectTransform.anchoredPosition, targetSlot.anchoredPosition);
            Debug.Log(gameObject.name + " 距离目标 Slot 的距离是: " + distance + " （当前吸附阈值是: " + snapThreshold + "）");

            if (distance < snapThreshold)
            {
                Debug.Log(gameObject.name + " 距离达标，成功吸附！");
                rectTransform.anchoredPosition = targetSlot.anchoredPosition;
                isCorrectlyPlaced = true;
                canvasGroup.interactable = false;

                PuzzleManager manager = FindObjectOfType<PuzzleManager>();
                if (manager != null)
                {
                    manager.CheckAllPieces();
                }
            }
            else
            {
                Debug.Log(gameObject.name + " 距离太远了，没有达到吸附标准。");
            }
        }
        else
        {
            Debug.LogWarning(gameObject.name + " 没有绑定 Target Slot！");
        }
    }
}