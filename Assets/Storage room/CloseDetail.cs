using UnityEngine;
using UnityEngine.EventSystems;

public class CloseDetail : MonoBehaviour, IPointerUpHandler
{
    // 完全取消防误触，抬起鼠标即刻关闭
    public void OnPointerUp(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }
}