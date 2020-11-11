using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Tool_ScriptToString : EditorWindow
{
    string _ScriptInput = "";
    string _ScriptOutput = "";

    [MenuItem("Tools/Convert Script to String")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Tool_ScriptToString));
    }

    void OnGUI()
    {
        //Input
        GUILayout.Label("Paste script here: ", EditorStyles.boldLabel);
        _ScriptInput = EditorGUILayout.TextField("", _ScriptInput);

        //Output
        GUILayout.Label("Paste script here: ", EditorStyles.boldLabel);
        EditorGUILayout.TextField("", _ScriptOutput);

        //Convert
        if (GUILayout.Button("Convert"))
        {
            _ScriptOutput = ConvertScriptToString();


        }
    }

    private string ConvertScriptToString()
    {
        string scriptasstring = "";

        List<string> scriptsplit = _ScriptInput.Split().ToList();

        for (int i = 0; i < scriptsplit.Count; i++)
        {
            if(i + 1 > scriptsplit.Count)
            {
                if(scriptsplit[i+1] == "\"")
                {
                    scriptsplit.Add("A");
                }
            }
        }

        for (int i = 0; i < scriptsplit.Count; i++)
        {
            scriptasstring += scriptsplit[i];
        }

        return scriptasstring;
    }
}
