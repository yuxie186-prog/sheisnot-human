using UnityEngine;

public class StoneClickUI : MonoBehaviour
{
    [Header("把当前场景的 UI 面板拖到这里")]
    public GameObject stoneUIPanel;

    private void Start()
    {
        if (stoneUIPanel != null)
        {
            stoneUIPanel.SetActive(false); // 默认关闭
        }
    }

    private void OnMouseUpAsButton()
    {
        if (stoneUIPanel != null)
        {
            bool isActive = stoneUIPanel.activeSelf;
            stoneUIPanel.SetActive(!isActive);
        }
    }
}