using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("要跳转的目标场景名称")]
    public string targetSceneName;

    // 世界中的 2D 物体只需要有 Collider 2D，点击时就会触发此方法
    private void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("没有在 Inspector 中设置目标场景名称！");
        }
    }
}