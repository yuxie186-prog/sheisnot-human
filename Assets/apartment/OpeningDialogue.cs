using UnityEngine;
using UnityEngine.UI;

public class OpeningDialogue : MonoBehaviour
{
    public Text dialogueText; // 关联 UI Text 文本组件

    // 使用字符串数组存储每一段/每一句对话
    private string[] dialogueLines = new string[]
    {
        "ドアを開け、薄暗いリビングに入った。",
    "いつもならいるはずの、彼女の姿はない。",
    "テーブルの上には、水に濡れた一幅の絵。",
    "……彼女は、どこへ行った？"
    };

    private int currentIndex = 0; // 记录当前播放到第几句

    void Start()
    {
        ShowCurrentLine();
    }

    void Update()
    {
        // 点击鼠标左键 或 按下空格键/Enter键 切换下一句
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            NextLine();
        }
    }

    // 显示当前句子
    void ShowCurrentLine()
    {
        if (dialogueText != null && currentIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentIndex];
        }
    }

    // 推进到下一句，如果播完则关闭/隐藏对话框
    void NextLine()
    {
        currentIndex++;

        if (currentIndex < dialogueLines.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            // 对话播放完毕，隐藏整个对话框面板（或挂载此脚本的 GameObject）
            gameObject.SetActive(false);
        }
    }
}