using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DustWiper : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private Texture2D dustTexture;
    private Image dustImage;

    [Header("橡皮擦的大小（数字越大，一笔擦除的面积越大）")]
    public int brushSize = 25;

    void Start()
    {
        dustImage = GetComponent<Image>();

        // 1. 复制一份灰尘贴图，让它变成可以被代码实时修改的动态图片
        Texture2D originTex = dustImage.sprite.texture;
        dustTexture = new Texture2D(originTex.width, originTex.height, TextureFormat.RGBA32, false);

        // 注意：如果原图不可读（Read/Write Enabled未勾选），这里可能会报错，
        // 记得在图片Inspector中勾选 Read/Write Enabled
        dustTexture.SetPixels(originTex.GetPixels());
        dustTexture.Apply();

        // 2. 将动态贴图赋回 UI
        dustImage.sprite = Sprite.Create(dustTexture, new Rect(0, 0, dustTexture.width, dustTexture.height), new Vector2(0.5f, 0.5f));
    }

    // 当鼠标按下或按住拖动时触发擦除
    public void OnPointerDown(PointerEventData eventData) { WipeDust(eventData); }
    public void OnDrag(PointerEventData eventData) { WipeDust(eventData); }

    void WipeDust(PointerEventData eventData)
    {
        // 把屏幕鼠标位置转换成 UI 图片内部的局部坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dustImage.rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        Rect rect = dustImage.rectTransform.rect;
        int x = (int)((localPoint.x - rect.x) / rect.width * dustTexture.width);
        int y = (int)((localPoint.y - rect.y) / rect.height * dustTexture.height);

        int width = dustTexture.width;
        int height = dustTexture.height;

        // 一次性获取整张图的 Color32 数组，极大提升性能，消除延迟
        Color32[] pixels = dustTexture.GetPixels32();
        bool isChanged = false;

        int bSize = brushSize;
        int bSizeSq = bSize * bSize;

        for (int i = -bSize; i <= bSize; i++)
        {
            int px = x + i;
            if (px < 0 || px >= width) continue;

            for (int j = -bSize; j <= bSize; j++)
            {
                if (i * i + j * j <= bSizeSq) // 圆形范围
                {
                    int py = y + j;
                    if (py >= 0 && py < height)
                    {
                        int index = py * width + px;
                        // 如果这里还有灰尘（alpha > 0）
                        if (pixels[index].a > 0)
                        {
                            pixels[index].a = 0; // 把透明度设为0（擦除）
                            isChanged = true;
                        }
                    }
                }
            }
        }

        // 如果像素被修改了，一并写入并刷新贴图
        if (isChanged)
        {
            dustTexture.SetPixels32(pixels);
            dustTexture.Apply();
        }
    }
}