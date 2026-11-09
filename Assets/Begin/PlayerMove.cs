using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 0.7f;     // 移动速度
    public float targetX = 50f;    // 边走边点台词时的移动上限
    private DialogueTrigger dialogueTrigger;

    private bool hasTriggeredCG = false; // 防止重复触发

    void Start()
    {
        dialogueTrigger = FindAnyObjectByType<DialogueTrigger>();

        if (dialogueTrigger == null)
        {
            Debug.LogError("场景里找不到 DialogueTrigger！", this);
        }
    }

    void Update()
    {
        if (hasTriggeredCG) return;

        bool isDialogueFinished = (dialogueTrigger != null && dialogueTrigger.IsFinished());

        // 台词没点完时继续往前走
        if (!isDialogueFinished && transform.position.x < targetX)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        // 台词一点完就触发 CG，不再要求走到终点
        if (isDialogueFinished)
        {
            hasTriggeredCG = true;
            dialogueTrigger.TriggerTransition();
        }
    }
}