using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Tool_ScriptToString : EditorWindow
{
    string _ScriptInput = "";
    string _ScriptOutput = "";

    List<string> _CustomCommandCheckKeywords = new List<string>();
    string _CustomCommandCheck;

    private Vector2 _ScrollPos = new Vector2();
    private bool _ToggleKeywords = false;

    [MenuItem("Tools/Convert Script to String")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Tool_ScriptToString));
    }

    void OnGUI()
    {
        if (GUILayout.Button("Convert", GUILayout.Height(30)))
            _ScriptOutput = ConvertScriptToString();

        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        Display_InputOutput();
        Show_Keywords();
        Display_TextEditor();
        EditorGUILayout.EndScrollView();
    }

    private void Display_InputOutput()
    {
        GUILayout.Space(20);
        //Input
        GUILayout.Label("Input: ", EditorStyles.boldLabel);
        _ScriptInput = EditorGUILayout.TextField("", _ScriptInput);

        //Output
        GUILayout.Label("Output: ", EditorStyles.boldLabel);
        EditorGUILayout.TextField("", _ScriptOutput);
        GUILayout.Space(20);
    }

    private void Show_Keywords()
    {
        //TextEditor Info
        GUILayout.Label("Use Custom Keywords to fix lines that should not be included into the commend. \n" +
            "Sometimes it leaves code after the command, you can addit it by adding a keyword below." +
            "The x on the left shows the lines that contain a comment.");

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Custom Keywords: ", EditorStyles.boldLabel);
        if (GUILayout.Button("Add common keywords", GUILayout.Width(200)))
        {
            AddCommonKeywords();
        }
        GUILayout.EndHorizontal();

        _CustomCommandCheck = EditorGUILayout.TextField("", _CustomCommandCheck);
        if (GUILayout.Button("AddKeyword"))
        {
            if (_CustomCommandCheck == "")
                Debug.Log("Enter a keyword");
            else
            {
                Add_Keyword(_CustomCommandCheck);
                _CustomCommandCheck = "";
                _ScriptOutput = ConvertScriptToString();
            }
        }

        _ToggleKeywords = EditorGUILayout.Foldout(_ToggleKeywords, "Show Keywords");

        if (_ToggleKeywords)
            for (int i = 0; i < _CustomCommandCheckKeywords.Count; i++)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(_CustomCommandCheckKeywords[i]);
                if (GUILayout.Button("Remove", GUILayout.Width(100)))
                {
                    _CustomCommandCheckKeywords.Remove(_CustomCommandCheckKeywords[i]);
                    _CustomCommandCheck = "";
                    _ScriptOutput = ConvertScriptToString();
                }
                GUILayout.EndHorizontal();
            }
    }
    private void Display_TextEditor()
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
        _ScriptOutput = "";
        string scriptasstring = "\"";

        //Split / add to array
        List<string> textedit = new List<string>();

        for (int i = 0; i < _ScriptInput.Length; i++)
        {
            textedit.Add(System.Convert.ToString(_ScriptInput[i]));
        }

        bool headercheck = false;
        bool forcheck = false;
        bool commentcheck = false;

        for (int i = 0; i < textedit.Count; i++)
        {
            //Header check
            if (i + 7 < textedit.Count)
            {
                if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] + textedit[i + 6] + textedit[i + 7] == "[Header(")
                    headercheck = true;
            }

            //For check
            if(i + 2 < textedit.Count)
            {
                if(textedit[i] + textedit[i+1] + textedit[i + 2] == "for")
                {
                    forcheck = true;
                }
            }

            //Comment check
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

                for (int j = 0; j < _CustomCommandCheckKeywords.Count; j++)
                {
                    if(_CustomCommandCheckKeywords[j].Length < textedit.Count)
                    {
                        string check = "";

                        for (int o = 0; o < _CustomCommandCheckKeywords[j].Length; o++)
                        {
                            check += textedit[i + o];
                        }
                        
                        if(check == _CustomCommandCheckKeywords[j])
                        {
                            scriptasstring += "\\n";
                            commentcheck = false;
                        }
                    }
                }
            }

            scriptasstring += textedit[i];
            //Endings check
            if (i + 2 < textedit.Count)
            {
                if (textedit[i + 1] == "\"")
                {
                    scriptasstring += "\\";
                }

                if (textedit[i] == "}")
                {
                    scriptasstring += "\\n";
                }
                if (textedit[i] == ";" && !forcheck)
                {
                    scriptasstring += "\\n";
                }
                if(textedit[i] == "]" && headercheck)
                {
                    scriptasstring += "\\n";
                    headercheck = false;
                }
                if (textedit[i] == ")" && forcheck)
                {
                    scriptasstring += "\\n";
                    forcheck = false;
                }
            }
        }
        scriptasstring += "\"";

        return scriptasstring;
    }

    private void AddCommonKeywords()
    {
        Add_Keyword("float");
        Add_Keyword("double");
        Add_Keyword("int");
        Add_Keyword("void");
        Add_Keyword("for");
        Add_Keyword("switch");
        Add_Keyword("private");
        Add_Keyword("public");
        Add_Keyword("[Header(");
        Add_Keyword("case");

        _ScriptOutput = ConvertScriptToString();
    }
    private void Add_Keyword(string keyword)
    {
        bool exist = false;
        for (int i = 0; i < _CustomCommandCheckKeywords.Count; i++)
        {
            if (_CustomCommandCheckKeywords[i] == keyword)
                exist = true;
        }

        if (!exist)
            _CustomCommandCheckKeywords.Add(keyword);
    }
}
