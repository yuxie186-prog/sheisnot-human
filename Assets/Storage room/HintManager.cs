using UnityEngine;
using TMPro; // 引入 TextMeshPro 命名空间

public class HintManager : MonoBehaviour
{
    // 静态单例，方便物品脚本直接调用
    public static HintManager Instance;

    // TextMeshPro 文本组件引用
    public TextMeshProUGUI hintText;

    private int currentFound = 0;
    private const int totalItems = 3;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateHintUI();
    }

    public void UpdateHintUI()
    {
        if (hintText != null)
        {
            if (currentFound < totalItems)
            {
                // 日语提示：调查房间，寻找3个线索 (当前进度 / 3)
                hintText.text = $"部屋の中を調べて、3つの手がかりを見つけよう ({currentFound}/{totalItems})";
            }
            else
            {
                // 收集完毕后的日文提示
                hintText.text = "すべての手がかりを見つけた！";
            }
        }
    }

    public void OnItemFound()
    {
        if (currentFound < totalItems)
        {
            currentFound++;
            UpdateHintUI();
        }
    }
}