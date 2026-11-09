using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class DialogueTrigger : MonoBehaviour
{
    public Text dialogueText;
    public string nextSceneName = "ApartmentMapScene";

    public VideoPlayer cgVideoPlayer;
    public GameObject cgDisplayObject;

    private string[] lines = new string[]
    {
        "この街の夜は、いつも濡っている。",
        "すれ違う人もなく、ただ足音だけが響く。",
        "…そういえば、彼女も雨が好きだ。",
        "水に触れている時だけ、どこか遠くを見るような顔をする。",
        "どこからともなく、潮の香りがする気がした。",
        "気のせいだ。ここは海から遠く離れた街なのに。",
        "…"
    };

    private int currentIndex = 0;
    private bool isDialogueFinished = false;
    private bool cgStarted = false;   // 只有 CG 真正开始播放后，视频结束才允许转场

    void Start()
    {
        if (lines.Length > 0 && dialogueText != null)
        {
            dialogueText.text = lines[0];
        }

        if (cgDisplayObject != null)
        {
            cgDisplayObject.SetActive(false); // 初始隐藏 CG
        }

        if (cgVideoPlayer != null)
        {
            // 强制关闭自动播放和循环，防止视频提前播放/提前结束
            cgVideoPlayer.playOnAwake = false;
            cgVideoPlayer.isLooping = false;
            cgVideoPlayer.Stop();

            cgVideoPlayer.loopPointReached += OnVideoFinished;
        }
        else
        {
            Debug.LogError("cgVideoPlayer 未在 Inspector 中绑定！", this);
        }
    }

    void Update()
    {
        // 玩家点击鼠标推进台词
        if (!isDialogueFinished && Input.GetMouseButtonDown(0))
        {
            currentIndex++;

            if (currentIndex < lines.Length)
            {
                dialogueText.text = lines[currentIndex];
            }
            else
            {
                dialogueText.text = "";
                isDialogueFinished = true; // 标记台词已完成
            }
        }
    }

    public bool IsFinished()
    {
        return isDialogueFinished;
    }

    // 当主角走到终点且台词完毕时，由 PlayerMove 统一调用此方法
    public void TriggerTransition()
    {
        if (cgStarted) return; // 防止重复调用

        enabled = false;

        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false); // 隐藏对话框
        }

        if (cgDisplayObject != null && cgVideoPlayer != null)
        {
            cgStarted = true; // 先标记，再播放
            cgDisplayObject.SetActive(true);
            cgVideoPlayer.Play();
            Debug.Log("【演出触发】台词已完且主角已到终点，开始播放 CG 视频！");
        }
        else
        {
            // 不再偷偷转场，直接报错，方便发现绑定问题
            Debug.LogError("错误：未在 Inspector 中绑定 cgDisplayObject 或 cgVideoPlayer！", this);
        }
    }

    // 唯一允许切场景的地方：必须等视频播完触发此函数
    private void OnVideoFinished(VideoPlayer vp)
    {
        if (!cgStarted)
        {
            Debug.LogWarning("视频在演出开始前就触发了结束事件，已忽略。", this);
            return;
        }

        Debug.Log("【演出结束】CG 视频播放完毕，正式跳转场景。");
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        if (cgVideoPlayer != null)
        {
            cgVideoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}