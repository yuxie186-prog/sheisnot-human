using UnityEngine;
using System.Collections;

public class InteractiveObject : MonoBehaviour
{
    [Header("镜子状态设置(Element 0:正常, 1-3:变化)")]
    public SpriteRenderer mirrorRenderer;
    public Sprite[] mirrorStates;

    [Header("特效设置")]
    public ParticleSystem mirrorParticle;
    public UnityEngine.Rendering.Universal.Light2D mirrorLight;

    [Header("切换节奏(数值越小越快)")]
    public float fadeDuration = 0.5f;

    // 用二维数组（或嵌套结构）来处理分页：
    // 每行代表一个“大阶段”，如果某阶段有多页，可以在里面加多段话
    private string[][] dialoguePages = new string[][]
    {
        // 阶段 0：初始状态 (1页)
        new string[] { "满地的水渍，浴缸里的水漫了出来。她出门时，总会忘记关水龙头吗？" },
        
        // 阶段 1：人类模样 —— 将长段落拆成了【第1页】和【第2页】分页显示！
        new string[] {
            "镜子里映出的，是她熟悉的脸。可家里分明空无一人。",
            "她还是自己剪了那头参差不齐的刘海……笨手笨脚的，看样子她依旧没真正学会怎么当一个人类。"
        },
        
        // 阶段 2：海灵非人模样 (1页)
        new string[] { "镜面泛起冰冷的涟漪。那头白发与幽蓝的微光……她从来都不是人类。" },
        
        // 阶段 3：定格异化 (1页)
        new string[] { "水波退去，镜面只剩下冰冷的鳞片与空洞的裂纹。她回海里去了吗？" }
    };

    private bool isPlayerInRange;
    private bool isShowingText;
    private bool isChanging = false;

    private int currentStageIndex = 0; // 当前处于第几个大阶段 (对应镜子状态)
    private int currentPageIndex = 0;  // 当前大阶段下的第几页文本
    private bool isFading = false;

    void Awake()
    {
        if (mirrorRenderer == null) mirrorRenderer = GetComponent<SpriteRenderer>();
    }

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
            // 1. 刚开始交互，显示第 0 阶段的第 0 页
            currentStageIndex = 0;
            currentPageIndex = 0;

            UIManager.Instance.ShowSubtitle(dialoguePages[currentStageIndex][currentPageIndex], this);
            isShowingText = true;

            if (!isChanging)
            {
                isChanging = true;
            }
        }
        else
        {
            if (isFading) return;
            HandleNext();
        }
    }

    public void OnDialogueClicked()
    {
        if (isFading) return;
        HandleNext();
    }

    // 核心推进逻辑：优先翻页，页翻完了再切换镜子画面的下一个状态
    void HandleNext()
    {
        // 检查当前阶段还有没有下一页
        if (currentPageIndex < dialoguePages[currentStageIndex].Length - 1)
        {
            // 同一个镜子状态内，进行【文字翻页】（不改变镜子图片）
            currentPageIndex++;
            UIManager.Instance.ShowSubtitle(dialoguePages[currentStageIndex][currentPageIndex], this);
        }
        else
        {
            // 当前阶段的文字播完了，准备进入【下一个镜子大阶段】
            currentStageIndex++;
            currentPageIndex = 0; // 重置页码为新阶段的第一页

            if (currentStageIndex < mirrorStates.Length)
            {
                // 还有下一个镜子状态：执行渐变切图，并显示新阶段的第一页文字
                StartCoroutine(StepToNextState(currentStageIndex));
                UIManager.Instance.ShowSubtitle(dialoguePages[currentStageIndex][currentPageIndex], this);
            }
            else
            {
                // 所有阶段全部播完：恢复初始状态并关闭旁白
                StartCoroutine(ResetToNormalSequence());
            }
        }
    }

    IEnumerator StepToNextState(int index)
    {
        isFading = true;

        if (mirrorLight != null) mirrorLight.intensity = 2.0f;

        yield return StartCoroutine(Fade(0f));
        mirrorRenderer.sprite = mirrorStates[index];

        if (mirrorLight != null) mirrorLight.intensity = 0f;

        yield return StartCoroutine(Fade(1f));

        isFading = false;
    }

    IEnumerator ResetToNormalSequence()
    {
        isFading = true;

        yield return StartCoroutine(Fade(0f));
        mirrorRenderer.sprite = mirrorStates[0];
        yield return StartCoroutine(Fade(1f));

        isFading = false;
        isChanging = false;
        currentStageIndex = 0;
        currentPageIndex = 0;

        UIManager.Instance.Hidesubtitle();
        isShowingText = false;
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
            UIManager.Instance.Hidesubtitle();
            isShowingText = false;
            isChanging = false;
            isFading = false;
            currentStageIndex = 0;
            currentPageIndex = 0;
        }
    }
}