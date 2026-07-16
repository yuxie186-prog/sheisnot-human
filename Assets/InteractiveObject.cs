using UnityEngine;
using System.Collections;

public class InteractiveObject : MonoBehaviour
{
    [Header("UI设置")]
    public string clueDescription;

    [Header("镜子状态设置 (Element 0:正常, 1-3:变化)")]
    public SpriteRenderer mirrorRenderer;
    public Sprite[] mirrorStates;

    [Header("特效设置")]
    public ParticleSystem mirrorParticle; // 拖入刚才创建的粒子系统
    public UnityEngine.Rendering.Universal.Light2D mirrorLight; // 拖入刚才创建的光

    [Header("切换节奏 (数值越小越快)")]
    public float fadeDuration = 0.1f; // 渐变时间
    public float stayDuration = 0.3f; // 状态停留时间

    private bool isPlayerInRange;
    private bool isShowingText;
    private bool isChanging = false;

    void Awake()
    {
        // 如果没拖拽，自动获取身上的 SpriteRenderer
        if (mirrorRenderer == null) mirrorRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isShowingText)
            {
                // 1. 弹出UI
                UIManager.Instance.ShowSubtitle(clueDescription);
                isShowingText = true;

                // 2. 只有第一次交互时触发一次循环切换
                if (!isChanging) StartCoroutine(ChangeMirrorSequence());
            }
            else
            {
                // 3. 关闭UI
                UIManager.Instance.HideSubtitle();
                isShowingText = false;
            }
        }
    }

    IEnumerator ChangeMirrorSequence()
    {
        isChanging = true;

        if (mirrorStates.Length < 4) yield break;

        for (int i = 1; i < mirrorStates.Length; i++)
        {
            // --- 切换前瞬间触发特效 ---
            if (mirrorParticle != null) mirrorParticle.Play();
            if (mirrorLight != null) mirrorLight.intensity = 2.0f; // 增加亮度

            yield return StartCoroutine(Fade(0f));
            mirrorRenderer.sprite = mirrorStates[i];

            // --- 切换后关闭特效 ---
            if (mirrorParticle != null) mirrorParticle.Stop();
            if (mirrorLight != null) mirrorLight.intensity = 0f;

            yield return StartCoroutine(Fade(1f));
            yield return new WaitForSeconds(stayDuration);
        }

        // 最后变回正常的处理
        yield return StartCoroutine(Fade(0f));
        mirrorRenderer.sprite = mirrorStates[0];
        yield return StartCoroutine(Fade(1f));

        isChanging = false;
    }

    IEnumerator Fade(float targetAlpha)
    {
        float elapsed = 0f;
        Color color = mirrorRenderer.color;
        float startAlpha = color.a;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            mirrorRenderer.color = color;
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) isPlayerInRange = true; }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            UIManager.Instance.HideSubtitle();
            isShowingText = false;
        }
    }
}