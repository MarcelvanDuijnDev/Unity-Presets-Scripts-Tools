using UnityEngine;
using UnityEditor;
using System.Collections;

class EditorWindowExample : EditorWindow
{
    string examplestring = "example";
    bool examplebool = false;

    [MenuItem("Tools/EditorWindowExample")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EditorWindowExample));
    }

    void OnGUI()
    {
        GUILayout.Label("Example Title", EditorStyles.boldLabel);
        examplestring = EditorGUILayout.TextField("Example string field", examplestring);
        examplebool = EditorGUILayout.Toggle("Example bool field", examplebool);
    }
}