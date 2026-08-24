using UnityEngine;
using System.Collections;

public class GridPlayerController : MonoBehaviour
{
    [Header("网格设置")]
    public float tileSize = 1.0f;
    public float moveSpeed = 6.0f;

    private Vector2Int gridPos;
    private bool isMoving = false;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        gridPos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / tileSize),
            Mathf.RoundToInt(transform.position.y / tileSize)
        );
    }

    void Update()
    {
        if (isMoving) return;

        int moveX = 0;
        int moveY = 0;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveY = 1;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveY = -1;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveX = -1;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveX = 1;

        if (moveX != 0 || moveY != 0)
        {
            if (moveX != 0) moveY = 0;

            if (spriteRenderer != null)
            {
                if (moveX < 0) spriteRenderer.flipX = true;
                else if (moveX > 0) spriteRenderer.flipX = false;
            }

            Vector2Int targetGridPos = gridPos + new Vector2Int(moveX, moveY);
            StartCoroutine(SmoothMove(targetGridPos));
        }
    }

    IEnumerator SmoothMove(Vector2Int targetPos)
    {
        isMoving = true;
        gridPos = targetPos;

        // 移动时：给动画参数 "Speed" 传入 1，触发走路动画
        if (animator != null)
        {
            animator.SetFloat("Speed", 1.0f);
            // 如果你的 Blend Tree 参数名字不叫 Speed 而是叫别的（比如 "Move" 或 "IsWalking"），把上面这行引号里的名字改成你的参数名
        }

        Vector3 startWorldPos = transform.position;
        Vector3 targetWorldPos = new Vector3(targetPos.x * tileSize, targetPos.y * tileSize, transform.position.z);

        float elapsedTime = 0;
        float duration = 1.0f / moveSpeed;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startWorldPos, targetWorldPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetWorldPos;
        isMoving = false;

        // 停下时：把 "Speed" 改回 0，切回站立动画
        if (animator != null)
        {
            animator.SetFloat("Speed", 0.0f);
        }
    }
}