using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SlideShowController : MonoBehaviour
{
    public static SlideShowController Instance;

    [Header("UI 组件")]
    public GameObject slideShowPanelObj;
    public Image slideImage;
    public TMP_Text subtitleText;

    [Header("图片与文案配置")]
    public Sprite[] slides;

    [TextArea]
    public string[] subtitles;

    [Header("播放参数")]
    public float displayTime = 3f;
    public float fadeDuration = 1f;

    [Header("调试")]
    public bool enableDebugLog = true;

    private int currentIndex = 0;
    private string targetSceneAfterPlay;
    private bool isPlaying = false;
    private Coroutine playCoroutine;

    // 缓存字幕自身及其父级上的 CanvasGroup
    private CanvasGroup[] subtitleCanvasGroups;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CacheSubtitleCanvasGroups();

        if (slideShowPanelObj != null)
        {
            slideShowPanelObj.SetActive(false);
        }

        if (slideImage != null)
        {
            SetImageAlpha(1f);
        }

        if (subtitleText != null)
        {
            SetTextAlpha(1f);
        }
    }

    // 获取字幕自身及所有父物体上的 CanvasGroup
    void CacheSubtitleCanvasGroups()
    {
        if (subtitleText == null)
        {
            subtitleCanvasGroups = new CanvasGroup[0];
            return;
        }

        subtitleCanvasGroups = subtitleText.GetComponentsInParent<CanvasGroup>(true);
    }

    public void BeginSlideShow(string targetScene)
    {
        if (isPlaying)
        {
            return;
        }

        if (slides == null || slides.Length == 0)
        {
            Debug.LogError("【错误】Slides 数组为空！");
            return;
        }

        if (slideImage == null)
        {
            Debug.LogError("【错误】没有指定 Slide Image！");
            return;
        }

        if (subtitleText == null)
        {
            Debug.LogError("【错误】没有指定 Subtitle Text！");
            return;
        }

        targetSceneAfterPlay = targetScene;
        currentIndex = 0;
        isPlaying = true;

        CacheSubtitleCanvasGroups();

        if (slideShowPanelObj != null)
        {
            slideShowPanelObj.SetActive(true);
        }

        SetImageAlpha(0f);
        SetTextAlpha(1f);
        ForceSubtitleVisible();

        if (enableDebugLog)
        {
            Debug.Log("【幻灯片】开始播放");
        }

        playCoroutine = StartCoroutine(PlaySlideShow());
    }

    IEnumerator PlaySlideShow()
    {
        for (currentIndex = 0; currentIndex < slides.Length; currentIndex++)
        {
            Sprite currentSprite = slides[currentIndex];

            if (currentSprite == null)
            {
                if (enableDebugLog)
                {
                    Debug.LogWarning($"【幻灯片】第 {currentIndex + 1} 张图片为空，跳过。");
                }
                continue;
            }

            // 1. 设置当前图片
            slideImage.sprite = currentSprite;
            slideImage.enabled = true;

            // 2. 设置对应字幕（增加安全检查，防止越界或显示成空）
            if (subtitles != null && currentIndex < subtitles.Length)
            {
                subtitleText.text = subtitles[currentIndex];
                if (enableDebugLog)
                {
                    Debug.Log($"【幻灯片】设置第 {currentIndex + 1} 句台词: {subtitles[currentIndex]}");
                }
            }
            else
            {
                subtitleText.text = "";
                if (enableDebugLog)
                {
                    Debug.LogWarning($"【幻灯片】第 {currentIndex + 1} 张图没有对应的台词（或台词数组长度不够），显示为空。");
                }
            }

            // 确保台词可见
            ForceSubtitleVisible();

            // 3. 图片淡入
            yield return StartCoroutine(FadeImageAlpha(0f, 1f, fadeDuration));

            // 4. 图片停留期间，持续计时并检查点击
            float timer = 0f;

            while (timer < displayTime)
            {
                ForceSubtitleVisible();

                // 点击鼠标左键直接跳过当前页停留
                if (Input.GetMouseButtonDown(0))
                {
                    if (enableDebugLog)
                    {
                        Debug.Log($"【幻灯片】玩家点击跳过第 {currentIndex + 1} 页停留");
                    }
                    break;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            // 5. 除最后一张外，图片淡出
            if (currentIndex < slides.Length - 1)
            {
                yield return StartCoroutine(FadeImageAlpha(1f, 0f, fadeDuration));
            }
        }

        playCoroutine = null;
        isPlaying = false;

        OnSlideShowFinished();
    }

    IEnumerator FadeImageAlpha(float startAlpha, float endAlpha, float duration)
    {
        if (duration <= 0f)
        {
            SetImageAlpha(endAlpha);
            ForceSubtitleVisible();
            yield break;
        }

        float elapsedTime = 0f;

        SetImageAlpha(startAlpha);
        ForceSubtitleVisible();

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, t);

            SetImageAlpha(currentAlpha);
            ForceSubtitleVisible();

            yield return null;
        }

        SetImageAlpha(endAlpha);
        ForceSubtitleVisible();
    }

    // 强制恢复字幕显示状态
    void ForceSubtitleVisible()
    {
        if (subtitleText == null)
        {
            return;
        }

        if (!subtitleText.enabled)
        {
            subtitleText.enabled = true;
        }

        if (!subtitleText.gameObject.activeSelf)
        {
            subtitleText.gameObject.SetActive(true);
        }

        SetTextAlpha(1f);

        if (subtitleCanvasGroups != null)
        {
            foreach (CanvasGroup group in subtitleCanvasGroups)
            {
                if (group != null && group.alpha < 1f)
                {
                    group.alpha = 1f;
                }
            }
        }
    }

    void SetImageAlpha(float alpha)
    {
        if (slideImage == null)
        {
            return;
        }

        Color c = slideImage.color;
        c.a = alpha;
        slideImage.color = c;
    }

    void SetTextAlpha(float alpha)
    {
        if (subtitleText == null)
        {
            return;
        }

        Color c = subtitleText.color;
        c.a = alpha;
        subtitleText.color = c;
    }

    void OnSlideShowFinished()
    {
        if (subtitleText != null)
        {
            subtitleText.text = "";
        }

        if (enableDebugLog)
        {
            Debug.Log("【幻灯片】播放结束");
        }

        if (!string.IsNullOrEmpty(targetSceneAfterPlay))
        {
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.FadeAndLoadScene(targetSceneAfterPlay);
            }
            else
            {
                SceneManager.LoadScene(targetSceneAfterPlay);
            }
        }
    }

    void OnDisable()
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }

        isPlaying = false;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}