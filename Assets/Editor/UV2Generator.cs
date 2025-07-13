using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class UV2Generator : EditorWindow
{
    [MenuItem("Tools/UV2 Generator")]
    static void GetMe()
    {
        EditorWindow.GetWindow<UV2Generator>();
    }

    static UnwrapParam param;

    void OnGUI()
    {
        param.angleError = EditorGUILayout.FloatField(param.angleError);
        param.areaError = EditorGUILayout.FloatField(param.areaError);
        param.hardAngle = EditorGUILayout.FloatField(param.hardAngle);
        param.packMargin = EditorGUILayout.FloatField(param.packMargin);

        if (GUILayout.Button("Set Defaults")) UnwrapParam.SetDefaults(out param);
        if (GUILayout.Button("Generate For Selected")) GenerateForSelected();
        if (GUILayout.Button("Clear Selected")) ClearSelected();
    }

    static void GenerateForSelected()
    {
        foreach (Object o in Selection.objects)
        {
            Mesh m = o as Mesh;
            if (m != null) Unwrapping.GenerateSecondaryUVSet(m, param);
            else
            {
                GameObject g = o as GameObject;
                if (g != null)
                {
                    MeshFilter m2 = g.GetComponent<MeshFilter>();
                    if (m2 != null) Unwrapping.GenerateSecondaryUVSet(m2.sharedMesh, param);
                }
            }

        }
    }

    static void ClearSelected()
    {
        /*foreach (Object o in Selection.objects)
            if (o is Mesh m) m.uv2 = null;
            else if (o is GameObject g) if (g.TryGetComponent(out MeshFilter m2)) m2.sharedMesh.uv2 = null;*/
    }
}