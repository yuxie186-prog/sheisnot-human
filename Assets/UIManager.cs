using UnityEngine;
using TMPro; // 引入TextMeshPro命名空间

public class UIManager : MonoBehaviour
{
    // 单例模式：让其他脚本能用 UIManager.Instance 轻易调用它
    public static UIManager Instance { get; private set; }

    [Header("UI 元素绑定")]
    public GameObject subtitlePanel; // 字幕底框
    public TextMeshProUGUI subtitleText; // 字幕文本

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 显示字幕的方法
    public void ShowSubtitle(string text)
    {
        subtitlePanel.SetActive(true);  // 显示底框
        subtitleText.text = text;       // 替换文字
    }

    // 隐藏字幕的方法
    public void HideSubtitle()
    {
        subtitlePanel.SetActive(false); // 隐藏底框
        subtitleText.text = "";
    }
}
