using UnityEngine;
using System.Collections;

public class InteractiveObject : MonoBehaviour
{
    [Header("UI设置")]
    public string clueDescription;

    [Header("镜子状态设置 (Element 0:正常, 1-3:变化)")]
    public SpriteRenderer mirrorRenderer;
    public Sprite[] mirrorStates;

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
        // 确保数组有值
        if (mirrorStates.Length < 4) yield break;

        for (int i = 1; i < mirrorStates.Length; i++)
        {
            yield return StartCoroutine(Fade(0f)); // 淡出
            mirrorRenderer.sprite = mirrorStates[i];
            yield return StartCoroutine(Fade(1f)); // 淡入
            yield return new WaitForSeconds(stayDuration);
        }

        // 最后变回正常
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