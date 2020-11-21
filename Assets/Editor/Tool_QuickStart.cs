using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;
using UnityEditor.PackageManager.UI;
using System;

public class Tool_QuickStart : EditorWindow
{
    private int _MenuID = 0;        // QuickStart/Scripts
    private int _DimensionID = 0;   // 2D/3D
    private int _Type2DID = 0;      // Platformer/TopDown/VisualNovel
    private int _Type3DID = 0;      // FPS/ThirdPerson/TopDown/Platformer

    private bool[] _ScriptExist = new bool[25];
    private string[] _ScriptNames = new string[] { // 17 Scripts
"AudioHandler",
"BasicNavMeshAI",
"Bullet",
"DialogSystem",
"DoEvent",
"EditorWindowExample",
"EnemySpawnHandler",
"Health",
"LightEffects",
"LoadScenes",
"MVD_Namespace",
"Movement_2D_Platformer",
"Movement_2D_TopDown",
"Movement_CC",
"Movement_CC_Platformer",
"Movement_CC_TopDown",
"Movement_Camera",
"Movement_FreeCamera",
"ObjectPool",
"ObjectPoolSimple",
"OnCollision",
"ReadTwitchChat",
"SaveLoad_JSON",
"SaveLoad_XML",
"ScriptebleGameObject",
"Shooting",
"ShootingRayCast",
"StringFormats",
"Tool_CreateHexagonGrid",
"Tool_QuickStart",
"Tool_ScriptToString",
"Tool_TerrainGenerator",
"Turret",
"UIEffects"
};
    private string[] _ScriptCode = new string[] // ScriptCodes Updated: 21-nov-2020
    {

    };
    private string[] _ScriptTags = new string[] {
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
        "",
        ""
    };

    private int _CreateSceneOptions = 0;
    private Vector2 _ScrollPos = new Vector2();

    private string _Search_Script = "";
    private string _Search_Tag = "";

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
            Menu_QuickStart();
        else
            Menu_Scripts();
    }

    //Menu QuickStart
    void Menu_QuickStart()
    {
        //Dimension
        _DimensionID = GUILayout.Toolbar(_DimensionID, new string[] { "2D", "3D" });

        //Type 2D/3D
        switch (_DimensionID)
        {
            case 0:
                _Type2DID = GUILayout.Toolbar(_Type2DID, new string[] { "Platformer", "TopDown", "VisualNovel" });
                break;
            case 1:
                _Type3DID = GUILayout.Toolbar(_Type3DID, new string[] { "FPS", "ThirdPerson", "TopDown", "Platformer" });
                break;
        }

        //Info
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);

        if (_DimensionID == 0)
            Menu_QuickStart2D();
        else
            Menu_QuickStart3D();

        //Create/Refresh
        GUI.backgroundColor = Color.white;
        GUILayout.Label("Build Options", EditorStyles.boldLabel);
        _CreateSceneOptions = GUILayout.Toolbar(_CreateSceneOptions, new string[] { "New scene", "This scene" });
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Create"))
            CreateTemplate();
        if (GUILayout.Button("Refresh"))
            SearchScripts();
    }
    void Menu_QuickStart2D()
    {
        switch (_Type2DID)
        {
            case 0: //Platformer
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("Movement_2D_Platformer");
                ScriptStatus("Movement_Camera");
                GUILayout.Label("Extra", EditorStyles.boldLabel);

                GUI.backgroundColor = Color.white;
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("Add:", EditorStyles.boldLabel, GUILayout.Width(30));
                if (GUILayout.Button("Essential"))
                    AddScriptsMultiple(new string[] { "Movement_2D_Platformer", "Movement_Camera" });
                if (GUILayout.Button("All"))
                    AddScriptsMultiple(new string[] { "" });
                EditorGUILayout.EndHorizontal();
                break;
            case 1: //TopDown
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("Movement_2D_TopDown");
                ScriptStatus("Movement_Camera");
                GUILayout.Label("Extra", EditorStyles.boldLabel);

                GUI.backgroundColor = Color.white;
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("Add:", EditorStyles.boldLabel, GUILayout.Width(30));
                if (GUILayout.Button("Essential"))
                    AddScriptsMultiple(new string[] { "Movement_2D_TopDown", "Movement_Camera" });
                if (GUILayout.Button("All"))
                    AddScriptsMultiple(new string[] { "" });
                EditorGUILayout.EndHorizontal();
                break;
            case 2: //VisualNovel
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("VisualNovelHandler");
                GUILayout.Label("Extra", EditorStyles.boldLabel);

                GUI.backgroundColor = Color.white;
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("Add:", EditorStyles.boldLabel, GUILayout.Width(30));
                if (GUILayout.Button("Essential"))
                    AddScriptsMultiple(new string[] { "" });
                if (GUILayout.Button("All"))
                    AddScriptsMultiple(new string[] { "" });
                EditorGUILayout.EndHorizontal();
                break;
        }
    }
    void Menu_QuickStart3D()
    {
        switch (_Type3DID)
        {
            case 0: //FPS
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("Movement_CC");
                GUILayout.Label("Extra", EditorStyles.boldLabel);
                //

                GUI.backgroundColor = Color.white;
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("Add:", EditorStyles.boldLabel, GUILayout.Width(30));
                if (GUILayout.Button("Essential"))
                    AddScriptsMultiple(new string[] { "Movement_CC"});
                if (GUILayout.Button("All"))
                    AddScriptsMultiple(new string[] { "" });
                EditorGUILayout.EndHorizontal();
                break;
            case 1: //ThirdPerson
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("Movement_CC");
                ScriptStatus("Movement_Camera");
                GUILayout.Label("Extra", EditorStyles.boldLabel);
                //

                GUI.backgroundColor = Color.white;
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("Add:", EditorStyles.boldLabel, GUILayout.Width(30));
                if (GUILayout.Button("Essential"))
                    AddScriptsMultiple(new string[] { "Movement_CC", "Movement_Camera" });
                if (GUILayout.Button("All"))
                    AddScriptsMultiple(new string[] { "" });
                EditorGUILayout.EndHorizontal();
                break;
            case 2: //TopDown
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("Movement_CC_TopDown");
                GUILayout.Label("Extra", EditorStyles.boldLabel);
                //

                GUI.backgroundColor = Color.white;
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("Add:", EditorStyles.boldLabel, GUILayout.Width(30));
                if (GUILayout.Button("Essential"))
                    AddScriptsMultiple(new string[] { "Movement_CC_TopDown" });
                if (GUILayout.Button("All"))
                    AddScriptsMultiple(new string[] { "" });
                EditorGUILayout.EndHorizontal();
                break;
            case 3: //Platformer
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("Movement_CC_Platformer");
                GUILayout.Label("Extra", EditorStyles.boldLabel);
                //

                GUI.backgroundColor = Color.white;
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("Add:", EditorStyles.boldLabel, GUILayout.Width(30));
                if (GUILayout.Button("Essential"))
                    AddScriptsMultiple(new string[] { "Movement_CC_Platformer" });
                if (GUILayout.Button("All"))
                    AddScriptsMultiple(new string[] { "" });
                EditorGUILayout.EndHorizontal();
                break;
        }
    }

    //Menu Scripts
    void Menu_Scripts()
    {
        //Refresh
        if (GUILayout.Button("Refresh"))
            SearchScripts();

        //Search Options
        _Search_Script = EditorGUILayout.TextField("Search: ", _Search_Script);
        _Search_Tag = EditorGUILayout.TextField("SearchTag: ", _Search_Tag);

        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        for (int i = 0; i < _ScriptNames.Length; i++)
        {
            if (_Search_Script == "" || _ScriptNames[i].ToLower().Contains(_Search_Script.ToLower()))
            {
                if (_ScriptTags[i].ToLower().Contains(_Search_Tag.ToLower()) || _ScriptTags[i] == "" || _ScriptTags[i] == null)
                {
                    //Set color
                    if (_ScriptExist[i])
                        GUI.backgroundColor = new Color(0, 1, 0);
                    else
                        GUI.backgroundColor = new Color(1, 0, 0);

                    //Script
                    EditorGUILayout.BeginHorizontal("Box");
                    GUILayout.Label(_ScriptNames[i] + ".cs", EditorStyles.boldLabel);
                    EditorGUI.BeginDisabledGroup(_ScriptExist[i]);
                    if (GUILayout.Button("Add", GUILayout.Width(50)))
                        AddScript(i);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    //Search Scripts
    void ScriptStatus(string name)
    {
        int scriptid = 999;
        for (int i = 0; i < _ScriptNames.Length; i++)
        {
            if (name == _ScriptNames[i])
            {
                scriptid = i;
                continue;
            }
        }

        if (scriptid != 999)
        {
            if (_ScriptExist[scriptid])
            { GUI.backgroundColor = new Color(0, 1, 0); }
            else
                GUI.backgroundColor = new Color(1, 0, 0);

            EditorGUILayout.BeginHorizontal("Box");
            GUILayout.Label(name + ".cs");
            EditorGUI.BeginDisabledGroup(_ScriptExist[scriptid]);
            if (GUILayout.Button("Add", GUILayout.Width(50)))
            {
                AddScript(scriptid);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }else
        {
            GUI.backgroundColor = Color.black;
            EditorGUILayout.BeginHorizontal("Box");
            GUILayout.Label(name + ".cs");
            EditorGUI.BeginDisabledGroup(true);
            if (GUILayout.Button("Add", GUILayout.Width(50))){ }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }
    }
    void SearchScripts()
    {
        bool[] checkexist = new bool[_ScriptNames.Length];
        for (int i = 0; i < _ScriptNames.Length; i++)
        {
            string[] search_results = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
            for (int o = 0; o < search_results.Length; o++)
            {
                if (search_results[o].Contains(_ScriptNames[i]))
                    checkexist[i] = true;
            }
        }
        _ScriptExist = checkexist;
    }
    bool ScriptExist(string name)
    {
        int scriptid = 0;
        for (int i = 0; i < _ScriptNames.Length; i++)
        {
            if (name == _ScriptNames[i])
            {
                scriptid = i;
                continue;
            }
        }
        return _ScriptExist[scriptid];
    }

    //Add Scripts
    void AddScriptsMultiple(string[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            for (int o = 0; o < _ScriptNames.Length; o++)
            {
                if (ids[i] == _ScriptNames[o])
                {
                    AddScript(o);
                }
            }
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
        AssetDatabase.Refresh();
        SearchScripts();
    }

    //Create scene
    void CreateTemplate()
    {
        if (_CreateSceneOptions == 0)
        {
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        }
        CreateObjects();
    }
    void CreateObjects()
    {
        //Check Scripts
        SearchScripts();

        //3D
        if (_DimensionID == 1)
        {
            GameObject groundCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            groundCube.name = "Ground";
            groundCube.transform.position = new Vector3(0, 0, 0);

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0, 2, 0);

            GameObject cameraObj = GameObject.Find("Main Camera");

            switch (_Type3DID)
            {
                case 0: //FPS
                    CreateObjects_3D_FPS(player, groundCube, cameraObj);
                    break;
                case 1: //ThirdPerson
                    CreateObjects_3D_ThirdPerson(player, groundCube, cameraObj);
                    break;
                case 2: //TopDown
                    CreateObjects_3D_TopDown(player, groundCube, cameraObj);
                    break;
                case 3: //Platformer
                    CreateObjects_3D_Platformer(player, groundCube, cameraObj);
                    break;
            }
        }

        //2D
        if (_DimensionID == 0)
        {
            GameObject groundCube = GameObject.CreatePrimitive(PrimitiveType.Quad);
            DestroyImmediate(groundCube.GetComponent<MeshCollider>());
            groundCube.AddComponent<BoxCollider2D>();
            groundCube.name = "Ground";
            groundCube.transform.position = new Vector3(0, 0, 0);

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Quad);
            DestroyImmediate(player.GetComponent<MeshCollider>());
            player.AddComponent<BoxCollider2D>();
            player.name = "Player";
            player.transform.position = new Vector3(0, 2, 0);

            GameObject cameraObj = GameObject.Find("Main Camera");
            Camera cam = cameraObj.GetComponent<Camera>();
            cam.orthographic = true;

            switch(_Type2DID)
            {
                case 0: //Platformer
                    CreateObjects_2D_Platformer(player, groundCube, cameraObj);
                    break;
                case 1: //TopDown
                    CreateObjects_2D_TopDown(player, groundCube, cameraObj);
                    break;
            }
        }
    }

    //Create Objects 3D / Set scripts
    void CreateObjects_3D_FPS(GameObject playerobj, GameObject groundobj, GameObject cameraobj)
    {
        //Setup Level
        groundobj.transform.localScale = new Vector3(25, 1, 25);
        cameraobj.transform.parent = playerobj.transform;
        cameraobj.transform.localPosition = new Vector3(0, 0.65f, 0);

        GameObject objpool = new GameObject();

        //Setup Scripts
        if (ScriptExist("Health"))
        {
            string UniType = "Health";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            playerobj.AddComponent(UnityType);
        }
        if (ScriptExist("ObjectPool"))
        {
            string UniType = "ObjectPool";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            objpool.AddComponent(UnityType);
            objpool.name = "ObjectPool";
        }
        if (ScriptExist("Movement_CC"))
        {
            string UniType = "Movement_CC";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            playerobj.AddComponent(UnityType);
        }
    }
    void CreateObjects_3D_ThirdPerson(GameObject playerobj, GameObject groundobj, GameObject cameraobj)
    {
        //Setup Level
        groundobj.transform.localScale = new Vector3(25, 1, 25);
        GameObject rotationPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rotationPoint.name = "rotationPoint";
        rotationPoint.transform.position = new Vector3(0, 2, 0);
        cameraobj.transform.parent = rotationPoint.transform;
        cameraobj.transform.localPosition = new Vector3(1, 0.65f, -1.5f);
        rotationPoint.transform.parent = playerobj.transform;

        //Setup Scripts
        if (ScriptExist("Health"))
        {
            string UniType = "Health";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            playerobj.AddComponent(UnityType);
        }
        if (ScriptExist("Movement_CC"))
        {
            string UniType = "Movement_CC";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            playerobj.AddComponent(UnityType);
        }
    }
    void CreateObjects_3D_TopDown(GameObject playerobj, GameObject groundobj, GameObject cameraobj)
    {
        //Setup Level
        groundobj.transform.localScale = new Vector3(25, 1, 25);
        cameraobj.transform.position = new Vector3(0, 10, -1.5f);
        cameraobj.transform.eulerAngles = new Vector3(80, 0, 0);

        //Setup Scripts
        if (ScriptExist("Health"))
        {
            string UniType = "Health";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            playerobj.AddComponent(UnityType);
        }
        if (ScriptExist("Movement_CC_TopDown"))
        {
            string UniType = "Movement_CC_TopDown";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            playerobj.AddComponent(UnityType);
            playerobj.GetComponent(UnityType).SendMessage("SetCamera", cameraobj.GetComponent<Camera>());
        }
        if (ScriptExist("Movement_Camera"))
        {
            string UniType = "Movement_Camera";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            cameraobj.AddComponent(UnityType);
            cameraobj.GetComponent(UnityType).SendMessage("Set_CameraTarget", playerobj);
        }
    }
    void CreateObjects_3D_Platformer(GameObject playerobj, GameObject groundobj, GameObject cameraobj)
    {
        //Setup Level
        groundobj.transform.localScale = new Vector3(25, 1, 1);

        //Setup Scripts
        if (ScriptExist("Health"))
        {
            string UniType = "Health";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            playerobj.AddComponent(UnityType);
        }
        if (ScriptExist("Movement_CC_Platformer"))
        {
            string UniType = "Movement_CC_Platformer";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            playerobj.AddComponent(UnityType);
        }
        if (ScriptExist("Movement_Camera"))
        {
            string UniType = "Movement_Camera";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            cameraobj.AddComponent(UnityType);
            cameraobj.GetComponent(UnityType).SendMessage("Set_CameraTarget", playerobj);
            cameraobj.GetComponent(UnityType).SendMessage("Set_OffSet", new Vector3(0, 5, -10));
        }
    }

    //Create Object 2D / Set scripts
    void CreateObjects_2D_Platformer(GameObject playerobj, GameObject groundobj, GameObject cameraobj)
    {
        groundobj.transform.localScale = new Vector3(25, 1, 1);

        if (ScriptExist("Movement_2D_Platformer"))
        {
            string UniType = "Movement_2D_Platformer";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            playerobj.AddComponent(UnityType);
        }
        if (ScriptExist("Movement_Camera"))
        {
            string UniType = "Movement_Camera";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            cameraobj.AddComponent(UnityType);
            cameraobj.GetComponent(UnityType).SendMessage("Set_CameraTarget", playerobj);
            cameraobj.GetComponent(UnityType).SendMessage("Set_OffSet", new Vector3(0, 3, -10));
        }
    }
    void CreateObjects_2D_TopDown(GameObject playerobj, GameObject groundobj, GameObject cameraobj)
    {
        DestroyImmediate(groundobj);

        if (ScriptExist("Movement_2D_TopDown"))
        {
            string UniType = "Movement_2D_TopDown";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            playerobj.AddComponent(UnityType);
        }
        if (ScriptExist("Movement_Camera"))
        {
            string UniType = "Movement_Camera";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            cameraobj.AddComponent(UnityType);
            cameraobj.GetComponent(UnityType).SendMessage("Set_CameraTarget", playerobj);
            cameraobj.GetComponent(UnityType).SendMessage("Set_OffSet", new Vector3(0, 3, -10));
        }
    }
}