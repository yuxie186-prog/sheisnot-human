using UnityEngine;

public class PuzzleScatter : MonoBehaviour
{
    void OnEnable()
    {
        foreach (Transform child in transform)
        {
            RectTransform rt = child.GetComponent<RectTransform>();
            if (rt != null)
            {
                float randomX, randomY;

                // 调整比例：横向更宽（适应屏幕宽度），纵向收窄（防止上下超出屏幕）
                if (Random.value > 0.5f)
                {
                    // 左右两侧：X 轴拉宽，Y 轴在合理高度内
                    randomX = Random.Range(380f, 780f) * (Random.value > 0.5f ? 1 : -1);
                    randomY = Random.Range(-300f, 300f);
                }
                else
                {
                    // 上下两侧：X 轴可以分布很宽，Y 轴限制在安全高度
                    randomX = Random.Range(-750f, 750f);
                    randomY = Random.Range(320f, 440f) * (Random.value > 0.5f ? 1 : -1);
                }

                rt.anchoredPosition = new Vector2(randomX, randomY);
            }
        }
    }
}