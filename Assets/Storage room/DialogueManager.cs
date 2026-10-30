using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour, IPointerClickHandler
{
    [Header("UI 控件引用")]
    public TMP_Text dialogueText;
    public GameObject detailPanelObj;
    public Image detailImageUI;

    private string[] currentLines;
    private int currentIndex = 0;
    private int openFrame = -1; // 记录打开时的帧数

    public void ShowDialogue(string[] lines, Sprite detailSprite)
    {
        currentLines = lines;
        currentIndex = 0;
        openFrame = Time.frameCount; // 记录当前是第几帧被打开的

        // 显示对话面板
        gameObject.SetActive(true);
        UpdateText();

        // 处理特写图
        if (detailPanelObj != null && detailImageUI != null && detailSprite != null)
        {
            detailImageUI.sprite = detailSprite;
            detailPanelObj.SetActive(true);
        }
        else if (detailPanelObj != null)
        {
            detailPanelObj.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 【防误触核心】：如果是打开对话框的那同一帧触发的点击，直接拦截不处理！
        if (Time.frameCount == openFrame) return;

        currentIndex++;

        if (currentLines != null && currentIndex < currentLines.Length)
        {
            UpdateText();
        }
        else
        {
            // 对话全部结束，关闭对话框和特写面板
            gameObject.SetActive(false);
            if (detailPanelObj != null)
            {
                detailPanelObj.SetActive(false);
            }
        }
    }

    private void UpdateText()
    {
        if (currentLines != null && currentIndex < currentLines.Length)
        {
            dialogueText.text = currentLines[currentIndex];
        }
    }
}