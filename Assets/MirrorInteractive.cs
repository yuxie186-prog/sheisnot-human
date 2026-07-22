using UnityEngine;
using System.Collections;
// 如果你的项目用的是 URP 的 2D Light，请确保这一行存在（如果报错可删掉）
using UnityEngine.Rendering.Universal;

public class MirrorInteractive : MonoBehaviour
{
    [Header("镜子状态设置(Element 0:正常, 1-3:变化)")]
    public SpriteRenderer mirrorRenderer;
    public Sprite[] mirrorStates;

    [Header("切换节奏(数值越小越快)")]
    public float fadeDuration = 0.5f;

    [Header("2D光照设置 (可选)")]
    public Light2D mirrorLight; // 可以在面板把镜子的 2D Light 拖进来
    public float targetLightIntensity = 1.5f; // 变化时光的亮度

    private bool isPlayerInRange;
    private bool isMirrorInteracting = false;

    private int currentStageIndex = 0;
    private int currentPageIndex = 0;
    private bool isFading = false;

    // 镜子专属的独立对话剧本
    private string[][] dialoguePages = new string[][]
    {
        new string[] { "这面镜子看起来有些年头了，镜面上隐隐约约泛着水气……" },
        new string[] {
            "镜子里映出的，是她熟悉的脸。可家里分明空无一人。",
            "她还是自己剪了那头参差不齐的刘海……笨手笨脚的，看样子她依旧没真正学会怎么当一个人类。"
        },
        new string[] { "镜面泛起冰冷的涟漪。那头白发与幽蓝的微光……她从来都不是人类。" },
        new string[] { "水波退去，镜面只剩下冰冷的鳞片与空洞的裂纹。她回海里去了吗？" }
    };

    void Awake()
    {
        if (mirrorRenderer == null) mirrorRenderer = GetComponent<SpriteRenderer>();
        if (mirrorLight == null) mirrorLight = GetComponentInChildren<Light2D>();
    }

    void Update()
    {
        if (isPlayerInRange && !isMirrorInteracting && Input.GetKeyDown(KeyCode.E))
        {
            StartInteraction();
        }
        else if (isMirrorInteracting && Input.GetMouseButtonDown(0))
        {
            if (isFading) return;
            HandleNext();
        }
    }

    void StartInteraction()
    {
        isMirrorInteracting = true;
        currentStageIndex = 0;
        currentPageIndex = 0;

        // 原地静止逻辑
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
                UIManager.Instance.ShowSubtitle(dialoguePages[currentStageIndex][currentPageIndex], null);
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

        // 画面淡出时，顺便让 2D 光闪一下或变亮
        if (mirrorLight != null)
        {
            mirrorLight.intensity = targetLightIntensity;
        }

        yield return StartCoroutine(Fade(0f));
        mirrorRenderer.sprite = mirrorStates[index];
        yield return StartCoroutine(Fade(1f));

        isFading = false;
    }

    IEnumerator EndInteractionSequence()
    {
        isFading = true;
        yield return StartCoroutine(Fade(0f));
        mirrorRenderer.sprite = mirrorStates[0];

        // 恢复初始光照强度
        if (mirrorLight != null)
        {
            mirrorLight.intensity = 1f;
        }

        yield return StartCoroutine(Fade(1f));

        isFading = false;
        currentStageIndex = 0;
        currentPageIndex = 0;
        isMirrorInteracting = false;

        // 恢复玩家移动控制权
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerMovement playerMove = player.GetComponent<PlayerMovement>();
            if (playerMove != null) playerMove.enabled = true;
        }

        UIManager.Instance.Hidesubtitle();
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
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}