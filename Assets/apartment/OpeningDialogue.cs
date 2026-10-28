using UnityEngine;
using UnityEngine.UI; // 使用原生 UI 命名空间

public class OpeningDialogue : MonoBehaviour
{
    public Text dialogueText; // 改回普通的 Text

    private string openingContent =
        "ドアを開け、灰青色の薄暗いリビングに足を踏み入れた。\n" +
        "いつもなら出迎えてくれる彼女の姿はない。\n" +
        "テーブルの上には、水に濡れた一幅の絵。\n" +
        "彼女はどこへ行った？";

    void Start()
    {
        if (dialogueText != null)
        {
            dialogueText.text = openingContent;
        }
    }
}