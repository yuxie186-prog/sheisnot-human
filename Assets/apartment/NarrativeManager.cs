using System;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager Instance;

    [Header("UI 组件引用")]
    public GameObject dialoguePanel;
    public Text jpTextComponent;
    public Text cnTextComponent;

    [Header("对话期间要禁用的脚本(可选,拖入玩家移动脚本)")]
    public MonoBehaviour[] scriptsToDisableDuringDialogue;

    [Header("游戏初始开场白")]
    public bool playOpeningOnStart = true;
    [TextArea(1, 2)] public string[] openingJpLines;
    [TextArea(1, 2)] public string[] openingCnLines;

    private int currentIndex = 0;
    private string[] currentJpBatch;
    private string[] currentCnBatch;
    private bool isPlayingSequence = false;
    private int startFrame = -1;
    private Action onSequenceComplete;

    /// <summary>外部可读:当前是否正在播放对话</summary>
    public bool IsPlaying => isPlayingSequence;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (playOpeningOnStart && openingJpLines != null && openingJpLines.Length > 0)
        {
            StartDialogueSequence(openingJpLines, openingCnLines);
        }
        else
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (!isPlayingSequence) return;

        // 刚开始的那一帧不响应输入,防止触发对话的同一次点击直接翻页
        if (Time.frameCount == startFrame) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            NextLine();
        }
    }

    /// <summary>
    /// 开启一段新的对话序列。onComplete 在整段对话播完(或被强制关闭)时调用。
    /// </summary>
    public void StartDialogueSequence(string[] jpLines, string[] cnLines, Action onComplete = null)
    {
        if (jpLines == null || jpLines.Length == 0) return;

        currentJpBatch = jpLines;
        currentCnBatch = cnLines;
        currentIndex = 0;
        onSequenceComplete = onComplete;
        isPlayingSequence = true;
        startFrame = Time.frameCount;

        SetPlayerScriptsEnabled(false);
        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        string jp = (currentJpBatch != null && currentIndex < currentJpBatch.Length)
            ? currentJpBatch[currentIndex] : "";

        string cn = "";
        if (currentCnBatch != null && currentCnBatch.Length > 0)
        {
            int safeCnIndex = Mathf.Clamp(currentIndex, 0, currentCnBatch.Length - 1);
            cn = currentCnBatch[safeCnIndex];
        }

        if (jpTextComponent != null) jpTextComponent.text = jp;
        if (cnTextComponent != null) cnTextComponent.text = cn;
    }

    void NextLine()
    {
        currentIndex++;

        if (currentJpBatch != null && currentIndex < currentJpBatch.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            HideDialogue();
        }
    }

    public void HideDialogue()
    {
        bool wasPlaying = isPlayingSequence;
        Action callback = onSequenceComplete;

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        isPlayingSequence = false;
        currentJpBatch = null;
        currentCnBatch = null;
        onSequenceComplete = null;

        SetPlayerScriptsEnabled(true);

        // 只有真正播放过的对话结束时才回调
        if (wasPlaying && callback != null) callback.Invoke();
    }

    void SetPlayerScriptsEnabled(bool enabled)
    {
        if (scriptsToDisableDuringDialogue == null) return;
        foreach (var s in scriptsToDisableDuringDialogue)
        {
            if (s != null) s.enabled = enabled;
        }
    }
}