using UnityEngine;
using System.Collections;

public class CameraFocusController : MonoBehaviour
{
    public static CameraFocusController Instance;

    private Camera cam;
    private Vector3 originalPosition;
    private float originalSize;

    private Coroutine currentFocusRoutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        cam = GetComponent<Camera>();
        originalSize = cam.orthographicSize;
    }

    /// <summary>
    /// 平滑聚焦到目标点
    /// </summary>
    /// <param name="targetPos">目标世界坐标（通常是物体位置）</param>
    /// <param name="focusSize">聚焦时的相机大小（值越小拉得越近，默认比原图小一点）</param>
    /// <param name="duration">过渡时间</param>
    public void FocusOn(Vector3 targetPos, float focusSize = 3.5f, float duration = 1.0f)
    {
        if (currentFocusRoutine != null) StopCoroutine(currentFocusRoutine);

        // 保持摄像机原本的 Z 轴不变
        Vector3 targetWithZ = new Vector3(targetPos.x, targetPos.y, transform.position.z);
        currentFocusRoutine = StartCoroutine(SmoothMoveAndZoom(targetWithZ, focusSize, duration));
    }

    /// <summary>
    /// 恢复原状
    /// </summary>
    public void ResetFocus(float duration = 1.0f)
    {
        if (currentFocusRoutine != null) StopCoroutine(currentFocusRoutine);
        originalPosition = new Vector3(0, 0, transform.position.z); // 或者记录初始位置
        currentFocusRoutine = StartCoroutine(SmoothMoveAndZoom(originalPosition, originalSize, duration));
    }

    private IEnumerator SmoothMoveAndZoom(Vector3 targetPos, float targetSize, float duration)
    {
        Vector3 startPos = transform.position;
        float startSize = cam.orthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }

        transform.position = targetPos;
        cam.orthographicSize = targetSize;
    }
}
