using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

public class Tool_MapEditor : EditorWindow
{
    #region Array Icons
    //Prefab Array
    private GameObject[] _Prefabs = new GameObject[0];
    private string[] _SearchResults = new string[0];

    //Array Options
    private string _SearchPrefab = "";
    private bool _HideNames = true;
    private float _ButtonSize = 1, _CollomLength = 4;

    //Array Selection
    private int _SelectedID = 99999999, _CheckSelectedID = 999999999;
    #endregion
    #region Options
    //Options
    private bool _HideOptions = true;
    private int _OptionsStates = 0, _PlacementStates = 0;

    //Placement Option
    private float _PaintSpeed = 1, _PaintTimer = 0;
    private bool _SnapPosActive = false;

    //Onscene Options
    private bool _ShowOptionsInScene;
    private int _InScene_SelectedID;
    #endregion
    #region Transform
    //Position
    private Vector3 _MousePos, _SnapPos, _ObjectPos;
    private Vector2 _GridSize = new Vector2(1, 1);

    //Rotation/Size
    private float _Rotation, _Size = 1;
    private bool _RandomRot = false;
    private Vector2 _PrevMousePos = new Vector3(0, 0, 0);
    #endregion
    #region Check
    //Check Buttons Event
    private bool _MouseDown, _ShiftDown, _CtrlDown, _ClickMenu;
    #endregion
    #region Other
    //Placement
    private GameObject _ParentObj, _ExampleObj;

    //Other
    private Vector2 _ScrollPos1, _ClickPos;
    private Texture2D[] _PrefabIcon = new Texture2D[0];
    #endregion

    //Start Window
    [MenuItem("Tools/Map Editor  %m")]
    static void Init()
    {
        Tool_MapEditor window = EditorWindow.GetWindow(typeof(Tool_MapEditor), false, "Tool_MapEditor") as Tool_MapEditor;
        window.Show();
    }

    //Load Objects
    private void Awake()
    {
        Load_Prefabs();
        Load_Prefabs();
    }

    //Enable/Disable
    void OnEnable()
    {
        SceneView.duringSceneGui += this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnScene;
    }
    void OnDisable()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneView.duringSceneGui -= this.OnScene;
        DestroyImmediate(_ExampleObj);
    }

    //OnGUI ObjectView
    void OnGUI()
    {
        GUILayout.BeginVertical("Box");

        //Refresh/Info
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh", GUILayout.Width(80)))
        {
            FixPreview();
            Load_Prefabs();
        }
        GUILayout.Label("Loaded objects: " + _SearchResults.Length);
        GUILayout.EndHorizontal();

        //Windows
        ObjectView_Header();
        ObjectView_Objects();
        ObjectView_Options();

        GUILayout.EndVertical();
    }
    private void ObjectView_Header()
    {
        GUILayout.BeginHorizontal();
        _OptionsStates = GUILayout.Toolbar(_OptionsStates, new string[] { "Icon", "Text" });
        _ButtonSize = EditorGUILayout.Slider(_ButtonSize, 0.25f, 2);
        if (!_HideNames)
        {
            if (GUILayout.Button("Hide Names", GUILayout.Width(100)))
                _HideNames = true;
        }
        else
        {
            if (GUILayout.Button("Show Names", GUILayout.Width(100)))
                _HideNames = false;
        }
        GUILayout.EndHorizontal();
        _SearchPrefab = EditorGUILayout.TextField("Search: ", _SearchPrefab);
    }
    private void ObjectView_Objects()
    {
        Color defaultColor = GUI.backgroundColor;
        GUILayout.BeginVertical("Box");
        float calcWidth = 100 * _ButtonSize;
        _CollomLength = position.width / calcWidth;
        int x = 0;
        int y = 0;

        //Show/Hide Options
        if (_HideOptions)
            _ScrollPos1 = GUILayout.BeginScrollView(_ScrollPos1, GUILayout.Width(position.width - 20), GUILayout.Height(position.height - 109));
        else
        {
            if (_PlacementStates == 0)
                _ScrollPos1 = GUILayout.BeginScrollView(_ScrollPos1, GUILayout.Width(position.width - 20), GUILayout.Height(position.height - 235));
            else
                _ScrollPos1 = GUILayout.BeginScrollView(_ScrollPos1, GUILayout.Width(position.width - 20), GUILayout.Height(position.height - 253));
        }

        //Object Icons
        for (int i = 0; i < _SearchResults.Length; i++)
        {
            if (_Prefabs[i] != null && _Prefabs[i].name.ToLower().Contains(_SearchPrefab.ToLower()))
            {
                if (_OptionsStates == 0) //Icons
                {
                    //Select Color
                    if (_SelectedID == i) { GUI.backgroundColor = new Color(0, 1, 0); } else { GUI.backgroundColor = new Color(1, 0, 0); }

                    //Create Button
                    GUIContent content = new GUIContent();
                    content.image = _PrefabIcon[i];
                    GUI.skin.button.imagePosition = ImagePosition.ImageAbove;
                    if (!_HideNames)
                        content.text = _Prefabs[i].name;
                    if (GUI.Button(new Rect(x * 100 * _ButtonSize, y * 100 * _ButtonSize, 100 * _ButtonSize, 100 * _ButtonSize), content))
                        if (_SelectedID == i) { _SelectedID = 99999999; _CheckSelectedID = 99999999; DestroyImmediate(_ExampleObj); } else { _SelectedID = i; }

                    //Reset Button Position
                    x++;
                    if (x >= _CollomLength - 1)
                    {
                        y++;
                        x = 0;
                    }
                    GUI.backgroundColor = defaultColor;
                }
                else //Text Buttons
                {
                    if (_SelectedID == i) { GUI.backgroundColor = new Color(0, 1, 0); } else { GUI.backgroundColor = defaultColor; }
                    if (GUILayout.Button(_Prefabs[i].name))
                        if (_SelectedID == i) { _SelectedID = 99999999; _CheckSelectedID = 99999999; DestroyImmediate(_ExampleObj); } else { _SelectedID = i; }
                    GUI.backgroundColor = defaultColor;
                }
            }
        }
        if (_OptionsStates == 0)
        {
            GUILayout.Space(y * 100 * _ButtonSize + 100);
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
    private void ObjectView_Options()
    {
        GUILayout.BeginVertical("Box");
        if (!_HideOptions)
        {
            //Paint Options
            GUILayout.BeginVertical("Box");
            _PlacementStates = GUILayout.Toolbar(_PlacementStates, new string[] { "Click", "Paint" });
            if (_PlacementStates == 1)
                _PaintSpeed = EditorGUILayout.FloatField("Paint Speed: ", _PaintSpeed);
            //Parent Options
            GUILayout.BeginHorizontal();
            _ParentObj = (GameObject)EditorGUILayout.ObjectField("Parent Object: ", _ParentObj, typeof(GameObject), true);
            if (_ParentObj != null)
                if (GUILayout.Button("Clean Parent"))
                    CleanParent();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            //Grid Options
            GUILayout.BeginVertical("Box");
            _GridSize = EditorGUILayout.Vector2Field("Grid Size: ", _GridSize);
            _RandomRot = EditorGUILayout.Toggle("Random Rotation: ", _RandomRot);
            _SnapPosActive = EditorGUILayout.Toggle("Use Grid: ", _SnapPosActive);
            GUILayout.EndVertical();
        }
        //Hide/Show Options
        if (_HideOptions)
        {
            if (GUILayout.Button("Show Options"))
                _HideOptions = false;
        }
        else
        {
            if (GUILayout.Button("Hide Options"))
                _HideOptions = true;
        }
        GUILayout.EndVertical();
    }

    //OnSceneGUI
    void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(worldRay, out hitInfo))
        {
            //Check MousePosition
            _MousePos = hitInfo.point;

            //Create Example Object
            if (_SelectedID <= _Prefabs.Length)
            {
                if (_CheckSelectedID != _SelectedID)
                {
                    DestroyImmediate(_ExampleObj);
                    _ExampleObj = Instantiate(_Prefabs[_SelectedID], hitInfo.point, Quaternion.identity);
                    _ExampleObj.layer = LayerMask.NameToLayer("Ignore Raycast");
                    for (int i = 0; i < _ExampleObj.transform.childCount; i++)
                    {
                        _ExampleObj.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                        for (int o = 0; o < _ExampleObj.transform.GetChild(i).childCount; o++)
                        {
                            _ExampleObj.transform.GetChild(i).GetChild(o).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                        }
                    }
                    _ExampleObj.name = "Example Object";
                    _CheckSelectedID = _SelectedID;
                }
            }

            //Set Example Object Position + Rotation
            if (_ExampleObj != null)
            {
                _ExampleObj.transform.rotation = Quaternion.Euler(0, _Rotation, 0);
                _ExampleObj.transform.localScale = new Vector3(_Size, _Size, _Size);
                if (!e.shift && !e.control)
                {
                    if (!_SnapPosActive)
                    { _ExampleObj.transform.position = hitInfo.point; }
                    else
                    { _ExampleObj.transform.position = _SnapPos; }
                }
            }

            //Check Buttons Pressed
            if (!Event.current.alt && _SelectedID != 99999999)
            {
                if (Event.current.type == EventType.Layout)
                    HandleUtility.AddDefaultControl(0);

                //Mouse Button 0 Pressed
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    _MouseDown = true;
                    _PaintTimer = _PaintSpeed;
                    if (e.mousePosition.y <= 20)
                        _ClickMenu = true;
                }

                //Mouse Button 0 Released
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    _MouseDown = false;
                    _ClickMenu = false;
                }

                //Check Shift
                if (e.shift)
                    _ShiftDown = true;
                else
                    _ShiftDown = false;

                //Check Ctrl
                if (e.control)
                    _CtrlDown = true;
                else
                    _CtrlDown = false;

                if (e.shift || e.control)
                {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        _ClickPos = Event.current.mousePosition;
                }

                //Place Object
                if (!_ShiftDown && !_CtrlDown && !_ClickMenu)
                {
                    if (_PlacementStates == 0)
                    {
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                            CreatePrefab(hitInfo.point);
                    }
                    else
                    {
                        float timer1Final = _PaintSpeed;
                        if (_MouseDown)
                        {
                            _PaintTimer += 1 * Time.deltaTime;
                            if (_PaintTimer >= timer1Final)
                            {
                                CreatePrefab(hitInfo.point);
                                _PaintTimer = 0;
                            }
                        }
                    }
                }
            }

            // Draw obj location
            if (_SelectedID != 99999999)
            {
                //Draw Red Cross + Sphere on object location
                Handles.color = new Color(1, 0, 0);
                Handles.DrawLine(new Vector3(hitInfo.point.x - 0.3f, hitInfo.point.y, hitInfo.point.z), new Vector3(hitInfo.point.x + 0.3f, hitInfo.point.y, hitInfo.point.z));
                Handles.DrawLine(new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z - 0.3f), new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z + 0.3f));
                if (_SnapPosActive)
                {
                    Handles.SphereHandleCap(1, new Vector3(_SnapPos.x, hitInfo.point.y, _SnapPos.z), Quaternion.identity, 0.1f, EventType.Repaint);
                }
                else
                    Handles.SphereHandleCap(1, new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z), Quaternion.identity, 0.1f, EventType.Repaint);

                //Check Snap Position
                if (_SnapPosActive)
                {
                    Vector2 calc = new Vector2(_MousePos.x / _GridSize.x, _MousePos.z / _GridSize.y);
                    Vector2 calc2 = new Vector2(Mathf.RoundToInt(calc.x) * _GridSize.x, Mathf.RoundToInt(calc.y) * _GridSize.y);

                    _SnapPos = new Vector3(calc2.x, _MousePos.y, calc2.y);

                    //Draw Grid
                    Handles.color = new Color(0, 1, 0);
                    float lineLength = 0;
                    if (_GridSize.x > _GridSize.y)
                        lineLength = _GridSize.x + 1;
                    else
                        lineLength = _GridSize.y + 1;

                    for (int hor = 0; hor < 3; hor++)
                    {
                        Handles.DrawLine(new Vector3(calc2.x - lineLength, hitInfo.point.y, calc2.y - _GridSize.y + _GridSize.y * hor), new Vector3(calc2.x + lineLength, hitInfo.point.y, calc2.y - _GridSize.y + _GridSize.y * hor));
                    }
                    for (int ver = 0; ver < 3; ver++)
                    {
                        Handles.DrawLine(new Vector3(calc2.x - _GridSize.x + _GridSize.x * ver, hitInfo.point.y, calc2.y - lineLength), new Vector3(calc2.x - _GridSize.x + _GridSize.x * ver, hitInfo.point.y, calc2.y + lineLength));
                    }
                }
            }
        }
    }

    //OnScene
    void OnScene(SceneView sceneView)
    {
        //InScene Option Bar
        Handles.BeginGUI();
        if (_ShowOptionsInScene)
        {
            //Option Bar
            GUI.Box(new Rect(0, 0, Screen.width, 22), GUIContent.none);
            _InScene_SelectedID = GUI.Toolbar(new Rect(22, 1, Screen.width / 2 - 30, 20), _InScene_SelectedID, new string[] { "Settings", "Placement", "Transform", "Grid" });
            switch (_InScene_SelectedID)
            {
                case 0: //Settings
                    GUI.Label(new Rect(Screen.width / 2 - 5, 3, 50, 20), "Parent: ");
                    _ParentObj = (GameObject)EditorGUI.ObjectField(new Rect(Screen.width / 2 + 50, 1, 150, 20), _ParentObj, typeof(GameObject), true);
                    if (GUI.Button(new Rect(Screen.width - 110, 1, 90, 20), "Clean Parent"))
                    {
                        CleanParent();
                    }
                    break;
                case 1: //Placement
                    _PlacementStates = GUI.Toolbar(new Rect(Screen.width / 2 - 5, 1, 100, 20), _PlacementStates, new string[] { "Click", "Paint" });
                    _PaintSpeed = EditorGUI.FloatField(new Rect(Screen.width / 2 + 185, 1, 50, 20), _PaintSpeed);
                    GUI.Label(new Rect(Screen.width / 2 + 100, 3, 500, 20), "Paint speed: ");
                    break;
                case 2: //Transform
                    _Size = EditorGUI.FloatField(new Rect(Screen.width / 2 + 125, 1, 100, 20), _Size);
                    break;
                case 3: //Grid
                    GUI.Label(new Rect(Screen.width / 2 + 80, 3, 100, 20), "Grid Size: ");
                    _GridSize.x = EditorGUI.FloatField(new Rect(Screen.width / 2 + 150, 1, 50, 20), _GridSize.x);
                    _GridSize.y = EditorGUI.FloatField(new Rect(Screen.width / 2 + 200, 1, 50, 20), _GridSize.y);
                    GUI.Label(new Rect(Screen.width / 2, 3, 100, 20), "Enable: ");
                    _SnapPosActive = EditorGUI.Toggle(new Rect(Screen.width / 2 + 50, 3, 20, 20), _SnapPosActive);
                    break;
            }
        }

        //Hotkeys Resize / Rotate
        //Shift+MouseDown = Resize
        Vector2 prevmove = _PrevMousePos - Event.current.mousePosition;
        if (_ShiftDown && _MouseDown)
        {
            _Size = EditorGUI.Slider(new Rect(_ClickPos.x - 15, _ClickPos.y - 40, 50, 20), _Size, 0.01f, 1000000);
            _Size -= (prevmove.x + prevmove.y) * 0.05f;
            GUI.Label(new Rect(_ClickPos.x - 50, _ClickPos.y - 40, 500, 20), "Size: ");
        }
        //Ctrl+MouseDown = Rotate
        if (_CtrlDown && _MouseDown)
        {
            _Rotation = EditorGUI.Slider(new Rect(_ClickPos.x - 15, _ClickPos.y - 40, 50, 20), _Rotation, -1000000, 1000000);
            _Rotation += prevmove.x + prevmove.y;
            GUI.Label(new Rect(_ClickPos.x - 80, _ClickPos.y - 40, 500, 20), "Rotation: ");
        }
        _PrevMousePos = Event.current.mousePosition;

        //Inscene Show OptionButton
        GUI.color = new Color(1f, 1f, 1f, 1f);
        if (!_ShowOptionsInScene)
        {
            if (GUI.Button(new Rect(1, 1, 20, 20), " +"))
                _ShowOptionsInScene = true;
        }
        else
        {
            if (GUI.Button(new Rect(1, 1, 20, 20), " -"))
                _ShowOptionsInScene = false;
        }
        Handles.EndGUI();
    }

    //Load/Fix
    void Load_Prefabs()
    {
        _SearchResults = System.IO.Directory.GetFiles("Assets/", "*.prefab", System.IO.SearchOption.AllDirectories);
        _Prefabs = new GameObject[_SearchResults.Length];
        _PrefabIcon = new Texture2D[_SearchResults.Length];

        for (int i = 0; i < _SearchResults.Length; i++)
        {
            Object prefab = null;
            prefab = AssetDatabase.LoadAssetAtPath(_SearchResults[i], typeof(GameObject));
            _Prefabs[i] = prefab as GameObject;
            _PrefabIcon[i] = AssetPreview.GetAssetPreview(_Prefabs[i]);
        }
    }
    void FixPreview()
    {
        Load_Prefabs();
        _SearchResults = System.IO.Directory.GetFiles("Assets/", "*.prefab", System.IO.SearchOption.AllDirectories);

        for (int i = 0; i < _SearchResults.Length; i++)
        {
            if (_PrefabIcon[i] == null)
                AssetDatabase.ImportAsset(_SearchResults[i]);
        }
        Load_Prefabs();
    }

    //Create Prefab/Clean Parent
    void CreatePrefab(Vector3 createPos)
    {
        GameObject createdObj = PrefabUtility.InstantiatePrefab(_Prefabs[_SelectedID]) as GameObject;
        createdObj.transform.position = createPos;
        createdObj.transform.localScale = new Vector3(_Size, _Size, _Size);

        if(_ParentObj == null)
        {
            _ParentObj = new GameObject();
            _ParentObj.name = "MapEditor_Parent";
        }

        createdObj.transform.parent = _ParentObj.transform;
        if (_SnapPosActive)
            createdObj.transform.position = _SnapPos;
        else
            createdObj.transform.position = _MousePos;
        if (_RandomRot)
            createdObj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        else
            createdObj.transform.rotation = Quaternion.Euler(0, _Rotation, 0);
    }
    void CleanParent()
    {
        int childAmount = _ParentObj.transform.childCount;
        int childCalc = childAmount - 1;
        for (int i = 0; i < childAmount; i++)
        {
            DestroyImmediate(_ParentObj.transform.GetChild(childCalc).gameObject);
            childCalc -= 1;
        }
    }
}