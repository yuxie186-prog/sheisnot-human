using UnityEngine;

public class StoneClickUI : MonoBehaviour
{
    [Header("石头自己的 UI 界面")]
    public GameObject stonePopUpUI;

    [Header("标记石头是否被点过（无需手动调，代码会自动变true）")]
    public bool hasBeenClicked = false; // 默认一开局绝对是 false（上锁）

    private void OnMouseDown()
    {
        // 1. 打开或关闭石头自己的 UI
        if (stonePopUpUI != null)
        {
            bool isActive = stonePopUpUI.activeSelf;
            stonePopUpUI.SetActive(!isActive);
        }

        // 2. 只要点了一次石头，就把这个标记永久变为 true（解锁画架）
        hasBeenClicked = true;
    }
}