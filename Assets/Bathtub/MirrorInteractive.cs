using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MirrorInteractive : MonoBehaviour
{
    [Header("镜子状态设置(Element 0:正常, 1-3:变化)")]
    public SpriteRenderer mirrorRenderer;
    public Sprite[] mirrorStates;

    [Header("切换节奏(数值越小越快)")]
    public float fadeDuration = 0.5f;

    [Header("操作指引UI（直接把InteractPrompt拖进来）")]
    public GameObject promptUI;

    [Header("2D光照设置 (可选)")]
    public Light2D mirrorLight;
    public float targetLightIntensity = 1.5f;

    private bool isPlayerInRange;
    private bool isMirrorInteracting = false;

    private int currentStageIndex = 0;
    private int currentPageIndex = 0;
    private bool isFading = false;
    private bool isWaitingForClick = true;

    // 触发锁
    private bool canTrigger = true;

    private string[][] dialoguePages = new string[][]
    {
      // 第一页：镜子
    new string[] { "古びた鏡に、水滴が浮いている……" },

    // 第二页：倒影与违和感
    new string[]
    {
        "鏡に映るのは、彼女の顔。でも、家に誰もいないはずだ。",
        "相変わらず不器用な前髪。……彼女は、まだ人間じゃない。"
    },

    // 第三页：真面目显露
    new string[] { "鏡の波紋と、青い光。彼女は最初から、人間じゃなかった。" },

    // 第四页：结局去向
    new string[] { "鏡に残ったのは、冷たいうろこと亀裂だけ。……海へ帰ったのだろうか？" }
    };

    void Awake()
    {
        if (mirrorRenderer == null) mirrorRenderer = GetComponent<SpriteRenderer>();
        if (mirrorLight == null) mirrorLight = GetComponentInChildren<Light2D>();
    }

    void Update()
    {
        if (isFading) return;

        // 触发开始
        if (isPlayerInRange && canTrigger && !isMirrorInteracting && Input.GetMouseButtonDown(0))
        {
            StartInteraction();
        }
        // 推进对话
        else if (isMirrorInteracting && isWaitingForClick && Input.GetMouseButtonDown(0))
        {
            HandleNext();
        }
    }

    void StartInteraction()
    {
        isMirrorInteracting = true;
        canTrigger = false;
        currentStageIndex = 0;
        currentPageIndex = 0;

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            Animator anim = player.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetFloat("Speed", 0f);
                anim.SetBool("IsWalking", false);
            }

            PlayerMovement playerMove = player.GetComponent<PlayerMovement>();
            if (playerMove != null) playerMove.enabled = false;
        }

        UIManager.Instance.ShowSubtitle(dialoguePages[currentStageIndex][currentPageIndex], null);
    }

    void HandleNext()
    {
        if (currentPageIndex < dialoguePages[currentStageIndex].Length - 1)
        {
            currentPageIndex++;
            UIManager.Instance.ShowSubtitle(dialoguePages[currentStageIndex][currentPageIndex], null);
        }
        else
        {
            currentStageIndex++;
            currentPageIndex = 0;

            if (currentStageIndex < mirrorStates.Length)
            {
                StartCoroutine(StepToNextState(currentStageIndex));
            }
            else
            {
                StartCoroutine(EndInteractionSequence());
            }
        }
    }

    IEnumerator StepToNextState(int index)
    {
        isFading = true;
        isWaitingForClick = false;

        if (mirrorLight != null)
        {
            mirrorLight.intensity = targetLightIntensity;
        }

        yield return StartCoroutine(Fade(0f));
        mirrorRenderer.sprite = mirrorStates[index];
        yield return StartCoroutine(Fade(1f));

        UIManager.Instance.ShowSubtitle(dialoguePages[currentStageIndex][currentPageIndex], null);

        isFading = false;
        isWaitingForClick = true;
    }

    IEnumerator EndInteractionSequence()
    {
        isFading = true;
        isWaitingForClick = false;

        yield return StartCoroutine(Fade(0f));
        mirrorRenderer.sprite = mirrorStates[0];

        if (mirrorLight != null)
        {
            mirrorLight.intensity = 1f;
        }

        yield return StartCoroutine(Fade(1f));

        isFading = false;
        isWaitingForClick = true;
        currentStageIndex = 0;
        currentPageIndex = 0;
        isMirrorInteracting = false;

        UIManager.Instance.Hidesubtitle();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerMovement playerMove = player.GetComponent<PlayerMovement>();
            if (playerMove != null)
            {
                playerMove.enabled = true;
            }
        }

        // 【修改点】强制增加冷却时间！
        // 交互结束后，必须强制等待 0.6 秒（你可以把 0.6 改大或改小），
        // 并且在此期间及之后，要求玩家必须松开鼠标再重新点击，才能触发下一轮。
        yield return new WaitForSeconds(0.6f);
        yield return new WaitUntil(() => !Input.GetMouseButton(0));

        // 重新开启触发权限（前提是玩家依然站在范围内）
        canTrigger = true;

        if (isPlayerInRange && promptUI != null)
        {
            promptUI.SetActive(true);
        }
    }

    IEnumerator Fade(float targetAlpha)
    {
        float elapsed = 0f;
        Color color = mirrorRenderer.color;
        float startAlpha = color.a;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float percent = Mathf.Clamp01(elapsed / fadeDuration);
            float smoothPercent = Mathf.SmoothStep(0f, 1f, percent);
            color.a = Mathf.Lerp(startAlpha, targetAlpha, smoothPercent);

            mirrorRenderer.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        mirrorRenderer.color = color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            canTrigger = true;

            if (promptUI != null && !isMirrorInteracting)
            {
                promptUI.SetActive(true);
                TextMeshProUGUI tmpText = promptUI.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null) tmpText.text = "[点击] 鏡を見る";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            canTrigger = false;
            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }
        }
    }
}