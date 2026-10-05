using UnityEngine;

[ExecuteAlways]
public class PuzzleAligner : MonoBehaviour
{
    [Header("按顺序把 16 个 Slot 拖到这里")]
    public RectTransform[] slots;

    [Header("按顺序把 16 个 PuzzlePiece 拖到这里")]
    public RectTransform[] pieces;

    [ContextMenu("一键彻底对齐 Slot 的大小与缩放")]
    public void AlignPiecesToSlots()
    {
        if (slots == null || pieces == null || slots.Length != pieces.Length)
        {
            Debug.LogError("Slot 和 Piece 的数量不匹配，或者没有赋值！");
            return;
        }

        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i] != null && slots[i] != null)
            {
                // 1. 同步宽和高
                pieces[i].sizeDelta = slots[i].sizeDelta;

                // 2. 同步缩放 (Scale)，防止因为缩放倍数不同导致一大一小
                pieces[i].localScale = slots[i].localScale;

                // 3. （可选）如果你想让它们瞬间归位到 Slot 的中心，可以取消下面这行的注释：
                // pieces[i].anchoredPosition = slots[i].anchoredPosition;
            }
        }
        Debug.Log("所有拼图块的大小、缩放已成功与 Slot 完全同步！");
    }
}
