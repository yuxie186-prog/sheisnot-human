using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 建议：确保刚体类型为 Kinematic
        // rb.bodyType = RigidbodyType2D.Kinematic; 
    }

    void Update()
    {
        // 1. 获取输入 (Raw 获取更清爽的 -1, 0, 1)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 2. 更新 Animator 参数 (Speed)
        if (animator != null)
        {
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }

        // 3. 镜像翻转逻辑
        if (movement.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // 使用 MovePosition 结合归一化向量，既能匀速移动，又能完美触发 2D 物理碰撞与墙壁阻挡
            Vector2 targetPosition = rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }
    }
}
