using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DrawPath))]
public class DrawPathEditor : Editor
{
    private bool isDrawing = false;
    private GameObject currentLineObj = null;

    public override void OnInspectorGUI()
    {
        // 绘制默认属性（这里会直接显示刚才加的 lineBrightness 滑动条）
        DrawDefaultInspector();

        DrawPath pathScript = target as DrawPath;
        if (pathScript == null) return;

        // 当你在 Inspector 拖动滑块松开时，立即更新所有线
        if (GUI.changed)
        {
            pathScript.UpdateAllLinesBrightness();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("在 Scene 画面按住鼠标左键可拉出一条虚线。松开后可继续拉出下一条。", MessageType.Info);

        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
        if (GUILayout.Button("清空所有线条", GUILayout.Height(30)))
        {
            Undo.RegisterFullObjectHierarchyUndo(pathScript.gameObject, "Clear All Lines");
            for (int i = pathScript.transform.childCount - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(pathScript.transform.GetChild(i).gameObject);
            }
        }
        GUI.backgroundColor = Color.white;
    }

    void OnSceneGUI()
    {
        DrawPath pathScript = target as DrawPath;
        if (pathScript == null) return;

        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0 && !e.alt)
                {
                    GUIUtility.hotControl = controlID;
                    isDrawing = true;

                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);

                    if (groundPlane.Raycast(ray, out float enter))
                    {
                        Vector3 startPoint = ray.GetPoint(enter);

                        currentLineObj = new GameObject("DrawnLine");
                        currentLineObj.transform.SetParent(pathScript.transform);
                        currentLineObj.transform.position = Vector3.zero;

                        LineRenderer lr = currentLineObj.AddComponent<LineRenderer>();
                        LineRenderer parentLr = pathScript.GetComponent<LineRenderer>();
                        if (parentLr != null)
                        {
                            lr.sharedMaterial = parentLr.sharedMaterial;
                            lr.startWidth = parentLr.startWidth;
                            lr.endWidth = parentLr.endWidth;
                        }

                        // 新建时应用当前的明暗度
                        float b = pathScript.lineBrightness;
                        Color initColor = new Color(b, b, b, 1f);
                        lr.startColor = initColor;
                        lr.endColor = initColor;

                        lr.positionCount = 2;
                        lr.SetPosition(0, startPoint);
                        lr.SetPosition(1, startPoint);

                        Undo.RegisterCreatedObjectUndo(currentLineObj, "Create Line");
                    }
                    e.Use();
                }
                break;

            case EventType.MouseDrag:
                if (isDrawing && e.button == 0 && currentLineObj != null)
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);

                    if (groundPlane.Raycast(ray, out float enter))
                    {
                        Vector3 currentPoint = ray.GetPoint(enter);
                        LineRenderer lr = currentLineObj.GetComponent<LineRenderer>();
                        if (lr != null)
                        {
                            lr.SetPosition(1, currentPoint);
                        }
                    }
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                if (isDrawing && e.button == 0)
                {
                    isDrawing = false;
                    currentLineObj = null;
                    GUIUtility.hotControl = 0;
                    e.Use();
                }
                break;
        }
    }
}