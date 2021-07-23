using UnityEngine;
using UnityEditor;
using System.Collections;

class DialogSystemEditor : EditorWindow
{
    DialogSystem _Dialog;

    Vector2 _ScrollPos_TimeLine = new Vector2();
    Vector2 _ScrollPos_Editor = new Vector2();

    Vector2Int _Selected = new Vector2Int();

    [MenuItem("Tools/DialogSystem Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogSystemEditor));
    }

    void OnGUI()
    {
        GUILayout.Label("Dialog Editor", EditorStyles.boldLabel);
        _Dialog = EditorGUILayout.ObjectField(_Dialog, typeof(DialogSystem), true) as DialogSystem;

        EditorGUILayout.BeginHorizontal("box");
        if (_Dialog != null)
        {
            //Editor
            Editor();

            //TimeLine
            TimeLine();
        }
        EditorGUILayout.EndHorizontal();
    }

    void Editor()
    {
        EditorGUILayout.BeginVertical("box");
        _ScrollPos_Editor = EditorGUILayout.BeginScrollView(_ScrollPos_Editor, GUILayout.Width(325));
        if (_Selected.x >= 0)
        {
            if (_Selected.x < _Dialog.Dialog.Count)
            {
                if (_Selected.y < _Dialog.Dialog[_Selected.x].DialogTree.Count)
                {
                    //Dialog
                    GUILayout.Label("Selected   " + "ID:" + _Selected.x.ToString() + "," + _Selected.y.ToString());
                    _Dialog.Dialog[_Selected.x].DialogTreeInfo = EditorGUILayout.TextField("Row Info:", _Dialog.Dialog[_Selected.x].DialogTreeInfo);
                    _Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Dialog = EditorGUILayout.TextArea(_Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Dialog, GUILayout.Height(200), GUILayout.Width(300));

                    //Dialog Options
                    GUILayout.Label("Options");
                    EditorGUILayout.BeginVertical("box");

                    int optionscount = 0;
                    for (int i = 0; i < _Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options.Count; i++)
                    {
                        optionscount++;
                        //Toggle
                        _Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].OptionToggle = EditorGUILayout.Foldout(_Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].OptionToggle, "(" + optionscount.ToString() + ") " + _Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].OptionInfo);
                        
                        //Options
                        if (_Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].OptionToggle)
                        {
                            //Option Dialog
                            GUILayout.Label("Option Dialog:");
                            _Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].OptionInfo = EditorGUILayout.TextArea(_Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].OptionInfo, GUILayout.Height(100), GUILayout.Width(300));

                            //Display options
                            EditorGUILayout.BeginVertical("box");
                            for (int o = 0; o < _Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].Options.Count; o++)
                            {
                                //Option dropdown/Remove Event
                                EditorGUILayout.BeginHorizontal();
                                _Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].Options[o].Option = (DialogSystem_DialogOption.Options)EditorGUILayout.EnumPopup(_Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].Options[o].Option);
                                if (GUILayout.Button("-", GUILayout.Width(20)))
                                {
                                    _Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].Options.RemoveAt(o);
                                    break;
                                }
                                EditorGUILayout.EndHorizontal();

                                //Options
                                switch (_Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].Options[o].Option)
                                {
                                    case DialogSystem_DialogOption.Options.GOTO:
                                        _Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].Options[o].NextID = EditorGUILayout.Vector2IntField("Next ID", _Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].Options[o].NextID);
                                        break;
                                }

                            }
                            if (GUILayout.Button("Add Event"))
                            {
                                DialogSystem_DialogOption newoption = new DialogSystem_DialogOption();
                                _Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options[i].Options.Add(newoption);
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    if(GUILayout.Button("Add Option Dialog"))
                    {
                        DialogSystem_DialogOptions newoption = new DialogSystem_DialogOptions();
                        newoption.OptionInfo = "new option";
                        _Dialog.Dialog[_Selected.x].DialogTree[_Selected.y].Options.Add(newoption);
                    }
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    GUILayout.Label("Selected");
                    GUILayout.Label("ID: --");
                    GUILayout.Label("Press a button on \nthe timeline to select!");
                }
            }
        }
        else
        {
            GUILayout.Label("Selected");
            GUILayout.Label("ID: --");
            GUILayout.Label("Press a button on \nthe timeline to select!");
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    void TimeLine()
    {
        EditorGUILayout.BeginVertical();
        _ScrollPos_TimeLine = EditorGUILayout.BeginScrollView(_ScrollPos_TimeLine);
        for (int i = 0; i < _Dialog.Dialog.Count; i++)
        {
            EditorGUILayout.BeginHorizontal("box");

            //Row Options
            EditorGUILayout.BeginVertical();

            //ID/Remove
            EditorGUILayout.BeginHorizontal();
            //GUILayout.Label(_Dialog.Dialog[i].DialogTreeID.ToString(), GUILayout.Width(20));
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                _Dialog.Dialog.RemoveAt(i);
                if(_Selected.x > _Dialog.Dialog.Count-1)
                    _Selected.x--;
                break;
            }
            EditorGUILayout.EndHorizontal();

            //ADD
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                DialogSystem_Dialog newdialog = new DialogSystem_Dialog();
                newdialog.Dialog = "dialogtext";
                _Dialog.Dialog[i].DialogTree.Add(newdialog);
            }

            EditorGUILayout.EndVertical();

            //TimeLineButtons
            for (int j = 0; j < 100; j++)
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Label(j.ToString());

                if (j < _Dialog.Dialog[i].DialogTree.Count)
                {
                    //if (GUILayout.Button("(" + _Dialog.Dialog[i].DialogTree[j].Options.Count.ToString() + ") " + _Dialog.Dialog[i].DialogTree[j].Dialog, GUILayout.Width(100), GUILayout.Height(30)))
                    if (GUILayout.Button(j.ToString() + "    (" + _Dialog.Dialog[i].DialogTree[j].Options.Count.ToString() + ") ", GUILayout.Width(100), GUILayout.Height(30)))
                    {
                        _Selected = new Vector2Int(i, j);
                    }
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Dialog Tree", GUILayout.Width(100), GUILayout.Height(50)))
        {
            DialogSystem_DialogTree newdialogtree = new DialogSystem_DialogTree();
            DialogSystem_Dialog newdialog = new DialogSystem_Dialog();
            newdialog.Dialog = "dialogtext";
            newdialogtree.DialogTree.Add(newdialog);
            _Dialog.Dialog.Add(newdialogtree);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}