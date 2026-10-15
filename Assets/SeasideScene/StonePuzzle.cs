using UnityEngine;

public class StonePuzzle : MonoBehaviour
{
    [Header("UI 弹窗或特写面板（可选）")]
    public GameObject puzzleUIPanel; // 如果你想点击石头后弹出一个全屏特写UI，可以把UI面板拖到这里

    [Header("描摹判定设置")]
    public Transform playerTransform; // 主角引用
    public float interactDistance = 2.0f; // 交互距离
    private bool isPuzzleActive = false;
    private bool isCompleted = false;

    // 绘画轨迹相关
    private LineRenderer lineRenderer;
    private bool isDrawing = false;

    [Header("判定目标（可简化为检测点数）")]
    public int totalPointsToDraw = 10; // 模拟需要画出的点数
    private int currentPointsDrawn = 0;

    void Start()
    {
        // 自动获取或添加 LineRenderer 组件（用于玩家画出蓝光轨迹）
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // 设置画笔样式
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(0.48f, 0.97f, 1.0f, 0.8f); // 冰蓝色光芒
        lineRenderer.endColor = new Color(0.48f, 0.97f, 1.0f, 0.8f);

        if (puzzleUIPanel != null)
        {
            puzzleUIPanel.SetActive(false);
        }
    }

    void Update()
    {
        // 1. 玩家靠近石头并点击交互
        if (!isCompleted && Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // 检查距离
                if (playerTransform == null || Vector2.Distance(playerTransform.position, transform.position) <= interactDistance)
                {
                    OpenPuzzle();
                }
            }
        }

        // 2. 谜题激活时的绘画逻辑
        if (isPuzzleActive && !isCompleted)
        {
            DrawingLogic();
        }
    }

    void OpenPuzzle()
    {
        isPuzzleActive = true;
        Debug.Log("已触发石头刻字谜题：开始描摹！");

        if (puzzleUIPanel != null)
        {
            puzzleUIPanel.SetActive(true);
        }
    }

    void DrawingLogic()
    {
        // 按下鼠标左键开始画线
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            lineRenderer.positionCount = 0;
        }

        // 持续滑动鼠标画线
        if (isDrawing && Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f; // 保持在2D平面

            // 限制只能在石头附近/上方画线
            if (lineRenderer.positionCount == 0 || Vector3.Distance(mousePos, lineRenderer.GetPosition(lineRenderer.positionCount - 1)) > 0.05f)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, mousePos);

                // 模拟判定进度累加
                currentPointsDrawn++;
                if (currentPointsDrawn >= totalPointsToDraw * 15) // 简单以轨迹长度/点数作为判定
                {
                    CompletePuzzle();
                }
            }
        }

        // 松开鼠标
        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }
    }

    void CompletePuzzle()
    {
        isCompleted = true;
        isPuzzleActive = false;
        Debug.Log("【谜题成功】修一，昭和63年8月 —— 记忆被唤醒！");

        // 触发后续演出：比如开启强烈的发光、触发海面涟漪、推进剧情
        // 这里可以写你自己的演出函数

        if (puzzleUIPanel != null)
        {
            puzzleUIPanel.SetActive(false);
        }
    }
}