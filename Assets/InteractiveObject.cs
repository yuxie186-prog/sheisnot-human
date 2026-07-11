using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [Header("线索名称")]
    public string clueName;
    [Header("触发后显示的字幕内容")]
    [TextArea(3, 5)] // 让输入框在编辑器里变成多行，方便写小作文
    public string clueDescription;

    private bool isPlayerInArea;
    private bool isShowingText;

    void Update()
    {
        // 玩家在范围内，按下 E 键
        if (isPlayerInArea && Input.GetKeyDown(KeyCode.E))
        {
            if (!isShowingText)
            {
                // 如果当前没显示字幕，就显示
                UIManager.Instance.ShowSubtitle(clueDescription);
                isShowingText = true;
            }
            else
            {
                // 如果已经显示了，再按一次 E 就关闭字幕
                UIManager.Instance.HideSubtitle();
                isShowingText = false;
            }
        }
    }

    // 玩家进入范围
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInArea = true;
            // 可选：进入范围时可以在控制台提示一下
            Debug.Log($"走进 {clueName}，按下 [E] 检查");
        }
    }

    // 玩家离开范围
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInArea = false;
            // 玩家强行离开时，自动关闭字幕
            if (isShowingText)
            {
                UIManager.Instance.HideSubtitle();
                isShowingText = false;
            }
        }
    }
}