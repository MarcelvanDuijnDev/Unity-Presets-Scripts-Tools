using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame_Breakout : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2Int _FieldSize = new Vector2Int(20, 10);
    [SerializeField] private float _MovementSpeed = 10;

    //Grid
    private GameObject[,] _FieldObjects;

    private Transform _Player;

    //Materials
    private Material _BorderMat;
    private Material _PickupMat;

    //Checks
    private bool _GameRunning;

    void Start()
    {
        //CreateField
        StartCoroutine(CreateField());
    }

    void Update()
    {
        if(_GameRunning)
        {
            //Movement
            float inputx = Input.GetAxis("Horizontal");

            _Player.transform.position += new Vector3(inputx,0) * _MovementSpeed * Time.deltaTime;
        }
        
    }


    private IEnumerator CreateField()
    {
        //Offset
        Vector2 offset = new Vector2(_FieldSize.x * -0.5f, _FieldSize.y * -0.5f);

        //Top
        GameObject cube_top = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube_top.transform.position = new Vector3(0, -offset.y, 0);
        cube_top.transform.localScale = new Vector3(_FieldSize.x - 1,1,1);
        cube_top.name = "top";
        cube_top.transform.parent = transform;

        //Bottom
        GameObject cube_bottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube_bottom.transform.position = new Vector3(0, offset.y, 0);
        cube_bottom.transform.localScale = new Vector3(_FieldSize.x - 1, 1, 1);
        cube_bottom.name = "bottom";
        cube_bottom.transform.parent = transform;

        //Left
        GameObject cube_left = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube_left.transform.position = new Vector3(-offset.x, 0, 0);
        cube_left.transform.localScale = new Vector3(1, _FieldSize.y+1, 1);
        cube_left.name = "left";
        cube_left.transform.parent = transform;

        //Right
        GameObject cube_right = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube_right.transform.position = new Vector3(offset.x, 0, 0);
        cube_right.transform.localScale = new Vector3(1, _FieldSize.y+1, 1);
        cube_right.name = "right";
        cube_right.transform.parent = transform;

        GameObject cube_player = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube_player.transform.position = new Vector3(0, offset.y + 2, 0);
        _Player = cube_player.transform;

        _GameRunning = true;
        yield return new WaitForSeconds(1f);
    }
}
