using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("全屏黑屏 Image")]
    public Image fadeImage;
    [Header("淡入淡出速度")]
    public float fadeSpeed = 1.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 如果希望切换场景时不被销毁，可以加上 DontDestroyOnLoad，
            // 但如果每个场景都有自己的 Canvas，也可以不用，这里按单场景过渡处理
        }
        else
        {
            Destroy(gameObject);
        }

        // 场景刚打开时，如果是海边场景，可以做一个从黑屏淡入的效果
        if (fadeImage != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    // 对外提供的跳转接口：淡出黑屏 -> 加载新场景
    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (fadeImage == null) yield break;

        // 开启 Raycast 拦截点击，防止切换时玩家乱点
        fadeImage.raycastTarget = true;
        float alpha = fadeImage.color.a;

        // 逐渐变黑（淡出）
        while (alpha < 1.0f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 黑屏完全后，正式加载新场景
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        fadeImage.raycastTarget = true;
        float alpha = 1.0f;
        fadeImage.color = new Color(0, 0, 0, alpha);

        // 逐渐变透明（淡入）
        while (alpha > 0.0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.raycastTarget = false;
    }
}