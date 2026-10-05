using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    // 单例模式：让其他脚本能用 UIManager.Instance 轻易调用
    public static UIManager Instance { get; private set; }

    [Header("UI 元素绑定")]
    public GameObject subtitlePanel; // 拖入场景中的字幕底框物体
    public TextMeshProUGUI subtitleText; // 拖入场景中的 TextMeshPro 文本组件

    // 记录当前正在互动的对象，方便点击时通知它
    private InteractiveObject currentActiveInteraction;
    private bool isSubtitleShowing = false;

    private void Awake()
    {
        // 确保单例唯一，并防止在切换场景或重复加载时报错
        if (Instance == null)
        {
            Instance = this;
            // 如果你想 UI 在场景切换时不被销毁，可以取消下面这行的注释
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 当旁白正在显示时，检测玩家是否点击了鼠标左键
        if (isSubtitleShowing && Input.GetMouseButtonDown(0))
        {
            OnScreenClicked();
        }
    }

    // 显示字幕的方法（增加了关联当前交互对象）
    public void ShowSubtitle(string text, InteractiveObject interObj)
    {
        currentActiveInteraction = interObj;
        isSubtitleShowing = true;

        if (subtitlePanel == null || subtitleText == null)
        {
            Debug.LogWarning("UIManager: UI 元素未绑定，请在 Inspector 中检查赋值！");
            return;
        }

        subtitlePanel.SetActive(true); // 显示底框
        subtitleText.text = text;      // 替换文字
    }

    // 隐藏字幕的方法
    public void Hidesubtitle()
    {
        isSubtitleShowing = false;
        currentActiveInteraction = null;

        if (subtitlePanel != null)
        {
            subtitlePanel.SetActive(false); // 隐藏底框
        }

        if (subtitleText != null)
        {
            subtitleText.text = ""; // 清空文字
        }
    }

    // 当玩家在旁白显示时点击屏幕
    void OnScreenClicked()
    {
        if (currentActiveInteraction != null)
        {
            currentActiveInteraction.OnDialogueClicked();
        }
    }
}