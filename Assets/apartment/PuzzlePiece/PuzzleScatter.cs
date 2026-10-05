using UnityEngine;

public class PuzzleScatter : MonoBehaviour
{
    private bool hasScattered = false;

    void OnEnable()
    {
        // 如果已经散落过一次了，就不再重复随机打散，保护玩家拼好的进度！
        if (hasScattered) return;

        foreach (Transform child in transform)
        {
            RectTransform rt = child.GetComponent<RectTransform>();
            if (rt != null)
            {
                // 检查这块拼图是不是已经拼对了，如果拼对了就不要再让它乱跑
                PuzzleController controller = child.GetComponent<PuzzleController>();
                if (controller != null && controller.isCorrectlyPlaced)
                {
                    continue;
                }

                float randomX, randomY;

                // 调整比例：横向更宽（适应屏幕宽度），纵向收窄（防止上下超出屏幕）
                if (Random.value > 0.5f)
                {
                    randomX = Random.Range(380f, 780f) * (Random.value > 0.5f ? 1 : -1);
                    randomY = Random.Range(-300f, 300f);
                }
                else
                {
                    randomX = Random.Range(-750f, 750f);
                    randomY = Random.Range(320f, 440f) * (Random.value > 0.5f ? 1 : -1);
                }

                rt.anchoredPosition = new Vector2(randomX, randomY);
            }
        }

        hasScattered = true; // 标记已经打散过
    }
}