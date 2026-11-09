using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    [Header("UI 面板引用")]
    public GameObject dialoguePanelObj;

    [Header("物品特写图片")]
    public Sprite itemDetailSprite; // 该物品对应的特写图片

    [Header("多段台词设置")]
    [TextArea(3, 5)]
    public string[] dialogueLines;

    // 记录该物品是否已经被调查过，防止重复点击计数
    private bool hasBeenInteracted = false;

    private void OnMouseDown()
    {
        if (dialoguePanelObj != null && dialogueLines != null && dialogueLines.Length > 0)
        {
            DialogueManager manager = dialoguePanelObj.GetComponent<DialogueManager>();
            if (manager != null)
            {
                // 把台词和特写图一起传给管理器
                manager.ShowDialogue(dialogueLines, itemDetailSprite);
            }

            // 如果该物品还没被调查过
            if (!hasBeenInteracted)
            {
                hasBeenInteracted = true; // 标记为已调查

                // 通知 UI 计数器 +1
                if (HintManager.Instance != null)
                {
                    HintManager.Instance.OnItemFound();
                }
            }
        }
    }
}