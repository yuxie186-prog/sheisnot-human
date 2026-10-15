using UnityEngine;
using UnityEngine.UI;

public class StoneUIPuzzle : MonoBehaviour
{
    [Header("UI 组件引用")]
    public GameObject puzzleUIPanel; // 拖入 StonePuzzleUI 面板
    public RectTransform drawAreaRect; // 拖入 Panel (Rect Transform)

    [Header("玩家引用与交互距离")]
    public Transform playerTransform;
    public float interactDistance = 10.0f; // 暂时设大一点方便测试

    private GameObject lineObj;
    private LineRenderer lineRenderer;
    private bool isDrawing = false;
    private bool isPuzzleOpen = false;
    private bool isCompleted = false;

    [Header("判定进度")]
    public int targetPointsToWin = 50;
    private int currentPointsCount = 0;

    void Start()
    {
        // 确保游戏开始时解谜 UI 是关闭的
        if (puzzleUIPanel != null)
        {
            puzzleUIPanel.SetActive(false);
        }
    }

    void Update()
    {
        // 谜题打开时的绘画逻辑
        if (isPuzzleOpen && !isCompleted)
        {
            HandleUIDrawing();
        }
    }

    // Unity 自带的 2D 点击事件：只要挂了 BoxCollider2D，点击它就会触发
    private void OnMouseDown()
    {
        if (isCompleted || isPuzzleOpen) return;

        // 检查与玩家的距离
        if (playerTransform != null)
        {
            float dist = Vector2.Distance(playerTransform.position, transform.position);
            if (dist > interactDistance)
            {
                Debug.Log("离石头太远了，走近一点再试试！当前距离: " + dist);
                return;
            }
        }

        OpenPuzzleUI();
    }

    void OpenPuzzleUI()
    {
        isPuzzleOpen = true;
        currentPointsCount = 0;

        if (puzzleUIPanel != null)
        {
            puzzleUIPanel.SetActive(true);
        }

        CreateNewLine();
        Debug.Log("【成功触发】打开石头特写 UI！");
    }

    void CreateNewLine()
    {
        if (lineObj != null) Destroy(lineObj);

        lineObj = new GameObject("PlayerStrokeLine");
        lineObj.transform.SetParent(puzzleUIPanel.transform, false);

        lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 15f;
        lineRenderer.endWidth = 15f;
        lineRenderer.startColor = new Color(0.48f, 0.97f, 1.0f, 0.9f);
        lineRenderer.endColor = new Color(0.48f, 0.97f, 1.0f, 0.9f);
        lineRenderer.positionCount = 0;
        lineRenderer.sortingOrder = 100;
    }

    void HandleUIDrawing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            if (lineRenderer != null) lineRenderer.positionCount = 0;
        }

        if (isDrawing && Input.GetMouseButton(0))
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                drawAreaRect,
                Input.mousePosition,
                null,
                out localPoint
            );

            if (drawAreaRect.rect.Contains(localPoint))
            {
                if (lineRenderer.positionCount == 0 || Vector3.Distance(localPoint, lineRenderer.GetPosition(lineRenderer.positionCount - 1)) > 5f)
                {
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(localPoint.x, localPoint.y, 0f));

                    currentPointsCount++;
                    if (currentPointsCount >= targetPointsToWin)
                    {
                        CompletePuzzle();
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }
    }

    void CompletePuzzle()
    {
        isCompleted = true;
        isPuzzleOpen = false;
        Debug.Log("【解谜成功】“修一，昭和63年8月” 笔迹已点亮！");
        Invoke("CloseUI", 1.0f);
    }

    void CloseUI()
    {
        if (puzzleUIPanel != null)
        {
            puzzleUIPanel.SetActive(false);
        }
    }
}