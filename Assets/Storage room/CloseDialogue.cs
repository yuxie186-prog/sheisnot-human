using UnityEngine;
using UnityEngine.EventSystems; // 需要引入这个命名空间

public class CloseDialogue : MonoBehaviour, IPointerClickHandler
{
    private int openFrame = -1; // 建议加上防同帧误触（防止点开物体的瞬间连击到面板导致刚开就关）

    private void OnEnable()
    {
        openFrame = Time.frameCount;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 如果是打开面板的同一帧，直接拦截不处理
        if (Time.frameCount == openFrame) return;

        // 只有当玩家点击到这个对话框面板本身时，才关闭
        gameObject.SetActive(false);
    }
}