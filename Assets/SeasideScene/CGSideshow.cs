using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class CGSideshow : MonoBehaviour, IPointerClickHandler
{
    [System.Serializable]
    public class CGData
    {
        public Sprite cgSprite;
        [TextArea(2, 5)]
        public string[] dialogues;
    }

    [Header("CG 数据列表（一张 CG 对应多段台词）")]
    public CGData[] cgList;

    [Header("UI 组件")]
    public Image targetImage;
    public TextMeshProUGUI dialogueText;

    [Header("场景跳转设置")]
    public string nextSceneName;

    private int currentCGIndex = 0;
    private int currentDialogueIndex = 0;

    void OnEnable()
    {
        currentCGIndex = 0;
        currentDialogueIndex = 0;
        UpdateDisplay();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        NextStep();
    }

    public void NextStep()
    {
        if (cgList == null || cgList.Length == 0) return;

        CGData currentCG = cgList[currentCGIndex];

        // 1. 当前 CG 还有下一段台词
        if (currentCG.dialogues != null && currentDialogueIndex < currentCG.dialogues.Length - 1)
        {
            currentDialogueIndex++;
            UpdateDialogueOnly();
        }
        // 2. 当前 CG 台词播完，切下一张 CG
        else if (currentCGIndex < cgList.Length - 1)
        {
            currentCGIndex++;
            currentDialogueIndex = 0;
            UpdateDisplay();
        }
        // 3. 全部播放完毕，跳转场景
        else
        {
            LoadNextScene();
        }
    }

    private void UpdateDisplay()
    {
        if (cgList == null || cgList.Length == 0) return;

        CGData currentCG = cgList[currentCGIndex];

        if (targetImage != null && currentCG.cgSprite != null)
        {
            targetImage.sprite = currentCG.cgSprite;
            targetImage.preserveAspect = true;
        }

        UpdateDialogueOnly();
    }

    private void UpdateDialogueOnly()
    {
        if (dialogueText == null || cgList == null || cgList.Length == 0) return;

        CGData currentCG = cgList[currentCGIndex];

        if (currentCG.dialogues != null && currentCG.dialogues.Length > 0 && currentDialogueIndex < currentCG.dialogues.Length)
        {
            dialogueText.text = currentCG.dialogues[currentDialogueIndex];
        }
        else
        {
            dialogueText.text = "";
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("未设置下一个场景的名字 (nextSceneName)！");
        }
    }
}