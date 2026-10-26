using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    [Header("要跳转的浴室场景名称")]
    public string targetSceneName = "BathubScene"; // 确保和你的浴室场景名称完全一致

    [Header("场景中所有的拼图块脚本")]
    public PuzzleController[] allPieces;

    void Start()
    {
        // 自动获取当前物体或子物体身上所有的拼图控制器
        if (allPieces == null || allPieces.Length == 0)
        {
            allPieces = GetComponentsInChildren<PuzzleController>();
        }
    }

    // 检查是否全部拼好
    public void CheckAllPieces()
    {
        foreach (var piece in allPieces)
        {
            // 只要有一块没拼好，就直接返回
            if (!piece.isCorrectlyPlaced)
            {
                return;
            }
        }

        // 全部拼好后执行
        Debug.Log("所有拼图已完成，准备切换场景！");
        Invoke("LoadBathScene", 0.5f); // 延迟0.5秒切换
    }

    void LoadBathScene()
    {
        // 调用转场管理器的淡出并加载接口
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.FadeAndLoadScene(targetSceneName);
        }
        else
        {
            // 万一场景里没有挂 SceneTransitionManager，保底直接切场景
            SceneManager.LoadScene(targetSceneName);
        }
    }
}