using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        // 1. 获取输入
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
            // 针对 Unity 6 特别适配的移动代码：使用 linearVelocity
            rb.linearVelocity = movement.normalized * moveSpeed;
        }
    }
}