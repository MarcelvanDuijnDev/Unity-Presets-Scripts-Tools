using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Tool_QuickStart : EditorWindow
{

    private int _MenuID; // QuickStart/Scripts
    private int _DimensionID; // 2D/3D
    private int _Type2DID; // 
    private int _Type3DID; // 

    private bool[] _ScriptExist = new bool[17];

    private string[] _ScriptNames = new string[] {
"Bullet",
"DoEvent",
"EditorWindowExample",
"LightEffects",
"LoadScenes",
"Movement_CC",
"Movement_CC_TopDown",
"Movement_Camera",
"Movement_FreeCamera",
"ObjectPool",
"ObjectPoolSimple",
"OnCollision",
"SaveLoad_JSON",
"ScriptebleGameObject",
"StringFormats",
"Tool_CreateHexagonGrid",
"UIEffects" };
    private string[] _ScriptCode = new string[]
    {
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Bullet : MonoBehaviour\n{\n    [SerializeField] private float _Speed;\n    [SerializeField] private float _Damage;\n\n    void FixedUpdate()\n    {\n        transform.Translate(Vector3.forward * _Speed * Time.deltaTime);\n    }\n\n    private void OnTriggerEnter(Collider other)\n    {\n        if (other.tag == \"ExampleTag\")\n        {\n            //DoDamage\n            gameObject.SetActive(false);\n        }\n    }\n}\n",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
    };

    [MenuItem("Tools/Tool_QuickStart")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Tool_QuickStart));
    }

    void OnGUI()
    {
        //Menu Type
        _MenuID = GUILayout.Toolbar(_MenuID, new string[] { "QuickStart", "Scripts" });

        if (_MenuID == 0)
        {
            //Dimension
            _DimensionID = GUILayout.Toolbar(_DimensionID, new string[] { "2D", "3D" });

            //Type 2D/3D
            switch (_DimensionID)
            {
                case 0:
                    _Type2DID = GUILayout.Toolbar(_Type2DID, new string[] { "Platformer", "TopDown" });
                    break;
                case 1:
                    _Type3DID = GUILayout.Toolbar(_Type3DID, new string[] { "FPS", "ThirdPerson", "TopDown", "Platformer" });
                    break;
            }

            //Refresh
            if (GUILayout.Button("Refresh"))
            {
                SearchScripts();
            }

            //Info
            if (_DimensionID == 0)
            {
                switch (_Type2DID)
                {
                    case 0:
                        ScriptStatus("Test1");
                        ScriptStatus("Movement_CC");
                        break;
                    case 1:
                        ScriptStatus("Testtest1");
                        break;
                }
            }
            else
            {

            }
        }
        else
        {
            //Refresh
            if (GUILayout.Button("Refresh"))
            {
                SearchScripts();
            }

            for (int i = 0; i < _ScriptNames.Length; i++)
            {
                if (_ScriptExist[i])
                { GUI.backgroundColor = new Color(0, 1, 0); }
                else
                    GUI.backgroundColor = new Color(1, 0, 0);
                EditorGUILayout.BeginHorizontal("Box");

                GUILayout.Label(_ScriptNames[i] + ".cs", EditorStyles.boldLabel);

                EditorGUI.BeginDisabledGroup(_ScriptExist[i]);
                if (GUILayout.Button("Add", GUILayout.Width(50)))
                {
                    AddScript(i);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }
        }

    }

    void ScriptStatus(string name)
    {
        EditorGUILayout.BeginHorizontal("Box");

        int scriptid = 0;
        for (int i = 0; i < _ScriptNames.Length; i++)
        {
            if (name == _ScriptNames[i])
            {
                scriptid = i;
                continue;
            }
        }

        if (_ScriptExist[scriptid])
        { GUI.backgroundColor = new Color(0, 1, 0); } else
            GUI.backgroundColor = new Color(1, 0, 0);
        GUILayout.Label(name + ".cs");


        EditorGUILayout.EndHorizontal();
    }

    void SearchScripts()
    {
        for (int i = 0; i < _ScriptNames.Length; i++)
        {
            string[] foundscript = AssetDatabase.FindAssets(name, null);
            if (foundscript.Length > 0)
                _ScriptExist[i] = true;
        }
    }

    void AddScript(int id)
    {
        SearchScripts();
        if (!_ScriptExist[id])
        {
            using (StreamWriter sw = new StreamWriter(string.Format(Application.dataPath + "/" + _ScriptNames[id] + ".cs",
                                               new object[] { _ScriptNames[id].Replace(" ", "") })))
            {
                sw.Write(_ScriptCode[id]);
            }
        }
    }
}


/*
========================================
                Info
========================================

                [Script ID]

0: Bullet.cs                    10: ObjectPoolSimple.cs
1: DoEvent.cs                   11: OnCollision.cs
2: EditorWindowExample.cs       12: SaveLoad_JSON.cs
3: LightEffects.cs              13: ScriptebleGameObject.cs
4: LoadScenes.cs                14: StringFormats.cs
5: Movement_CC.cs               15: Tool_CreateHexagonGrid.cs
6: Movement_CC_TopDown.cs       16: UIEffects.cs
7: Movement_Camera.cs           17: 
8: Movement_FreeCamera.cs       18: 
9: ObjectPool.cs                19: 






*/