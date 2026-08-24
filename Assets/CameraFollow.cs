using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // 拖入 Player_Container
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("背景图（自动计算边界）")]
    public SpriteRenderer roomBackground;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // 如果拖入了背景图，实时计算边界
        if (roomBackground != null && cam != null)
        {
            Bounds bgBounds = roomBackground.bounds;

            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            float minX = bgBounds.min.x + camWidth;
            float maxX = bgBounds.max.x - camWidth;
            float minY = bgBounds.min.y + camHeight;
            float maxY = bgBounds.max.y - camHeight;

            if (minX > maxX) minX = maxX = bgBounds.center.x;
            if (minY > maxY) minY = maxY = bgBounds.center.y;

            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        // 平滑跟随
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}