using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Tool_ScriptToString : EditorWindow
{
    MonoScript _InputScript;
    string _ScriptOutput = "";

    private Vector2 _ScrollPos = new Vector2();

    [MenuItem("Tools/Convert Script to String")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Tool_ScriptToString));
    }

    void OnGUI()
    {
        if (GUILayout.Button("Convert", GUILayout.Height(30)))
            if(_InputScript != null)
            _ScriptOutput = ConvertScriptToString();

        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        Display_InputOutput();
        Display_StringExample();
        EditorGUILayout.EndScrollView();
    }

    private void Display_InputOutput()
    {
        GUILayout.Space(20);
        //Input
        GUILayout.Label("Input: ", EditorStyles.boldLabel);
        _InputScript = EditorGUILayout.ObjectField(_InputScript, typeof(MonoScript), false) as MonoScript;

        //Output
        GUILayout.Label("Output: ", EditorStyles.boldLabel);
        EditorGUILayout.TextField("", _ScriptOutput);
        GUILayout.Space(20);
    }

    private void Display_StringExample()
    {
        //Preview
        List<string> output = new List<string>();
        List<string> output2 = new List<string>();

        for (int i = 0; i < _ScriptOutput.Length; i++)
        {
            output.Add(System.Convert.ToString(_ScriptOutput[i]));
        }

        int begincalc = 0;
        int endcalc = 0;

        for (int i = 0; i < output.Count; i++)
        {
            if (i + 1 < output.Count)
            {
                if (output[i] + output[i + 1] == "\\n")
                {
                    endcalc = i;
                    string addstring = "";
                    for (int j = 0; j < endcalc - begincalc; j++)
                    {
                        addstring += output[begincalc + j];
                    }
                    addstring += output[endcalc] + output[endcalc + 1];

                    output2.Add(addstring);
                    endcalc = endcalc + 1;
                    begincalc = endcalc + 1;
                }
            }
        }

        for (int i = 0; i < output2.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (output2[i].Contains("//"))
            {
                EditorGUILayout.TextField("", "x", GUILayout.MaxWidth(15));
            }
            else
            {
                EditorGUILayout.TextField("", "", GUILayout.MaxWidth(15));
            }

            EditorGUILayout.TextField("", output2[i]);
            GUILayout.EndHorizontal();
        }
    }

    private string ConvertScriptToString()
    {
        string newstring = "\"";
        string[] readText = File.ReadAllLines(GetPath());

        for (int i = 0; i < readText.Length; i++)
        {
            string newline = "";
            for (int j = 0; j < readText[i].Length; j++)
            {
                if(System.Convert.ToString(readText[i][j]) == "\"")
                    newline += "\\";
                newline += System.Convert.ToString(readText[i][j]);
            }
            readText[i] = newline + "\\n";
            newstring += readText[i];
        }

        newstring += "\"";

        return newstring;
    }

    private string GetPath()
    {
        string[] filepaths = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
        for (int i = 0; i < filepaths.Length; i++)
        {
            if (filepaths[i].Contains(_InputScript.name + ".cs"))
            {
                string[] filepathsplit = filepaths[i].Split(char.Parse("\\"));
                if (filepathsplit[filepathsplit.Length-1] == _InputScript.name + ".cs")
                    return filepaths[i];
            }
        }
        return "";
    }
}
