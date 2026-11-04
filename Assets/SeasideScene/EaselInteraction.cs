using UnityEngine;

public class EaselInteraction : MonoBehaviour
{
    [Header("画架自己的 UI 界面")]
    public GameObject easelPopUpUI;

    [Header("【核心】拖入石头物体 (Stone)")]
    public GameObject stoneObject; // 直接把场景里的 stone 物体拖过来！

    [Header("【防抢跑】拖入石头的 UI 界面")]
    public GameObject stonePopUpUI;

    private void OnMouseDown()
    {
        // 1. 核心安全检查：检查石头物体上有没有一个标记“石头已经被点过”的组件或状态
        // 如果石头的 UI 根本没有被打开过/石头没被触发过，就直接返回锁死！
        if (stoneObject != null)
        {
            StoneClickUI stoneScript = stoneObject.GetComponent<StoneClickUI>();
            // 假设我们在石头脚本里加一个 hasClicked 标记
            if (stoneScript == null || !stoneScript.hasBeenClicked)
            {
                Debug.Log("画架是锁着的，请先去触发石头！");
                return;
            }
        }

        // 2. 防抢跑机制：如果石头的 UI 当前正在显示，绝对不准打开画架 UI
        if (stonePopUpUI != null && stonePopUpUI.activeSelf)
        {
            return;
        }

        // 3. 满足所有条件，正常开合画架的 UI
        if (easelPopUpUI != null)
        {
            bool isActive = easelPopUpUI.activeSelf;
            easelPopUpUI.SetActive(!isActive);
        }
    }
}