using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class Tool_NamingConvention : EditorWindow
{
    string[] _SearchResults; //Script Names
    string[] _SearchResultsFull; //Script Path
    string[] _CurrentScriptCode; //Script Code
    List<string> _CurrentScriptCodeFormated = new List<string>(); //Script Code Without Spaces

    List<string> _Variables = new List<string>();

    bool _ShowScripts;
    bool _ShowScript;
    bool _ShowVariables;

    Vector3 _ScrollPos;

    [MenuItem("Tools/Tool Naming Convention")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Tool_NamingConvention));
    }

    void OnGUI()
    {
        GUILayout.Label("Naming Convetion (WIP)", EditorStyles.boldLabel);

        if (GUILayout.Button("Scan"))
        {
            SearchScripts();

            _CurrentScriptCode = File.ReadAllLines(_SearchResultsFull[0]);
        }

        if (_SearchResults == null)
            return;

        GUILayout.Label("Scripts Found: " + _SearchResults.Length.ToString());

        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);

        //Scripts
        _ShowScripts = EditorGUILayout.Foldout(_ShowScripts, "Scripts");
        if (_ShowScripts)
        {
            for (int i = 0; i < _SearchResults.Length; i++)
            {
                GUILayout.Label(_SearchResults[i]);
            }
        }

        //Variables
        _ShowVariables = EditorGUILayout.Foldout(_ShowVariables, "Variables");
        for (int i = 0; i < _Variables.Count; i++)
        {
            _Variables[i] = EditorGUILayout.TextField(_Variables[i]);
        }


        //Script Code
        _ShowScript = EditorGUILayout.Foldout(_ShowScript, "Script Code");
        if (_ShowScript)
        {
            //Selected
            GUILayout.Label("Script Selected: " + _SearchResults[0]);

            GUILayout.Label("Code:");
            EditorGUILayout.BeginVertical("box");
            for (int i = 0; i < _CurrentScriptCode.Length; i++)
            {
                DetectType(i);
                EditorGUILayout.BeginVertical("box");

                string[] splitcode = _CurrentScriptCode[i].Split(char.Parse(" "));

                //Remove Empty
                List<string> removeempty = new List<string>();
                for (int j = 0; j < splitcode.Length; j++)
                {
                    if (splitcode[j] != "")
                    {
                        removeempty.Add(splitcode[j]);
                    }
                }


                GUILayout.Label("Script");
                GUILayout.Label(_CurrentScriptCode[i]);
                for (int j = 0; j < removeempty.Count; j++)
                {
                    GUILayout.Label("> " + removeempty[j]);
                    if(CheckVariable(removeempty[j]))
                    {

                    }
                }

                EditorGUILayout.EndVertical();
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }

    void GetScriptInfo()
    {

    }
    void DetectType(int codeid)
    {
        if (_CurrentScriptCode[codeid].Contains("private"))
            GUI.backgroundColor = Color.red;
        if (_CurrentScriptCode[codeid].Contains("public"))
            GUI.backgroundColor = Color.yellow;
        if (_CurrentScriptCode[codeid].Contains("void"))
            GUI.backgroundColor = Color.green;
        if (_CurrentScriptCode[codeid].Contains("class"))
            GUI.backgroundColor = Color.cyan;
    }
    void SearchScripts()
    {
        _SearchResultsFull = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
        _SearchResults = new string[_SearchResultsFull.Length];

        for (int i = 0; i < _SearchResultsFull.Length; i++)
        {
            string[] scriptpathsplit = _SearchResultsFull[i].Split(new Char[] { '/', '\\' },
                                 StringSplitOptions.RemoveEmptyEntries);

            _SearchResults[i] = scriptpathsplit[scriptpathsplit.Length -1];
        }
    }
    bool CheckVariable(string codeline)
    {
        if(codeline == "float" || codeline == "int" || codeline == "class" || codeline == "string")
            return true;

        return false;
    }
}

public class Tool_NamingConvention_SaveFile
{

}

public class Tool_NamingConvention_Script
{
    public string ScriptName;
    public List<string> Code;
}