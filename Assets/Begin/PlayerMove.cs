using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 2.0f;          // 移动速度
    public float targetX = 5.0f;        // 默认的停止位置

    private DialogueTrigger dialogueTrigger;

    void Start()
    {
        // 自动获取场景中的对话控制器
        dialogueTrigger = FindObjectOfType<DialogueTrigger>();
    }

    void Update()
    {
        // 检查台词是否全部点完了
        bool isDialogueFinished = (dialogueTrigger == null) || dialogueTrigger.IsFinished();

        // 核心修改逻辑：
        // 只要台词还没有全部点完，主角就可以一直往前走（不被 targetX 卡死）
        // 只有当“台词点完了”并且“走到了 targetX”时，才真正停下并转场

        if (!isDialogueFinished)
        {
            // 只要台词没读完，不管走到了哪里，都继续向前走
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else
        {
            // 台词已经点完了
            if (transform.position.x < targetX)
            {
                // 如果还没走到目标位置，继续走完剩下的路
                transform.Translate(Vector3.right * speed * Time.deltaTime);
            }
            else
            {
                // 既点完了台词，又到达了目标位置 -> 触发转场切入公寓
                transform.Translate(Vector3.zero); // 停下
                dialogueTrigger.TriggerTransition();
            }
        }
    }
}