using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame_Snake : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2Int _FieldSize = new Vector2Int(20,10);
    [SerializeField] private float _UpdateSpeed = 1;
    private float _UpdateTimer;

    //Grid
    private GameObject[,] _FieldObjects;
    private GameObject[,] _SnakeObjects;
    private List<Vector2Int> _BorderPositions = new List<Vector2Int>();
    private List<Vector2Int> _SnakeLocation = new List<Vector2Int>();

    //Movement/CurrentLocation/Pickup
    private Vector2Int _Movement = new Vector2Int(1, 0);
    private Vector2Int _NewMovement;
    private Vector2Int _CurrentLocation;
    private Vector2Int _PickupLocation;

    //Materials
    private Material _BorderMat;
    private Material _PickupMat;

    //Checks
    private bool _PickupActivated;
    private bool _GameRunning;

    void Start()
    {
        //Create Materials
        _BorderMat = new Material(Shader.Find("Specular"));
        _BorderMat.color = Color.black;
        _PickupMat = new Material(Shader.Find("Specular"));
        _PickupMat.color = Color.green;

        //Set FieldSize
        _FieldObjects = new GameObject[_FieldSize.x,_FieldSize.y];
        _SnakeObjects = new GameObject[_FieldSize.x, _FieldSize.y];

        //Set Snake
        _CurrentLocation = new Vector2Int(Mathf.RoundToInt(_FieldSize.x * 0.5f), Mathf.RoundToInt(_FieldSize.y * 0.5f));
        _SnakeLocation.Add(new Vector2Int(Mathf.RoundToInt(_FieldSize.x * 0.5f), Mathf.RoundToInt(_FieldSize.y * 0.5f)));

        //CreateField
        StartCoroutine(CreateField());
    }
    void Update()
    {
        if (_GameRunning)
        {
            //Input
            float inputx = Input.GetAxis("Horizontal");
            float inputy = Input.GetAxis("Vertical");
            if (inputx > 0 && _Movement != new Vector2Int(-1, 0))
                _NewMovement = new Vector2Int(1, 0);
            if (inputx < 0 && _Movement != new Vector2Int(1, 0))
                _NewMovement = new Vector2Int(-1, 0);
            if (inputy > 0 && _Movement != new Vector2Int(0, -1))
                _NewMovement = new Vector2Int(0, 1);
            if (inputy < 0 && _Movement != new Vector2Int(0, 1))
                _NewMovement = new Vector2Int(0, -1);

            //Update Movement
            _UpdateTimer += 1 * Time.deltaTime;
            if (_UpdateTimer >= _UpdateSpeed)
            {
                if (_NewMovement != Vector2.zero)
                    _Movement = _NewMovement;
                MoveSnake();
                _UpdateTimer = 0;
            }
        }
    }

    //Set Pickup Location/MoveSnake
    private void SetPickupLocation()
    {
        int xpos = Random.Range(1, _FieldSize.x - 1);
        int ypos = Random.Range(1, _FieldSize.y - 1);

        //Check random position
        for (int i = 0; i < _SnakeLocation.Count; i++)
            if(_SnakeLocation[i] == new Vector2Int(xpos,ypos))
            {
                SetPickupLocation();
                break;
            }

        _PickupLocation = new Vector2Int(xpos,ypos);

        for (int x = 1; x < _FieldSize.x -1; x++)
            for (int y = 1; y < _FieldSize.y -1; y++)
            {
                if (_PickupLocation == new Vector2Int(x, y))
                    _FieldObjects[x, y].SetActive(true);
                else
                    _FieldObjects[x, y].SetActive(false);
            }
    }
    private void MoveSnake()
    {
        //Apply Movement
        Vector2Int newpos = new Vector2Int(_CurrentLocation.x + _Movement.x, _CurrentLocation.y + _Movement.y);

        //Check border
        for (int i = 0; i < _BorderPositions.Count; i++)
            if(_BorderPositions[i] == newpos)
                StartCoroutine(GameOver());

        //Check if snake exist
        for (int i = 0; i < _SnakeLocation.Count; i++)
            if(_SnakeLocation[i] == newpos)
                StartCoroutine(GameOver());

        //Check for pickup
        if(newpos == _PickupLocation)
        {
            _PickupActivated = true;
            SetPickupLocation();
        }

        //Add new body / check if pickedup = true
        _SnakeLocation.Add(newpos);
        if (_PickupActivated)
            _PickupActivated = false;
        else
            _SnakeLocation.RemoveAt(0);

        //Update Current Location
        _CurrentLocation = new Vector2Int(_CurrentLocation.x + _Movement.x, _CurrentLocation.y + _Movement.y);

        //Update Objects
        for (int x = 0; x < _FieldSize.x; x++)
            for (int y = 0; y < _FieldSize.y; y++)
                _SnakeObjects[x, y].SetActive(false);
        for (int i = 0; i < _SnakeLocation.Count; i++)
            _SnakeObjects[_SnakeLocation[i].x, _SnakeLocation[i].y].SetActive(true);
    }

    //CreateField/GameOver
    private IEnumerator CreateField()
    {
        //Offset
        Vector2 offset = new Vector2(_FieldSize.x * -0.5f, _FieldSize.y * -0.5f);

        //Create Field
        for (int y = 0; y < _FieldSize.y; y++)
            for (int x = 0; x < _FieldSize.x; x++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x + offset.x, y + offset.y, 0);
                cube.transform.parent = transform;
                _FieldObjects[x, y] = cube;
                _FieldObjects[x, y].name = "FieldObj_" + x.ToString() + y.ToString();
            }
        yield return new WaitForSeconds(1f);

        //Disable Objects in playarea / Add pickup material
        for (int y = 1; y < _FieldSize.y - 1; y++)
            for (int x = 1; x < _FieldSize.x - 1; x++)
            {
                _FieldObjects[x, y].GetComponent<MeshRenderer>().material = _PickupMat;
                _FieldObjects[x,y].name = "PickupObj_" + x.ToString() + y.ToString();
                _FieldObjects[x, y].SetActive(false);
            }
        yield return new WaitForSeconds(1f);

        //Assign Material to border
        for (int y = 0; y < _FieldSize.y; y++)
            for (int x = 0; x < _FieldSize.x; x++)
                if (_FieldObjects[x, y].activeSelf)
                {
                    _BorderPositions.Add(new Vector2Int(x,y));
                    _FieldObjects[x, y].GetComponent<MeshRenderer>().material = _BorderMat;
                    yield return new WaitForSeconds(.01f);
                }
        yield return new WaitForSeconds(.5f);

        //Create Snake Objects
        for (int y = 0; y < _FieldSize.y; y++)
            for (int x = 0; x < _FieldSize.x; x++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x + offset.x, y + offset.y, 0);
                cube.name = "SnakeObj_" + x.ToString() + y.ToString();
                cube.transform.parent = transform;
                _SnakeObjects[x, y] = cube;
                _SnakeObjects[x, y].SetActive(false);
            }
        _GameRunning = true;
        yield return new WaitForSeconds(1f);
        SetPickupLocation();
    }
    private IEnumerator GameOver()
    {
        _GameRunning = false;
        Debug.Log("GameOver");

        //Flash Snake
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < _SnakeLocation.Count; j++)
            {
                _SnakeObjects[_SnakeLocation[j].x, _SnakeLocation[j].y].SetActive(true);
            }
            yield return new WaitForSeconds(.1f);
            for (int j = 0; j < _SnakeLocation.Count; j++)
            {
                _SnakeObjects[_SnakeLocation[j].x, _SnakeLocation[j].y].SetActive(false);
            }
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(1f);

        //Reset Game
        _SnakeLocation.Clear();
        SetPickupLocation();
        _NewMovement = new Vector2Int(1, 0);
        _CurrentLocation = new Vector2Int(Mathf.RoundToInt(_FieldSize.x * 0.5f), Mathf.RoundToInt(_FieldSize.y * 0.5f));
        _SnakeLocation.Add(new Vector2Int(Mathf.RoundToInt(_FieldSize.x * 0.5f), Mathf.RoundToInt(_FieldSize.y * 0.5f)));
        _GameRunning = true;
    }
}