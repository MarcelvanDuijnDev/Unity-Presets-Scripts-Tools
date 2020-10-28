using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Tool_TerrainGenerator : EditorWindow
{
    private Terrain _Terrain;
    private GameObject _SelectionObj;
    private bool _Active;
    private int _TerrainTab;
    private Vector3 _TerrainSize;

    //Generate
    private float _Gen_Height;
    private float _Gen_Mountains;
    private float _Gen_Detail;

    [MenuItem("Tools/TerrainEditor")]
    static void Init()
    {
        Tool_TerrainGenerator customTerrain = (Tool_TerrainGenerator)EditorWindow.GetWindow(typeof(Tool_TerrainGenerator));
        customTerrain.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical("box");
        if (_Terrain == null)
        {
            GUILayout.Label("Select Terrain");
        }
        else
        {
            _TerrainTab = GUILayout.Toolbar(_TerrainTab, new string[] { "Settings", "Generate", "Draw" });

            if (_TerrainTab == 0)
            {
                GUILayout.Label("Settings");

                _TerrainSize.x = EditorGUILayout.FloatField("Size X: ", _TerrainSize.x);
                _TerrainSize.z = EditorGUILayout.FloatField("Size Z: ", _TerrainSize.z);

                if (GUILayout.Button("Confirm"))
                {
                    _Terrain.terrainData.size = _TerrainSize;
                }
            }
            if (_TerrainTab == 1)
            {
                GUILayout.Label("Generate");


                _TerrainSize.x = EditorGUILayout.FloatField("Size X: ", _TerrainSize.x);
                _TerrainSize.z = EditorGUILayout.FloatField("Size Z: ", _TerrainSize.z);

                _Gen_Height = EditorGUILayout.FloatField("Height: ", _Gen_Height);

                _Gen_Mountains = EditorGUILayout.Slider("Mountains", _Gen_Mountains, 0, 100);
                _Gen_Detail = EditorGUILayout.Slider("Detail", _Gen_Detail, 0, 100);

                if (GUILayout.Button("Generate"))
                {
                    Generator();
                }
            }
            if (_TerrainTab == 2)
            {
                GUILayout.Label("Draw");
            }
        }
        GUILayout.EndVertical();
    }

    void Update()
    {
        _SelectionObj = Selection.activeGameObject;
        try
        {
            if (_Terrain == null && _SelectionObj.GetComponent<Terrain>() != null)
            {
                _Terrain = _SelectionObj.GetComponent<Terrain>();
                _Active = true;
            }
        }
        catch
        {
            _Active = false;
            _Terrain = null;
        }

        if (_Active)
        {
            //OnDrawGizmos();
        }
    }

    void GetInfo()
    {
        _TerrainSize = _Terrain.terrainData.size;
    }

    void Generator()
    {
        TerrainData terrainData = new TerrainData();

        terrainData.size = new Vector3(_TerrainSize.x * 0.1f, _Gen_Height, _TerrainSize.z * 0.1f);
        terrainData.heightmapResolution = 512;
        terrainData.baseMapResolution = 1024;
        terrainData.SetDetailResolution(1024, 10);

        int _heightmapWidth = terrainData.heightmapResolution;
        int _heightmapHeight = terrainData.heightmapResolution;

        _Terrain.terrainData = terrainData;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_SelectionObj.transform.position, 1);
    }
}