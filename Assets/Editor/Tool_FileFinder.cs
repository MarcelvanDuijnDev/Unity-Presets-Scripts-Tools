using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tool_FileFinder : EditorWindow
{
    //Tool Mode
    int _ToolState = 0;
    int _ToolStateCheck = 1;

    //FileFinder
    string _Type = "";
    string _Search = "";
    string _SearchCheck = "a";
    int _Results = 0;
    int _Total = 0;
    Vector2 _ScrollPos = new Vector2();

    //NameChange

    //Scene
    string _Scene_Search = "";
    bool _Scene_InsceneInfo = false;
    GameObject[] _Scene_Objects = new GameObject[0];

    //Results
    string[] _SearchResults = new string[0];
    string[] _SearchResultsChange = new string[0];

    [MenuItem("Tools/Tool_FileFinder")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Tool_FileFinder));
    }

    void OnGUI()
    {
        _ToolState = GUILayout.Toolbar(_ToolState, new string[] { "Assets", "Scene" });

        if (_ToolState == 0)
        {
            FileFinder_Search();
            FileFinder_SearchAssets();
        }
        else
        {
            FileFinder_SceneSearch();
            _Scene_InsceneInfo = EditorGUILayout.Toggle("InScene Info", _Scene_InsceneInfo);
            FileFinder_Scene();
        }

        //stop focus when switching
        if(_ToolStateCheck != _ToolState)
        {
            EditorGUI.FocusTextInControl("searchproject");
            _ToolStateCheck = _ToolState;
        } 
    }

    void FileFinder_Search()
    {
        _Search = EditorGUILayout.TextField("Search:", _Search);
        _Type = EditorGUILayout.TextField("Type:", _Type);
        GUILayout.Label("(" + _Results + "/" + _Total + ")");

        _Results = 0;
        _Total = 0;

        if (_Search != _SearchCheck)
        {
            _SearchResults = System.IO.Directory.GetFiles("Assets/", "*" + _Type, System.IO.SearchOption.AllDirectories);
            _SearchResultsChange = _SearchResults;
            _SearchCheck = _Search;
        }
    }
    void FileFinder_SearchAssets()
    {
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        for (int i = 0; i < _SearchResults.Length; i++)
        {
            if (_SearchResults[i].ToLower().Contains(_Search.ToLower()))
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label(_SearchResults[i], GUILayout.Width(Screen.width - 80));
                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(_SearchResults[i]);
                }
                GUILayout.EndHorizontal();
                _Results++;
            }
            _Total++;
        }
        EditorGUILayout.EndScrollView();
    }

    void FileFinder_SceneSearch()
    {
        _Scene_Search = EditorGUILayout.TextField("Search:", _Scene_Search);
        GUILayout.Label("(" + _Results + "/" + _Total + ")");

        _Results = 0;
        _Total = 0;

        if (_Scene_Objects.Length == 0)
            _Scene_Objects = FindObjectsOfType<GameObject>();
    }
    void FileFinder_Scene()
    {
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        for (int i = 0; i < _Scene_Objects.Length; i++)
        {
            if (_Scene_Objects[i].name.ToLower().Contains(_Scene_Search.ToLower()))
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label(_Scene_Objects[i].name, GUILayout.Width(Screen.width - 80));
                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    Selection.activeObject = _Scene_Objects[i];
                }
                GUILayout.EndHorizontal();
                _Results++;
            }
            _Total++;
        }
        EditorGUILayout.EndScrollView();
    }

    //wip
    void FileFinder_NameChange()
    {
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        for (int i = 0; i < _SearchResults.Length; i++)
        {
            if (_SearchResults[i].ToLower().Contains(_Search.ToLower()))
            {
                GUILayout.BeginHorizontal("Box");
                _SearchResultsChange[i] = EditorGUILayout.TextField("Object Name: ", _SearchResultsChange[i]);
                if (GUILayout.Button("Save", GUILayout.Width(50)))
                {
                    _SearchResults[i] = _SearchResultsChange[i];
                    Debug.Log(_SearchResults[i] + " to > " + _SearchResultsChange[i]);
                }
                if (GUILayout.Button("Revert", GUILayout.Width(50)))
                {
                    _SearchResultsChange[i] = _SearchResults[i];
                    Debug.Log(_SearchResultsChange[i] + " to > " + _SearchResults[i]);
                }
                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(_SearchResults[i]);
                }
                GUILayout.EndHorizontal();
                _Results++;
            }
            _Total++;
        }
        EditorGUILayout.EndScrollView();
    }


    //Enable/Disable
    void OnEnable()
    {
        SceneView.duringSceneGui += this.OnSceneGUI;
    }
    void OnDisable()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    //OnSceneGUI
    void OnSceneGUI(SceneView sceneView)
    {
        Handles.color = new Color(0, 1, 0, 0.3f);
        if (_Scene_InsceneInfo)
        {
            for (int i = 0; i < _Scene_Objects.Length; i++)
            {
                if (_Scene_Objects[i].name.ToLower().Contains(_Scene_Search.ToLower()))
                {
                    
                    Handles.SphereHandleCap(1, _Scene_Objects[i].transform.position, Quaternion.identity, 3f, EventType.Repaint);
                    Handles.Label(_Scene_Objects[i].transform.position, _Scene_Objects[i].name);
                }
            }
        }
    }
}
