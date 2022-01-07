using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Tool_Math : EditorWindow
{
    string _Search = "";

    int _Selected = 0;
    int _CheckSelected = 1;
    string[] _Options = new string[]
    {
        "sin", "cos", "tan",
    };
    string[] _Results = new string[]
    {
        "Mathf.Sin(float);",
        "Mathf.Cos(float);",
        "Mathf.Tan(float);",
    };

    string _Result;
    private Vector2 _ScrollPos = new Vector2();

    [MenuItem("Tools/Tool_Math")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Tool_Math));
    }

    void OnGUI()
    {
        //Result
        GUILayout.Label("Result:");
        EditorGUILayout.TextField("", _Result);

        //Check Selected
        if (_Selected != _CheckSelected)
        {
            _Result = _Results[_Selected];
            _CheckSelected = _Selected;
        }

        //Seach
        GUILayout.Space(20);
        GUILayout.Label("Search Formula:");
        _Search = EditorGUILayout.TextField("", _Search);

        //Search Results
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        for (int i = 0; i < _Results.Length; i++)
        {
            if (_Options[i].Contains(_Search))
                if (GUILayout.Button(_Options[i]))
                    _Selected = i;
        }
        EditorGUILayout.EndScrollView();
    }
}
