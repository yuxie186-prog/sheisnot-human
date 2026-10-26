using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // 点击“开始游戏”按钮时调用
    public void OnStartButtonClicked()
    {
        // 填写你要跳转的主游戏场景名称（比如你的 ApartmentMapScene）
        SceneManager.LoadScene("ApartmentMapScene");
    }

    // 点击“退出游戏”按钮时调用（可选）
    public void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 在编辑器中停止运行
#else
            Application.Quit(); // 在打包后的游戏里退出程序
#endif
    }
}