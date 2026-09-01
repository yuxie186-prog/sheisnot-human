using UnityEngine;

public class DrawClickInterator : MonoBehaviour
{
    public GameObject puzzleCanvas; // 把你的 PuzzleCanvas 拖到这里

    void OnMouseDown()
    {
        if (puzzleCanvas != null)
        {
            puzzleCanvas.SetActive(true); // 点击时打开拼图界面
        }
    }
}