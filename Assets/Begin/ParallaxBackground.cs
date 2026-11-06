using UnityEngine;

public class AutoTileBackground : MonoBehaviour
{
    public float scrollSpeed = 0.5f; // 背景滚动速度
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 自动计算并偏移材质纹理，实现无缝自动无限延伸
        float offset = Time.time * scrollSpeed;
        sr.material.mainTextureOffset = new Vector2(offset, 0);
    }
}