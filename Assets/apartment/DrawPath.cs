using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class DrawPath : MonoBehaviour
{
    [Range(0.1f, 2f)]
    [Tooltip("调节线条的明暗/亮度，数值越小越暗")]
    public float lineBrightness = 1.0f;

    private float lastBrightness = 1.0f;

    void Update()
    {
        // 在编辑模式下，只要你拖动滑块，就实时更新所有线条的明暗
        if (!Application.isPlaying && lineBrightness != lastBrightness)
        {
            lastBrightness = lineBrightness;
            UpdateAllLinesBrightness();
        }
    }

    public void UpdateAllLinesBrightness()
    {
        // 遍历所有子线条，改变它们的颜色（用灰度相乘来压暗，完美保留原图纹理细节）
        Color targetColor = new Color(lineBrightness, lineBrightness, lineBrightness, 1f);

        LineRenderer[] childLines = GetComponentsInChildren<LineRenderer>();
        foreach (var lr in childLines)
        {
            lr.startColor = targetColor;
            lr.endColor = targetColor;
        }
    }
}