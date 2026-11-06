using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video; // 引入视频命名空间

public class DialogueTrigger : MonoBehaviour
{
    public Text dialogueText; // 拖入 Text (Legacy)
    public string nextSceneName = "ApartmentMapScene"; // 下一幕公寓的场景名

    public VideoPlayer cgVideoPlayer;       // 拖入挂了 Video Player 的物体
    public GameObject cgDisplayObject;      // 拖入你的 RawImage（即 CG 画布物体）

    // 六句连贯日文心境台词（现在式日常描写）
    private string[] lines = new string[]
    {
        "この街の夜は、いつも湿っている。",
        "すれ違う人もなく、ただ足音だけが響く。",
        "……そういえば、彼女も雨が好きだ。",
        "水に触れている時だけ、どこか遠くを見るような顔をする。",
        "どこからともなく、潮の香りがする気がした。",
        "気のせいだ。ここは、海から遠く離れた街なのに。",
        "……"
    };

    private int currentIndex = 0;           // 当前显示的台词索引
    private bool isDialogueFinished = false;  // 标记所有台词是否已经放完

    void Start()
    {
        if (lines.Length > 0)
        {
            // 初始化显示第一句
            dialogueText.text = lines[0];
        }

        // 1. 游戏开局时，自动把 CG 画面隐藏，避免挡住游戏场景
        if (cgDisplayObject != null)
        {
            cgDisplayObject.SetActive(false);
        }

        // 2. 注册视频播放完毕的事件监听
        if (cgVideoPlayer != null)
        {
            cgVideoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    void Update()
    {
        // 如果台词还没播完，检测鼠标左键点击来切换下一句
        if (!isDialogueFinished && Input.GetMouseButtonDown(0))
        {
            currentIndex++;

            if (currentIndex < lines.Length)
            {
                // 显示下一句
                dialogueText.text = lines[currentIndex];
            }
            else
            {
                // 所有台词播放完毕，清空文本，标记结束
                dialogueText.text = "";
                isDialogueFinished = true;
            }
        }
    }

    // ==========================================
    // 供 PlayerMove 调用的公有方法
    // ==========================================

    // 1. 查询台词是否全部点完
    public bool IsFinished()
    {
        return isDialogueFinished;
    }

    // 2. 触发转场（台词点完且主角走到终点时调用）
    public void TriggerTransition()
    {
        enabled = false;

        // 隐藏台词文本框（可选）
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
        }

        // 显示 CG 画面并开始播放视频
        if (cgDisplayObject != null && cgVideoPlayer != null)
        {
            cgDisplayObject.SetActive(true); // 唤醒 CG 画布
            cgVideoPlayer.Play();            // 播放视频
        }
        else
        {
            // 如果没配置 CG，则直接切场景
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // 3. 视频播完后的回调函数
    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName); // 视频播完，无缝切入公寓场景
    }
}