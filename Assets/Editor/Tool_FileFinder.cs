using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tool_FileFinder : EditorWindow
{
    //Tool State / Scrollpos
    int _ToolState = 0;
    int _ToolStateCheck = 1;
    Vector2 _ScrollPos = new Vector2();

    //Project
    string _Project_Type = "";
    string _Project_TypeCheck = "";
    string _Project_Search = "";
    string _Project_SearchCheck = "a";
    bool _Project_ExcludeMeta = true;
    int _Project_Results = 0;
    int _Project_Total = 0;

    //Project > Results
    string[] _SearchResults = new string[0];
    string[] _SearchResultsChange = new string[0];

    //Scene
    string _Scene_Search = "";
    bool _Scene_InsceneInfo = true;

    //Scene > Results
    bool[] _Scene_Objects_Toggle = new bool[0];
    GameObject[] _Scene_Objects = new GameObject[0];

    //GetWindow
    [MenuItem("Tools/Tool_FileFinder")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Tool_FileFinder));
    }

    //Menu/HomePage
    void OnGUI()
    {
        _ToolState = GUILayout.Toolbar(_ToolState, new string[] { "Assets", "Scene" });

        if (_ToolState == 0)
        {
            FileFinder_Search();
            FileFinder_SearchProject();
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

    //Project
    void FileFinder_Search()
    {
        _Project_Search = EditorGUILayout.TextField("Search:", _Project_Search);
        _Project_Type = EditorGUILayout.TextField("Type:", _Project_Type);
        _Project_ExcludeMeta = EditorGUILayout.Toggle("Exlude Meta:", _Project_ExcludeMeta);
        GUILayout.Label("(" + _Project_Results + "/" + _Project_Total + ")");

        _Project_Results = 0;
        _Project_Total = 0;

        if (_Project_Search != _Project_SearchCheck || _Project_Type != _Project_TypeCheck)
        {
            _SearchResults = System.IO.Directory.GetFiles("Assets/", "*" + _Project_Type, System.IO.SearchOption.AllDirectories);
            _SearchResultsChange = _SearchResults;
            _Project_SearchCheck = _Project_Search;
            _Project_TypeCheck = _Project_Type;
        }
    }
    void FileFinder_SearchProject()
    {
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        for (int i = 0; i < _SearchResults.Length; i++)
        {
            if (_SearchResults[i].ToLower().Contains(_Project_Search.ToLower()))
            {
                if(_Project_ExcludeMeta)
                {
                    if (!_SearchResults[i].ToLower().Contains(".meta"))
                        FileFinder_SearchProject_Results(i);
                }
                else
                    FileFinder_SearchProject_Results(i);
            }
            _Project_Total++;
        }
        EditorGUILayout.EndScrollView();
    }
    void FileFinder_SearchProject_Results(int id)
    {
        GUILayout.BeginHorizontal("Box");
        GUILayout.Label(_SearchResults[id], GUILayout.Width(Screen.width - 80));
        if (GUILayout.Button("Select", GUILayout.Width(50)))
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(_SearchResults[id]);
        }
        GUILayout.EndHorizontal();
        _Project_Results++;
    }

    //Scene
    void FileFinder_SceneSearch()
    {
        _Scene_Search = EditorGUILayout.TextField("Search:", _Scene_Search);
        GUILayout.Label("(" + _Project_Results + "/" + _Project_Total + ")");

        if (GUILayout.Button("Refresh"))
        {
            _Scene_Objects = new GameObject[0];
        }

        _Project_Results = 0;
        _Project_Total = 0;

        if (_Scene_Objects.Length == 0)
        {
            _Scene_Objects = FindObjectsOfType<GameObject>();
            _Scene_Objects_Toggle = new bool[_Scene_Objects.Length];
        }
    }
    void FileFinder_Scene()
    {
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        try
        {
            for (int i = 0; i < _Scene_Objects.Length; i++)
            {
                if (_Scene_Objects[i].name.ToLower().Contains(_Scene_Search.ToLower()))
                {
                    GUILayout.BeginHorizontal("Box");
                    _Scene_Objects_Toggle[i] = EditorGUILayout.Foldout(_Scene_Objects_Toggle[i], "");

                    GUILayout.Label(_Scene_Objects[i].name, GUILayout.Width(Screen.width - 80));
                    if (GUILayout.Button("Select", GUILayout.Width(50)))
                    {
                        Selection.activeObject = _Scene_Objects[i];
                    }

                    if (_Scene_Objects_Toggle[i])
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginVertical("box");
                        _Scene_Objects[i].name = EditorGUILayout.TextField("Name:", _Scene_Objects[i].name);
                        _Scene_Objects[i].transform.position = EditorGUILayout.Vector3Field("Position:", _Scene_Objects[i].transform.position);
                        _Scene_Objects[i].transform.eulerAngles = EditorGUILayout.Vector3Field("Rotation:", _Scene_Objects[i].transform.eulerAngles);
                        GUILayout.EndVertical();
                        GUILayout.BeginHorizontal();
                    }

                    GUILayout.EndHorizontal();
                    _Project_Results++;
                }
                _Project_Total++;
            }
        }
        catch
        {
            _Scene_Objects = new GameObject[0];
        }
        EditorGUILayout.EndScrollView();
    }

    //wip
    void FileFinder_NameChange()
    {
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        for (int i = 0; i < _SearchResults.Length; i++)
        {
            if (_SearchResults[i].ToLower().Contains(_Project_Search.ToLower()))
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
                _Project_Results++;
            }
            _Project_Total++;
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
        try
        {
            if (_Scene_InsceneInfo)
            {
                Handles.color = new Color(0, 1, 0, 0.1f);
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
        catch { }
    }
}
