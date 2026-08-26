using UnityEngine;
using UnityEngine.SceneManagement; // 引入场景管理命名空间

public class SceneChangeTrigger : MonoBehaviour
{
    [Header("要跳转的目标场景名称")]
    public string targetSceneName = "BathtubScene"; // 请把这里改成你的浴室场景名字

    // 当有物体进入触发器时调用
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查撞到我的是不是主角（通过标签或者名字）
        if (collision.CompareTag("Player") || collision.name.Contains("Player"))
        {
            Debug.Log("触发切换场景，目标场景: " + targetSceneName);
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
