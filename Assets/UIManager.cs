using UnityEngine;
using TMPro; // 引入TextMeshPro命名空间

public class UIManager : MonoBehaviour
{
    // 单例模式：让其他脚本能用 UIManager.Instance 轻易调用
    public static UIManager Instance { get; private set; }

    [Header("UI 元素绑定")]
    public GameObject subtitlePanel; // 拖入场景中的字幕底框物体
    public TextMeshProUGUI subtitleText; // 拖入场景中的 TextMeshPro 文本组件

    private void Awake()
    {
        // 确保单例唯一性，并防止在切换场景或重复加载时报错
        if (Instance == null)
        {
            Instance = this;
            // 如果你想让 UI 在场景切换时不被销毁，可以取消下面这行的注释
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 显示字幕的方法
    public void ShowSubtitle(string text)
    {
        if (subtitlePanel == null || subtitleText == null)
        {
            Debug.LogWarning("UIManager: UI 元素未绑定，请在 Inspector 中检查赋值！");
            return;
        }

        subtitlePanel.SetActive(true); // 显示底框
        subtitleText.text = text;      // 替换文字
    }

    // 隐藏字幕的方法
    public void HideSubtitle()
    {
        if (subtitlePanel != null)
        {
            subtitlePanel.SetActive(false); // 隐藏底框
        }

        if (subtitleText != null)
        {
            subtitleText.text = ""; // 清空文字
        }
    }
}
