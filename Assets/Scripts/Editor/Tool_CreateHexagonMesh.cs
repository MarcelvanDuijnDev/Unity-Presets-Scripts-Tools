using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Tool_CreateHexagonMesh : EditorWindow
{
    private GameObject _CenterObj;
    private List<GameObject> _ObjSaved = new List<GameObject>();
    private int _TotalObjects = 100;

    //Hex
    private int _HexLengthX = 10, _HexLengthZ = 10;
    private float _HexSize = 1;
    private float _DistanceBetween = 1;

    private bool _Center = true;
    private bool _Invert = false;

    [MenuItem("Tools/CreateHexagonGrid")]
    static void Init()
    {
        Tool_CreateHexagonMesh window = (Tool_CreateHexagonMesh)EditorWindow.GetWindow(typeof(Tool_CreateHexagonMesh));
        window.Show();
    }

    void OnGUI()
    { 
        GUILayout.BeginVertical("Box");
        _CenterObj = (GameObject)EditorGUILayout.ObjectField("Center Object", _CenterObj, typeof(GameObject), true);
        GUILayout.EndVertical();

        GUILayout.BeginVertical("Box");
        _HexSize = EditorGUILayout.FloatField("Size: ", _HexSize);
        _HexLengthX = EditorGUILayout.IntField("Collom: ", _HexLengthX);
        _HexLengthZ = EditorGUILayout.IntField("Row: ", _HexLengthZ);

        GUILayout.BeginHorizontal("Box");
        if (GUILayout.Button("Calculate Total Objects"))
            _TotalObjects = _HexLengthX * _HexLengthZ;
        EditorGUILayout.LabelField("Total: " + _TotalObjects.ToString());
        GUILayout.EndHorizontal();

        _Center = EditorGUILayout.Toggle("Center", _Center);
        _Invert = EditorGUILayout.Toggle("Invert: ", _Invert);
        _DistanceBetween = EditorGUILayout.FloatField("Distance Between: ", _DistanceBetween);
        GUILayout.EndVertical();

        GUILayout.BeginVertical("Box");
        if (GUILayout.Button("Create"))
        {
            if (_CenterObj != null)
            {
                if (_ObjSaved.Count > 0)
                {
                    for (int i = 0; i < _ObjSaved.Count; i++)
                    {
                        DestroyImmediate(_ObjSaved[i]);
                    }
                    _ObjSaved.Clear();
                }

                Vector3 objPos = _CenterObj.transform.position;
                CreateHexagon(new Vector3(_HexLengthX, 0, _HexLengthZ));
                SetParent();
            }
            else
            {
                Debug.Log("Center Object not selected!");
            }
        }

        if (GUILayout.Button("Destroy"))
        {
            if (_CenterObj != null)
            {
                for (int i = 0; i < _ObjSaved.Count; i++)
                {
                    DestroyImmediate(_ObjSaved[i]);
                }
                _ObjSaved.Clear();


                int childs = _CenterObj.transform.childCount;
                for (int i = childs -1; i >= 0; i--)
                {
                    DestroyImmediate(_CenterObj.transform.GetChild(i).gameObject);
                }
            }
            else
            {
                Debug.Log("Center Object not selected!");
            }
    }

        if (GUILayout.Button("Confirm"))
        {
            _ObjSaved.Clear();
        }
        GUILayout.EndVertical();
    }

    void CreateHexagon(Vector3 dimentsions)
    {
        Vector3 objPos = _CenterObj.transform.position;
        if (_Center && !_Invert)
        {
            objPos.x -= dimentsions.x * 0.5f * 1.7321f * _HexSize;
            objPos.z -= dimentsions.z * 0.5f * -1.5f * _HexSize;
        }
        if (_Center && _Invert)
        {
            objPos.x -= dimentsions.x * 0.5f * 1.7321f * _HexSize;
            objPos.z += dimentsions.z * 0.5f * -1.5f * _HexSize;
        }

        for (int xas = 0; xas < dimentsions.x; xas++)
        {
            CreateHax(new Vector3(objPos.x + 1.7321f  * _HexSize * _DistanceBetween * xas, objPos.y, objPos.z));
            for (int zas = 1; zas < dimentsions.z; zas++)
            {
                float offset = 0;
                if (zas % 2 == 1)
                {
                    offset = 0.86605f * _HexSize * _DistanceBetween;
                }
                else
                {
                    offset = 0;
                }
                if (!_Invert)
                {
                    CreateHax(new Vector3(objPos.x + 1.7321f * _HexSize * _DistanceBetween * xas - offset, objPos.y, objPos.z + -1.5f * _HexSize * _DistanceBetween * zas));
                }
                else
                {
                    CreateHax(new Vector3(objPos.x + 1.7321f * _HexSize * _DistanceBetween * xas - offset, objPos.y, objPos.z + +1.5f * _HexSize * _DistanceBetween * zas));
                }
            }
        }
    }
    void CreateHax(Vector3 positions)
    {
        Vector3 objPos = _CenterObj.transform.position;

        GameObject gridObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gridObj.transform.position = new Vector3(positions.x, positions.y, positions.z);

        DestroyImmediate(gridObj.GetComponent<BoxCollider>());

        float size = _HexSize;
        float width = Mathf.Sqrt(3) * size;
        float height = size * 2f;
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[7];

        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i - 30;
            float angle_rad = Mathf.Deg2Rad * angle_deg;

            vertices[i + 1] = new Vector3(size * Mathf.Cos(angle_rad), 0f, size * Mathf.Sin(angle_rad));
        }
        mesh.vertices = vertices;

        mesh.triangles = new int[]
        {
            2,1,0,
            3,2,0,
            4,3,0,
            5,4,0,
            6,5,0,
            1,6,0
        };

        Vector2[] uv = new Vector2[7];
        for (int i = 0; i < 7; i++)
        {
            uv[i] = new Vector2(
                (vertices[i].x + -width * .5f) * .5f / size,
                (vertices[i].z + -height * .5f) * .5f / size);
        }

        mesh.uv = uv;
        gridObj.GetComponent<MeshFilter>().sharedMesh = mesh;

        _ObjSaved.Add(gridObj);
    }

    void SetParent()
    {
        for (int i = 0; i < _ObjSaved.Count; i++)
        {
            _ObjSaved[i].transform.parent = _CenterObj.transform;
        }
    }
}