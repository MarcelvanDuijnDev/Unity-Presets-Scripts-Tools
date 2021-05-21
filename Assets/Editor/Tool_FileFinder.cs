using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tool_FileFinder : EditorWindow
{
    //FileFinder (FF)
    string _Type = "";
    string _Search = "";
    int _Results = 0;
    int _Total = 0;

    Vector2 _ScrollPos = new Vector2();

    [MenuItem("Tools/Tool_FileFinder")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Tool_FileFinder));
    }

    void OnGUI()
    {
        _Search = EditorGUILayout.TextField("Search:", _Search);
        _Type = EditorGUILayout.TextField("Type:", _Type);
        GUILayout.Label("(" + _Results + "/" + _Total + ")");

        _Results = 0;
        _Total = 0;

        string[] search_results = System.IO.Directory.GetFiles("Assets/", "*" + _Type, System.IO.SearchOption.AllDirectories);

        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        for (int i = 0; i < search_results.Length; i++)
        {
            if (search_results[i].ToLower().Contains(_Search.ToLower()))
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label(search_results[i], GUILayout.Width(Screen.width - 80));
                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(search_results[i]);
                }
                GUILayout.EndHorizontal();
                _Results++;
            }
            _Total++;
        }
        EditorGUILayout.EndScrollView();
    }
}
