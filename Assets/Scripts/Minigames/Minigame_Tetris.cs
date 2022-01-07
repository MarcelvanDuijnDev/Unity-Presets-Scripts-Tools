using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame_Tetris : MonoBehaviour
{
    [SerializeField] private Vector2Int _FieldSize = new Vector2Int(10,40);

    private GameObject[,] _FieldObjects;
    private GameObject[,] _TetrisObjects;
    private MeshRenderer[,] _TetrisRenderer;

    //Materials
    private Material _PlayfieldMat;

    void Start()
    {
        //Create Materials
        _PlayfieldMat = new Material(Shader.Find("Specular"));
        _PlayfieldMat.color = Color.black;

        //Set FieldSize
        _FieldSize += new Vector2Int(2,2);
        _FieldObjects = new GameObject[_FieldSize.x, _FieldSize.y];
        _TetrisObjects = new GameObject[_FieldSize.x, _FieldSize.y];
        _TetrisRenderer = new MeshRenderer[_FieldSize.x, _FieldSize.y];

        //CreateField
        StartCoroutine(CreateField());
    }

    void Update()
    {

    }

    private void DropObject()
    {
        //
    }

    private IEnumerator CreateField()
    {
        //Offset
        Vector2 offset = new Vector2(10 * -0.5f, 40 * -0.5f);

        //Create Field
        for (int y = 0; y < _FieldSize.y; y++)
            for (int x = 0; x < _FieldSize.x; x++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x + offset.x, y + offset.y, 0);
                cube.transform.parent = transform;
                _FieldObjects[x, y] = cube;
                _FieldObjects[x, y].name = "BorderObj_" + x.ToString() + y.ToString();
                yield return new WaitForSeconds(0);
            }
        yield return new WaitForSeconds(1f);

        //Disable Objects in playarea / Add pickup material
        for (int y = 1; y < _FieldSize.y-1; y++)
            for (int x = 1; x < _FieldSize.x-1; x++)
            {
                _FieldObjects[x, y].name = "TetrisObj_" + x.ToString() + y.ToString();
                _TetrisObjects[x, y] = _FieldObjects[x, y];
                _TetrisRenderer[x, y] = _FieldObjects[x, y].GetComponent<MeshRenderer>();
                _FieldObjects[x, y].SetActive(false);
            }

        for (int y = 1; y < _FieldSize.y - 1; y++)
            for (int x = 1; x < _FieldSize.x - 1; x++)
            {
                _TetrisObjects[x, y].SetActive(true);
                yield return new WaitForSeconds(0);
                _TetrisRenderer[x, y].material = _PlayfieldMat;
            }
    }
}
