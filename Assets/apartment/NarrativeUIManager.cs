using UnityEngine;
using UnityEngine.UI; // 使用传统 UI 命名空间

public class NarrativeUIManager : MonoBehaviour
{
    public static NarrativeUIManager Instance;

    [Header("UI 组件引用")]
    public GameObject dialoguePanel;
    public Text jpTextComponent; // 改为普通的 Text
    public Text cnTextComponent; // 改为普通的 Text

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideDialogue();
    }

    public void ShowDialogue(string japanese, string chinese)
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        if (jpTextComponent != null) jpTextComponent.text = japanese;
        if (cnTextComponent != null) cnTextComponent.text = chinese;
    }

    public void HideDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }
}