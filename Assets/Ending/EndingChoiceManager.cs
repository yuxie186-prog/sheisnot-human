using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果使用 TextMeshPro 命名空间
using UnityEngine.SceneManagement;

public class EndingChoiceManager : MonoBehaviour
{
    [Header("触发点击的目标对象 (须挂载Collider2D)")]
    public Transform seaTarget;
    public Transform sheTarget;

    // 定义“一张CG对应多段台词”的数据结构
    [System.Serializable]
    public class EndingCGData
    {
        public Sprite cgSprite;          // 结局CG图片
        [TextArea(2, 5)]
        public string[] dialogues;       // 该CG对应的多段台词字幕数组
    }

    [Header("结局 A 数据列表（海浪结局）")]
    public EndingCGData[] endingAList;

    [Header("结局 B 数据列表（女人结局）")]
    public EndingCGData[] endingBList;

    [Header("UI 渲染组件")]
    public Image cgDisplayImage;         // 用于显示CG的UI Image组件
    public TextMeshProUGUI dialogueText; // 用于显示多段台词的文本组件
    public GameObject blackPanel;        // 纯黑背景Panel (用来防穿帮/加黑边)

    // 定义状态枚举
    private enum EndingState { Choosing, PlayingEndingA, PlayingEndingB }
    private EndingState currentState = EndingState.Choosing;

    private EndingCGData[] currentEndingList;
    private int currentCGIndex = 0;
    private int currentDialogueIndex = 0;

    void Start()
    {
        // 游戏一开始，自动把 CG 显示组件和黑色面板隐藏掉
        if (cgDisplayImage != null)
        {
            cgDisplayImage.gameObject.SetActive(false);
            cgDisplayImage.preserveAspect = true; // 防拉伸
        }

        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
        }

        if (blackPanel != null)
        {
            blackPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentState == EndingState.Choosing)
            {
                DetectClick();
            }
            else if (currentState == EndingState.PlayingEndingA || currentState == EndingState.PlayingEndingB)
            {
                // 点击推进下一步（优先推进台词，台词完了切下一张CG）
                ShowNextStep();
            }
        }
    }

    void DetectClick()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            Transform hitTransform = hit.transform;

            if (hitTransform == seaTarget || hitTransform.IsChildOf(seaTarget))
            {
                StartEnding(EndingState.PlayingEndingA, endingAList);
            }
            else if (hitTransform == sheTarget || hitTransform.IsChildOf(sheTarget))
            {
                StartEnding(EndingState.PlayingEndingB, endingBList);
            }
        }
    }

    void StartEnding(EndingState targetState, EndingCGData[] endingData)
    {
        if (endingData == null || endingData.Length == 0)
        {
            Debug.LogWarning("该结局没有配置图片或台词数据！");
            return;
        }

        currentState = targetState;
        currentEndingList = endingData;
        currentCGIndex = 0;
        currentDialogueIndex = 0;

        // 1. 激活纯黑面板
        if (blackPanel != null)
        {
            blackPanel.SetActive(true);
        }

        // 2. 激活CG展示面板和字幕组件
        if (cgDisplayImage != null)
        {
            cgDisplayImage.gameObject.SetActive(true);
        }
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(true);
        }

        // 3. 更新并显示第一张CG和第一句台词
        UpdateDisplay();

        Debug.Log($"进入结局，开始播放，共 {currentEndingList.Length} 组CG数据");
    }

    // 核心推进逻辑：点一下，先换台词；当前CG台词完了，才换下一张图
    void ShowNextStep()
    {
        if (currentEndingList == null || currentEndingList.Length == 0) return;

        EndingCGData currentCG = currentEndingList[currentCGIndex];

        // 1. 如果当前 CG 还有下一段台词，则只切换台词
        if (currentCG.dialogues != null && currentDialogueIndex < currentCG.dialogues.Length - 1)
        {
            currentDialogueIndex++;
            UpdateDialogueOnly();
        }
        // 2. 如果当前 CG 的台词已经播完，但还有下一张 CG 图片，则切换到下一张 CG
        else if (currentCGIndex < currentEndingList.Length - 1)
        {
            currentCGIndex++;
            currentDialogueIndex = 0; // 重置台词索引
            UpdateDisplay();          // 刷新图片和第一句台词
        }
        // 3. 所有 CG 和所有台词都播放完毕，结束结局
        else
        {
            OnEndingFinished();
        }
    }

    // 更新画面（更换 CG 图片 + 刷新当前台词）
    void UpdateDisplay()
    {
        EndingCGData currentCG = currentEndingList[currentCGIndex];

        if (cgDisplayImage != null && currentCG.cgSprite != null)
        {
            cgDisplayImage.sprite = currentCG.cgSprite;
            cgDisplayImage.preserveAspect = true;
        }

        UpdateDialogueOnly();
        Debug.Log($"切换到第 {currentCGIndex + 1} 张CG");
    }

    // 仅刷新文本内容
    void UpdateDialogueOnly()
    {
        if (dialogueText == null) return;

        EndingCGData currentCG = currentEndingList[currentCGIndex];

        if (currentCG.dialogues != null && currentCG.dialogues.Length > 0 && currentDialogueIndex < currentCG.dialogues.Length)
        {
            dialogueText.text = currentCG.dialogues[currentDialogueIndex];
        }
        else
        {
            dialogueText.text = ""; // 如果没填台词则清空
        }
    }

    void OnEndingFinished()
    {
        Debug.Log("结局CG与台词播放完毕");

        // 取消注释或直接写入这行，切回标题界面
        SceneManager.LoadScene("TitleScene");

        this.enabled = false;
    }
}