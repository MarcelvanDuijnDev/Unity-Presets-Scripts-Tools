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
        GUILayout.Label("Converted to string: ", EditorStyles.boldLabel);
        EditorGUILayout.TextField("", _ScriptOutput);

        //Convert
        if (GUILayout.Button("Convert"))
        {
            _ScriptOutput = ConvertScriptToString();
        }
    }

    private string ConvertScriptToString()
    {
        _ScriptOutput = "";
        string scriptasstring = "\"";

        //Split / add to array
        List<string> textedit = new List<string>();

        for (int i = 0; i < _ScriptInput.Length; i++)
        {
            textedit.Add(System.Convert.ToString(_ScriptInput[i]));
        }

        bool headercheck = false;
        bool commentcheck = false;

        for (int i = 0; i < textedit.Count; i++)
        {
            //Header check
            if (i + 7 < textedit.Count)
            {
                if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] + textedit[i + 6] + textedit[i + 7] == "[Header(")
                    headercheck = true;
            }

            // Comment check
            if (i + 1 < textedit.Count)
            {
                if (textedit[i] + textedit[i + 1] == "//" || textedit[i] + textedit[i + 1] == "/*")
                    commentcheck = true;
            }

            //Comment /* + */
            if (commentcheck)
            {
                if (textedit[i - 1] + textedit[i] == "*/")
                {
                    scriptasstring += "\\n";
                    commentcheck = false;
                }

                if (i + 6 < textedit.Count)
                {
                    //if
                    if (textedit[i] + textedit[i + 1] == "if")
                    {
                        scriptasstring += "\\n";
                        commentcheck = false;
                    }
                    //switch
                    if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] == "switch")
                    {
                        scriptasstring += "\\n";
                        commentcheck = false;
                    }
                    //case
                    if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] == "case")
                    {
                        scriptasstring += "\\n";
                        commentcheck = false;
                    }
                    //public
                    if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] == "public")
                    {
                        scriptasstring += "\\n";
                        commentcheck = false;
                    }
                    //private
                    if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] + textedit[i + 6] == "private")
                    {
                        scriptasstring += "\\n";
                        commentcheck = false;
                    }
                    //void
                    if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] == "void")
                    {
                        scriptasstring += "\\n";
                        commentcheck = false;
                    }
                }
            }

            scriptasstring += textedit[i];

            //Endings check
            if (i + 1 < textedit.Count)
            {
                if (textedit[i + 1] == "\"")
                {
                    scriptasstring += "\\";
                }

                if (textedit[i] == "}")
                {
                    scriptasstring += "\\n";
                }
                if (textedit[i] == ";")
                {
                    scriptasstring += "\\n";
                }
                if(textedit[i] == "]" && headercheck)
                {
                    scriptasstring += "\\n";
                    headercheck = false;
                }
            }




        }

        scriptasstring += "\"";
        return scriptasstring;
    }
}
