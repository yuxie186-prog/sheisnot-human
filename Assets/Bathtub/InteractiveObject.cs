using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractiveObject : MonoBehaviour
{
    [Header("提示文本")]
    [TextArea(2, 5)]
    public string clueDescription = "浴缸里满水，地板上有湿脚印...她在水里才能呼吸顺畅。";

    [Header("是否是跳转到海边的浴缸")]
    public bool isBathTubForTransition = false;
    public string targetSceneName = "SeasideScene";

    private bool isPlayerInRange;
    private bool isShowingText;

    void Update()
    {
        // 玩家在范围内、未显示字幕，按 E 键或鼠标左键触发
        if (isPlayerInRange && !isShowingText && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)))
        {
            TriggerInteraction();
        }
    }

    public void TriggerInteraction()
    {
        isShowingText = true;

        // 1. 调用 UIManager 显示字幕
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowSubtitle(clueDescription, null);
        }

        // 2. 如果是浴缸，开启协程：停留 2.5 秒看清字幕后，优雅淡出并跳转！
        if (isBathTubForTransition)
        {
            StartCoroutine(WaitAndTransition());
        }
    }

    // 保留此方法给 UIManager 调用，删掉隐藏字幕的方法调用，避免报错！
    public void OnDialogueClicked()
    {
        isShowingText = false;
    }

    IEnumerator WaitAndTransition()
    {
        // 让字幕在屏幕上稳稳停留 2.5 秒，保证玩家能轻松看完文字
        yield return new WaitForSeconds(2.5f);

        isShowingText = false;

        // 触发平滑黑屏淡出并跳转到海边场景
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.FadeAndLoadScene(targetSceneName);
        }
        else
        {
            SceneManager.LoadScene(targetSceneName);
        }
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
            isShowingText = false;
        }
    }
}