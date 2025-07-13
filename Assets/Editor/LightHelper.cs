using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class LightHelper : EditorWindow
{
    [MenuItem("Tools/Light Helper")]
    static void GetMe()
    {
        EditorWindow.GetWindow<LightHelper>();
    }

    static float multiply;

    void OnGUI()
    {
        multiply = EditorGUILayout.FloatField(multiply);

        if (GUILayout.Button("Multiply")) GenerateForSelected();
    }

    static void GenerateForSelected()
    {
        foreach (Object o in Selection.objects)
        {
            GameObject g = o as GameObject;
            if (g != null)
            {
                Light m2 = g.GetComponent<Light>();
                if (m2 != null) m2.intensity *= multiply;
            }
        }
    }
}