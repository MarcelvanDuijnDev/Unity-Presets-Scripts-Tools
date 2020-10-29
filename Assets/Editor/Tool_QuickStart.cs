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
    private int _MenuID; // QuickStart/Scripts
    private int _DimensionID; // 2D/3D
    private int _Type2DID; // Platformer/TopDown
    private int _Type3DID; // FPS/ThirdPerson/TopDown/Platformer

    private bool[] _ScriptExist = new bool[18];
    private string[] _ScriptNames = new string[] { // 17 Scripts
"Bullet",
"DoEvent",
"EditorWindowExample",
"Health",
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
    private string[] _ScriptCode = new string[] // Updated: 27-oct-2020
    {
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Bullet : MonoBehaviour\n{\n    [SerializeField] private float _Speed;\n    [SerializeField] private float _Damage;\n\n    void FixedUpdate()\n    {\n        transform.Translate(Vector3.forward * _Speed * Time.deltaTime);\n    }\n\n    private void OnTriggerEnter(Collider other)\n    {\n        if (other.tag == \"ExampleTag\")\n        {\n            //DoDamage\n            gameObject.SetActive(false);\n        }\n    }\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine.Events;\nusing UnityEngine;\nusing UnityEngine.SceneManagement;\n\npublic class DoEvent : MonoBehaviour\n{\n    [SerializeField] private UnityEvent _Event;\n    [SerializeField] private bool _OnStart;\n    [SerializeField] private bool _OnUpdate;\n    [SerializeField] private bool _OnButtonPressed;\n\n    void Start()\n    {\n        if (_OnStart)\n            DoEvents();\n    }\n\n    void Update()\n    {\n        if (_OnUpdate)\n            DoEvents();\n\n        if (_OnButtonPressed)\n            if (Input.anyKey)\n                DoEvents();\n    }\n\n    private void DoEvents()\n    {\n        _Event.Invoke();\n    }\n\n    public void SetGameobject_InActive(GameObject targetobject)\n    {\n        targetobject.SetActive(false);\n    }\n\n    public void SetGameobject_Active(GameObject targetobject)\n    {\n        targetobject.SetActive(true);\n    }\n\n    public void SetGameObject_Negative(GameObject targetobject)\n    {\n        if (targetobject.activeSelf)\n            targetobject.SetActive(false);\n        else\n            targetobject.SetActive(true);\n    }\n\n    public void LoadScene(int sceneid)\n    {\n        SceneManager.LoadScene(sceneid);\n    }\n    public void LoadScene(string scenename)\n    {\n        SceneManager.LoadScene(scenename);\n    }\n    public void Quit()\n    {\n        Application.Quit();\n    }\n}\n",
        "using UnityEngine;\nusing UnityEditor;\nusing System.Collections;\n\nclass EditorWindowExample : EditorWindow\n{\n    string examplestring = \"example\";\n    bool examplebool = false;\n\n    [MenuItem(\"Tools/EditorWindowExample\")]\n    public static void ShowWindow()\n    {\n        EditorWindow.GetWindow(typeof(EditorWindowExample));\n    }\n\n    void OnGUI()\n    {\n        GUILayout.Label(\"Example Title\", EditorStyles.boldLabel);\n        examplestring = EditorGUILayout.TextField(\"Example string field\", examplestring);\n        examplebool = EditorGUILayout.Toggle(\"Example bool field\", examplebool);\n    }\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Health : MonoBehaviour\n{\n    [SerializeField] private float _MaxHealth = 100;\n\n    private float _CurrentHealth;\n\n    private void OnEnable()\n    {\n        _CurrentHealth = _MaxHealth;\n    }\n\n    public void DoDamage(float damageamount)\n    {\n        _CurrentHealth -= damageamount;\n        if (_CurrentHealth <= 0)\n        {\n            _CurrentHealth = 0;\n            gameObject.SetActive(false);\n        }\n    }\n\n    public float GetCurrentHealth()\n    {\n        return _CurrentHealth;\n    }\n    public float GetMaxHealth()\n    {\n        return GetMaxHealth();\n    }\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class LightEffects : MonoBehaviour\n{\n    private enum LightEffectOptions { Flickering };\n\n    [Header(\"Settings\")]\n    [SerializeField] private LightEffectOptions _LightEffectOption = LightEffectOptions.Flickering;\n    [SerializeField] private Vector2 _MinMaxIncrease = new Vector2(0.8f, 1.2f);\n    [Range(0.01f, 100)] [SerializeField] private float _EffectStrength = 50;\n\n    Queue<float> _LightFlickerQ;\n    private float _LastSum = 0;\n    private Light _Light;\n    private float _LightIntensity = 0;\n\n    public void Reset()\n    {\n        if (_LightEffectOption == LightEffectOptions.Flickering)\n        {\n            _LightFlickerQ.Clear();\n            _LastSum = 0;\n        }\n    }\n\n    void Start()\n    {\n        _Light = GetComponent<Light>();\n        _LightIntensity = _Light.intensity;\n        _LightFlickerQ = new Queue<float>(Mathf.RoundToInt(_EffectStrength));\n    }\n\n    void Update()\n    {\n        switch (_LightEffectOption)\n        {\n            case LightEffectOptions.Flickering:\n                while (_LightFlickerQ.Count >= _EffectStrength)\n                    _LastSum -= _LightFlickerQ.Dequeue();\n\n                float newVal = Random.Range(_LightIntensity * _MinMaxIncrease.x, _LightIntensity * _MinMaxIncrease.y);\n                _LightFlickerQ.Enqueue(newVal);\n\n               _LastSum += newVal;\n                _Light.intensity = _LastSum / (float)_LightFlickerQ.Count;\n                break;\n        }\n    }\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEngine.SceneManagement;\n\npublic class LoadScenes : MonoBehaviour\n{\n    public void Action_LoadScene(int sceneid)\n    {\n        SceneManager.LoadScene(sceneid);\n    }\n    public void Action_LoadScene(string scenename)\n    {\n        SceneManager.LoadScene(scenename);\n   }\n\n    public void Action_ReloadScene()\n    {\n        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);\n    }\n\n    public void Action_QuitApplication()\n    {\n        Application.Quit();\n    }\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\n[RequireComponent(typeof(CharacterController))]\npublic class Movement_CC : MonoBehaviour\n{\n    //Movement\n    [SerializeField] private float _NormalSpeed = 5, _SprintSpeed = 8;\n    [SerializeField] private float _JumpSpeed = 5;\n    [SerializeField] private float _Gravity = 20;\n    private Vector3 _MoveDirection = Vector3.zero;\n    //Look around\n    public float _CameraSensitivity = 1;\n    [SerializeField] private Transform _Head = null;\n    private float _RotationX = 90.0f;\n    private float _RotationY = 0.0f;\n    private float _Speed;\n\n    private CharacterController _CC;\n    private bool _LockRotation;\n\n    void Start()\n    {\n        Cursor.lockState = CursorLockMode.Locked;\n        Cursor.visible = false;\n        _CC = GetComponent<CharacterController>();\n    }\n\n    void Update()\n    {\n        //Look around\n        if (!_LockRotation)\n        {\n            _RotationX += Input.GetAxis(\"Mouse X\") * _CameraSensitivity;\n            _RotationY += Input.GetAxis(\"Mouse Y\") * _CameraSensitivity;\n            _RotationY = Mathf.Clamp(_RotationY, -90, 90);\n\n            transform.localRotation = Quaternion.AngleAxis(_RotationX, Vector3.up);\n            _Head.transform.localRotation = Quaternion.AngleAxis(_RotationY, Vector3.left);\n        }\n\n        //Movement\n        if (_CC.isGrounded)\n        {\n            _MoveDirection = new Vector3(Input.GetAxis(\"Horizontal\"), 0, Input.GetAxis(\"Vertical\"));\n            _MoveDirection = transform.TransformDirection(_MoveDirection);\n            _MoveDirection *= _Speed;\n            if (Input.GetButton(\"Jump\"))\n                _MoveDirection.y = _JumpSpeed;\n        }\n\n        //Sprint\n        if (Input.GetKey(KeyCode.LeftShift))\n            _Speed = _SprintSpeed;\n        else\n            _Speed = _NormalSpeed;\n\n        //Apply Movement\n        _MoveDirection.y -= _Gravity * Time.deltaTime;\n        _CC.Move(_MoveDirection * Time.deltaTime);\n    }\n\n    public void LockRotation(bool state)\n    {\n        _LockRotation = state;\n    }\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\n[RequireComponent(typeof(CharacterController))]\npublic class Movement_CC_TopDown : MonoBehaviour\n{\n    //Movement\n    [Header(\"Settings Camera\")]\n    [SerializeField] private Camera _Camera;\n    [Header(\"Settings\")]\n    [SerializeField] private float _NormalSpeed = 5;\n    [SerializeField] private float _SprintSpeed = 8;\n    [SerializeField] private float _JumpSpeed = 5;\n    [SerializeField] private float _Gravity = 20;\n    [SerializeField] private bool _MovementRelativeToRotation = false;\n\n    private float _Speed = 0;\n    private Vector3 _MoveDirection = Vector3.zero;\n    private CharacterController _CC;\n\n    void Start()\n    {\n        _CC = GetComponent<CharacterController>();\n    }\n\n    void Update()\n    {\n        //Movement\n        if (_CC.isGrounded)\n        {\n            _MoveDirection = new Vector3(Input.GetAxis(\"Horizontal\"), 0, Input.GetAxis(\"Vertical\"));\n            if (_MovementRelativeToRotation)\n                _MoveDirection = transform.TransformDirection(_MoveDirection);\n            _MoveDirection *= _Speed;\n            if (Input.GetButton(\"Jump\"))\n                _MoveDirection.y = _JumpSpeed;\n        }\n\n        _MoveDirection.y -= _Gravity * Time.deltaTime;\n        _CC.Move(_MoveDirection * Time.deltaTime);\n\n        //Sprint\n        if (Input.GetKey(KeyCode.LeftShift))\n            _Speed = _SprintSpeed;\n        else\n            _Speed = _NormalSpeed;\n\n        Ray cameraRay = _Camera.ScreenPointToRay(Input.mousePosition);\n        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);\n        float rayLength;\n        if (groundPlane.Raycast(cameraRay, out rayLength))\n        {\n            Vector3 pointToLook = cameraRay.GetPoint(rayLength);\n            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));\n        }\n    }\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Movement_Camera : MonoBehaviour\n{\n    private enum CameraOptionsPos { None, Follow }\n    private enum CameraOptionsRot { None, Follow }\n\n    [Header(\"Options\")]\n    [SerializeField] private CameraOptionsPos _CameraOptionPos = CameraOptionsPos.Follow;\n    [SerializeField] private CameraOptionsRot _CameraOptionRot = CameraOptionsRot.Follow;\n    [Header(\"Settings Position\")]\n    [SerializeField] private Vector3 _OffsetPosition = new Vector3(0, 12, -4);\n    [SerializeField] private bool _UseOffsetYAsDefaultHeight = true;\n    [Header(\"Settings Rotation\")]\n    [SerializeField] private Vector3 _OffsetRotation;\n\n    [Header(\"Other\")]\n    [SerializeField] private Transform _Target;\n\n    void Update()\n    {\n        // Movement\n        switch (_CameraOptionPos)\n        {\n            case CameraOptionsPos.Follow:\n                if (_UseOffsetYAsDefaultHeight)\n                    transform.position = new Vector3(_Target.position.x + _OffsetPosition.x, _OffsetPosition.y, _Target.position.z + _OffsetPosition.z);\n                else\n                    transform.position = new Vector3(_Target.position.x + _OffsetPosition.x, _Target.position.y + _OffsetPosition.y, _Target.position.z + _OffsetPosition.z);\n                break;\n        }\n        // Rotation\n        switch (_CameraOptionRot)\n        {\n            case CameraOptionsRot.Follow:\n                Vector3 rpos = _Target.position - transform.position;\n                Quaternion lookrotation = Quaternion.LookRotation(rpos, Vector3.up);\n                transform.eulerAngles = new Vector3(lookrotation.eulerAngles.x + _OffsetRotation.x, lookrotation.eulerAngles.y + _OffsetRotation.y, lookrotation.eulerAngles.z + _OffsetRotation.z);\n                break;\n        }\n    }\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Movement_FreeCamera : MonoBehaviour\n{\n    [SerializeField] private float _Speed;\n    [SerializeField] private float _SprintSpeed;\n\n    private float _CurrentSpeed;\n\n    void Start()\n    {\n        Cursor.visible = false;\n        Cursor.lockState = CursorLockMode.Locked;\n    }\n\n    void Update()\n    {\n        if (Input.GetKey(KeyCode.LeftShift))\n            _CurrentSpeed = _SprintSpeed;\n        else\n            _CurrentSpeed = _Speed;\n\n        float xas = Input.GetAxis(\"Horizontal\");\n        float zas = Input.GetAxis(\"Vertical\");\n\n        transform.Translate(new Vector3(xas, 0, zas) * _CurrentSpeed * Time.deltaTime);\n\n        float mousex = Input.GetAxis(\"Mouse X\");\n        float mousey = Input.GetAxis(\"Mouse Y\");\n        transform.eulerAngles += new Vector3(-mousey, mousex, 0);\n    }\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class ObjectPool : MonoBehaviour\n{\n\n    [SerializeField] private ObjectPool_Pool[] _ObjectPools;\n    private List<Transform> _Parents = new List<Transform>();\n\n    private void Awake()\n    {\n        GameObject emptyobject = GameObject.CreatePrimitive(PrimitiveType.Cube);\n        Destroy(emptyobject.GetComponent<MeshRenderer>());\n        Destroy(emptyobject.GetComponent<BoxCollider>());\n\n        for (int i = 0; i < _ObjectPools.Length; i++)\n        {\n            //Create parent\n            GameObject poolparent = Instantiate(emptyobject, transform.position, Quaternion.identity);\n            Destroy(poolparent.GetComponent<MeshRenderer>());\n            Destroy(poolparent.GetComponent<BoxCollider>());\n\n            //Set parent\n            poolparent.transform.parent = transform;\n            poolparent.transform.name = \"Pool_\" + _ObjectPools[i]._Name;\n            _Parents.Add(poolparent.transform);\n\n            //Create objects\n            for (int o = 0; o < _ObjectPools[i]._Amount; o++)\n            {\n                GameObject obj = (GameObject)Instantiate(_ObjectPools[i]._Prefab);\n                obj.transform.parent = poolparent.transform;\n                obj.transform.position = new Vector2(9999, 9999);\n                obj.SetActive(false);\n                _ObjectPools[i]._Objects.Add(obj);\n            }\n        }\n        Destroy(emptyobject);\n    }\n\n    public GameObject GetObject(string objname)\n    {\n        int id = FindObjectPoolID(objname);\n        return GetObject(id);\n    }\n\n    public GameObject GetObject(GameObject obj)\n    {\n        int id = FindObjectPoolID(obj);\n        return GetObject(id);\n    }\n\n    public GameObject GetObject(int id)\n    {\n        GameObject freeObject = null;\n        bool checkfreeobj = false;\n        for (int i = 0; i < _ObjectPools[id]._Objects.Count; i++)\n        {\n            if (!_ObjectPools[id]._Objects[i].activeInHierarchy)\n            {\n                _ObjectPools[id]._Objects[i].transform.position = new Vector3(0, 0, 0);\n                _ObjectPools[id]._Objects[i].SetActive(true);\n                freeObject = _ObjectPools[id]._Objects[i];\n                checkfreeobj = true;\n                break;\n            }\n        }\n\n        if (!checkfreeobj)\n        {\n            _ObjectPools[id]._Objects.Clear();\n            freeObject = (GameObject)Instantiate(_ObjectPools[id]._Prefab);\n            freeObject.transform.parent = _Parents[id];\n            _ObjectPools[id]._Objects.Add(freeObject);\n        }\n\n        return freeObject;\n    }\n\n    public List<GameObject> GetAllObjects(GameObject objtype)\n    {\n        int id = FindObjectPoolID(objtype);\n        return _ObjectPools[id]._Objects;\n    }\n\n    private int FindObjectPoolID(GameObject obj)\n    {\n        int id = 0;\n        for (int i = 0; i < _ObjectPools.Length; i++)\n        {\n            if (obj == _ObjectPools[i]._Prefab)\n            {\n\n              id = i;\n            }\n        }\n        return id;\n    }\n    private int FindObjectPoolID(string objname)\n    {\n        int id = 0;\n        for (int i = 0; i < _ObjectPools.Length; i++)\n        {\n            if (objname == _ObjectPools[i]._Name)\n            {\n                id = i;\n            }\n        }\n        return id;\n    }\n}\n\n[System.Serializable]\npublic class ObjectPool_Pool\n{\n    public string _Name;\n    public GameObject _Prefab;\n    public int _Amount;\n    [HideInInspector] public List<GameObject> _Objects;\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class ObjectPoolSimple : MonoBehaviour\n{\n    public GameObject prefabGameObject;\n    public int pooledAmount;\n\n    [HideInInspector] public List<GameObject> objects;\n\n    void Awake()\n    {\n        for (int i = 0; i < pooledAmount; i++)\n        {\n            GameObject obj = (GameObject)Instantiate(prefabGameObject);\n            obj.transform.parent = gameObject.transform;\n            obj.SetActive(false);\n            objects.Add(obj);\n        }\n    }\n}\n\n\n/* Use Pool\n\n    [SerializeField]private ObjectPoolSimple _ObjectPool;\n\n    private void Spawn() {\n        for (int i = 0; i < _ObjectPool.objects.Count; i++) {\n            if (!_ObjectPool.objects[i].activeInHierarchy) {\n                _ObjectPool.objects[i].transform.position = new Vector3(0,0,0);\n                _ObjectPool.objects[i].transform.rotation = Quaternion.Euler(0, 0, 0);\n                _ObjectPool.objects[i].SetActive(true);\n                break;\n            }\n        }\n    }\n*/\n",
        "using System;\nusing System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEngine.Events;\n\npublic class OnCollision : MonoBehaviour\n{\n    private enum Options { OnTriggerEnter, OnTriggerExit, OnTriggerStay, OnCollisionEnter, OnCollisionExit, OnCollisionStay };\n    [SerializeField] private LayerMask _LayerMask = ~0;\n    [SerializeField] private Options _Option;\n    [SerializeField] private string _Tag;\n    [SerializeField] private UnityEvent _Event;\n\n    private bool _HasTag;\n\n    private void Start()\n    {\n        if (_Tag != \"\" || _Tag != null)\n            _HasTag = true;\n}\n\n    private void Action(Collider other)\n{\n        if (_HasTag)\n            if (other.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)\n                _Event.Invoke();\n    }\n    private void Action(Collision other)\n{\n        if (_HasTag)\n            if (other.gameObject.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)\n                _Event.Invoke();\n    }\n\n    private void OnTriggerEnter(Collider other)\n{\n        if (_Option == Options.OnTriggerEnter)\n            Action(other);\n    }\n    private void OnTriggerExit(Collider other)\n{\n        if (_Option == Options.OnTriggerExit)\n            Action(other);\n    }\n    private void OnTriggerStay(Collider other)\n{\n        if (_Option == Options.OnTriggerStay)\n            Action(other);\n    }\n    void OnCollisionEnter(Collision other)\n{\n        if (_Option == Options.OnCollisionEnter)\n            Action(other);\n    }\n    void OnCollisionExit(Collision other)\n{\n        if (_Option == Options.OnCollisionExit)\n            Action(other);\n    }\n    void OnCollisionStay(Collision other)\n{\n        if (_Option == Options.OnCollisionStay)\n            Action(other);\n    }\n}\n",
        "using System;\nusing System.Collections.Generic;\nusing System.IO;\nusing UnityEngine;\nusing Random = UnityEngine.Random;\n\npublic class SaveLoad_JSON : MonoBehaviour\n{\n    private SaveData _SaveData = new SaveData();\n\n    void Start()\n    {\n        LoadData();\n    }\n\n    public void SaveData()\n    {\n        string jsonData = JsonUtility.ToJson(_SaveData, true);\n        File.WriteAllText(Application.persistentDataPath + \"/SaveData.json\", jsonData);\n    }\n    public void LoadData()\n    {\n        try\n        {\n            string dataAsJson = File.ReadAllText(Application.persistentDataPath + \"/SaveData.json\");\n            _SaveData = JsonUtility.FromJson<SaveData>(dataAsJson);\n        }\n        catch\n        {\n            SaveData();\n        }\n    }\n    public SaveData GetSaveData()\n    {\n        return _SaveData;\n    }\n    public void CreateNewSave()\n    {\n        ExampleData newsave = new ExampleData();\n        newsave.randomfloat = Random.Range(0, 100);\n        _SaveData.saveData.Add(newsave);\n    }\n}\n\n[System.Serializable]\npublic class SaveData\n{\n    public List<ExampleData> saveData = new List<ExampleData>();\n}\n[System.Serializable]\npublic class ExampleData\n{\n    public float randomfloat = 0;\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\n[CreateAssetMenu(fileName = \"Example\", menuName = \"SO/ExampleSO\", order = 1)]\npublic class ScriptebleGameObject : ScriptableObject\n{\n    public string examplestring;\n    public int exampleint;\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing TMPro;\n\npublic class StringFormats : MonoBehaviour\n{\n    private enum FormatOptions { Time };\n    [SerializeField] private FormatOptions _FormatOption;\n    [SerializeField] private TextMeshProUGUI _ExampleText;\n\n    private float _Timer;\n\n    void Update()\n    {\n        _Timer += 1 * Time.deltaTime;\n\n        switch (_FormatOption)\n        {\n            case FormatOptions.Time:\n                _ExampleText.text = string.Format(\"{0:00}:{1:00}:{2:00}\", Mathf.Floor(_Timer / 3600), Mathf.Floor((_Timer / 60) % 60), _Timer % 60);\n                break;\n        }\n    }\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEditor;\n\npublic class Tool_CreateHexagonGrid : EditorWindow\n{\n    private GameObject _CenterObj;\n    private List<GameObject> _ObjSaved = new List<GameObject>();\n    private int _TotalObjects = 100;\n\n    //Hex\n    private int _HexLengthX = 10, _HexLengthZ = 10;\n    private float _HexSize = 1;\n    private float _DistanceBetween = 1;\n\n    private bool _Center = true;\n    private bool _Invert = false;\n\n\n    [MenuItem(\"Tools/CreateHexagonGrid\")]\n    static void Init()\n    {\n        Tool_CreateHexagonGrid window = (Tool_CreateHexagonGrid)EditorWindow.GetWindow(typeof(Tool_CreateHexagonGrid));\n        window.Show();\n    }\n\n    void OnGUI()\n    {\n\n        GUILayout.BeginVertical(\"Box\");\n        _CenterObj = (GameObject)EditorGUILayout.ObjectField(\"Center Object\", _CenterObj, typeof(GameObject), true);\n        GUILayout.EndVertical();\n\n        GUILayout.BeginVertical(\"Box\");\n        _HexSize = EditorGUILayout.FloatField(\"Size: \", _HexSize);\n        _HexLengthX = EditorGUILayout.IntField(\"Collom: \", _HexLengthX);\n        _HexLengthZ = EditorGUILayout.IntField(\"Row: \", _HexLengthZ);\n\n        GUILayout.BeginHorizontal(\"Box\");\n        if (GUILayout.Button(\"Calculate Total Objects\"))\n            _TotalObjects = _HexLengthX * _HexLengthZ;\n        EditorGUILayout.LabelField(\"Total: \" + _TotalObjects.ToString());\n        GUILayout.EndHorizontal();\n\n        _Center = EditorGUILayout.Toggle(\"Center\", _Center);\n        _Invert = EditorGUILayout.Toggle(\"Invert: \", _Invert);\n        _DistanceBetween = EditorGUILayout.FloatField(\"Distance Between: \", _DistanceBetween);\n        GUILayout.EndVertical();\n\n        GUILayout.BeginVertical(\"Box\");\n        if (GUILayout.Button(\"Create\"))\n        {\n            if (_CenterObj != null)\n            {\n                if (_ObjSaved.Count > 0)\n                {\n                    for (int i = 0; i < _ObjSaved.Count; i++)\n                    {\n                        DestroyImmediate(_ObjSaved[i]);\n                    }\n                    _ObjSaved.Clear();\n                }\n\n                Vector3 objPos = _CenterObj.transform.position;\n                CreateHexagon(new Vector3(_HexLengthX, 0, _HexLengthZ));\n                SetParent();\n            }\n            else\n            {\n                Debug.Log(\"Center Object not selected!\");\n            }\n        }\n\n        if (GUILayout.Button(\"Destroy\"))\n        {\n            if (_CenterObj != null)\n            {\n                for (int i = 0; i < _ObjSaved.Count; i++)\n                {\n                    DestroyImmediate(_ObjSaved[i]);\n                }\n                _ObjSaved.Clear();\n\n\n                int childs = _CenterObj.transform.childCount;\n                for (int i = childs - 1; i >= 0; i--)\n                {\n                    DestroyImmediate(_CenterObj.transform.GetChild(i).gameObject);\n                }\n            }\n            else\n            {\n                Debug.Log(\"Center Object not selected!\");\n            }\n        }\n\n        if (GUILayout.Button(\"Confirm\"))\n        {\n            _ObjSaved.Clear();\n        }\n        GUILayout.EndVertical();\n    }\n\n    void CreateHexagon(Vector3 dimentsions)\n    {\n        Vector3 objPos = _CenterObj.transform.position;\n        if (_Center && !_Invert)\n        {\n            objPos.x -= dimentsions.x * 0.5f * 1.7321f * _HexSize;\n            objPos.z -= dimentsions.z * 0.5f * -1.5f * _HexSize;\n        }\n        if (_Center && _Invert)\n        {\n            objPos.x -= dimentsions.x * 0.5f * 1.7321f * _HexSize;\n            objPos.z += dimentsions.z * 0.5f * -1.5f * _HexSize;\n        }\n\n        for (int xas = 0; xas < dimentsions.x; xas++)\n        {\n            CreateHax(new Vector3(objPos.x + 1.7321f * _HexSize * _DistanceBetween * xas, objPos.y, objPos.z));\n            for (int zas = 1; zas < dimentsions.z; zas++)\n            {\n                float offset = 0;\n                if (zas % 2 == 1)\n                {\n                    offset = 0.86605f * _HexSize * _DistanceBetween;\n                }\n                else\n                {\n                    offset = 0;\n                }\n                if (!_Invert)\n                {\n                    CreateHax(new Vector3(objPos.x + 1.7321f * _HexSize * _DistanceBetween * xas - offset, objPos.y, objPos.z + -1.5f * _HexSize * _DistanceBetween * zas));\n                }\n                else\n                {\n                    CreateHax(new Vector3(objPos.x + 1.7321f * _HexSize * _DistanceBetween * xas - offset, objPos.y, objPos.z + +1.5f * _HexSize * _DistanceBetween * zas));\n                }\n            }\n        }\n    }\n    void CreateHax(Vector3 positions)\n    {\n        Vector3 objPos = _CenterObj.transform.position;\n\n        GameObject gridObj = GameObject.CreatePrimitive(PrimitiveType.Cube);\n        gridObj.transform.position = new Vector3(positions.x, positions.y, positions.z);\n\n        DestroyImmediate(gridObj.GetComponent<BoxCollider>());\n\n        float size = _HexSize;\n        float width = Mathf.Sqrt(3) * size;\n        float height = size * 2f;\n        Mesh mesh = new Mesh();\n        Vector3[] vertices = new Vector3[7];\n\n        for (int i = 0; i < 6; i++)\n        {\n            float angle_deg = 60 * i - 30;\n            float angle_rad = Mathf.Deg2Rad * angle_deg;\n\n            vertices[i + 1] = new Vector3(size * Mathf.Cos(angle_rad), 0f, size * Mathf.Sin(angle_rad));\n        }\n        mesh.vertices = vertices;\n\n        mesh.triangles = new int[]\n        {\n            2,1,0,\n            3,2,0,\n            4,3,0,\n            5,4,0,\n            6,5,0,\n            1,6,0\n        };\n\n        Vector2[] uv = new Vector2[7];\n        for (int i = 0; i < 7; i++)\n        {\n            uv[i] = new Vector2(\n                (vertices[i].x + -width * .5f) * .5f / size,\n                (vertices[i].z + -height * .5f) * .5f / size);\n        }\n\n        mesh.uv = uv;\n        gridObj.GetComponent<MeshFilter>().sharedMesh = mesh;\n\n        _ObjSaved.Add(gridObj);\n    }\n\n    void SetParent()\n    {\n        for (int i = 0; i < _ObjSaved.Count; i++)\n        {\n            _ObjSaved[i].transform.parent = _CenterObj.transform;\n        }\n    }\n}\n",
        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEngine.EventSystems;\n\npublic class UIEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler\n{\n    private enum UIEffectOptions { Grow, Shrink }\n    [SerializeField] private UIEffectOptions _UIEffect = UIEffectOptions.Grow;\n    [SerializeField] private Vector3 _MinDefaultMaxSize = new Vector3(0.9f, 1f, 1.1f);\n    [SerializeField] private float _IncreaseSpeed = 1;\n\n    private Vector3 _OriginalSize;\n    private bool _MouseOver;\n\n    void Start()\n    {\n        _OriginalSize = transform.localScale;\n    }\n\n    void Update()\n    {\n        switch (_UIEffect)\n        {\n            case UIEffectOptions.Grow:\n                if (_MouseOver)\n                {\n                    if (transform.localScale.y < _MinDefaultMaxSize.z)\n                        transform.localScale += new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;\n                }\n                else\n                    if (transform.localScale.y > _OriginalSize.y)\n                    transform.localScale -= new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;\n                else\n                    transform.localScale = new Vector3(_OriginalSize.y, _OriginalSize.z, _OriginalSize.z);\n                break;\n            case UIEffectOptions.Shrink:\n                if (_MouseOver)\n                {\n                    if (transform.localScale.y > _MinDefaultMaxSize.x)\n                        transform.localScale -= new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;\n                }\n                else\n                   if (transform.localScale.y < _OriginalSize.x)\n                    transform.localScale += new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;\n                else\n                    transform.localScale = new Vector3(_OriginalSize.x, _OriginalSize.y, _OriginalSize.z);\n                break;\n        }\n    }\n\n    public void OnPointerEnter(PointerEventData eventData)\n    {\n        _MouseOver = true;\n    }\n\n    public void OnPointerExit(PointerEventData eventData)\n    {\n        _MouseOver = false;\n    }\n}\n"
    };

    private bool _CreateNewScene = true;

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
            if (GUI.Button(new Rect(0,position.height - 25, position.width,25), "Refresh"))
            {
                SearchScripts();
            }

            //Settings
            EditorGUILayout.BeginHorizontal("box");
            if (_CreateNewScene)
                GUI.backgroundColor = new Color(0, 1, 0);
            else
                GUI.backgroundColor = new Color(1, 0, 0);
            if (GUILayout.Button("NewScene"))
                _CreateNewScene = !_CreateNewScene;
            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = Color.white;
            if (GUILayout.Button("Create"))
                CreateTemplate();

            //Info
            if (_DimensionID == 0)
            {
                switch (_Type2DID)
                {
                    case 0: //Platformer
                        GUILayout.Label("essential", EditorStyles.boldLabel);
                        ScriptStatus("Bullet");
                        GUILayout.Label("Extra", EditorStyles.boldLabel);
                        ScriptStatus("UIEffects");
                        ScriptStatus("DoEvent");
                        ScriptStatus("LoadScenes");

                        GUI.backgroundColor = Color.white;
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add Essential"))
                            AddScriptsMultiple(new string[] { "Bullet" });
                        if (GUILayout.Button("Add All"))
                            AddScriptsMultiple(new string[] { "Bullet", "UIEffects", "DoEvent", "LoadScenes" });
                        EditorGUILayout.EndHorizontal();
                        break;
                    case 1: //TopDown

                        break;
                }
            }
            else
            {
                switch (_Type3DID)
                {
                    case 0: //FPS
                        GUILayout.Label("essential", EditorStyles.boldLabel);
                        ScriptStatus("Bullet");
                        ScriptStatus("Movement_CC");
                        ScriptStatus("ObjectPool");
                        GUILayout.Label("Extra", EditorStyles.boldLabel);
                        ScriptStatus("UIEffects");
                        ScriptStatus("DoEvent");
                        ScriptStatus("OnCollision");
                        ScriptStatus("LoadScenes");
                        break;
                    case 1: //ThirdPerson
                        GUILayout.Label("essential", EditorStyles.boldLabel);
                        ScriptStatus("Bullet");
                        ScriptStatus("Movement_CC");
                        ScriptStatus("Movement_Camera");
                        ScriptStatus("ObjectPool");
                        GUILayout.Label("Extra", EditorStyles.boldLabel);
                        ScriptStatus("UIEffects");
                        ScriptStatus("DoEvent");
                        ScriptStatus("OnCollision");
                        ScriptStatus("LoadScenes");
                        break;
                    case 2: //TopDown
                        GUILayout.Label("essential", EditorStyles.boldLabel);
                        ScriptStatus("Bullet");
                        ScriptStatus("Movement_CC_TopDown");
                        ScriptStatus("ObjectPool");
                        GUILayout.Label("Extra", EditorStyles.boldLabel);
                        ScriptStatus("UIEffects");
                        ScriptStatus("DoEvent");
                        ScriptStatus("OnCollision");
                        ScriptStatus("LoadScenes");
                        break;
                    case 3: //Platformer

                        break;
                }
            }
        }
        else
        {
            //Refresh
            if (GUILayout.Button("Refresh"))
                SearchScripts();

            for (int i = 0; i < _ScriptNames.Length; i++)
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

    void ScriptStatus(string name)
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
    }

    void SearchScripts()
    {
        for (int i = 0; i < _ScriptNames.Length; i++)
        {
            string[] search_results = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
            for (int o = 0; o < search_results.Length; o++)
            {
                if (search_results[o].Contains(_ScriptNames[i]))
                {
                    _ScriptExist[i] = true;
                }
            }
        }
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
        SearchScripts();
    }

    void CreateTemplate()
    {
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        CreateObjects();
    }
    void CreateObjects()
    {
        //3D
        if (_DimensionID == 1)
        {
            GameObject groundCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            groundCube.name = "Ground";
            groundCube.transform.position = new Vector3(0, 0, 0);

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0, 2, 0);

            if(ScriptExist("Health"))
            {
                string UniType = "Health";
                Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
                player.AddComponent(UnityType);
            }

            GameObject cameraObj = GameObject.Find("Main Camera");

            switch (_Type3DID)
            {
                case 0: //FPS
                    groundCube.transform.localScale = new Vector3(25, 1, 25);
                    cameraObj.transform.parent = player.transform;
                    cameraObj.transform.localPosition = new Vector3(0, 0.65f, 0);
                    break;
                case 1: //ThirdPerson
                    groundCube.transform.localScale = new Vector3(25, 1, 25);
                    GameObject rotationPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    rotationPoint.name = "rotationPoint";
                    rotationPoint.transform.position = new Vector3(0, 2, 0);
                    cameraObj.transform.parent = rotationPoint.transform;
                    cameraObj.transform.localPosition = new Vector3(1, 0.65f, -1.5f);
                    rotationPoint.transform.parent = player.transform;
                    break;
                case 2: //TopDown
                    groundCube.transform.localScale = new Vector3(25, 1, 25);
                    cameraObj.transform.position = new Vector3(0, 10, -1.5f);
                    cameraObj.transform.eulerAngles = new Vector3(80, 0, 0);

                    if (ScriptExist("Movement_CC_TopDown"))
                    {
                        string UniType = "Movement_CC_TopDown";
                        Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
                        player.AddComponent(UnityType);
                    }

                    if (ScriptExist("Movement_Camera"))
                    {
                        string UniType = "Movement_Camera";
                        Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
                        cameraObj.AddComponent(UnityType);
                    }
                    break;
                case 3: //Platformer
                    groundCube.transform.localScale = new Vector3(25, 1, 1);
                    break;
            }
        }

        //2D
        if (_DimensionID == 0)
        {
            GameObject groundCube = GameObject.CreatePrimitive(PrimitiveType.Quad);
            groundCube.name = "Ground";
            groundCube.transform.position = new Vector3(0, 0, 0);

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Quad);
            player.name = "Player";
            player.transform.position = new Vector3(0, 2, 0);

            GameObject cameraObj = GameObject.Find("Main Camera");
            Camera cam = cameraObj.GetComponent<Camera>();
            cam.orthographic = true;

            //Platformer
            if (_Type2DID == 0)
            {
                groundCube.transform.localScale = new Vector3(25, 1, 1);
            }
            //TopDown
            if (_Type2DID == 1)
            {
                groundCube.transform.localScale = new Vector3(100, 100, 1);
                groundCube.transform.position = new Vector3(0, 0, 1);
            }
        }
    }
}


/*
========================================
                Info
========================================

             [Script ID]

0: Bullet.cs                    
1: DoEvent.cs                   
2: EditorWindowExample.cs       
3: Health.cs
4: LightEffects.cs              
5: LoadScenes.cs                
6: Movement_CC.cs               
7: Movement_CC_TopDown.cs       
8: Movement_Camera.cs           
9: Movement_FreeCamera.cs       
10: ObjectPool.cs                
11: ObjectPoolSimple.cs
12: OnCollision.cs
13: SaveLoad_JSON.cs
14: ScriptebleGameObject.cs
15: StringFormats.cs
16: Tool_CreateHexagonGrid.cs
17: UIEffects.cs

*/