using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [Header("提示文本")]
    [TextArea(2, 5)]
    public string clueDescription = "浴缸里的水还在不停漫出来……她在水里才能呼吸顺畅。";

    private bool isPlayerInRange;
    private bool isShowingText;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TriggerInteraction();
        }
    }

    public void TriggerInteraction()
    {
        if (!isShowingText)
        {
            UIManager.Instance.ShowSubtitle(clueDescription, null);
            isShowingText = true;
        }
        else
        {
            OnDialogueClicked();
        }
    }

    public void OnDialogueClicked()
    {
        UIManager.Instance.Hidesubtitle();
        isShowingText = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            UIManager.Instance.Hidesubtitle();
            isShowingText = false;
        }
    }
}