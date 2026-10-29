
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

    [Header("交互参数")]
    public float subtitleDisplayTime = 2.5f;

    private bool isPlayerInRange;
    private bool isShowingText;
    private bool isTransitionPending;

    private Coroutine transitionCoroutine;

    void Update()
    {
        // 玩家不在范围内时不允许交互
        if (!isPlayerInRange)
            return;

        // 防止字幕显示期间或转场期间重复触发
        if (isShowingText || isTransitionPending)
            return;

        // 按 E 键或鼠标左键触发交互
        if (Input.GetKeyDown(KeyCode.E) ||
            Input.GetMouseButtonDown(0))
        {
            TriggerInteraction();
        }
    }

    public void TriggerInteraction()
    {
        if (!isPlayerInRange)
            return;

        if (isShowingText || isTransitionPending)
            return;

        isShowingText = true;

        // 如果是浴缸，立即锁定转场，避免重复启动
        if (isBathTubForTransition)
        {
            isTransitionPending = true;
        }

        // 显示交互提示文本
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowSubtitle(clueDescription, null);
        }
        else
        {
            Debug.LogWarning(
                "未找到 UIManager，无法显示交互提示文本。"
            );
        }

        // 浴缸交互：等待字幕显示完毕后播放幻灯片
        if (isBathTubForTransition)
        {
            transitionCoroutine = StartCoroutine(
                WaitAndTransition()
            );
        }
    }

    public void OnDialogueClicked()
    {
        // 供外部对话 UI 调用
        // 普通交互可以解除锁定，浴缸转场仍由协程控制
        if (!isTransitionPending)
        {
            isShowingText = false;
        }
    }

    IEnumerator WaitAndTransition()
    {
        // 让玩家有时间阅读浴缸提示
        yield return new WaitForSeconds(subtitleDisplayTime);

        transitionCoroutine = null;
        isShowingText = false;

        // 如果等待期间玩家已经离开交互范围，则取消转场
        if (!isPlayerInRange)
        {
            isTransitionPending = false;
            yield break;
        }

        // 开始幻灯片转场
        if (SlideShowController.Instance != null)
        {
            SlideShowController.Instance.BeginSlideShow(
                targetSceneName
            );
        }
        else
        {
            Debug.LogWarning(
                "未找到 SlideShowController，直接跳转场景。"
            );

            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.FadeAndLoadScene(
                    targetSceneName
                );
            }
            else
            {
                SceneManager.LoadScene(targetSceneName);
            }
        }

        // 转场已经交给幻灯片控制器或场景管理器处理
        // 保持锁定，防止同一场景中重复触发
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        isPlayerInRange = false;

        // 如果玩家在等待期间离开，则取消尚未开始的转场
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;

            isTransitionPending = false;
            isShowingText = false;
        }
        else if (!isTransitionPending)
        {
            isShowingText = false;
        }
    }

    private void OnDisable()
    {
        // 物体被禁用时清理等待中的协程
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }

        isPlayerInRange = false;
        isShowingText = false;
        isTransitionPending = false;
    }
}
