using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTrigger : MonoBehaviour
{
    [Header("目标场景名称")]
    public string targetSceneName = "Storage room";

    [Header("玩家的 Tag 标签")]
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 当带有 Player 标签的物体进入这个区域时
        if (collision.CompareTag(playerTag))
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogWarning("未设置目标场景名称！");
            }
        }
    }
}