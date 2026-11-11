using UnityEngine;

public class SceneDialogueArea : MonoBehaviour
{
    [Header("多句台词内容")]
    [TextArea(2, 4)] public string[] japaneseLines;
    [TextArea(2, 4)] public string[] chineseLines;

    [Header("相机聚焦参数")]
    public float focusOrthographicSize = 3.0f;
    public float transitionDuration = 1.0f;

    [SerializeField] private bool triggerOnce = true;
    private bool hasTriggered = false;
    private bool playerInside = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInside = true;

        if (triggerOnce && hasTriggered) return;
        hasTriggered = true;

        // 1. 相机拉近
        if (CameraFocusController.Instance != null)
        {
            CameraFocusController.Instance.FocusOn(transform.position, focusOrthographicSize, transitionDuration);
        }

        // 2. 播放整段对话,播完后回调 OnDialogueFinished
        if (NarrativeManager.Instance != null)
        {
            NarrativeManager.Instance.StartDialogueSequence(japaneseLines, chineseLines, OnDialogueFinished);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInside = false;

        // 对话还在播放:不打断,也先不复位相机,等对话播完再处理
        if (NarrativeManager.Instance != null && NarrativeManager.Instance.IsPlaying)
            return;

        ResetCamera();
    }

    // 对话播完:如果玩家已经不在区域内,就复位相机
    private void OnDialogueFinished()
    {
        if (!playerInside)
        {
            ResetCamera();
        }
    }

    private void ResetCamera()
    {
        if (CameraFocusController.Instance != null)
        {
            CameraFocusController.Instance.ResetFocus(transitionDuration);
        }
    }
}