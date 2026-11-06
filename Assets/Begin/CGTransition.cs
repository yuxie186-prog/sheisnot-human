using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CGTransition : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public string nextSceneName = "ApartmentMapScene"; // 目标公寓场景

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // 注册视频播放完毕的事件监听
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // 视频播完后自动切场景
        SceneManager.LoadScene(nextSceneName);
    }
}