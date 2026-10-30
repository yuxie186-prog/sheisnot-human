using UnityEngine;
using UnityEngine.EventSystems;

public class CloseDetail : MonoBehaviour, IPointerUpHandler // 注意这里改成了 IPointerUpHandler
{
    private float openTime;
    [Header("冷却时间（秒），在此时间内点击无效")]
    public float coolDownTime = 0.2f; // 0.2秒内绝对安全，防止连击穿透

    private void OnEnable()
    {
        // 记录面板被打开的精确时间
        openTime = Time.time;
    }

    // 使用 OnPointerUp（鼠标抬起时触发），而不是 OnPointerDown
    public void OnPointerUp(PointerEventData eventData)
    {
        // 如果玩家是从上一个场景或上一个物体“按着鼠标”点过来的，
        // 或者打开面板的时间还没超过设置的冷却时间，直接拦截！
        if (Time.time - openTime < coolDownTime)
        {
            return;
        }

        // 超过冷却时间后，玩家抬起鼠标才会真正关闭面板
        gameObject.SetActive(false);
    }
}