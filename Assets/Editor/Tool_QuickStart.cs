using System.Collections.Generic;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Tool_QuickStart : EditorWindow
{
    //Navigation Tool
    int _MenuID = 0;        // QuickStart/Scripts/QuickUI
    int _DimensionID = 0;   // 2D/3D
    int _Type2DID = 0;      // Platformer/TopDown/VisualNovel
    int _Type3DID = 0;      // FPS/ThirdPerson/TopDown/Platformer
    bool _SelectWindow = false;

    //Navigation Tool Windows
    int _WindowID = 0;      // Default/FileFinder/ScriptToString/MapEditor
    string[] _WindowNames = new string[] {"Home","FileFinder","ScriptToString","MapEditor" };

    //Scripts
    Tool_QuickStart_Script[] QuickStart_Scripts = new Tool_QuickStart_Script[] {
        //                              NAME                      TAGS                        STATE         CODE
        new Tool_QuickStart_Script("AudioHandler",          "Audio_Handler",                "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine.SceneManagement;\n using UnityEngine;\n using UnityEngine.Audio;\n  public class AudioHandler : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] private bool _RefreshSettingsOnUpdate = false;\n     [SerializeField] private AudioMixerGroup _AudioMixer = null;\n      [Header(\"Audio\")]\n     [SerializeField] private List<AudioHandler_Sound> _Sound = new List<AudioHandler_Sound>();\n      private string _CurrentScene;\n      void Start()     {         //PlayOnStart         \nfor (int i = 0; i < _Sound.Count; i++)\n         {             //AudioSource             \nif (_Sound[i].Settings.CreateAudioSource)             {                 _Sound[i].Settings.AudioSource = this.gameObject.AddComponent<AudioSource>();\n                 _Sound[i].Settings.AudioSource.outputAudioMixerGroup = _AudioMixer;\n             }\n              //AudioClip             \n_Sound[i].Settings.AudioSource.clip = _Sound[i].Settings.AudioClip;\n              //Settings             \nif (_Sound[i].AudioSettings.PlayOnStart)             {                 _Sound[i].Settings.AudioSource.playOnAwake = _Sound[i].AudioSettings.PlayOnStart;\n                 _Sound[i].Settings.AudioSource.Play();\n             }\n             if (_Sound[i].AudioEffects.FadeIn)             {                 _Sound[i].Settings.AudioSource.volume = 0;\n                 _Sound[i].AudioEffects.FadeInSpeed = _Sound[i].AudioSettings.Volume / _Sound[i].AudioEffects.FadeInDuration;\n             }\n             if (_Sound[i].AudioEffects.FadeOut)             {                 _Sound[i].AudioEffects.FadeOutSpeed = _Sound[i].AudioSettings.Volume / _Sound[i].AudioEffects.FadeOutDuration;\n             }\n         }\n          RefreshSettings();\n     }\n      void Update()     {         CheckNewScene();\n          if (_RefreshSettingsOnUpdate)             RefreshSettings();\n          for (int i = 0; i < _Sound.Count; i++)\n         {             //FadeIn             \nif (_Sound[i].AudioEffects.FadingIn)             {                 if (_Sound[i].AudioEffects.FadeIn && !_Sound[i].AudioEffects.FadeInDone)                 {                     if (_Sound[i].Settings.AudioSource.volume < _Sound[i].AudioSettings.Volume)                     {                         _Sound[i].Settings.AudioSource.volume += _Sound[i].AudioEffects.FadeInSpeed * Time.deltaTime;\n                     }\n                     else                     {                         _Sound[i].AudioEffects.FadeInDone = true;\n                         _Sound[i].Settings.AudioSource.volume = _Sound[i].AudioSettings.Volume;\n                     }\n                 }\n             }\n             //FadeOut             \nif (_Sound[i].AudioEffects.FadingOut)             {                 if (_Sound[i].AudioEffects.FadeOutAfterTime > -0.1f)                 {                     _Sound[i].AudioEffects.FadeOutAfterTime -= 1 * Time.deltaTime;\n                 }\n                 else                 {                     if (_Sound[i].AudioEffects.FadeOut && !_Sound[i].AudioEffects.FadeOutDone)                     {                         if (_Sound[i].Settings.AudioSource.volume > 0)                         {                             _Sound[i].Settings.AudioSource.volume -= _Sound[i].AudioEffects.FadeOutSpeed * Time.deltaTime;\n                         }\n                         else                         {                             _Sound[i].AudioEffects.FadeOutDone = true;\n                             _Sound[i].Settings.AudioSource.volume = 0;\n                             _Sound[i].Settings.AudioSource.Stop();\n                         }\n                     }\n                 }\n             }\n         }\n     }\n      private void CheckNewScene()     {         if (_CurrentScene != SceneManager.GetActiveScene().name)         {             _CurrentScene = SceneManager.GetActiveScene().name;\n             for (int i = 0; i < _Sound.Count; i++)\n             {                 for (int o = 0; o < _Sound[i].AudioControl.StartAudioOnScene.Count; o++)\n                 {                     if (_Sound[i].AudioControl.StartAudioOnScene[o] == _CurrentScene)                     {                         //FadeIn                         \nif (_Sound[i].AudioEffects.FadeIn)                         {                             _Sound[i].AudioEffects.FadingOut = false;\n                             _Sound[i].AudioEffects.FadeInDone = false;\n                             _Sound[i].AudioEffects.FadingIn = true;\n                         }\n                         _Sound[i].Settings.AudioSource.Play();\n                     }\n                 }\n                 for (int o = 0; o < _Sound[i].AudioControl.StopAudioOnScene.Count; o++)\n                 {                     if (_Sound[i].AudioControl.StopAudioOnScene[o] == _CurrentScene)                     {                         //FadeOut                         \nif (_Sound[i].AudioEffects.FadeOut && !_Sound[i].AudioEffects.FadingOut)                         {                             _Sound[i].AudioEffects.FadingIn = false;\n                             _Sound[i].AudioEffects.FadeOutDone = false;\n                             _Sound[i].AudioEffects.FadingOut = true;\n                         }\n                         else                             _Sound[i].Settings.AudioSource.Stop();\n                     }\n                 }\n             }\n         }\n     }\n      public void PlayTrack(string trackname)     {         for (int i = 0; i < _Sound.Count; i++)\n         {             if (_Sound[i].AudioTrackName == trackname)                 AudioHandler_PlayTrack(i);\n         }\n     }\n     private void AudioHandler_PlayTrack(int trackid)     {         _Sound[trackid].Settings.AudioSource.Play();\n     }\n     public void RefreshSettings()     {         for (int i = 0; i < _Sound.Count; i++)\n         {             //SetClip             \nif (_Sound[i].Settings.AudioSource.clip != _Sound[i].Settings.AudioClip)                 _Sound[i].Settings.AudioSource.clip = _Sound[i].Settings.AudioClip;\n             //SetEffects             \nif (!_Sound[i].AudioEffects.FadeIn || _Sound[i].AudioEffects.FadeIn && _Sound[i].AudioEffects.FadeInDone)                 _Sound[i].Settings.AudioSource.volume = _Sound[i].AudioSettings.Volume;\n             _Sound[i].Settings.AudioSource.loop = _Sound[i].AudioSettings.Loop;\n         }\n     }\n      public void SetAudioSource(string trackname, AudioSource audiosource)     {         for (int i = 0; i < _Sound.Count; i++)\n         {             if (_Sound[i].AudioTrackName == trackname)                 _Sound[i].Settings.AudioSource = audiosource;\n         }\n     }\n }\n  [System.Serializable] public class AudioHandler_Sound {     public string AudioTrackName;\n     public AudioHandler_Settings Settings;\n     public AudioHandler_AudioSettings AudioSettings;\n     public AudioHandler_Control AudioControl;\n     public AudioHandler_Effects AudioEffects;\n }\n  [System.Serializable] public class AudioHandler_Settings {     [Header(\"AudioClip\")]\n     public AudioClip AudioClip;\n      [Header(\"AudioSource\")]\n     public AudioSource AudioSource;\n     public bool CreateAudioSource;\n }\n  [System.Serializable] public class AudioHandler_AudioSettings {     [Header(\"AudioSettings\")]\n     [Range(0, 1)] public float Volume = 1;\n     public bool Loop;\n     public bool PlayOnStart;\n }\n  [System.Serializable] public class AudioHandler_Control {     [Header(\"Enable/Disable Song\")]\n     public List<string> StartAudioOnScene = new List<string>();\n     public List<string> StopAudioOnScene = new List<string>();\n     public bool StopOnNextScene;\n     [HideInInspector] public int SceneEnabled;\n }\n  [System.Serializable] public class AudioHandler_Effects {     [Header(\"FadeIn\")]\n     public bool FadeIn;\n     public float FadeInDuration;\n     [HideInInspector] public float FadeInSpeed;\n     [HideInInspector] public bool FadeInDone;\n     [HideInInspector] public bool FadingIn;\n     [Header(\"FadeOut\")]\n     public bool FadeOut;\n     public float FadeOutAfterTime;\n     public float FadeOutDuration;\n     [HideInInspector] public float FadeOutSpeed;\n     [HideInInspector] public bool FadeOutDone;\n     [HideInInspector] public bool FadingOut;\n }"),
        new Tool_QuickStart_Script("BasicNavMeshAI",        "AI_NavMesh",                   "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.AI;\n  public class BasicNavMeshAI : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] private Transform _Target = null;     [SerializeField] private float _Speed = 2;      private NavMeshAgent _Agent;      private void Awake()\n     {         if (_Target == null)         {             try             {                 _Target = GameObject.Find(\"Player\").transform;             }\n             catch             {                 Debug.Log(\"No Target\")\n;\n             }\n         }\n          _Agent = GetComponent<NavMeshAgent>();\n         _Agent.speed = _Speed;\n     }\n      private void Update()     {         if (_Target != null)         {             _Agent.SetDestination(_Target.position);\n         }\n     }\n }\n "),
        new Tool_QuickStart_Script("Bullet",                "Shooting_Bullet",              "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Bullet : MonoBehaviour {     [SerializeField] private float _Speed = 5;\n     [SerializeField] private float _Damage = 25;\n      void FixedUpdate()     {         transform.Translate(Vector3.forward * _Speed * Time.deltaTime)\n;\n     }\n      private void OnTriggerEnter(Collider other)     {         if(other.tag == \"ExampleTag\")         {             other.GetComponent<Health>().DoDamage(_Damage);\n             gameObject.SetActive(false);\n         }\n     }\n }"),
        new Tool_QuickStart_Script("CarArcade",             "Car_Drive_Vehicle",            "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class CarArcade : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] private float _ForwardAccel = 8;\n     [SerializeField] private float _ReverseAccel = 4;\n     [SerializeField] private float _TurnStrength = 180;\n     [SerializeField] private float _GravityForce = 15;\n      [Header(\"GroundCheck\")]\n     [SerializeField] private LayerMask _GroundMask = ~0;\n     [SerializeField] private float _GroundCheckLength = 0.5f;\n      [Header(\"RigidBody\")]\n     [SerializeField] private Rigidbody _RB = null;\n      private float _SpeedInput;\n     private float _TurnInput;\n     private bool _Grounded;\n      void Start() => _RB.transform.parent = null;      void Update()\n     {         _SpeedInput = 0;\n         if(Input.GetAxis(\"Vertical\") > 0)             _SpeedInput = Input.GetAxis(\"Vertical\") * _ForwardAccel * 1000;\n         else if(Input.GetAxis(\"Vertical\") < 0)             _SpeedInput = Input.GetAxis(\"Vertical\") * _ReverseAccel * 1000;\n          _TurnInput = Input.GetAxis(\"Horizontal\");\n          if(_Grounded)         transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, _TurnInput * _TurnStrength * Time.deltaTime, 0)\n);\n          transform.position = _RB.transform.position;     }\n      private void FixedUpdate()\n     {         _Grounded = GroundCheck();\n          if (_Grounded)         {             if (Mathf.Abs(_SpeedInput) > 0)                 _RB.AddForce(transform.forward * _SpeedInput)\n;\n         }\n         else             _RB.AddForce(Vector3.up * -_GravityForce * 100);\n     }\n      private bool GroundCheck()     {         _Grounded = false;\n         RaycastHit hit;\n          if(Physics.Raycast(transform.position, -transform.up, out hit, _GroundCheckLength, _GroundMask)\n)         {             _Grounded = true;\n             transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal)\n * transform.rotation;         }\n          return _Grounded;     }\n }\n "),
        new Tool_QuickStart_Script("CarRealistic",          "Car_Drive_Vehicle",            "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class CarRealistic : MonoBehaviour {     [Header(\"Motor\")]\n     [SerializeField] private List<AxleInfo> axleInfos = null;\n     [SerializeField] private float maxMotorTorque = 1000;\n      [Header(\"Steering\")]\n     [SerializeField] private float maxSteeringAngle = 50;\n      public void FixedUpdate()     {         float motor = maxMotorTorque * Input.GetAxis(\"Vertical\");\n         float steering = maxSteeringAngle * Input.GetAxis(\"Horizontal\");\n          foreach (AxleInfo axleInfo in axleInfos)\n         {             if (axleInfo.steering)             {                 axleInfo.leftWheel.steerAngle = steering;\n                 axleInfo.rightWheel.steerAngle = steering;\n             }\n             if (axleInfo.motor)             {                 axleInfo.leftWheel.motorTorque = motor;\n                 axleInfo.rightWheel.motorTorque = motor;\n             }\n         }\n     }\n  }\n  [System.Serializable] public class AxleInfo {     public WheelCollider leftWheel;\n     public WheelCollider rightWheel;\n     public bool motor;\n     public bool steering;\n }"),
        new Tool_QuickStart_Script("DebugCommandBase",      "Debug_Console",                "stable",     "using System;\n using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class DebugCommandBase {     private string _CommandID;\n     private string _CommandDescription;\n     private string _CommandFormat;\n      public string CommandID { get { return _CommandID;\n }\n }\n     public string CommandDescription { get { return _CommandDescription;\n }\n }\n     public string CommandFormat { get { return _CommandFormat;\n }\n }\n      public DebugCommandBase(string id, string description, string format)\n     {         _CommandID = id;\n         _CommandDescription = description;\n         _CommandFormat = format;     }\n }\n  public class DebugCommand : DebugCommandBase {     private Action command;      public DebugCommand(string id, string description, string format, Action command)\n : base (id, description, format)\n     {         this.command = command;\n     }\n      public void Invoke()     {         command.Invoke();\n     }\n }\n  public class DebugCommand<T1> : DebugCommandBase {     private Action<T1> command;\n      public DebugCommand(string id, string description, string format, Action<T1> command)\n : base (id, description, format)\n     {         this.command = command;\n     }\n      public void Invoke(T1 value)     {         command.Invoke(value);\n     }\n }"),
        new Tool_QuickStart_Script("DebugConsole",          "Debug_Console",                "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class DebugConsole : MonoBehaviour {     private bool _ShowConsole;\n     private bool _ShowHelp;\n     private string _Input;\n     private Vector2 _Scroll;\n      public static DebugCommand TEST;\n     public static DebugCommand HELP;\n     public static DebugCommand HIDEHELP;\n     public static DebugCommand<float> SETVALUE;\n      public List<object> commandList;\n      private void Awake()     {         HELP = new DebugCommand(\"help\", \"Shows a list of commands\", \"help\", () =>         {             _ShowHelp = !_ShowHelp;\n         }\n);\n          HIDEHELP = new DebugCommand(\"hidehelp\", \"hide help info\", \"hidehelp\", () =>         {             _ShowHelp = false;\n         }\n);\n          TEST = new DebugCommand(\"test\", \"example command\", \"test\", () =>         {             Debug.Log(\"test command triggered\");\n         }\n);\n          SETVALUE = new DebugCommand<float>(\"setvalue\", \"example set value\", \"setvalue <value>\", (x) =>         {             Debug.Log(\"Value added: \" + x.ToString());\n         }\n);\n          commandList = new List<object>         {                      HELP,             HIDEHELP,             TEST,             SETVALUE         }\n;\n     }\n      private void OnGUI()     {         //Check input         \nif (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F1)         {             _ShowConsole = !_ShowConsole;\n         }\n          if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return && _ShowConsole)         {             HandleInput();\n             _Input = \"\";\n         }\n          //Console active         \nif (!_ShowConsole) return;\n          GUI.FocusControl(\"FOCUS\");\n                  float y = 0f;\n          if(_ShowHelp)         {             GUI.Box(new Rect(0, y, Screen.width, 100), \"\");\n             Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);\n             _Scroll = GUI.BeginScrollView(new Rect(0, y + 5, Screen.width, 90), _Scroll, viewport);\n              for (int i=0; i<commandList.Count; i++)\n             {                 DebugCommandBase command = commandList[i] as DebugCommandBase;\n                 string label = $\"{command.CommandFormat}\n - {command.CommandDescription}\";\n Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);\n GUI.Label(labelRect, label);\n }\n GUI.EndScrollView();\n y += 100;\n         }\n GUI.Box(new Rect(0, y, Screen.width, 30), \"\");\n          GUI.backgroundColor = new Color(0,0,0,0);\n         GUI.SetNextControlName(\"FOCUS\");\n         _Input = GUI.TextField(new Rect(10, y + 5, Screen.width - 20, 20), _Input);\n     }\n      private void HandleInput()     {         string[] properties = _Input.Split(' ');\n          for(int i=0; i < commandList.Count; i++)\n         {             DebugCommandBase commandBase = commandList[i] as DebugCommandBase;\n              if (_Input.Contains(commandBase.CommandID))             {                 if (commandList[i] as DebugCommand != null)                     (commandList[i] as DebugCommand).Invoke();\n                 else if (commandList[i] as DebugCommand<int> != null && properties.Length > 1)                     if (CheckInput(properties[1]))                         (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));\n             }\n         }\n     }\n      private bool CheckInput(string str)     {         foreach (char c in str)\n         {             if (c < '0' || c > '9')                 return false;\n         }\n         return true;\n     }\n }"),
        new Tool_QuickStart_Script("DialogSystem",          "Dialog",                       "wip",        "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class DialogSystem : MonoBehaviour {       void Start()     {              }\n      void Update()     {              }\n }\n "),
        new Tool_QuickStart_Script("Disable",               "Practicle",                    "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Disable : MonoBehaviour {      [SerializeField] private GameObject _Object;\n      public void DisableObject(float seconds) {         StartCoroutine(StartDisable(seconds));\n     }\n      private IEnumerator StartDisable(float seconds)     {         yield return new WaitForSeconds(seconds);\n         _Object.SetActive(false);\n     }\n }\n "),
        new Tool_QuickStart_Script("DoEvent",               "Practical_Event_UnityEvent",   "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine.Events;\n using UnityEngine;\n using UnityEngine.SceneManagement;\n  public class DoEvent : MonoBehaviour {     [SerializeField] private UnityEvent _Event = null;\n     [SerializeField] private bool _OnStart = false;\n     [SerializeField] private bool _OnUpdate = false;\n     [SerializeField] private bool _OnButtonPressed = false;\n      void Start()     {         if (_OnStart)             DoEvents();\n     }\n      void Update()     {         if (_OnUpdate)             DoEvents();\n          if (_OnButtonPressed)             if (Input.anyKey)                 DoEvents();\n     }\n      private void DoEvents()     {         _Event.Invoke();\n     }\n      public void SetGameobject_InActive(GameObject targetobject)     {         targetobject.SetActive(false);\n     }\n      public void SetGameobject_Active(GameObject targetobject)     {         targetobject.SetActive(true);\n     }\n      public void SetGameObject_Negative(GameObject targetobject)     {         if (targetobject.activeSelf)             targetobject.SetActive(false);\n         else             targetobject.SetActive(true);\n     }\n      public void LoadScene(int sceneid)     {         SceneManager.LoadScene(sceneid);\n     }\n     public void LoadScene(string scenename)     {         SceneManager.LoadScene(scenename);\n     }\n     public void Quit()     {         Application.Quit();\n     }\n }"),
        new Tool_QuickStart_Script("DontDestroy",           "Practical",                    "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class DontDestroy : MonoBehaviour {     void Start()     {         DontDestroyOnLoad(this.gameObject);\n     }\n }"),
        new Tool_QuickStart_Script("EditorWindowExample",   "Editor_Window",                "stable",     "using UnityEngine;\n using UnityEditor;\n using System.Collections;\n  class EditorWindowExample : EditorWindow {     string examplestring = \"example\";\n     bool examplebool = false;\n      [MenuItem(\"Tools/EditorWindowExample\")]     public static void ShowWindow()     {         EditorWindow.GetWindow(typeof(EditorWindowExample));\n     }\n      void OnGUI()     {         GUILayout.Label(\"Example Title\", EditorStyles.boldLabel);\n         examplestring = EditorGUILayout.TextField(\"Example string field\", examplestring);\n         examplebool = EditorGUILayout.Toggle(\"Example bool field\", examplebool);\n     }\n }"),
        new Tool_QuickStart_Script("EnemySpawnHandler",     "Enemy_Spawn_Handler",          "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class EnemySpawnHandler : MonoBehaviour {     private enum Options {Endless, Waves}\n      [Header(\"Settings\")]\n     [SerializeField] private Options _Option = Options.Endless;\n     [SerializeField] private int _Seed = 0;\n     [SerializeField] private bool _SetRandomSeed = true;\n      [Header(\"Object Pool\")]\n     [SerializeField] private ObjectPool _ObjectPool = null;\n      [Header(\"Enemies\")]\n     [SerializeField] private EnemySpawnHandler_Enemy[] _Enemies = null;\n      [Header(\"SpawnLocations\")]\n     [SerializeField] private Transform[] _SpawnLocations = null;      [Header(\"Settings - Endless\")\n]\n     [SerializeField] private float _SpawnRate = 5;\n // Seconds between spawning     \n[SerializeField] private float _SpawnRateEncrease = 0.05f;\n // Decrease time between spawning per sec     \n[SerializeField] private bool _RandomEnemy = true;\n     [SerializeField] private bool _RandomSpawn = true;\n      [Header(\"Settings - Waves\")]\n     [SerializeField] private EnemySpawnHandler_WaveSettings _Waves = null;\n      private float _Timer = 0;\n     private int _CurrentWave = 0;\n     private int _CheckWave = 999;\n     private float _TimerBetweenWaves = 0;\n     private float _SpawnSpeed = 0;\n      private void Start()     {         if (_SetRandomSeed)             Random.InitState(Random.Range(0, 10000));\n         else             Random.InitState(_Seed);\n          if (_Waves.WaveOption == EnemySpawnHandler_WaveSettings.WaveOptions.Generate)             GenerateWaves();\n         if (_Waves.WaveOption == EnemySpawnHandler_WaveSettings.WaveOptions.Endless)         {             _Waves.WaveAmount = 1;\n             GenerateWaves();\n             GenerateWaves();\n         }\n     }\n      void Update()     {         _Timer += 1 * Time.deltaTime;\n          switch (_Option)         {             case Options.Endless:                 Update_Endless();\n                 break;\n             case Options.Waves:                 Update_Waves();\n                 break;\n         }\n     }\n      //Update     \nprivate void Update_Endless()     {         if (_Timer >= _SpawnRate)         {             int randomenemyid = 0;\n             int randomspawnid = 0;\n             if (_RandomEnemy)                  randomenemyid = Random.Range(0, _Enemies.Length);\n             if (_RandomSpawn)                 randomspawnid = Random.Range(0, _SpawnLocations.Length);\n             Spawn(randomenemyid, randomspawnid);\n             _Timer = 0;\n         }\n         _SpawnRate -= _SpawnRateEncrease * Time.deltaTime;\n     }\n     private void Update_Waves()     {         if (_CurrentWave < _Waves.WaveAmount)         {             if (_CheckWave != _CurrentWave)             {                 //Get info / time between                 \n_TimerBetweenWaves += 1 * Time.deltaTime;\n                 if (_TimerBetweenWaves >= _Waves.TimeBetweenWaves)                 {                     _TimerBetweenWaves = 0;\n                     _CheckWave = _CurrentWave;\n                     _SpawnSpeed = _Waves.Waves[_CurrentWave].SpawnDuration / _Waves.Waves[_CurrentWave].TotalEnemies;\n                     if (_Waves.WaveOption == EnemySpawnHandler_WaveSettings.WaveOptions.Endless)                         GenerateWaves();\n                 }\n             }\n             else             {                 //Spawn                 \nif (_Waves.Waves[_CurrentWave].TotalEnemies > 0)                 {                     if (_Timer > _SpawnSpeed)                     {                         bool spawncheck = false;\n                         while (!spawncheck)                         {                             int spawnid = Random.Range(0, _Enemies.Length);\n                             if (_Waves.Waves[_CurrentWave].EnemyID[spawnid] > 0)                             {                                 Spawn(spawnid, Random.Range(0, _SpawnLocations.Length));\n                                 _Waves.Waves[_CheckWave].EnemyID[spawnid]--;\n                                 _Waves.Waves[_CurrentWave].TotalEnemies--;\n                                 spawncheck = true;\n                             }\n                         }\n                         _Timer = 0;\n                     }\n                 }\n                 else                 {                     _CurrentWave++;\n                 }\n             }\n         }\n     }\n      //Generate Waves     \nprivate void GenerateWaves()     {         int enemytypes = _Enemies.Length;\n         for (int i = 0; i < _Waves.WaveAmount; i++)\n         {             EnemySpawnHandler_Wave newwave = new EnemySpawnHandler_Wave();\n             int enemyamount = Mathf.RoundToInt(_Waves.EnemyAmount * ((_Waves.EnemyIncreaseAmount * i) + 1));\n              //Set enemy amount             \nnewwave.EnemyID = new int[enemytypes];\n             int checkenemyamount = 0;\n             newwave.TotalEnemies = enemyamount;\n              while (checkenemyamount < enemyamount)             {                 for (int j = 0; j < enemytypes; j++)\n                 {                     if (_Enemies[j].StartWave <= i)                     {                         int addamount = 0;\n                         if (enemyamount < 2)                             addamount = Random.Range(0, enemyamount);\n                         else                             addamount = Random.Range(0, Mathf.RoundToInt(enemyamount*0.5f));\n                          if (enemyamount > checkenemyamount + addamount)                         {                             newwave.EnemyID[j] += addamount;\n                             checkenemyamount += addamount;\n                         }\n                         else                         {                             newwave.EnemyID[j] += enemyamount - checkenemyamount;\n                             checkenemyamount = enemyamount;\n                             continue;\n                         }\n                     }\n                 }\n             }\n             _Waves.Waves.Add(newwave);\n         }\n     }\n      public void Spawn(int enemyid, int spawnid)     {         GameObject obj = _ObjectPool.GetObjectPrefabName(_Enemies[enemyid].EnemyPrefab.name, false);\n         obj.transform.position = _SpawnLocations[spawnid].position;         obj.SetActive(true)\n;\n     }\n }\n  [System.Serializable] public class EnemySpawnHandler_Enemy {     public string EnemyName;\n     public GameObject EnemyPrefab;\n      [Header(\"Settings\")]\n     public int StartWave;\n }\n  [System.Serializable] public class EnemySpawnHandler_WaveSettings {     public enum WaveOptions {Endless, Manually, Generate}\n     public WaveOptions WaveOption;\n      [Header(\"Endless\")]\n     public float EnemyIncreaseAmount;\n      [Header(\"Manual\")]\n     public List<EnemySpawnHandler_Wave> Waves;\n      [Header(\"Generate\")]\n     public int WaveAmount;\n     public int EnemyAmount;\n      [Header(\"Other\")]\n     public float TimeBetweenWaves;\n }\n  [System.Serializable] public class EnemySpawnHandler_Wave {     public int[] EnemyID;\n     public float SpawnDuration = 5;\n      [HideInInspector] public int TotalEnemies;\n }"),
        new Tool_QuickStart_Script("Health",                "Health",                       "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Health : MonoBehaviour {     [SerializeField] private float _MaxHealth = 100;\n      private float _CurrentHealth;\n      private void OnEnable()     {         _CurrentHealth = _MaxHealth;\n     }\n      public void DoDamage(float damageamount)     {         _CurrentHealth -= damageamount;\n         if(_CurrentHealth <= 0)         {             _CurrentHealth = 0;\n             gameObject.SetActive(false);\n         }\n     }\n      public float GetCurrentHealth()     {         return _CurrentHealth;\n     }\n     public float GetMaxHealth()     {         return GetMaxHealth();\n     }\n }\n "),
        new Tool_QuickStart_Script("Interactable",          "Interaction",                  "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.Events;\n  public class Interactable : MonoBehaviour {     public enum InteractableType {Move, Door, SetLight, SetLightNegative, Lever, Button, Item }\n     public InteractableType _Type;\n      private enum AxisOptions {x,y,z}\n     [SerializeField] private AxisOptions _AxisOption = AxisOptions.x;\n      [SerializeField] private bool _InvertMouse = false;\n      [Header(\"Type - Light\")]\n     [SerializeField] private GameObject _Light = null;\n     [SerializeField] private bool _Light_StartOff = false;\n     [Header(\"Type - Lever/Door\")]\n     [SerializeField] private Transform _LeverRotationPoint = null;     [SerializeField] private Vector2 _LeverMinMaxRotation = Vector2.zero;     [SerializeField] private float _CompleteDeathZone = 0;     [Header(\"Type - Button\")\n]\n     [SerializeField] private float _ButtonPressDepth = 0;\n     private bool _ButtonPressed;\n     [Header(\"Type - Item\")]\n     [SerializeField] private string _ItemInfo = \"\";\n     [Header(\"Speed\")]\n     [SerializeField] private float _Speed = 1;\n      [Header(\"OnHigh\")]\n     [SerializeField] private UnityEvent _OnHighEvent = null;\n     [Header(\"OnLow\")]\n     [SerializeField] private UnityEvent _OnLowEvent = null;\n     [Header(\"OnNeutral\")]\n     [SerializeField] private UnityEvent _OnNeutral = null;\n           private Vector3 velocity = Vector3.zero;\n     private Rigidbody _RB;\n     private Vector3 _DefaultLocalPosition;\n     private Vector3 _DefaultPosition;\n     private bool _MovingBack;\n      private void Start()     {         _DefaultLocalPosition = transform.localPosition;         _DefaultPosition = transform.position;         _RB = GetComponent<Rigidbody>()\n;\n         if(_Type == InteractableType.SetLight || _Type == InteractableType.SetLightNegative)         {             if (_Light_StartOff)                 _Light.SetActive(false);\n             else                 _Light.SetActive(true);\n         }\n     }\n      private void Update()     {         if (_Type == InteractableType.Button)         {             UpdateButton();\n         }\n          if(_MovingBack)         {             transform.localPosition = Vector3.MoveTowards(transform.localPosition, _DefaultLocalPosition, 10 * Time.deltaTime)\n;\n             if (transform.localPosition == _DefaultLocalPosition)\n                 _MovingBack = false;\n         }\n     }\n      public InteractableType Type()     {         return _Type;\n     }\n      public void GotoPickupPoint(Transform point)\n     {         _RB.velocity = Vector3.zero;\n         transform.position = Vector3.SmoothDamp(transform.position, point.position, ref velocity, 0.2f)\n;\n         transform.rotation = Quaternion.RotateTowards(transform.rotation, point.rotation, 5f)\n;\n     }\n     public void SetVelocity(Vector3 velocity)     {         _RB.velocity = velocity;\n     }\n     public void TrowObject(Transform transformtrow)\n     {         GetComponent<Rigidbody>().AddForce(transformtrow.forward * 5000)\n;\n     }\n     public void OpenDoor()     {         float mouseY = Input.GetAxis(\"Mouse Y\");\n         float angle = 0;\n         switch (_AxisOption)         {             case AxisOptions.x:                 angle = _LeverRotationPoint.localEulerAngles.x;\n                 break;\n             case AxisOptions.y:                 angle = _LeverRotationPoint.localEulerAngles.y;\n                 break;\n             case AxisOptions.z:                 angle = _LeverRotationPoint.localEulerAngles.z;\n                 break;\n         }\n         angle = (angle > 180) ? angle - 360 : angle;\n          HandleRotation(_LeverRotationPoint, new Vector2(0, mouseY), _LeverMinMaxRotation, 1.2f, angle);\n     }\n     public void MoveLever()     {         float mouseY = Input.GetAxis(\"Mouse Y\");\n         float angle = 0;\n         switch (_AxisOption)         {             case AxisOptions.x:                 angle = _LeverRotationPoint.localEulerAngles.x;\n                 break;\n             case AxisOptions.y:                 angle = _LeverRotationPoint.localEulerAngles.y;\n                 break;\n             case AxisOptions.z:                 angle = _LeverRotationPoint.localEulerAngles.z;\n                 break;\n         }\n         angle = (angle > 180) ? angle - 360 : angle;\n          HandleRotation(_LeverRotationPoint, new Vector2(0, mouseY), _LeverMinMaxRotation, 1.2f, angle);\n          //Check         \nif (angle < _LeverMinMaxRotation.x + _CompleteDeathZone)         {             _OnLowEvent.Invoke();\n         }\n         if (angle > _LeverMinMaxRotation.y - _CompleteDeathZone)         {             _OnHighEvent.Invoke();\n         }\n         if (angle > _LeverMinMaxRotation.x + _CompleteDeathZone && angle < _LeverMinMaxRotation.y - _CompleteDeathZone)         {             _OnNeutral.Invoke();\n         }\n     }\n     public void PressButton(bool option)     {         _ButtonPressed = true;\n     }\n     public void PressButtonNegative()     {         _ButtonPressed = !_ButtonPressed;\n     }\n     public void SetLight(bool option)     {         _Light.SetActive(option);\n     }\n     public void SetLightNegative()     {         if (_Light.activeSelf)             _Light.SetActive(false);\n         else             _Light.SetActive(true);\n     }\n     public void ReturnToDefaultPos()     {         _MovingBack = true;\n     }\n     public string GetItemInfo()     {         return _ItemInfo;\n     }\n     private void HandleRotation(Transform effectedtransform, Vector2 mousemovement, Vector2 minmaxangle, float speed, float angle)\n     {         if (_InvertMouse)         {             mousemovement.x = mousemovement.x * -2;\n             mousemovement.y = mousemovement.y * -2;\n         }\n          switch (_AxisOption)         {             case AxisOptions.x:                 effectedtransform.localEulerAngles += new Vector3((mousemovement.x + mousemovement.y)\n * speed, 0, 0);\n                  if (angle < minmaxangle.x)                     effectedtransform.localEulerAngles = new Vector3(minmaxangle.x + 0.5f, 0, 0)\n;\n                 if (angle > minmaxangle.y)                     effectedtransform.localEulerAngles = new Vector3(minmaxangle.y - 0.5f, 0, 0)\n;\n                 break;\n             case AxisOptions.y:                 effectedtransform.localEulerAngles += new Vector3(0, (mousemovement.x + mousemovement.y)\n * speed, 0);\n                  if (angle < minmaxangle.x)                     effectedtransform.localEulerAngles = new Vector3(0, minmaxangle.x + 0.5f, 0)\n;\n                 if (angle > minmaxangle.y)                     effectedtransform.localEulerAngles = new Vector3(0, minmaxangle.y - 0.5f, 0)\n;\n                 break;\n             case AxisOptions.z:                 effectedtransform.localEulerAngles += new Vector3(0, 0, (mousemovement.x + mousemovement.y)\n * speed);\n                  if (angle < minmaxangle.x)                     effectedtransform.localEulerAngles = new Vector3(0, 0, minmaxangle.x + 0.5f)\n;\n                 if (angle > minmaxangle.y)                     effectedtransform.localEulerAngles = new Vector3(0, 0, minmaxangle.y - 0.5f)\n;\n                 break;\n         }\n     }\n      private void UpdateButton()     {         switch (_AxisOption)         {             case AxisOptions.x:                 if (_ButtonPressed)                 {                     if (transform.localPosition.x > _DefaultLocalPosition.x - _ButtonPressDepth)\n                         transform.localPosition -= new Vector3(_Speed, 0, 0)\n * Time.deltaTime;\n                     else                     {                          transform.localPosition = new Vector3(_DefaultLocalPosition.x - _ButtonPressDepth - 0.001f, transform.localPosition.y, transform.localPosition.z)\n;\n                         _OnLowEvent.Invoke();\n                     }\n                 }\n                 else                 {                     if (transform.localPosition.x < _DefaultLocalPosition.x + _ButtonPressDepth)\n                         transform.localPosition += new Vector3(_Speed, 0, 0)\n * Time.deltaTime;\n                     else                     {                          transform.localPosition = new Vector3(_DefaultLocalPosition.x + _ButtonPressDepth, transform.localPosition.y, transform.localPosition.z)\n;\n                         _OnHighEvent.Invoke();\n                     }\n                  }\n                 break;\n             case AxisOptions.y:                 if (_ButtonPressed)                 {                     if (transform.localPosition.y > _DefaultLocalPosition.y - _ButtonPressDepth)\n                         transform.localPosition -= new Vector3(0, _Speed, 0)\n * Time.deltaTime;\n                     else                     {                          transform.localPosition = new Vector3(_DefaultLocalPosition.x, _DefaultLocalPosition.y - _ButtonPressDepth - 0.001f, _DefaultLocalPosition.z)\n;\n                         _OnLowEvent.Invoke();\n                     }\n                 }\n                 else                 {                     if (transform.localPosition.y < _DefaultLocalPosition.y)\n                         transform.localPosition += new Vector3(0, _Speed, 0)\n * Time.deltaTime;\n                     else                     {                         transform.localPosition = _DefaultLocalPosition;                         _OnHighEvent.Invoke()\n;\n                     }\n                 }\n                 break;\n             case AxisOptions.z:                 if (_ButtonPressed)                 {                     if (transform.localPosition.z > _DefaultLocalPosition.z - _ButtonPressDepth)\n                         transform.localPosition -= new Vector3(0, 0, _Speed)\n * Time.deltaTime;\n                     else                     {                         transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _DefaultLocalPosition.z - _ButtonPressDepth - 0.001f)\n;\n                         _OnLowEvent.Invoke();\n                     }\n                 }\n                 else                 {                     if (transform.localPosition.z < _DefaultLocalPosition.z + _ButtonPressDepth)\n                         transform.localPosition += new Vector3(0, 0, _Speed)\n * Time.deltaTime;\n                     else                     {                          transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _DefaultLocalPosition.z + _ButtonPressDepth)\n;\n                         _OnHighEvent.Invoke();\n                     }\n                 }\n                 break;\n         }\n     }\n }\n "),
        new Tool_QuickStart_Script("InteractionHandler",    "Interaction_Handler",          "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.UI;\n using TMPro;\n  public class InteractionHandler : MonoBehaviour {     [SerializeField] private Image _Cursor = null;\n     [SerializeField] private LayerMask _LayerMask = 0;\n     [SerializeField] private Transform _Head = null;     [Header(\"Pickup\")\n]\n     [SerializeField] private GameObject _PickupPoint = null;\n     [SerializeField] private Vector2 _PickupMinMaxRange = Vector2.zero;\n     [SerializeField] private float _Range = 0;\n     [Header(\"Item\")]\n     [SerializeField] private Transform _ItemPreviewPoint = null;     [SerializeField] private TextMeshProUGUI _ItemInfoText = null;      private string _ItemInfo;      private Vector3 _PickupPointPosition;     private Vector3 _CalcVelocity;     private Vector3 _PrevPosition;      private GameObject _ActiveObject;     private GameObject _CheckObject;     private Interactable _Interactable;      private bool _Interacting;     private bool _Previewing;      private Movement_CC _CCS; //Script that handles rotation      \nvoid Start()\n     {         _CCS = GetComponent<Movement_CC>();\n         _PickupPointPosition.z = _PickupMinMaxRange.x;\n     }\n      void Update()     {         if (!_Interacting)         {             RaycastHit hit;\n              if (Physics.Raycast(_Head.position, _Head.TransformDirection(Vector3.forward)\n, out hit, _Range, _LayerMask))             {                 Debug.DrawRay(_Head.position, _Head.TransformDirection(Vector3.forward)\n * hit.distance, Color.yellow);\n                  _ActiveObject = hit.transform.gameObject;                  _Cursor.color = Color.white;             }\n             else             {                 Debug.DrawRay(_Head.position, _Head.TransformDirection(Vector3.forward)\n * _Range, Color.white);\n                 _Cursor.color = Color.red;\n                  _ActiveObject = null;\n                 _CheckObject = null;\n             }\n              if (_ActiveObject != _CheckObject)             {                 _Interactable = _ActiveObject.GetComponent<Interactable>();\n                 _CheckObject = _ActiveObject;\n             }\n         }\n          if(_ActiveObject != null)         {             if (_Interactable._Type != Interactable.InteractableType.Item)             {                 //OnDown                 \nif (Input.GetMouseButtonDown(0))                     OnDown();\n                  if (_Interacting)                 {                     //OnUp                     \nif (Input.GetMouseButtonUp(0))                         OnUp();\n                      //OnActive                     \nOnActive();\n                 }\n             }\n             else             {                 if (!_Previewing)                 {                     //Start Preview                     \nif (Input.GetKeyDown(KeyCode.E))                     {                         _ItemInfo = _Interactable.GetItemInfo();\n                         _CCS.LockRotation(true);\n                         _Previewing = true;\n                     }\n                 }\n                 else                 {                     _ActiveObject.transform.position = _ItemPreviewPoint.position;                     _Interactable.gameObject.transform.eulerAngles += new Vector3(-Input.GetAxis(\"Mouse Y\")\n, Input.GetAxis(\"Mouse X\"), 0);\n                      //Reset Preview                     \nif (Input.GetKeyDown(KeyCode.E))                     {                         _ItemInfo = \"\";\n                         _CCS.LockRotation(false);\n                         _Interactable.ReturnToDefaultPos();\n                         _Previewing = false;\n                     }\n                 }\n             }\n         }\n          _ItemInfoText.text = _ItemInfo;\n     }\n      void FixedUpdate()     {         if (_Interacting)         {             OnActiveFixed();\n             OnActiveFixed();\n         }\n     }\n      private void OnUp()     {         _Interacting = false;\n         switch (_Interactable._Type)         {             case Interactable.InteractableType.Lever:                 _CCS.LockRotation(false);\n                 break;\n             case Interactable.InteractableType.Door:                 _CCS.LockRotation(false);\n                 break;\n             case Interactable.InteractableType.Move:                 _Interactable.SetVelocity(_CalcVelocity);\n                 break;\n         }\n     }\n     private void OnDown()     {         _Interacting = true;\n          //OnClick         \nswitch (_Interactable._Type)         {             case Interactable.InteractableType.SetLight:                 _Interactable.SetLight(true);\n                 break;\n             case Interactable.InteractableType.SetLightNegative:                 _Interactable.SetLightNegative();\n                 break;\n             case Interactable.InteractableType.Move:                 _PickupPoint.transform.rotation = _ActiveObject.transform.rotation;                 _PickupPointPosition.z = Vector3.Distance(_Head.position, _ActiveObject.transform.position)\n;\n                 break;\n             case Interactable.InteractableType.Lever:                 _CCS.LockRotation(true);\n                 _PickupPointPosition.z = Vector3.Distance(_Head.position, _ActiveObject.transform.position)\n;\n                 break;\n             case Interactable.InteractableType.Door:                 _CCS.LockRotation(true);\n                 break;\n             case Interactable.InteractableType.Button:                 _Interactable.PressButtonNegative();\n                 break;\n         }\n     }\n     private void OnActive()     {         switch (_Interactable._Type)         {             case Interactable.InteractableType.Move:                 if (_PickupPointPosition.z < _PickupMinMaxRange.y && Input.mouseScrollDelta.y > 0)                     _PickupPointPosition.z += Input.mouseScrollDelta.y * 0.5f;\n                 if (_PickupPointPosition.z > _PickupMinMaxRange.x && Input.mouseScrollDelta.y < 0)                     _PickupPointPosition.z += Input.mouseScrollDelta.y * 0.5f;\n                  if(Input.GetMouseButtonDown(1))                 {                     _Interactable.TrowObject(_Head.transform)\n;\n                     OnUp();\n                 }\n                 break;\n             case Interactable.InteractableType.Door:                 _Interactable.OpenDoor();\n                 break;\n             case Interactable.InteractableType.Lever:                 _Interactable.MoveLever();\n                 break;\n         }\n          if (Vector3.Distance(_Head.transform.position, _ActiveObject.transform.position)\n > _Range)         {             _Interacting = false;\n             OnUp();\n         }\n     }\n      private void OnActiveFixed()     {         switch (_Interactable._Type)         {             case Interactable.InteractableType.Move:                 _Interactable.GotoPickupPoint(_PickupPoint.transform)\n;\n                  _PickupPoint.transform.localPosition = _PickupPointPosition;                  _CalcVelocity = (_ActiveObject.transform.position - _PrevPosition)\n / Time.deltaTime;\n                 _PrevPosition = _ActiveObject.transform.position;                 break;         }\n     }\n }"),
        new Tool_QuickStart_Script("InteractWithPhysics",   "Interact_Physics",             "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class InteractWithPhysics : MonoBehaviour {      void OnControllerColliderHit(ControllerColliderHit hit)     {         Rigidbody body = hit.collider.attachedRigidbody;\n         if (body != null && !body.isKinematic)              body.velocity += hit.controller.velocity;\n     }\n }\n "),
        new Tool_QuickStart_Script("Inventory",             "Inventory",                    "wip",        "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.UI;\n  public class Inventory : MonoBehaviour {     [Header(\"Inventory\")]\n     [SerializeField] private int[] _Inventory;\n     [SerializeField] private Image[] _InventoryVisuals;\n      [Header(\"Items\")]\n     [SerializeField] private List<InventoryItems> _Items = new List<InventoryItems>();\n      private void Start()     {         _Inventory = new int[_InventoryVisuals.Length];\n     }\n      public void AddItem(GameObject addobj)     {         for (int i = 0; i < _Inventory.Length; i++)\n         {             if(_Inventory[i] == 0)             {                 for (int j = 0; j < _Items.Count; j++)\n                 {                     if(addobj == _Items[j].Item_Prefab)                     {                         _Inventory[i] = j + 1;\n                     }\n                 }\n             }\n         }\n     }\n }\n   [System.Serializable] public class InventoryItems {     public GameObject Item_Prefab;\n     public Sprite Item_Sprite;\n }"),
        new Tool_QuickStart_Script("LightEffects",          "Light_Effect",                 "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class LightEffects : MonoBehaviour {     public enum LightEffectOptions { Flickering, Off, On }\n;\n      [Header(\"Settings\")]\n     [SerializeField] private LightEffectOptions _LightEffectOption = LightEffectOptions.Flickering;\n     [SerializeField] private Vector2 _MinMaxIncrease = new Vector2(0.8f, 1.2f);\n     [Range(0.01f, 100)] [SerializeField] private float _EffectStrength = 50;\n      Queue<float> _LightFlickerQ;\n     private float _LastSum = 0;\n     private Light _Light;\n     private float _LightIntensity = 0;\n      public void Reset()     {         if (_LightEffectOption == LightEffectOptions.Flickering)         {             _LightFlickerQ.Clear();\n             _LastSum = 0;\n         }\n     }\n      void Start()     {         _Light = GetComponent<Light>();\n         _LightIntensity = _Light.intensity;\n         _LightFlickerQ = new Queue<float>(Mathf.RoundToInt(_EffectStrength));\n     }\n      void Update()     {         switch(_LightEffectOption)         {             case LightEffectOptions.Flickering:                 while (_LightFlickerQ.Count >= _EffectStrength)                     _LastSum -= _LightFlickerQ.Dequeue();\n                  float newVal = Random.Range(_LightIntensity * _MinMaxIncrease.x, _LightIntensity * _MinMaxIncrease.y);\n                 _LightFlickerQ.Enqueue(newVal);\n                 _LastSum += newVal;\n                 _Light.intensity = _LastSum / (float)_LightFlickerQ.Count;\n                 break;\n             case LightEffectOptions.Off:                 _Light.intensity = 0;\n                 break;\n             case LightEffectOptions.On:                 _Light.intensity = _LightIntensity = _MinMaxIncrease.x;\n                 break;\n         }\n      }\n      public void SetEffect(LightEffectOptions options)     {         _LightEffectOption = options;\n     }\n }"),
        new Tool_QuickStart_Script("LoadScenes",            "Load_Scenes",                  "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.SceneManagement;\n  public class LoadScenes : MonoBehaviour {     public void Action_LoadScene(int sceneid)     {         SceneManager.LoadScene(sceneid);\n     }\n     public void Action_LoadScene(string scenename)     {         SceneManager.LoadScene(scenename);\n     }\n      public void Action_ReloadScene()     {         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);\n     }\n      public void Action_QuitApplication()     {         Application.Quit();\n     }\n }\n "),
        new Tool_QuickStart_Script("MenuHandler",           "Menu_Handler",                 "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.SceneManagement;\n  public class MenuHandler : MonoBehaviour {     public void LoadScene(int sceneid)     {         SceneManager.LoadScene(sceneid);\n     }\n      public void LoadScene(string scenename)     {         SceneManager.LoadScene(scenename);\n     }\n      public int Get_CurrentSceneID()     {         return SceneManager.GetActiveScene().buildIndex;\n     }\n      public string Get_CurrentSceneName()     {         return SceneManager.GetActiveScene().name;\n     }\n      public void QuitGame()     {         Application.Quit();\n     }\n }\n "),
        new Tool_QuickStart_Script("Movement_2D_Platformer","Movement_2D",                  "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  [RequireComponent(typeof(Rigidbody2D))] public class Movement_2D_Platformer : MonoBehaviour {     [Header(\"Settings\")\n]\n     [SerializeField] private float _NormalSpeed = 5;\n     [SerializeField] private float _SprintSpeed = 8;\n     [SerializeField] private float _JumpSpeed = 300;\n     [SerializeField] private float _GroundCheck = 0.6f;\n     [Header(\"Set ground layer\")]\n     [SerializeField] private LayerMask _GroundMask = ~1;\n      private float _Speed = 0;\n     private Rigidbody2D _RB;\n      void Start()     {         //Get Rigidbody / Lock z rotation         \n_RB = GetComponent<Rigidbody2D>();\n         _RB.constraints = RigidbodyConstraints2D.FreezeRotation;\n     }\n      void Update()     {         //Sprint         \nif (Input.GetKey(KeyCode.LeftShift))             _Speed = _SprintSpeed;\n         else             _Speed = _NormalSpeed;\n          //Jumping         \nif (Input.GetButtonDown(\"Jump\") && IsGrounded())             _RB.AddForce(new Vector2(0, _JumpSpeed));\n          //Apply Movement         \n_RB.velocity = new Vector2(Input.GetAxis(\"Horizontal\") * _Speed, _RB.velocity.y);\n     }\n      bool IsGrounded()     {         RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _GroundCheck, _GroundMask)\n;\n         if (hit.collider != null)         {             return true;\n         }\n         return false;\n     }\n }\n "),
        new Tool_QuickStart_Script("Movement_2D_TopDown",   "Movement_2D",                  "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  [RequireComponent(typeof(Rigidbody2D))] public class Movement_2D_TopDown : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] private float _NormalSpeed = 5;\n     [SerializeField] private float _SprintSpeed = 8;\n      private float _Speed = 0;\n     private Rigidbody2D _RB;\n      void Start()     {         //Get Rigidbody / Lock z rotation         \n_RB = GetComponent<Rigidbody2D>();\n         _RB.constraints = RigidbodyConstraints2D.FreezeRotation;\n         _RB.gravityScale = 0;\n     }\n      void Update()     {         //Sprint         \nif (Input.GetKey(KeyCode.LeftShift))             _Speed = _SprintSpeed;\n         else             _Speed = _NormalSpeed;\n          //Apply Movement         \n_RB.velocity = new Vector2(Input.GetAxis(\"Horizontal\") * _Speed, Input.GetAxis(\"Vertical\") * _Speed);\n     }\n }\n "),
        new Tool_QuickStart_Script("Movement_Camera",       "Movement_3D_Camera",           "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Movement_Camera : MonoBehaviour {     private enum CameraOptionsPos { None, Follow }\n     private enum CameraOptionsRot { None, Follow }\n      [Header(\"Options\")]\n     [SerializeField] private CameraOptionsPos _CameraOptionPos = CameraOptionsPos.Follow;\n     [SerializeField] private CameraOptionsRot _CameraOptionRot = CameraOptionsRot.Follow;\n      [Header(\"Settings - Position\")]\n     [SerializeField] private Vector3 _OffsetPosition = new Vector3(0,12,-4);\n      [Header(\"Settings - Rotation\")]\n     [SerializeField] private Vector3 _OffsetRotation = Vector3.zero;\n      [Header(\"Settings\")]\n     [SerializeField] private float _Speed = 1000;\n      [Header(\"Other\")]\n     [SerializeField] private Transform _Target = null;     [SerializeField] private bool _LockAxis_X = false;     [SerializeField] private bool _LockAxis_Y = false;     [SerializeField] private bool _LockAxis_Z = false;      private Vector3 _TargetPosition;     private float _ScreenShakeDuration;     private float _ScreenShakeIntensity;      void Update()\n     {         if(Input.GetKeyDown(KeyCode.J))         {             Effect_ScreenShake(3, 0.5f);\n         }\n          //Update Target Location         \nfloat x_axis = _Target.transform.position.x + _OffsetPosition.x;         float y_axis = _Target.transform.position.y + _OffsetPosition.y;         float z_axis = _Target.transform.position.z + _OffsetPosition.z;          if (_LockAxis_X)\n             x_axis = _OffsetPosition.x;\n         if (_LockAxis_Y)             y_axis = _OffsetPosition.y;\n         if (_LockAxis_Z)             z_axis = _OffsetPosition.z;\n          _TargetPosition = new Vector3(x_axis, y_axis, z_axis);\n          // Movement         \nswitch (_CameraOptionPos)         {             case CameraOptionsPos.Follow:                 transform.position = Vector3.MoveTowards(transform.position, _TargetPosition, _Speed * Time.deltaTime)\n;\n                 break;\n         }\n          //ScreenShake         \nif(_ScreenShakeDuration > 0)         {             transform.localPosition = new Vector3(_TargetPosition.x + Random.insideUnitSphere.x * _ScreenShakeIntensity, _TargetPosition.y + Random.insideUnitSphere.y * _ScreenShakeIntensity, _TargetPosition.z)\n;\n             _ScreenShakeDuration -= 1 * Time.deltaTime;\n         }\n         else         {             // Rotation             \nswitch (_CameraOptionRot)             {                 case CameraOptionsRot.Follow:                     Vector3 rpos = _Target.position - transform.position;                     Quaternion lookrotation = Quaternion.LookRotation(rpos, Vector3.up)\n;\n                     transform.eulerAngles = new Vector3(lookrotation.eulerAngles.x + _OffsetRotation.x, lookrotation.eulerAngles.y + _OffsetRotation.y, lookrotation.eulerAngles.z + _OffsetRotation.z)\n;\n                     break;\n             }\n         }\n     }\n      //Effects     \npublic void Effect_ScreenShake(float duration, float intesity)     {         _ScreenShakeDuration = duration;\n         _ScreenShakeIntensity = intesity;\n     }\n      //Set     \npublic void Set_CameraTarget(GameObject targetobj)     {         _Target = targetobj.transform;     }\n     public void Set_OffSet(Vector3 offset)\n     {         _OffsetPosition = offset;\n     }\n }\n "),
        new Tool_QuickStart_Script("Movement_CC_FirstPerson","Movement_3D",                 "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  [RequireComponent(typeof(CharacterController))] public class Movement_CC : MonoBehaviour {     //Movement     \n[SerializeField] private float _NormalSpeed = 5, _SprintSpeed = 8;\n     [SerializeField] private float _JumpSpeed = 5;\n     [SerializeField] private float _Gravity = 20;\n     private Vector3 _MoveDirection = Vector3.zero;\n     //Look around     \npublic float _CameraSensitivity = 1;\n     [SerializeField] private Transform _Head = null;     private float _RotationX = 90.0f;     private float _RotationY = 0.0f;     private float _Speed;      private CharacterController _CC;     private bool _LockRotation;      void Start()\n     {         Cursor.lockState = CursorLockMode.Locked;\n         Cursor.visible = false;\n         _CC = GetComponent<CharacterController>();\n         if (_Head == null)             _Head = transform.GetChild(0)\n.transform;     }\n      void Update()\n     {         //Look around         \nif (!_LockRotation)         {             _RotationX += Input.GetAxis(\"Mouse X\") * _CameraSensitivity;\n             _RotationY += Input.GetAxis(\"Mouse Y\") * _CameraSensitivity;\n             _RotationY = Mathf.Clamp(_RotationY, -90, 90);\n              transform.localRotation = Quaternion.AngleAxis(_RotationX, Vector3.up)\n;\n             _Head.transform.localRotation = Quaternion.AngleAxis(_RotationY, Vector3.left)\n;\n         }\n          //Movement         \nif (_CC.isGrounded)         {             _MoveDirection = new Vector3(Input.GetAxis(\"Horizontal\"), 0, Input.GetAxis(\"Vertical\"));\n             _MoveDirection = transform.TransformDirection(_MoveDirection)\n;\n             _MoveDirection *= _Speed;\n             if (Input.GetButton(\"Jump\"))                 _MoveDirection.y = _JumpSpeed;\n         }\n          //Sprint         \nif (Input.GetKey(KeyCode.LeftShift))             _Speed = _SprintSpeed;\n         else             _Speed = _NormalSpeed;\n          //Apply Movement         \n_MoveDirection.y -= _Gravity * Time.deltaTime;\n         _CC.Move(_MoveDirection * Time.deltaTime);\n     }\n      public void LockRotation(bool state)     {         _LockRotation = state;\n     }\n }"),
        new Tool_QuickStart_Script("Movement_CC_Platformer","Movement_3D",                  "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  [RequireComponent(typeof(CharacterController))] public class Movement_CC_Platformer : MonoBehaviour {     [Header(\"Settings\")\n]\n     [SerializeField] private float _NormalSpeed = 5, _SprintSpeed = 8;\n     [SerializeField] private float _JumpSpeed = 5;\n     [SerializeField] private float _Gravity = 20;\n     [SerializeField] private bool _ZMovementActive = false;\n          private Vector3 _MoveDirection = Vector3.zero;\n     private float _Speed;\n     private CharacterController _CC;\n      void Start()     {         _CC = GetComponent<CharacterController>();\n     }\n      void Update()     {         //Movement         \nif (_CC.isGrounded)         {             float verticalmovement = 0;\n             if (_ZMovementActive)                 verticalmovement = Input.GetAxis(\"Vertical\");\n              _MoveDirection = new Vector3(Input.GetAxis(\"Horizontal\"), 0, verticalmovement);\n             _MoveDirection = transform.TransformDirection(_MoveDirection)\n;\n             _MoveDirection *= _Speed;\n             if (Input.GetButton(\"Jump\"))                 _MoveDirection.y = _JumpSpeed;\n         }\n          //Sprint         \nif (Input.GetKey(KeyCode.LeftShift))             _Speed = _SprintSpeed;\n         else             _Speed = _NormalSpeed;\n          //Apply Movement         \n_MoveDirection.y -= _Gravity * Time.deltaTime;\n         _CC.Move(_MoveDirection * Time.deltaTime);\n     }\n }\n "),
        new Tool_QuickStart_Script("Movement_CC_TopDown",   "Movement_3D",                  "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  [RequireComponent(typeof(CharacterController))] public class Movement_CC_TopDown : MonoBehaviour {     //Movement     \n[Header(\"Settings Camera\")]\n     [SerializeField] private Camera _Camera;\n     [Header(\"Settings\")]\n     [SerializeField] private float _NormalSpeed = 5;\n     [SerializeField] private float _SprintSpeed = 8;\n     [SerializeField] private float _JumpSpeed = 5;\n     [SerializeField] private float _Gravity = 20;\n     [SerializeField] private bool _MovementRelativeToRotation = false;\n      private float _Speed = 0;\n     private Vector3 _MoveDirection = Vector3.zero;\n     private CharacterController _CC;\n      void Start()     {         _CC = GetComponent<CharacterController>();\n     }\n      void Update()     {         //Movement         \nif (_CC.isGrounded)         {             _MoveDirection = new Vector3(Input.GetAxis(\"Horizontal\"), 0, Input.GetAxis(\"Vertical\"));\n             if (_MovementRelativeToRotation)                 _MoveDirection = transform.TransformDirection(_MoveDirection)\n;\n             _MoveDirection *= _Speed;\n             if (Input.GetButton(\"Jump\"))                 _MoveDirection.y = _JumpSpeed;\n         }\n          _MoveDirection.y -= _Gravity * Time.deltaTime;\n         _CC.Move(_MoveDirection * Time.deltaTime);\n          //Sprint         \nif (Input.GetKey(KeyCode.LeftShift))             _Speed = _SprintSpeed;\n         else             _Speed = _NormalSpeed;\n          Ray cameraRay = _Camera.ScreenPointToRay(Input.mousePosition);\n         Plane groundPlane = new Plane(Vector3.up, Vector3.zero);\n         float rayLength;\n         if (groundPlane.Raycast(cameraRay, out rayLength))         {             Vector3 pointToLook = cameraRay.GetPoint(rayLength);\n             transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z)\n);\n         }\n     }\n      public void SetCamera(Camera cameraobj)     {         _Camera = cameraobj;\n     }\n }\n "),
        new Tool_QuickStart_Script("Movement_FreeCamera",   "Movement_3D_Camera",           "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Movement_FreeCamera : MonoBehaviour {     [SerializeField] private float _Speed = 5;\n     [SerializeField] private float _SprintSpeed = 8;\n      private float _CurrentSpeed;\n      void Start()     {         Cursor.visible = false;\n         Cursor.lockState = CursorLockMode.Locked;\n     }\n      void Update()     {         if (Input.GetKey(KeyCode.LeftShift))             _CurrentSpeed = _SprintSpeed;\n         else             _CurrentSpeed = _Speed;\n          float xas = Input.GetAxis(\"Horizontal\");\n         float zas = Input.GetAxis(\"Vertical\");\n          transform.Translate(new Vector3(xas,0, zas)\n * _CurrentSpeed * Time.deltaTime);\n          float mousex = Input.GetAxis(\"Mouse X\");\n         float mousey = Input.GetAxis(\"Mouse Y\");\n         transform.eulerAngles += new Vector3(-mousey, mousex, 0)\n;\n     }\n }\n "),
        new Tool_QuickStart_Script("Movement_RB_FirstPerson","Movement_3D",                 "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  [RequireComponent(typeof(Rigidbody))] public class Movement_RB_FirstPerson : MonoBehaviour {     [Header(\"Set Refference\")]\n     [SerializeField] private Transform _Head = null;      [Header(\"Settings\")\n]\n     [SerializeField] private float _MovementSpeed = 5;\n     [SerializeField] private float _JumpSpeed = 5;\n     [SerializeField] private float _CameraSensitivity = 1;\n      private Vector2 _LookRot = new Vector2(90,0);\n     private Rigidbody _RB;\n     private bool _Grounded;\n      void Start()     {         Cursor.lockState = CursorLockMode.Locked;\n         Cursor.visible = false;\n          _RB = GetComponent<Rigidbody>();\n     }\n      void Update()     {         //Check Grounded         \n_Grounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 1, transform.position.z)\n, 0.4f);\n          //Movement         \nfloat x = Input.GetAxisRaw(\"Horizontal\") * _MovementSpeed;\n         float y = Input.GetAxisRaw(\"Vertical\") * _MovementSpeed;\n          //Jump         \nif (Input.GetKeyDown(KeyCode.Space) && _Grounded)             _RB.velocity = new Vector3(_RB.velocity.x, _JumpSpeed, _RB.velocity.z);\n          //Apply Movement         \nVector3 move = transform.right * x + transform.forward * y;         _RB.velocity = new Vector3(move.x, _RB.velocity.y, move.z)\n;\n          //Look around         \n_LookRot.x += Input.GetAxis(\"Mouse X\") * _CameraSensitivity;\n         _LookRot.y += Input.GetAxis(\"Mouse Y\") * _CameraSensitivity;\n         _LookRot.y = Mathf.Clamp(_LookRot.y, -90, 90);\n          transform.localRotation = Quaternion.AngleAxis(_LookRot.x, Vector3.up)\n;\n         _Head.transform.localRotation = Quaternion.AngleAxis(_LookRot.y, Vector3.left)\n;\n     }\n }\n "),
        new Tool_QuickStart_Script("ObjectPool",            "ObjectPool",                   "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class ObjectPool : MonoBehaviour {     [SerializeField] private ObjectPool_Pool[] _ObjectPools = null;\n     private List<Transform> _Parents = new List<Transform>()\n;\n      private void Awake()     {         GameObject emptyobject = GameObject.CreatePrimitive(PrimitiveType.Cube);\n         Destroy(emptyobject.GetComponent<MeshRenderer>());\n         Destroy(emptyobject.GetComponent<BoxCollider>());\n          for (int i = 0; i < _ObjectPools.Length; i++)\n         {             //Create parent             \nGameObject poolparent = Instantiate(emptyobject, transform.position, Quaternion.identity)\n;\n             Destroy(poolparent.GetComponent<MeshRenderer>());\n             Destroy(poolparent.GetComponent<BoxCollider>());\n              //Set parent             \npoolparent.transform.parent = transform;             poolparent.transform.name = \"Pool_\" + _ObjectPools[i]._Name;             _Parents.Add(poolparent.transform)\n;\n              //Create objects             \nfor (int o = 0; o < _ObjectPools[i]._Amount; o++)\n             {                 GameObject obj = (GameObject)Instantiate(_ObjectPools[i]._Prefab);\n                 obj.transform.parent = poolparent.transform;                 obj.transform.position = new Vector2(9999, 9999)\n;\n                 obj.SetActive(false);\n                 _ObjectPools[i]._Objects.Add(obj);\n             }\n         }\n         Destroy(emptyobject);\n     }\n      //GetObject     \npublic GameObject GetObject(string objname, bool setactive)     {         int id = FindObjectPoolID(objname, false);\n         return GetObject(id, setactive);\n     }\n     public GameObject GetObject(GameObject obj, bool setactive)     {         int id = FindObjectPoolID(obj);\n         return GetObject(id, setactive);\n     }\n     public GameObject GetObjectPrefabName(string prefabname, bool setactive)     {         int id = FindObjectPoolID(prefabname, true);\n         return GetObject(id, setactive);\n     }\n      public GameObject GetObject(int id, bool setactive)     {         GameObject freeObject = null;\n         bool checkfreeobj = false;\n          for (int i = 0; i < _ObjectPools[id]._Objects.Count; i++)\n         {             if (!_ObjectPools[id]._Objects[i].activeInHierarchy)             {                 _ObjectPools[id]._Objects[i].transform.position = new Vector3(999, 999, 999)\n;\n                 _ObjectPools[id]._Objects[i].SetActive(setactive);\n                 freeObject = _ObjectPools[id]._Objects[i];\n                 return freeObject;\n             }\n         }\n          if (!checkfreeobj)         {             _ObjectPools[id]._Objects.Clear();\n             freeObject = (GameObject)Instantiate(_ObjectPools[id]._Prefab, new Vector3(999,999,999), Quaternion.identity);\n             freeObject.transform.parent = _Parents[id];             freeObject.SetActive(setactive)\n;\n             _ObjectPools[id]._Objects.Add(freeObject);\n             return freeObject;\n         }\n          Debug.Log(\"No Object Found\");\n         return null;\n     }\n      public List<GameObject> GetAllObjects(GameObject objtype)     {         int id = FindObjectPoolID(objtype);\n         return _ObjectPools[id]._Objects;\n     }\n      private int FindObjectPoolID(GameObject obj)     {         int id = 0;\n         for (int i = 0; i < _ObjectPools.Length; i++)\n         {             if (obj == _ObjectPools[i]._Prefab)             {                 id = i;\n             }\n         }\n         return id;\n     }\n     private int FindObjectPoolID(string objname, bool isprefab)     {         for (int i = 0; i < _ObjectPools.Length; i++)\n         {             if (isprefab)                 if (objname == _ObjectPools[i]._Prefab.name)                 {                     return i;\n                 }\n                 else             if (objname == _ObjectPools[i]._Name)                 {                     return i;\n                 }\n         }\n         Debug.Log(objname + \" Not Found\");\n         return 0;\n     }\n }\n  [System.Serializable] public class ObjectPool_Pool {     public string _Name;\n     public GameObject _Prefab;\n     public int _Amount;\n     [HideInInspector] public List<GameObject> _Objects;\n }"),
        new Tool_QuickStart_Script("ObjectPoolSimple",      "ObjectPool",                   "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class ObjectPoolSimple : MonoBehaviour {     public GameObject prefabGameObject;\n     public int pooledAmount;\n      [HideInInspector] public List<GameObject> objects;\n      void Awake()     {         for (int i = 0; i < pooledAmount; i++)\n         {             GameObject obj = (GameObject)Instantiate(prefabGameObject);\n             obj.transform.parent = gameObject.transform;             obj.SetActive(false)\n;\n             objects.Add(obj);\n         }\n     }\n }\n   /* Use Pool          \n[SerializeField]private ObjectPoolSimple _ObjectPool;\n      private void Spawn() {         for (int i = 0; i < _ObjectPool.objects.Count; i++)\n {             if (!_ObjectPool.objects[i].activeInHierarchy) {                 _ObjectPool.objects[i].transform.position = new Vector3(0,0,0)\n;\n                 _ObjectPool.objects[i].transform.rotation = Quaternion.Euler(0, 0, 0)\n;\n                 _ObjectPool.objects[i].SetActive(true);\n                 break;\n             }\n         }\n     }\n */ "),
        new Tool_QuickStart_Script("OnCollision",           "Collision_Practical",          "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.Events;\n  public class OnCollision : MonoBehaviour {     private enum Options {OnTriggerEnter, OnTriggerExit, OnTriggerStay, OnCollisionEnter, OnCollisionExit, OnCollisionStay, OnAll}\n;\n     [SerializeField] private LayerMask _LayerMask = ~0;\n     [SerializeField] private Options _Option = Options.OnAll;\n     [SerializeField] private string _Tag = \"\";\n     [SerializeField] private UnityEvent _Event = null;\n      private bool _HasTag;\n      private void Start()     {         if (_Tag != \"\" || _Tag != null)             _HasTag = true;\n     }\n      private void Action(Collider other)     {         if (_HasTag)             if (other.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)                 _Event.Invoke();\n     }\n     private void Action(Collision other)     {         if (_HasTag)             if (other.gameObject.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)                 _Event.Invoke();\n     }\n      private void OnTriggerEnter(Collider other)     {         if (_Option == Options.OnTriggerEnter || _Option == Options.OnAll)             Action(other);\n     }\n     private void OnTriggerExit(Collider other)     {         if (_Option == Options.OnTriggerExit || _Option == Options.OnAll)             Action(other);\n     }\n     private void OnTriggerStay(Collider other)     {         if (_Option == Options.OnTriggerStay || _Option == Options.OnAll)             Action(other);\n     }\n     private void OnCollisionEnter(Collision other)     {         if (_Option == Options.OnCollisionEnter || _Option == Options.OnAll)             Action(other);\n     }\n     private void OnCollisionExit(Collision other)     {         if (_Option == Options.OnCollisionExit || _Option == Options.OnAll)             Action(other);\n     }\n     private void OnCollisionStay(Collision other)     {         if (_Option == Options.OnCollisionStay || _Option == Options.OnAll)             Action(other);\n     }\n }"),
        new Tool_QuickStart_Script("OnCollision2D",         "Collision_2D_Practical",       "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.Events;\n  public class OnCollision2D : MonoBehaviour {     private enum Options {OnTriggerEnter, OnTriggerExit, OnTriggerStay, OnCollisionEnter, OnCollisionExit, OnCollisionStay, OnAll}\n;\n     [SerializeField] private LayerMask _LayerMask = ~0;\n     [SerializeField] private Options _Option = Options.OnAll;\n     [SerializeField] private string _Tag = \"\";\n     [SerializeField] private UnityEvent _Event = null;\n      private bool _HasTag;\n      private void Start()     {         if (_Tag != \"\" || _Tag != null)             _HasTag = true;\n     }\n      private void Action(Collider2D other)     {         if (_HasTag)             if (other.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)                 _Event.Invoke();\n     }\n     private void Action(Collision2D other)     {         if (_HasTag)             if (other.gameObject.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)                 _Event.Invoke();\n     }\n      private void OnTriggerEnter2D(Collider2D other)     {         if (_Option == Options.OnTriggerEnter || _Option == Options.OnAll)             Action(other);\n     }\n     private void OnTriggerExit2D(Collider2D other)     {         if (_Option == Options.OnTriggerExit || _Option == Options.OnAll)             Action(other);\n     }\n     private void OnTriggerStay2D(Collider2D other)     {         if (_Option == Options.OnTriggerStay || _Option == Options.OnAll)             Action(other);\n     }\n     private void OnCollisionEnter2D(Collision2D other)     {         if (_Option == Options.OnCollisionEnter || _Option == Options.OnAll)             Action(other);\n     }\n     private void OnCollisionExit2D(Collision2D other)     {         if (_Option == Options.OnCollisionExit || _Option == Options.OnAll)             Action(other);\n     }\n     private void OnCollisionStay2D(Collision2D other)     {         if (_Option == Options.OnCollisionStay || _Option == Options.OnAll)             Action(other);\n     }\n }"),
        new Tool_QuickStart_Script("PosToPos",              "Practical",                    "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class PosToPos : MonoBehaviour {     [SerializeField] private Transform _GotoPosition = null;     [SerializeField] private float _Speed = 0;      private bool _Activated;      void Update()\n     {         if (_Activated)             transform.position = Vector3.MoveTowards(transform.position, _GotoPosition.position, _Speed)\n;\n     }\n      public void StartMoving()     {         _Activated = true;\n     }\n }\n "),
        new Tool_QuickStart_Script("ReadTwitchChat",        "Networking",                   "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using System;\n using System.ComponentModel;\n using System.Net.Sockets;\n using System.IO;\n  public class ReadTwitchChat : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] private float _RefreshConnectionTimer = 60;\n     private float _Timer;\n      [Header(\"Twitch\")]\n     private TcpClient twitchClient;\n     private StreamReader reader;\n     private StreamWriter writer;\n      [SerializeField] private string _Username = \"\";\n //Twitch user name     \n[SerializeField] private string _OauthToken = \"\";\n //Get token from https://twitchapps.com/tmi     \n[SerializeField] private string _Channelname = \"\";\n //Twitch channel name      \nvoid Start()     {         Connect();\n     }\n      void Update()     {         //Check connection         \nif (!twitchClient.Connected)             Connect();\n          _Timer -= 1 * Time.deltaTime;\n         if (_Timer <= 0)         {             Connect();\n             _Timer = _RefreshConnectionTimer;\n         }\n          ReadChat();\n     }\n      private void Connect()     {         twitchClient = new TcpClient(\"irc.chat.twitch.tv\", 6667);\n         reader = new StreamReader(twitchClient.GetStream());\n         writer = new StreamWriter(twitchClient.GetStream());\n          writer.WriteLine(\"PASS \" + _OauthToken);\n         writer.WriteLine(\"NICK \" + _Username);\n         writer.WriteLine(\"USER \" + _Username + \" 8 * :\" + _Username);\n         writer.WriteLine(\"JOIN #\" + _Channelname);\n          writer.Flush();\n     }\n      private void ReadChat()     {         if (twitchClient.Available > 0)         {             var message = reader.ReadLine();\n              if (message.Contains(\"PRIVMSG\"))             {                 //Split                 \nvar splitPoint = message.IndexOf(\"!\", 1);\n                 var chatName = message.Substring(0, splitPoint);\n                  //Name                 \nchatName = chatName.Substring(1);\n                  //Message                 \nsplitPoint = message.IndexOf(\":\", 1);\n                 message = message.Substring(splitPoint + 1);\n                 print(string.Format(\"{0}\n: {1}\\n\", chatName, message));\n                  if (message.ToLower().Contains(\"example\"))                 {                     Debug.Log(\"<color=green>\" + chatName + \" has used the command example </color>\");\n                 }\n             }\n         }\n     }\n  }"),
        new Tool_QuickStart_Script("Rotation",              "Practical",                    "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Rotation : MonoBehaviour {     [SerializeField] private Vector3 _RotationSpeed = Vector3.zero;\n      void Update()     {         transform.Rotate(new Vector3(_RotationSpeed.x, _RotationSpeed.y, _RotationSpeed.z)\n * Time.deltaTime);\n     }\n }\n "),
        new Tool_QuickStart_Script("SaveLoad_JSON",         "Json_Save_Load",               "stable",     "using System.Collections.Generic;\n using System.IO;\n using UnityEngine;\n  public class SaveLoad_JSON : MonoBehaviour {     private Json_SaveData _SaveData = new Json_SaveData();\n      void Start()     {         LoadData();\n     }\n      public void SaveData()     {         string jsonData = JsonUtility.ToJson(_SaveData, true);\n         File.WriteAllText(Application.persistentDataPath + \"/SaveData.json\", jsonData);\n     }\n     public void LoadData()     {         try         {             string dataAsJson = File.ReadAllText(Application.persistentDataPath + \"/SaveData.json\");\n             _SaveData = JsonUtility.FromJson<Json_SaveData>(dataAsJson);\n         }\n         catch         {             SaveData();\n         }\n     }\n     public Json_SaveData GetSaveData()     {         return _SaveData;\n     }\n     public void CreateNewSave()     {         Json_ExampleData newsave = new Json_ExampleData();\n         newsave.exampleValue = 10;\n         _SaveData.saveData.Add(newsave);\n     }\n }\n  [System.Serializable] public class Json_SaveData {     public List <Json_ExampleData> saveData = new List<Json_ExampleData>();\n }\n [System.Serializable] public class Json_ExampleData {     public float exampleValue = 0;\n }"),
        new Tool_QuickStart_Script("SaveLoad_XML",          "XML_Save_Load",                "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using System.Xml.Serialization;\n using System.IO;\n  public class SaveLoad_XML : MonoBehaviour {     private XML_SaveData _SaveData = new XML_SaveData();\n      void Start()     {         LoadData();\n     }\n      public void SaveData()     {         XmlSerializer serializer = new XmlSerializer(typeof(XML_SaveData));\n          using (FileStream stream = new FileStream(Application.persistentDataPath + \"/SaveData.xml\", FileMode.Create))         {             serializer.Serialize(stream, _SaveData);\n         }\n     }\n      public void LoadData()     {         try         {             XmlSerializer serializer = new XmlSerializer(typeof(XML_SaveData));\n              using (FileStream stream = new FileStream(Application.persistentDataPath + \"/SaveData.xml\", FileMode.Open))             {                 _SaveData = serializer.Deserialize(stream) as XML_SaveData;\n             }\n         }\n         catch         {             SaveData();\n         }\n     }\n      public XML_SaveData GetSaveData()     {         return _SaveData;\n     }\n     public void CreateNewSave()     {         XML_ExampleData newsave = new XML_ExampleData();\n         newsave.exampleValue = 10;\n         _SaveData.saveData.Add(newsave);\n     }\n }\n  [System.Serializable] public class XML_SaveData {     public List<XML_ExampleData> saveData = new List<XML_ExampleData>();\n }\n [System.Serializable] public class XML_ExampleData {     public float exampleValue = 0;\n }"),
        new Tool_QuickStart_Script("ScriptebleGameObject",  "SO_ScriptebleGameObject",      "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  [CreateAssetMenu(fileName = \"Example\", menuName = \"SO/ExampleSO\", order = 1)] public class ScriptebleGameObject : ScriptableObject {     public string examplestring;\n     public int exampleint;\n }"),
        new Tool_QuickStart_Script("Shooting",              "Shooting",                     "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Shooting : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] ObjectPool _ObjectPool = null;\n     [SerializeField] private GameObject _BulletPrefab = null;\n     [SerializeField] private GameObject _ShootPoint = null;\n      [Header(\"Semi\")]\n     [SerializeField] private int _SemiAutomaticBulletAmount = 3;\n     [SerializeField] private float _SemiShootSpeed = 0.2f;\n     [Header(\"Automatic\")]\n     [SerializeField] private float _SecondsBetweenShots = 0.5f;\n      private enum ShootModes { SingleShot, SemiAutomatic, Automatic }\n     [SerializeField] private ShootModes _ShootMode = ShootModes.SingleShot;\n      private bool _CheckSingleShot;\n     private float _Timer;\n     private bool _LockShooting;\n      void Update()     {         if (Input.GetMouseButton(0))         {             switch (_ShootMode)             {                 case ShootModes.SingleShot:                     if (!_CheckSingleShot)                         Shoot();\n                     _CheckSingleShot = true;\n                     break;\n                 case ShootModes.SemiAutomatic:                     if (!_CheckSingleShot && !_LockShooting)                         StartCoroutine(SemiShot());\n                     _CheckSingleShot = true;\n                     break;\n                 case ShootModes.Automatic:                     _Timer += 1 * Time.deltaTime;\n                     if (_Timer >= _SecondsBetweenShots)                     {                         Shoot();\n                         _Timer = 0;\n                     }\n                     break;\n             }\n         }\n         if (Input.GetMouseButtonUp(0))         {             _CheckSingleShot = false;\n         }\n     }\n      IEnumerator SemiShot()     {         _LockShooting = true;\n         for (int i = 0; i < _SemiAutomaticBulletAmount; i++)\n         {             Shoot();\n             yield return new WaitForSeconds(_SemiShootSpeed);\n         }\n         _LockShooting = false;\n     }\n      void Shoot()     {        GameObject bullet = _ObjectPool.GetObject(_BulletPrefab, true);\n         bullet.SetActive(true);\n         bullet.transform.position = _ShootPoint.transform.position;         bullet.transform.rotation = _ShootPoint.transform.rotation;     }\n }\n "),
        new Tool_QuickStart_Script("ShootingRayCast",       "Shooting",                     "stable",     "using System.Collections;\n using System.Collections.Generic;\n using System.Threading;\n using UnityEngine;\n  public class ShootingRayCast : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] private float _Damage = 20;\n     [SerializeField] private float _ShootDistance = 50;\n     [SerializeField] private string _EnemyTag = \"Enemy\";\n      [Header(\"Semi\")]\n     [SerializeField] private int _SemiAutomaticBulletAmount = 3;\n     [SerializeField] private float _SemiShootSpeed = 0.2f;\n     [Header(\"Automatic\")]\n     [SerializeField] private float _SecondsBetweenShots = 0.5f;\n      private enum ShootModes {SingleShot, SemiAutomatic, Automatic }\n     [SerializeField] private ShootModes _ShootMode = ShootModes.SingleShot;\n      private bool _CheckSingleShot;\n     private float _Timer;\n     private bool _LockShooting;\n      void Update()     {         if (Input.GetMouseButton(0))         {             switch (_ShootMode)             {                 case ShootModes.SingleShot:                     if (!_CheckSingleShot)                         Shoot();\n                     _CheckSingleShot = true;\n                     break;\n                 case ShootModes.SemiAutomatic:                     if (!_CheckSingleShot && !_LockShooting)                         StartCoroutine(SemiShot());\n                     _CheckSingleShot = true;\n                     break;\n                 case ShootModes.Automatic:                     _Timer += 1 * Time.deltaTime;\n                     if(_Timer >= _SecondsBetweenShots)                     {                         Shoot();\n                         _Timer = 0;\n                     }\n                     break;\n             }\n         }\n         if(Input.GetMouseButtonUp(0))         {             _CheckSingleShot = false;\n         }\n     }\n      IEnumerator SemiShot()     {         _LockShooting = true;\n         for (int i = 0; i < _SemiAutomaticBulletAmount; i++)\n         {             Shoot();\n             yield return new WaitForSeconds(_SemiShootSpeed);\n         }\n         _LockShooting = false;\n     }\n      void Shoot()     {         RaycastHit hit;\n         if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward)\n, out hit, _ShootDistance))             if (hit.transform.tag == _EnemyTag)\n             {                 hit.transform.GetComponent<Health>()\n.DoDamage(_Damage);\n             }\n     }\n }"),
        new Tool_QuickStart_Script("StringFormats",         "String_Format",                "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using TMPro;\n  public class StringFormats : MonoBehaviour {     private enum FormatOptions {DigitalTime }\n;\n     [SerializeField] private FormatOptions _FormatOption = FormatOptions.DigitalTime;\n     [SerializeField] private TextMeshProUGUI _ExampleText = null;\n      private float _Timer;\n      void Update()     {         _Timer += 1 * Time.deltaTime;\n          switch (_FormatOption)         {             case FormatOptions.DigitalTime:                 _ExampleText.text = string.Format(\"{0:00}\n:{1:00}\n:{2:00}\\n\", Mathf.Floor(_Timer / 3600), Mathf.Floor((_Timer / 60) % 60), _Timer % 60);\n                 break;\n }\n     }\n }\n "),
        new Tool_QuickStart_Script("Tool_CreateHexagonMesh","Tool_Editor",                  "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEditor;\n  public class Tool_CreateHexagonGrid : EditorWindow {     private GameObject _CenterObj;\n     private List<GameObject> _ObjSaved = new List<GameObject>();\n     private int _TotalObjects = 100;\n      //Hex     \nprivate int _HexLengthX = 10, _HexLengthZ = 10;\n     private float _HexSize = 1;\n     private float _DistanceBetween = 1;\n      private bool _Center = true;\n     private bool _Invert = false;\n      [MenuItem(\"Tools/CreateHexagonGrid\")]     static void Init()     {         Tool_CreateHexagonGrid window = (Tool_CreateHexagonGrid)EditorWindow.GetWindow(typeof(Tool_CreateHexagonGrid));\n         window.Show();\n     }\n      void OnGUI()     {          GUILayout.BeginVertical(\"Box\");\n         _CenterObj = (GameObject)EditorGUILayout.ObjectField(\"Center Object\", _CenterObj, typeof(GameObject), true);\n         GUILayout.EndVertical();\n          GUILayout.BeginVertical(\"Box\");\n         _HexSize = EditorGUILayout.FloatField(\"Size: \", _HexSize);\n         _HexLengthX = EditorGUILayout.IntField(\"Collom: \", _HexLengthX);\n         _HexLengthZ = EditorGUILayout.IntField(\"Row: \", _HexLengthZ);\n          GUILayout.BeginHorizontal(\"Box\");\n         if (GUILayout.Button(\"Calculate Total Objects\"))             _TotalObjects = _HexLengthX * _HexLengthZ;\n         EditorGUILayout.LabelField(\"Total: \" + _TotalObjects.ToString());\n         GUILayout.EndHorizontal();\n          _Center = EditorGUILayout.Toggle(\"Center\", _Center);\n         _Invert = EditorGUILayout.Toggle(\"Invert: \", _Invert);\n         _DistanceBetween = EditorGUILayout.FloatField(\"Distance Between: \", _DistanceBetween);\n         GUILayout.EndVertical();\n          GUILayout.BeginVertical(\"Box\");\n         if (GUILayout.Button(\"Create\"))         {             if (_CenterObj != null)             {                 if (_ObjSaved.Count > 0)                 {                     for (int i = 0; i < _ObjSaved.Count; i++)\n                     {                         DestroyImmediate(_ObjSaved[i]);\n                     }\n                     _ObjSaved.Clear();\n                 }\n                  Vector3 objPos = _CenterObj.transform.position;                 CreateHexagon(new Vector3(_HexLengthX, 0, _HexLengthZ)\n);\n                 SetParent();\n             }\n             else             {                 Debug.Log(\"Center Object not selected!\");\n             }\n         }\n          if (GUILayout.Button(\"Destroy\"))         {             if (_CenterObj != null)             {                 for (int i = 0; i < _ObjSaved.Count; i++)\n                 {                     DestroyImmediate(_ObjSaved[i]);\n                 }\n                 _ObjSaved.Clear();\n                   int childs = _CenterObj.transform.childCount;                 for (int i = childs -1; i >= 0; i--)\n                 {                     DestroyImmediate(_CenterObj.transform.GetChild(i)\n.gameObject);\n                 }\n             }\n             else             {                 Debug.Log(\"Center Object not selected!\");\n             }\n     }\n          if (GUILayout.Button(\"Confirm\"))         {             _ObjSaved.Clear();\n         }\n         GUILayout.EndVertical();\n     }\n      void CreateHexagon(Vector3 dimentsions)     {         Vector3 objPos = _CenterObj.transform.position;         if (_Center && !_Invert)\n         {             objPos.x -= dimentsions.x * 0.5f * 1.7321f * _HexSize;\n             objPos.z -= dimentsions.z * 0.5f * -1.5f * _HexSize;\n         }\n         if (_Center && _Invert)         {             objPos.x -= dimentsions.x * 0.5f * 1.7321f * _HexSize;\n             objPos.z += dimentsions.z * 0.5f * -1.5f * _HexSize;\n         }\n          for (int xas = 0; xas < dimentsions.x; xas++)\n         {             CreateHax(new Vector3(objPos.x + 1.7321f  * _HexSize * _DistanceBetween * xas, objPos.y, objPos.z));\n             for (int zas = 1; zas < dimentsions.z; zas++)\n             {                 float offset = 0;\n                 if (zas % 2 == 1)                 {                     offset = 0.86605f * _HexSize * _DistanceBetween;\n                 }\n                 else                 {                     offset = 0;\n                 }\n                 if (!_Invert)                 {                     CreateHax(new Vector3(objPos.x + 1.7321f * _HexSize * _DistanceBetween * xas - offset, objPos.y, objPos.z + -1.5f * _HexSize * _DistanceBetween * zas));\n                 }\n                 else                 {                     CreateHax(new Vector3(objPos.x + 1.7321f * _HexSize * _DistanceBetween * xas - offset, objPos.y, objPos.z + +1.5f * _HexSize * _DistanceBetween * zas));\n                 }\n             }\n         }\n     }\n     void CreateHax(Vector3 positions)     {         Vector3 objPos = _CenterObj.transform.position;          GameObject gridObj = GameObject.CreatePrimitive(PrimitiveType.Cube)\n;\n         gridObj.transform.position = new Vector3(positions.x, positions.y, positions.z)\n;\n          DestroyImmediate(gridObj.GetComponent<BoxCollider>());\n          float size = _HexSize;\n         float width = Mathf.Sqrt(3) * size;\n         float height = size * 2f;\n         Mesh mesh = new Mesh();\n         Vector3[] vertices = new Vector3[7];\n          for (int i = 0; i < 6; i++)\n         {             float angle_deg = 60 * i - 30;\n             float angle_rad = Mathf.Deg2Rad * angle_deg;\n              vertices[i + 1] = new Vector3(size * Mathf.Cos(angle_rad), 0f, size * Mathf.Sin(angle_rad));\n         }\n         mesh.vertices = vertices;\n          mesh.triangles = new int[]         {             2,1,0,             3,2,0,             4,3,0,             5,4,0,             6,5,0,             1,6,0         }\n;\n          Vector2[] uv = new Vector2[7];\n         for (int i = 0; i < 7; i++)\n         {             uv[i] = new Vector2(                 (vertices[i].x + -width * .5f) * .5f / size,                 (vertices[i].z + -height * .5f) * .5f / size);\n         }\n          mesh.uv = uv;\n         gridObj.GetComponent<MeshFilter>().sharedMesh = mesh;\n          _ObjSaved.Add(gridObj);\n     }\n      void SetParent()     {         for (int i = 0; i < _ObjSaved.Count; i++)\n         {             _ObjSaved[i].transform.parent = _CenterObj.transform;         }\n     }\n }"),
        new Tool_QuickStart_Script("Tool_ScriptToString",   "Tool_Editor",                  "wip",        "using System.Collections;\n using System.Collections.Generic;\n using System.Linq;\n using UnityEditor;\n using UnityEngine;\n  public class Tool_ScriptToString : EditorWindow {     string _ScriptInput = \"\";\n     string _ScriptOutput = \"\";\n      List<string> _CustomCommandCheckKeywords = new List<string>();\n     string _CustomCommandCheck;\n      private Vector2 _ScrollPos = new Vector2();\n      [MenuItem(\"Tools/Convert Script to String\")]     public static void ShowWindow()     {         EditorWindow.GetWindow(typeof(Tool_ScriptToString));\n     }\n      void OnGUI()     {         //Convert         \nif (GUILayout.Button(\"Convert\", GUILayout.Height(30)))         {             _ScriptOutput = ConvertScriptToString();\n         }\n          GUILayout.Space(20);\n          //Input         \nGUILayout.Label(\"Input: \", EditorStyles.boldLabel);\n         _ScriptInput = EditorGUILayout.TextField(\"\", _ScriptInput);\n          //Output         \nGUILayout.Label(\"Output: \", EditorStyles.boldLabel);\n         EditorGUILayout.TextField(\"\", _ScriptOutput);\n          GUILayout.Space(20);\n          GUILayout.Label(\"Use Custom Keywords to fix lines that should not be included into the commend. \n\" +             \"The x on the left shows the lines that contain a comment.\");\n          GUILayout.Space(20);\n          //Other Settings         \nGUILayout.Label(\"Custom Keywords: \", EditorStyles.boldLabel);\n         _CustomCommandCheck = EditorGUILayout.TextField(\"\", _CustomCommandCheck);\n         if (GUILayout.Button(\"AddKeyword\"))         {             _CustomCommandCheckKeywords.Add(_CustomCommandCheck);\n             _CustomCommandCheck = \"\";\n             _ScriptOutput = ConvertScriptToString();\n         }\n         for (int i = 0; i < _CustomCommandCheckKeywords.Count; i++)\n         {             GUILayout.BeginHorizontal(\"box\");\n             GUILayout.Label(_CustomCommandCheckKeywords[i]);\n             if (GUILayout.Button(\"Remove\", GUILayout.Width(100)))             {                 _CustomCommandCheckKeywords.RemoveAt(i);\n             }\n             GUILayout.EndHorizontal();\n         }\n          //Preview         \nList<string> output = new List<string>();\n         List<string> output2 = new List<string>();\n          for (int i = 0; i < _ScriptOutput.Length; i++)\n         {             output.Add(System.Convert.ToString(_ScriptOutput[i]));\n         }\n          int begincalc = 0;\n         int endcalc = 0;\n          for (int i = 0; i < output.Count; i++)\n         {             if(i + 1 < output.Count)             {                 if(output[i] + output[i + 1] == \"\\n\")                 {                     endcalc = i;\n                     string addstring = \"\";\n                     for (int j = 0; j < endcalc - begincalc; j++)\n                     {                         addstring += output[begincalc + j];\n                     }\n                     addstring += output[endcalc] + output[endcalc + 1];\n                      output2.Add(addstring);\n                     endcalc = endcalc + 1;\n                     begincalc = endcalc + 1;\n                 }\n             }\n         }\n          _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);\n          for (int i = 0; i < output2.Count; i++)\n         {             GUILayout.BeginHorizontal();\n             if(output2[i].Contains(\"//\"))             {                 Editor\nGUILayout.TextField(\"\", \"x\", GUILayout.MaxWidth(15));\n             }\n             else             {                 EditorGUILayout.TextField(\"\", \"\", GUILayout.MaxWidth(15));\n             }\n              EditorGUILayout.TextField(\"\", output2[i]);\n             GUILayout.EndHorizontal();\n         }\n                  EditorGUILayout.EndScrollView();\n     }\n      private string ConvertScriptToString()     {         _ScriptOutput = \"\";\n         string scriptasstring = \"\\\"\";\n          //Split / add to array         \nList<string> textedit = new List<string>();\n          for (int i = 0; i < _ScriptInput.Length; i++)\n         {             textedit.Add(System.Convert.ToString(_ScriptInput[i]));\n         }\n          bool headercheck = false;\n         bool forcheck = false;         bool commentcheck = false;          for (int i = 0; i < textedit.Count; i++)\n         {             //Header check             \nif (i + 7 < textedit.Count)             {                 if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] + textedit[i + 6] + textedit[i + 7] == \"[Header(\")                     headercheck = true;\n             }\n              //For check             \nif(i + 2 < textedit.Count)             {                 if(textedit[i]\n + textedit[i+1] + textedit[i + 2] == \"for\")\n                 {                     forcheck = true;                 }\n             }\n              //Comment check             \nif (i + 1 < textedit.Count)\n             {                 if (textedit[i] + textedit[i + 1] == \"//\" || textedit[i] + textedit[i + 1] == \"/*\")                     commentcheck = true;\n             }\n              //Comment /* + *\n/             if (commentcheck)             {                 if (textedit[i - 1] + textedit[i] == \"*/\")                 {                     scriptasstring += \"\\n\";\n                     commentcheck = false;\n                 }\n                  if (i + 6 < textedit.Count)                 {                     //\nif                     if (textedit[i] + textedit[i + 1] == \"if\")                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                     //\nfor                     if (textedit[i] + textedit[i + 1] + textedit[i + 2] == \"for\")\n                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                     //\nswitch                     if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] == \"switch\")                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                     //\ncase                     if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] == \"case\")                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                     //\npublic                     if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] == \"public\")                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                     //\nprivate                     if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] + textedit[i + 6] == \"private\")                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                     //\nvoid                     if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] == \"void\")                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                 }\n                  for (int j = 0; j < _CustomCommandCheckKeywords.Count; j++)\n                 {                     if(_CustomCommandCheckKeywords[j].Length < textedit.Count)                     {                         string check = \"\";\n                          for (int o = 0; o < _CustomCommandCheckKeywords[j].Length; o++)\n                         {                             check += textedit[i + o];\n                         }\n                                                  if(check == _CustomCommandCheckKeywords[j])                         {                             scriptasstring += \"\\n\";\n                             commentcheck = false;\n                         }\n                     }\n                 }\n             }\n              scriptasstring += textedit[i];\n              //Endings check             \nif (i + 1 < textedit.Count)             {                 if (textedit[i + 1] == \"\\\"\")                 {                     scriptasstring += \"\\\";\n                 }\n                  if (textedit[i] == \"}\\n\")                 {                     scriptasstring += \"\\n\";\n                 }\n                 if (textedit[i] == \";\\n\" && !forcheck)\n                 {                     scriptasstring += \"\\n\";\n                 }\n                 if(textedit[i] == \"]\" && headercheck)                 {                     scriptasstring += \"\\n\";\n                     headercheck = false;\n                 }\n                 if (textedit[i] == \")\" && forcheck)\n                 {                     scriptasstring += \"\\n\";\n                     forcheck = false;                 }\n             }\n         }\n          scriptasstring += \"\\\"\";         return scriptasstring;     }\n }\n "),
        new Tool_QuickStart_Script("Turret",                "Turret_Shooting",              "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Turret : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] private Vector2 _MinMaxRange = Vector2.zero;\n     [SerializeField] private float _SecondsBetweenShots = 2;\n     [SerializeField] private float _Damage = 25;\n     [SerializeField] private GameObject _ShootPart = null;\n     [SerializeField] private string _Tag = \"Enemy\";\n          private float _Timer;\n     private GameObject _Target;\n      void Update()     {         if (_Target != null)         {             _ShootPart.transform.LookAt(_Target.transform.position)\n;\n             _Timer += 1 * Time.deltaTime;\n             if (_Timer >= _SecondsBetweenShots)             {                 _Target.GetComponent<Health>().DoDamage(_Damage);\n                 _Timer = 0;\n             }\n         }\n         else         {             _ShootPart.transform.rotation = Quaternion.Euler(90, 0, 0)\n;\n         }\n          _Target = FindEnemy();\n     }\n      public GameObject FindEnemy()     {         GameObject[] m_Targets = GameObject.FindGameObjectsWithTag(_Tag);\n         GameObject closest = null;\n         float distance = Mathf.Infinity;\n         Vector3 position = transform.position;          _MinMaxRange.x = _MinMaxRange.x * _MinMaxRange.x;         _MinMaxRange.y = _MinMaxRange.y * _MinMaxRange.y;         foreach (GameObject target in m_Targets)\n         {             Vector3 diff = target.transform.position - position;             float curDistance = diff.sqrMagnitude;             if (curDistance < distance && curDistance >= _MinMaxRange.x && curDistance <= _MinMaxRange.y)\n             {                 closest = target;\n                 distance = curDistance;\n             }\n         }\n         return closest;\n     }\n }"),
        new Tool_QuickStart_Script("UIEffects",             "UI_Effect",                    "stable",     "using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.EventSystems;\n  public class UIEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {     private enum UIEffectOptions { Grow, Shrink }\n     [SerializeField] private UIEffectOptions _UIEffect = UIEffectOptions.Grow;\n     [SerializeField] private Vector3 _MinDefaultMaxSize = new Vector3(0.9f,1f,1.1f);\n     [SerializeField] private float _IncreaseSpeed = 1;\n      private Vector3 _OriginalSize;\n     private bool _MouseOver;\n      void Start()     {         _OriginalSize = transform.localScale;     }\n      void Update()\n     {         switch (_UIEffect)         {             case UIEffectOptions.Grow:                 if (_MouseOver)                 {                     if (transform.localScale.y < _MinDefaultMaxSize.z)\n                         transform.localScale += new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed)\n * Time.deltaTime;\n                 }\n                 else                     if (transform.localScale.y > _OriginalSize.y)\n                     transform.localScale -= new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed)\n * Time.deltaTime;\n                 else                     transform.localScale = new Vector3(_OriginalSize.y, _OriginalSize.z, _OriginalSize.z)\n;\n                 break;\n             case UIEffectOptions.Shrink:                 if (_MouseOver)                 {                     if (transform.localScale.y > _MinDefaultMaxSize.x)\n                         transform.localScale -= new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed)\n * Time.deltaTime;\n                 }\n                 else                    if (transform.localScale.y < _OriginalSize.x)\n                     transform.localScale += new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed)\n * Time.deltaTime;\n                 else                     transform.localScale = new Vector3(_OriginalSize.x, _OriginalSize.y, _OriginalSize.z)\n;\n                 break;\n         }\n     }\n      public void OnPointerEnter(PointerEventData eventData)     {         _MouseOver = true;\n     }\n      public void OnPointerExit(PointerEventData eventData)     {         _MouseOver = false;\n     }\n }")
    };

    //Settings
    int _CreateSceneOptions = 0;
    Vector2 _ScrollPos = new Vector2();
    bool _StartUpSearch = false;

    //Search
    string _Search_Script = "";
    string _Search_Tag = "";
    string _Search_Window = ""; 
    string[] _Project_Scripts = new string[0];
    bool _Search_QuickStartScripts_Toggle = true;
    bool _Searcg_ProjectScripts_Toggle = false;
    int _Search_ProjectScripts_Results = 0;
    int _Search_ProjectScripts_Total = 0;
    int _Search_Results = 0;
    int _Search_InProject_Results = 0;

    //HUD Settings
    bool _HUD_EnableLiveEdit = true;

    //HUD Tabs
    int _HUDTabID;
    List<Tool_QuickStartUI_Tab> _HUDTab = new List<Tool_QuickStartUI_Tab>();

    //HUD Profiles
    enum HUDProfiles { Default };
    HUDProfiles _HUD_Profiles;

    //Other
    GameObject _MainCanvas;
    RectTransform _MainCanvasRect;
    Vector2 _CheckMainCanvasRectSize;

    //Tool Mode
    int _ToolState = 0;
    int _ToolStateCheck = 1;

    //FileFinder (FF) ----------------------------------------------
    #region FileFinder
    string _FF_Type = "";
    string _FF_Search = "";
    string _FF_SearchCheck = "a";
    int _FF_Results = 0;
    int _FF_Total = 0;

    //Scene
    string _FF_Scene_Search = "";
    bool _FF_Scene_InsceneInfo = false;
    GameObject[] _FF_Scene_Objects = new GameObject[0];

    //Results
    string[] _FF_SearchResults = new string[0];
    string[] _FF_SearchResultsChange = new string[0];
    #endregion


    //Script To String (STS) ----------------------------------------------
    #region Script To String
    string _STS_ScriptInput = "";
    string _STS_ScriptOutput = "";
    string _STS_CustomCommandCheck = "";
    private bool _STS_ToggleKeywords = false;
    List<string> _STS_CustomCommandCheckKeywords = new List<string>();
    #endregion


    //Map Editor (ME) ----------------------------------------------
    #region MapEditor
    //Prefab Array
    GameObject[] _ME_Prefabs = new GameObject[0];
    string[] _ME_SearchResults = new string[0];

    //Array Options
    string _ME_SearchPrefab = "";
    bool _ME_HideNames = true;
    float _ME_ButtonSize = 1, _ME_CollomLength = 4;

    //Array Selection
    int _ME_SelectedID = 99999999, _ME_CheckSelectedID = 999999999;

    //Options
    bool _ME_HideOptions = true;
    int _ME_OptionsStates = 0, _ME_PlacementStates = 0;

    //Placement Option
    float _ME_PaintSpeed = 1, _ME_PaintTimer = 0;
    bool _ME_SnapPosActive = false;

    //Onscene Options
    bool _ME_ShowOptionsInScene;
    int _ME_InScene_SelectedID;

    //Position
    Vector3 _ME_MousePos, _ME_SnapPos, _ME_ObjectPos;
    Vector2 _ME_GridSize = new Vector2(1, 1);

    //Rotation/Size
    float _ME_Rotation, _ME_Size = 1;
    bool _ME_RandomRot = false;
    Vector2 _ME_PrevMousePos = new Vector3(0,0,0);

    //Check Buttons Event
    bool _ME_MouseDown, _ME_ShiftDown, _ME_CtrlDown, _ME_ClickMenu;

    //Placement
    GameObject _ME_ParentObj, _ME_ExampleObj;
    Transform _ME_HitObject;
    bool _ME_RotateWithObject = false;

    //Other
    Vector2 _ME_ScrollPos1, _ME_ClickPos;
    Texture2D[] _ME_PrefabIcon = new Texture2D[0];
    bool _ME_FirstLoad = true;
    #endregion


    [MenuItem("Tools/Tool_QuickStart %q")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Tool_QuickStart));
    }

    //Menu
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("=", GUILayout.Width(20)))
        {
            _WindowID = 0;
            _SelectWindow = !_SelectWindow;
        }
        if (_SelectWindow)
        {
            GUILayout.Label("Tool Navigation");
            GUILayout.EndHorizontal();

            _Search_Window = EditorGUILayout.TextField("Search: ", _Search_Window);

            for (int i = 1; i < _WindowNames.Length; i++)
            {
                if (_Search_Window == "" || _WindowNames[i].ToLower().Contains(_Search_Window.ToLower()))
                    if (GUILayout.Button(_WindowNames[i], GUILayout.Height(30))) { _WindowID = i; _SelectWindow = false; _Search_Window = ""; ChangeTab(); }
            }
        }
        else
        {
            switch (_WindowID)
            {
                case 0: //Default
                        //Menu Type
                    _MenuID = GUILayout.Toolbar(_MenuID, new string[] { "QuickStart", "Scripts", "QuickUI (wip)" });
                    GUILayout.EndHorizontal();

                    switch (_MenuID)
                    {
                        case 0: //QuickStart
                            Menu_QuickStart();
                            break;
                        case 1: //Scripts
                            Menu_Scripts();
                            break;
                        case 2: //QuickUI
                            Menu_QuickUI();
                            break;
                    }
                    break;
                case 1: //FileFinder
                    GUILayout.EndHorizontal();
                    FileFinder();
                    break;
                case 2: //ScriptToString
                    GUILayout.EndHorizontal();
                    ScriptToString_Menu();
                    break;
                case 3: //MapEditor
                    GUILayout.EndHorizontal();
                    MapEditor_Menu();
                    break;
            }
        }
    }

    //Home > QuickStart : Menu
    void Menu_QuickStart()
    {
        //FirstSearch
        if(!_StartUpSearch)
        {
            SearchScripts();
            _StartUpSearch = true;
        }

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
                ScriptStatus("Movement_CC_FirstPerson");
                GUILayout.Label("Extra", EditorStyles.boldLabel);
                ScriptStatus("Health");
                //

                GUI.backgroundColor = Color.white;
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("Add:", EditorStyles.boldLabel, GUILayout.Width(30));
                if (GUILayout.Button("Essential"))
                    AddScriptsMultiple(new string[] { "Movement_CC_FirstPerson"});
                if (GUILayout.Button("All"))
                    AddScriptsMultiple(new string[] { "Movement_CC_FirstPerson", "Health" });
                EditorGUILayout.EndHorizontal();
                break;
            case 1: //ThirdPerson
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("Movement_CC_FirstPerson");
                GUILayout.Label("Extra", EditorStyles.boldLabel);
                ScriptStatus("Health");
                //

                GUI.backgroundColor = Color.white;
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("Add:", EditorStyles.boldLabel, GUILayout.Width(30));
                if (GUILayout.Button("Essential"))
                    AddScriptsMultiple(new string[] { "Movement_CC_FirstPerson" });
                if (GUILayout.Button("All"))
                    AddScriptsMultiple(new string[] { "Movement_CC_FirstPerson", "Health" });
                EditorGUILayout.EndHorizontal();
                break;
            case 2: //TopDown
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("Movement_CC_TopDown");
                GUILayout.Label("Extra", EditorStyles.boldLabel);
                ScriptStatus("Health");
                //

                GUI.backgroundColor = Color.white;
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("Add:", EditorStyles.boldLabel, GUILayout.Width(30));
                if (GUILayout.Button("Essential"))
                    AddScriptsMultiple(new string[] { "Movement_CC_TopDown" });
                if (GUILayout.Button("All"))
                    AddScriptsMultiple(new string[] { "Movement_CC_TopDown", "Health" });
                EditorGUILayout.EndHorizontal();
                break;
            case 3: //Platformer
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("Movement_CC_Platformer");
                GUILayout.Label("Extra", EditorStyles.boldLabel);
                ScriptStatus("Health");
                //

                GUI.backgroundColor = Color.white;
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("Add:", EditorStyles.boldLabel, GUILayout.Width(30));
                if (GUILayout.Button("Essential"))
                    AddScriptsMultiple(new string[] { "Movement_CC_Platformer" });
                if (GUILayout.Button("All"))
                    AddScriptsMultiple(new string[] { "Movement_CC_Platformer", "Health" });
                EditorGUILayout.EndHorizontal();
                break;
        }
    }

    //Home > QuickStart : CreateScene
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

    //Home > QuickStart : Create Objects 3D / Set scripts
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
        if (ScriptExist("Movement_CC_FirstPerson"))
        {
            string UniType = "Movement_CC_FirstPerson";
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
        DestroyImmediate(rotationPoint.GetComponent<BoxCollider>());
        rotationPoint.GetComponent<MeshRenderer>().enabled = false;

        //Setup Scripts
        if (ScriptExist("Health"))
        {
            string UniType = "Health";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            playerobj.AddComponent(UnityType);
        }
        if (ScriptExist("Movement_CC_FirstPerson"))
        {
            string UniType = "Movement_CC_FirstPerson";
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

    //Home > QuickStart : Create Object 2D / Set scripts
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


    //Home > Scripts
    void Menu_Scripts()
    {
        //Refresh
        if (GUILayout.Button("Refresh"))
            SearchScripts();

        //Search Options
        _Search_Script = EditorGUILayout.TextField("Search: ", _Search_Script);
        _Search_Tag = EditorGUILayout.TextField("SearchTag: ", _Search_Tag);
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);

        //Quickstart Scripts
        _Search_QuickStartScripts_Toggle = EditorGUILayout.Foldout(_Search_QuickStartScripts_Toggle, "QuickStart" + "     ||     Results(" + _Search_Results.ToString() + "/" + QuickStart_Scripts.Length.ToString() + ") In Project: " + _Search_InProject_Results.ToString());
        if (_Search_QuickStartScripts_Toggle)
        {
            _Search_Results = 0;
            _Search_InProject_Results = 0;
            for (int i = 0; i < QuickStart_Scripts.Length; i++)
            {
                if (_Search_Script == "" || QuickStart_Scripts[i].ScriptName.ToLower().Contains(_Search_Script.ToLower()))
                {
                    if (QuickStart_Scripts[i].ScriptTag.ToLower().Contains(_Search_Tag.ToLower()) || QuickStart_Scripts[i].ScriptTag == "" || QuickStart_Scripts[i].ScriptTag == null)
                    {
                        //Update results
                        _Search_Results++;

                        //Set color
                        if (QuickStart_Scripts[i].Exist)
                        {
                            GUI.backgroundColor = new Color(0, 1, 0);
                            _Search_InProject_Results++;
                        }
                        else
                            GUI.backgroundColor = new Color(1, 0, 0);

                        //Script
                        EditorGUILayout.BeginHorizontal("Box");

                        if (Screen.width <= 325)
                            EditorGUILayout.LabelField(QuickStart_Scripts[i].ScriptName + ".cs", EditorStyles.boldLabel, GUILayout.Width(Screen.width - 130));
                        else
                            EditorGUILayout.LabelField(QuickStart_Scripts[i].ScriptName + ".cs", EditorStyles.boldLabel, GUILayout.Width(Screen.width - 180));

                        if (Screen.width > 325)
                            EditorGUILayout.LabelField(QuickStart_Scripts[i].ScriptState, EditorStyles.miniLabel, GUILayout.Width(50));

                        //Select Script
                        if (!QuickStart_Scripts[i].Exist)
                        {
                            EditorGUI.BeginDisabledGroup(true);
                            if (GUILayout.Button("Select", GUILayout.Width(50)))
                                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(QuickStart_Scripts[i].ScriptPath);
                            EditorGUI.EndDisabledGroup();
                        }
                        else
                        {
                            if (GUILayout.Button("Select", GUILayout.Width(50)))
                                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(QuickStart_Scripts[i].ScriptPath);
                        }

                        //Add Script
                        EditorGUI.BeginDisabledGroup(QuickStart_Scripts[i].Exist);
                        if (GUILayout.Button("Add", GUILayout.Width(50)))
                            AddScript(i);
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        GUI.backgroundColor = Color.white;

        //ProjectScripts
        _Searcg_ProjectScripts_Toggle = EditorGUILayout.Foldout(_Searcg_ProjectScripts_Toggle, "Project" + "     ||     Results(" + _Search_ProjectScripts_Results.ToString() + "/" + _Search_ProjectScripts_Total.ToString() + ")");
        if (_Searcg_ProjectScripts_Toggle)
        {
            _Search_ProjectScripts_Results = 0;

            _Search_ProjectScripts_Total = _Project_Scripts.Length;
            for (int i = 0; i < _Project_Scripts.Length; i++)
            {
                if (_Search_Script == "" || _Project_Scripts[i].ToLower().Contains(_Search_Script.ToLower()))
                {
                    //Update results
                    _Search_ProjectScripts_Results++;

                    //Script
                    EditorGUILayout.BeginHorizontal("Box");
                    EditorGUILayout.LabelField(_Project_Scripts[i], EditorStyles.boldLabel);
                    if (GUILayout.Button("Select", GUILayout.Width(50)))
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(_Project_Scripts[i]);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    //Home > Scripts : Search
    void ScriptStatus(string name)
    {
        int scriptid = 999;
        for (int i = 0; i < QuickStart_Scripts.Length; i++)
        {
            if (name == QuickStart_Scripts[i].ScriptName)
            {
                scriptid = i;
                continue;
            }
        }

        if (scriptid != 999)
        {
            if (QuickStart_Scripts[scriptid].Exist)
            { GUI.backgroundColor = new Color(0, 1, 0); }
            else
                GUI.backgroundColor = new Color(1, 0, 0);

            EditorGUILayout.BeginHorizontal("Box");
            GUILayout.Label(name + ".cs");
            EditorGUI.BeginDisabledGroup(QuickStart_Scripts[scriptid].Exist);
            if (GUILayout.Button("Add", GUILayout.Width(50)))
            {
                AddScript(scriptid);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            GUI.backgroundColor = Color.black;
            EditorGUILayout.BeginHorizontal("Box");
            GUILayout.Label(name + ".cs");
            EditorGUI.BeginDisabledGroup(true);
            if (GUILayout.Button("Add", GUILayout.Width(50))) { }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }
    }
    void SearchScripts()
    {
        //QuickStart Scripts
        bool[] checkexist = new bool[QuickStart_Scripts.Length];

        for (int i = 0; i < QuickStart_Scripts.Length; i++)
        {
            string[] search_results = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
            for (int o = 0; o < search_results.Length; o++)
            {
                if (search_results[o].ToLower().Contains(QuickStart_Scripts[i].ScriptName.ToLower()))
                {
                    checkexist[i] = true;
                    QuickStart_Scripts[i].ScriptPath = search_results[o];
                }
            }
        }

        for (int i = 0; i < QuickStart_Scripts.Length; i++)
        {
            QuickStart_Scripts[i].Exist = checkexist[i];
        }

        //Scripts Project
        _Project_Scripts = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
    }
    bool ScriptExist(string name)
    {
        int scriptid = 0;
        for (int i = 0; i < QuickStart_Scripts.Length; i++)
        {
            if (name == QuickStart_Scripts[i].ScriptName)
            {
                scriptid = i;
                continue;
            }
        }
        return QuickStart_Scripts[scriptid].Exist;
    }

    //Home > Scripts : Add
    void AddScriptsMultiple(string[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            for (int o = 0; o < QuickStart_Scripts.Length; o++)
            {
                if (ids[i] == QuickStart_Scripts[o].ScriptName)
                {
                    AddScript(o);
                }
            }
        }
    }
    void AddScript(int id)
    {
        SearchScripts();
        if (!QuickStart_Scripts[id].Exist)
        {
            using (StreamWriter sw = new StreamWriter(string.Format(Application.dataPath + "/" + QuickStart_Scripts[id].ScriptName + ".cs",
                                               new object[] { QuickStart_Scripts[id].ScriptName.Replace(" ", "") })))
            {
                sw.Write(QuickStart_Scripts[id].ScriptCode);
            }
        }
        AssetDatabase.Refresh();
        SearchScripts();
    }


    //Home > QuickUI : Menu
    void Menu_QuickUI()
    {
        GUILayout.BeginHorizontal();
        _MainCanvas = (GameObject)EditorGUILayout.ObjectField("Canvas", _MainCanvas, typeof(GameObject), true);
        if (_MainCanvas == null)
        {
            if (GUILayout.Button("Search"))
            {
                _MainCanvas = GameObject.FindObjectOfType<Canvas>().gameObject;
                HUD_Add_Tab();
            }
            if (GUILayout.Button("Create"))
            {
                _MainCanvas = HUD_Create_Canvas();
                HUD_Add_Tab();
            }
        }
        else
        {
            if (GUILayout.Button("Delete"))
                if (_MainCanvas != null)
                {
                    DestroyImmediate(_MainCanvas);
                    _HUDTab.Clear();
                    _CheckMainCanvasRectSize = Vector2.zero;
                    _MainCanvasRect = null;
                    _MainCanvas = null;
                }
        }
        GUILayout.EndHorizontal();

        //LiveEditor
        if (_MainCanvas != null)
            HUD_Editor();
    }

    //Home > QuickUI : HUD Editor
    void HUD_Editor()
    {
        HUD_Editor_Profile();
        HUD_Editor_Tabs();

        //HUD Settings
        _HUD_EnableLiveEdit = EditorGUILayout.Toggle("Enable LiveUpdate",_HUD_EnableLiveEdit);
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        HUD_Editor_Obj();
        HUD_Editor_CanvasOptions();

        EditorGUILayout.EndScrollView();
    }
    void HUD_Editor_Obj()
    {
        for (int i = 0; i < _HUDTab[_HUDTabID].HUD_TabOjects.Count; i++)
        {
            GUILayout.BeginHorizontal();
            _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Name = EditorGUILayout.TextField("", _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Name);
            _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Type = (Tool_QuickStartUI_Object.HUD_Types)EditorGUILayout.EnumPopup("", _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Type);
            if (_HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Type != _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_CheckType)
            {
                if (GUILayout.Button("Update"))
                {
                    _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_RectTransform = null;
                    DestroyImmediate(_HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Object);
                    HUD_Change_Type(_HUDTab[_HUDTabID].HUD_TabOjects[i]);
                    _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_CheckType = _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Type;
                }
            }
            GUILayout.EndHorizontal();


            //MoreInfo
            _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_FoldOut = EditorGUILayout.Foldout(_HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_FoldOut, "More: ");
            if (_HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_FoldOut)
            {
                _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Location = (Tool_QuickStartUI_Object.HUD_Locations)EditorGUILayout.EnumPopup("Location:", _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Location);

                //Size
                _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Size = EditorGUILayout.Vector2Field("Size", _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Size);

                //Scale
                _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Scale = EditorGUILayout.Vector3Field("Scale", _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Scale);

                //Offset
                _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Offset = EditorGUILayout.Vector2Field("Offset", _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Offset);

                if (GUILayout.Button("Remove"))
                {
                    DestroyImmediate(_HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Object);
                    _HUDTab[_HUDTabID].HUD_TabOjects.Remove(_HUDTab[_HUDTabID].HUD_TabOjects[i]);
                }
            }
        }
    }
    void HUD_Editor_Tabs()
    {
        String[] tabs = new string[_HUDTab.Count];
        for (int i = 0; i < _HUDTab.Count; i++)
        {
            tabs[i] = i.ToString();
        }

        GUILayout.BeginHorizontal();
        _HUDTabID = GUILayout.Toolbar(_HUDTabID, tabs);

        if (GUILayout.Button("Add", GUILayout.Width(50)))
        {
            HUD_Add_Tab();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("ToggleActive"))
        {
            _HUDTab[_HUDTabID].HUD_TabParent.SetActive(!_HUDTab[_HUDTabID].HUD_TabParent.activeSelf);
        }

    }
    void HUD_Editor_Profile()
    {
        GUILayout.BeginHorizontal();
        _HUD_Profiles = (HUDProfiles)EditorGUILayout.EnumPopup("Load Profile:", _HUD_Profiles);
        if (GUILayout.Button("Load", GUILayout.Width(50)))
        {
            HUD_ClearLoaded();
            switch (_HUD_Profiles)
            {
                case HUDProfiles.Default:
                    HUD_LoadProfile_Default();
                    break;
            }
        }
        GUILayout.EndHorizontal();
    }
    void HUD_Editor_CanvasOptions()
    {
        if (_MainCanvas != null)
        {
            //LiveEdit
            if (_HUD_EnableLiveEdit)
            {
                if (GUILayout.Button("Create"))
                {
                    Tool_QuickStartUI_Object newuiobj = new Tool_QuickStartUI_Object();
                    newuiobj.HUD_Object = HUD_Create_Text();
                    newuiobj.HUD_RectTransform = newuiobj.HUD_Object.GetComponent<RectTransform>();
                    newuiobj.HUD_CheckType = Tool_QuickStartUI_Object.HUD_Types.Text;
                    newuiobj.HUD_Object.name = "New Text";
                    newuiobj.HUD_Name = "New Text";
                    newuiobj.HUD_Size = new Vector2(200, 60);
                    newuiobj.HUD_Object.transform.SetParent(_HUDTab[_HUDTabID].HUD_TabParent.transform);
                    _HUDTab[_HUDTabID].HUD_TabOjects.Add(newuiobj);
                }
                LiveHUDEditorUpdate();
            }
            else
            {
                if (GUILayout.Button("Add"))
                {
                    Tool_QuickStartUI_Object newuiobj = new Tool_QuickStartUI_Object();
                    _HUDTab[_HUDTabID].HUD_TabOjects.Add(newuiobj);
                }
                if (GUILayout.Button("Update"))
                {
                    LiveHUDEditorUpdate();
                }
            }
        }
        else
            GUILayout.Label("Add or assign canvas to create/add");
    }

    //Home > QuickUI : HUD Updator
    void LiveHUDEditorUpdate()
    {
        for (int i = 0; i < _HUDTab[_HUDTabID].HUD_TabOjects.Count; i++)
        {
            if(_HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Object != null)
            {
                //Update HUD
                HUD_Change_Position(_HUDTab[_HUDTabID].HUD_TabOjects[i]);
                HUD_Set_Size(_HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_RectTransform, _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Size);
                HUD_Set_Scale(_HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_RectTransform, _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Scale);
                HUD_Set_SetOffSet(_HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_RectTransform, _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Offset);
                _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Object.name = _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Name;

                HUD_TextSize(_HUDTab[_HUDTabID].HUD_TabOjects[i]);

                //Update canvas size / tab size
                if(_MainCanvasRect == null)
                    _MainCanvasRect = _MainCanvas.GetComponent<RectTransform>();

                if (_CheckMainCanvasRectSize != _MainCanvasRect.sizeDelta)
                {
                    for (int j = 0; j < _HUDTab.Count; j++)
                    {
                        _HUDTab[j].HUD_TabParent.GetComponent<RectTransform>().sizeDelta = _MainCanvasRect.sizeDelta;
                    }
                    _CheckMainCanvasRectSize = _MainCanvasRect.sizeDelta;
                }

                //Update text size
                for (int j = 0; j < _HUDTab[_HUDTabID].HUD_TabOjects.Count; j++)
                {
                    if(_HUDTab[_HUDTabID].HUD_TabOjects[j].HUD_Type == Tool_QuickStartUI_Object.HUD_Types.Button)
                    {
                        for (int o = 0; o < _HUDTab[_HUDTabID].HUD_TabOjects[j].HUD_Text.Count; o++)
                        {
                            _HUDTab[_HUDTabID].HUD_TabOjects[j].HUD_Text[o].rectTransform.sizeDelta = _HUDTab[_HUDTabID].HUD_TabOjects[j].HUD_Size;
                        }
                    }
                }
            }
        }
    }

    //Home > QuickUI : HUD Edit
    void HUD_Change_Position(Tool_QuickStartUI_Object obj)
    {
        switch(obj.HUD_Location)
        {
            case Tool_QuickStartUI_Object.HUD_Locations.TopLeft: HUD_Set_Rect(obj.HUD_RectTransform, "topleft"); break;
            case Tool_QuickStartUI_Object.HUD_Locations.TopMiddle: HUD_Set_Rect(obj.HUD_RectTransform, "topmiddle"); break;
            case Tool_QuickStartUI_Object.HUD_Locations.TopRight: HUD_Set_Rect(obj.HUD_RectTransform, "topright"); break;
            case Tool_QuickStartUI_Object.HUD_Locations.RightMiddle: HUD_Set_Rect(obj.HUD_RectTransform, "rightmiddle"); break;
            case Tool_QuickStartUI_Object.HUD_Locations.LeftMiddle: HUD_Set_Rect(obj.HUD_RectTransform, "leftmiddle"); break;
            case Tool_QuickStartUI_Object.HUD_Locations.BottomLeft: HUD_Set_Rect(obj.HUD_RectTransform, "bottomleft"); break;
            case Tool_QuickStartUI_Object.HUD_Locations.BottomMiddle: HUD_Set_Rect(obj.HUD_RectTransform, "bottommiddle"); break;
            case Tool_QuickStartUI_Object.HUD_Locations.BottomRight: HUD_Set_Rect(obj.HUD_RectTransform, "bottomright"); break;
            case Tool_QuickStartUI_Object.HUD_Locations.Middle: HUD_Set_Rect(obj.HUD_RectTransform, "middle"); break;
        }
    }
    void HUD_Change_Type(Tool_QuickStartUI_Object obj)
    {
        //Change Type
        switch(obj.HUD_Type)
        {
            case Tool_QuickStartUI_Object.HUD_Types.Text:
                obj.HUD_Object = HUD_Create_Text();
                obj.HUD_Object.name = "New Text";
                break;
            case Tool_QuickStartUI_Object.HUD_Types.Button:
                obj.HUD_Object = HUD_Create_Button();
                obj.HUD_Object.name = "New Button";
                break;
            case Tool_QuickStartUI_Object.HUD_Types.Dropdown:
                obj.HUD_Object = HUD_Create_DropDown();
                obj.HUD_Object.name = "New Dropdown";
                break;
            case Tool_QuickStartUI_Object.HUD_Types.Slider:
                obj.HUD_Object = HUD_Create_Slider();
                obj.HUD_Object.name = "New Slider";
                obj.HUD_Size = new Vector2(obj.HUD_Size.x, obj.HUD_Size.y / 3);
                break;
            case Tool_QuickStartUI_Object.HUD_Types.Bar:
                obj.HUD_Object = HUD_Create_Bar();
                obj.HUD_Object.name = "New Bar";
                break;
        }

        if(obj.HUD_Type != Tool_QuickStartUI_Object.HUD_Types.Slider && obj.HUD_CheckType == Tool_QuickStartUI_Object.HUD_Types.Slider)
            obj.HUD_Size = new Vector2(obj.HUD_Size.x, obj.HUD_Size.y * 3);

        if (obj.HUD_Name == "" || obj.HUD_Name == null || obj.HUD_Name == "New Text" || obj.HUD_Name == "New Button" || obj.HUD_Name == "New Dropdown" || obj.HUD_Name == "New Slider" || obj.HUD_Name == "New Bar")
            obj.HUD_Name = obj.HUD_Object.name;

        obj.HUD_RectTransform = obj.HUD_Object.GetComponent<RectTransform>();
        HUD_Change_Position(obj);

        //Add to tab
        obj.HUD_Object.transform.SetParent(_HUDTab[_HUDTabID].HUD_TabParent.transform);

        //Update UI Obj text ref
        obj.HUD_Text.Clear();
        for (int i = 0; i < obj.HUD_Object.transform.childCount; i++)
        {
            if (obj.HUD_Object.transform.GetChild(i).GetComponent<TextMeshProUGUI>() != null)
                obj.HUD_Text.Add(obj.HUD_Object.transform.GetChild(i).GetComponent<TextMeshProUGUI>());
        }
    }
    void HUD_TextSize(Tool_QuickStartUI_Object obj)
    {
        for (int i = 0; i < obj.HUD_Text.Count; i++)
        {
            obj.HUD_Text[i].fontSize = obj.HUD_TextFontSize;
        }
    }

    //Home > QuickUI : HUD Create
    GameObject HUD_Create_Text()
    {
        GameObject newhud_text = HUD_Create_Template();
        newhud_text.AddComponent<TextMeshProUGUI>().text = "New Text";

        return newhud_text;
    }
    GameObject HUD_Create_Button()
    {
        GameObject newhud_button = HUD_Create_Template();

        newhud_button.AddComponent<CanvasRenderer>();
        Image buttonimage = newhud_button.AddComponent<Image>();
        buttonimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        buttonimage.type = Image.Type.Sliced;
        Button buttonbutton = newhud_button.AddComponent<Button>();
        buttonbutton.targetGraphic = buttonimage;

        GameObject buttontextemplate = new GameObject();
        RectTransform buttontextrect = buttontextemplate.AddComponent<RectTransform>();
        buttontextrect.anchoredPosition = new Vector3(5,0,0);

        TextMeshProUGUI buttontexttmpro = buttontextemplate.AddComponent<TextMeshProUGUI>();
        buttontexttmpro.text = "New Button";
        buttontexttmpro.alignment = TextAlignmentOptions.MidlineLeft;
        buttontexttmpro.color = Color.black;


        buttontextemplate.name = name + "text";

        buttontextemplate.transform.SetParent(newhud_button.transform);

        newhud_button.transform.SetParent(_MainCanvas.transform);

        return newhud_button;
    }
    GameObject HUD_Create_DropDown()
    {
        //Create objects
        GameObject dropdownobj = new GameObject();
        GameObject dropdown_label = new GameObject();
        GameObject dropdown_arrow = new GameObject();
        GameObject dropdown_template = new GameObject();

        GameObject dropdown_viewport = new GameObject();
        GameObject dropdown_content = new GameObject();
        GameObject dropdown_item = new GameObject();

        GameObject dropdown_item_background = new GameObject();
        GameObject dropdown_item_checkmark = new GameObject();
        GameObject dropdown_item_label = new GameObject();

        GameObject dropdown_scrollbar = new GameObject();
        GameObject dropdown_slidingarea = new GameObject();
        GameObject dropdown_handle = new GameObject();

        dropdown_template.SetActive(false);

        //Set Name
        dropdownobj.name = name;
        dropdown_label.name = "Label";
        dropdown_arrow.name = "Arrow";
        dropdown_template.name = "Template";

        dropdown_viewport.name = "Viewport";
        dropdown_content.name = "Content";
        dropdown_item.name = "Item";

        dropdown_item_background.name = "Item Background";
        dropdown_item_checkmark.name = "Item Checkmark";
        dropdown_item_label.name = "Item Label";

        dropdown_scrollbar.name = "Scrollbar";
        dropdown_slidingarea.name = "Sliding Area";
        dropdown_handle.name = "Handle";

        //Add RectTransform
        RectTransform dropdownobjrect = dropdownobj.AddComponent<RectTransform>();
        RectTransform dropdown_labelrect = dropdown_label.AddComponent<RectTransform>();
        RectTransform dropdown_arrowrect = dropdown_arrow.AddComponent<RectTransform>();
        RectTransform dropdown_templaterect = dropdown_template.AddComponent<RectTransform>();

        RectTransform dropdown_viewportrect = dropdown_viewport.AddComponent<RectTransform>();
        RectTransform dropdown_contentrect = dropdown_content.AddComponent<RectTransform>();
        RectTransform dropdown_itemrect = dropdown_item.AddComponent<RectTransform>();

        RectTransform dropdown_item_backgroundrect = dropdown_item_background.AddComponent<RectTransform>();
        RectTransform dropdown_item_checkmarkrect = dropdown_item_checkmark.AddComponent<RectTransform>();
        RectTransform dropdown_item_labelrect = dropdown_item_label.AddComponent<RectTransform>();

        RectTransform dropdown_scrollbarrect = dropdown_scrollbar.AddComponent<RectTransform>();
        RectTransform dropdown_slidingarearect = dropdown_slidingarea.AddComponent<RectTransform>();
        RectTransform dropdown_handlerect = dropdown_handle.AddComponent<RectTransform>();

        //SetParent
        dropdown_label.transform.SetParent(dropdownobj.transform);
        dropdown_arrow.transform.SetParent(dropdownobj.transform);
        dropdown_template.transform.SetParent(dropdownobj.transform);

        dropdown_viewport.transform.SetParent(dropdown_template.transform);
        dropdown_content.transform.SetParent(dropdown_viewport.transform);
        dropdown_item.transform.SetParent(dropdown_content.transform);

        dropdown_item_background.transform.SetParent(dropdown_item.transform);
        dropdown_item_checkmark.transform.SetParent(dropdown_item.transform);
        dropdown_item_label.transform.SetParent(dropdown_item.transform);

        dropdown_scrollbar.transform.SetParent(dropdown_template.transform);
        dropdown_slidingarea.transform.SetParent(dropdown_scrollbar.transform);
        dropdown_handle.transform.SetParent(dropdown_slidingarea.transform);

        //Set Rect dropdownobj
        Image dropdownimage = dropdownobj.AddComponent<Image>();
        dropdownimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        dropdownimage.type = Image.Type.Sliced;
        TMP_Dropdown dropdowntmp = dropdownobj.AddComponent<TMP_Dropdown>();
        List<TMP_Dropdown.OptionData> newoptions = new List<TMP_Dropdown.OptionData>();

        TMP_Dropdown.OptionData option1 = new TMP_Dropdown.OptionData();
        TMP_Dropdown.OptionData option2 = new TMP_Dropdown.OptionData();
        TMP_Dropdown.OptionData option3 = new TMP_Dropdown.OptionData();

        option1.text = "Option A";
        option2.text = "Option B";
        option3.text = "Option C";

        newoptions.Add(option1);
        newoptions.Add(option2);
        newoptions.Add(option3);

        dropdowntmp.AddOptions(newoptions);

        //Set Rect Label
        dropdown_labelrect.anchorMin = new Vector2(0, 0);
        dropdown_labelrect.anchorMax = new Vector2(1, 1);
        dropdown_labelrect.pivot = new Vector2(0.5f, 0.5f);
        dropdown_labelrect.sizeDelta = new Vector2(0, 0);
        dropdown_labelrect.anchoredPosition = new Vector4(0, 0);

        //Set Rect Arrow
        dropdown_arrowrect.anchorMin = new Vector2(1, 0.5f);
        dropdown_arrowrect.anchorMax = new Vector2(1, 0.5f);
        dropdown_arrowrect.pivot = new Vector2(0.5f, 0.5f);
        dropdown_arrowrect.sizeDelta = new Vector2(20, 20);
        dropdown_arrowrect.anchoredPosition = new Vector4(-15, 0);

        //Set Rect Template
        dropdown_templaterect.anchorMin = new Vector2(0, 0);
        dropdown_templaterect.anchorMax = new Vector2(1, 0);
        dropdown_templaterect.pivot = new Vector2(0.5f, 1);
        dropdown_templaterect.sizeDelta = new Vector2(0, 150);
        dropdown_templaterect.anchoredPosition = new Vector4(0, 2);

        //Set Rect Viewport
        dropdown_viewportrect.anchorMin = new Vector2(0, 0);
        dropdown_viewportrect.anchorMax = new Vector2(1, 1);
        dropdown_viewportrect.pivot = new Vector2(0, 1);
        dropdown_viewportrect.sizeDelta = new Vector2(0, 0);
        dropdown_viewportrect.anchoredPosition = new Vector4(0, 0);

        //Set Rect Content
        dropdown_contentrect.anchorMin = new Vector2(0, 1);
        dropdown_contentrect.anchorMax = new Vector2(1, 1);
        dropdown_contentrect.pivot = new Vector2(0.5f, 1);
        dropdown_contentrect.sizeDelta = new Vector2(0, 28);
        dropdown_contentrect.anchoredPosition = new Vector4(0, 0);

        //Set Rect Item
        dropdown_itemrect.anchorMin = new Vector2(0, 0.5f);
        dropdown_itemrect.anchorMax = new Vector2(1, 0.5f);
        dropdown_itemrect.pivot = new Vector2(0.5f, 0.5f);
        dropdown_itemrect.sizeDelta = new Vector2(0,28);
        dropdown_itemrect.anchoredPosition = new Vector4(0, -15); //NotDy

        //Set Rect Item Background
        dropdown_item_backgroundrect.anchorMin = new Vector2(0, 0);
        dropdown_item_backgroundrect.anchorMax = new Vector2(1, 1);
        dropdown_item_backgroundrect.pivot = new Vector2(0.5f, 0.5f);
        dropdown_item_backgroundrect.sizeDelta = new Vector2(0, 0);
        dropdown_item_backgroundrect.anchoredPosition = new Vector4(0, 0);

        //Set Rect Item Checkmark
        dropdown_item_checkmarkrect.anchorMin = new Vector2(0, 0.5f);
        dropdown_item_checkmarkrect.anchorMax = new Vector2(0, 0.5f);
        dropdown_item_checkmarkrect.pivot = new Vector2(0.5f, 0.5f);
        dropdown_item_checkmarkrect.sizeDelta = new Vector2(20, 20);
        dropdown_item_checkmarkrect.anchoredPosition = new Vector4(10, 0);

        //Set Rect Item Label
        dropdown_item_labelrect.anchorMin = new Vector2(0, 0);
        dropdown_item_labelrect.anchorMax = new Vector2(1, 1);
        dropdown_item_labelrect.pivot = new Vector2(0.5f, 0.5f);
        dropdown_item_labelrect.sizeDelta = new Vector2(10, 1);
        dropdown_item_labelrect.anchoredPosition = new Vector4(20, 2);

        //Set Rect Scrollbar
        dropdown_scrollbarrect.anchorMin = new Vector2(1, 0);
        dropdown_scrollbarrect.anchorMax = new Vector2(1, 1);
        dropdown_scrollbarrect.pivot = new Vector2(1, 1);
        dropdown_scrollbarrect.sizeDelta = new Vector2(20, 0);
        dropdown_scrollbarrect.anchoredPosition = new Vector4(0, 0);

        //Set Rect Sliding Area
        dropdown_slidingarearect.anchorMin = new Vector2(0, 0);
        dropdown_slidingarearect.anchorMax = new Vector2(1, 1);
        dropdown_slidingarearect.pivot = new Vector2(0.5f, 0.5f);
        dropdown_slidingarearect.sizeDelta = new Vector2(10, 10);
        dropdown_slidingarearect.anchoredPosition = new Vector4(10, 10);

        //Set Rect Handle
        dropdown_handlerect.anchorMin = new Vector2(0, 0);
        dropdown_handlerect.anchorMax = new Vector2(1, 0.2f);
        dropdown_handlerect.pivot = new Vector2(0.5f, 0.5f);
        dropdown_handlerect.sizeDelta = new Vector2(-10, -10);
        dropdown_handlerect.anchoredPosition = new Vector4(-10, -10);

        //
        dropdown_arrow.AddComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd"); ;

        //
        dropdowntmp.template = dropdown_templaterect;
        dropdowntmp.captionText = dropdown_label.GetComponent<TextMeshProUGUI>();
        dropdowntmp.itemText = dropdown_item_label.GetComponent<TextMeshProUGUI>();

        //handle
        Image dropdown_handleimage = dropdown_handle.AddComponent<Image>();
        dropdown_handleimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"); ;
        dropdown_handleimage.type = Image.Type.Sliced;

        //scrollbar
        Image dropdown_scrollbarimage = dropdown_scrollbar.AddComponent<Image>();
        dropdown_scrollbarimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"); ;
        dropdown_scrollbarimage.type = Image.Type.Sliced;
        Scrollbar dropdown_scrollbar_scroll = dropdown_scrollbar.AddComponent<Scrollbar>();
        dropdown_scrollbar_scroll.targetGraphic = dropdown_handleimage;
        dropdown_scrollbar_scroll.handleRect = dropdown_handlerect;
        dropdown_scrollbar_scroll.direction = Scrollbar.Direction.BottomToTop;

        //Template
        Image dropdown_templateimage = dropdown_template.AddComponent<Image>();
        dropdown_templateimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        dropdown_templateimage.type = Image.Type.Sliced;
        ScrollRect dropdown_templatescrollrect = dropdown_template.AddComponent<ScrollRect>();
        dropdown_templatescrollrect.content = dropdown_contentrect;
        dropdown_templatescrollrect.decelerationRate = 0.135f;
        dropdown_templatescrollrect.scrollSensitivity = 1;
        dropdown_templatescrollrect.viewport = dropdown_viewportrect;
        dropdown_templatescrollrect.movementType = ScrollRect.MovementType.Clamped;
        dropdown_templatescrollrect.verticalScrollbar = dropdown_scrollbar_scroll;
        dropdown_templatescrollrect.horizontal = false;
        dropdown_templatescrollrect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        dropdown_templatescrollrect.verticalScrollbarSpacing = -3;

        //viewport
        Mask dropdown_viewportmask = dropdown_viewport.AddComponent<Mask>();
        Image dropdown_viewportimage = dropdown_viewport.AddComponent<Image>();
        dropdown_viewportimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");
        dropdown_viewportimage.type = Image.Type.Sliced;

        //Item Background
        dropdown_item_background.AddComponent<Image>();

        //Item Checkmark
        dropdown_item_checkmark.AddComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd"); ;

        //Item Label
        TextMeshProUGUI dropdown_item_labeltmp = dropdown_item_label.AddComponent<TextMeshProUGUI>();
        dropdown_item_labeltmp.text = "Option A";
        dropdown_item_labeltmp.color = Color.black;

        //LabelText
        TextMeshProUGUI dropdown_labeltext = dropdown_label.AddComponent<TextMeshProUGUI>();
        dropdown_labeltext.alignment = TextAlignmentOptions.MidlineLeft;
        dropdown_labeltext.color = Color.black;
        dropdown_labeltext.text = "Option A";

        //Item
        Toggle dropdown_itemtoggle = dropdown_item.AddComponent<Toggle>();
        dropdown_itemtoggle.targetGraphic = dropdown_item_background.GetComponent<Image>();
        dropdown_itemtoggle.graphic = dropdown_item_checkmark.GetComponent<Image>();
        dropdown_itemtoggle.isOn = true;

        //dropdownobj
        dropdowntmp.targetGraphic = dropdownimage;
        dropdowntmp.itemText = dropdown_item_labeltmp;

        //AddToOptions
        dropdownobj.transform.SetParent(_MainCanvas.transform);

        dropdowntmp.captionText = dropdown_labeltext;

        //dropdownobjrect.sizeDelta = new Vector2(0,0);

        return dropdownobj;
    }
    GameObject HUD_Create_Slider()
    {
        //Create Objects
        GameObject newsliderbackground = new GameObject();
        GameObject newsliderobj = new GameObject();
        GameObject newsliderfillarea = new GameObject();
        GameObject newsliderfill = new GameObject();
        GameObject newsliderslidearea = new GameObject();
        GameObject newsliderhandle = new GameObject();

        newsliderobj.name = name;

        //Set Parents
        newsliderbackground.transform.SetParent(newsliderobj.transform);
        newsliderfill.transform.SetParent(newsliderfillarea.transform);
        newsliderfillarea.transform.SetParent(newsliderobj.transform);
        newsliderhandle.transform.SetParent(newsliderslidearea.transform);
        newsliderslidearea.transform.SetParent(newsliderobj.transform);

        //Add RectTransform
        RectTransform newsliderbackgroundrect = newsliderbackground.AddComponent<RectTransform>();
        RectTransform buttonfillarearect = newsliderfillarea.AddComponent<RectTransform>();
        RectTransform buttonfillrect = newsliderfill.AddComponent<RectTransform>();
        RectTransform buttonslidearearect = newsliderslidearea.AddComponent<RectTransform>();
        RectTransform buttonhandlerect = newsliderhandle.AddComponent<RectTransform>();

        //Add Images
        Image newsliderbackgroundimage = newsliderbackground.AddComponent<Image>();
        Image newsliderfillimage = newsliderfill.AddComponent<Image>();
        newsliderfillimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        newsliderfillimage.type = Image.Type.Sliced;
        newsliderfillimage.color = Color.grey;
        Image newsliderhandleimage = newsliderhandle.AddComponent<Image>();

        //Set Rect NewObj
        Slider newsliderslider = newsliderobj.AddComponent<Slider>();

        //Set Rect Background
        newsliderbackgroundrect.anchorMin = new Vector2(0, 0.25f);
        newsliderbackgroundrect.anchorMax = new Vector2(1, 0.75f);
        newsliderbackgroundrect.pivot = new Vector2(0.5f, 0.5f);
        newsliderbackgroundrect.sizeDelta = new Vector2(0, 0);
        newsliderbackgroundrect.anchoredPosition = new Vector2(0, 0);
        newsliderbackground.name = "BackGround";

        //Set Rect FillArea
        buttonfillarearect.anchorMin = new Vector2(0, 0.25f);
        buttonfillarearect.anchorMax = new Vector2(1, 0.75f);
        buttonfillarearect.pivot = new Vector2(0.5f, 0.5f);
        buttonfillarearect.sizeDelta = new Vector2(15, 0);
        buttonfillarearect.anchoredPosition = new Vector2(5, 0);
        newsliderfillarea.name = "FillArea";

        //Set Rect Fill
        buttonfillrect.anchorMin = new Vector2(0, 0.25f);
        buttonfillrect.anchorMax = new Vector2(1, 0.75f);
        buttonfillrect.pivot = new Vector2(0.5f, 0.5f);
        buttonfillrect.sizeDelta = new Vector2(10, 0);
        buttonfillrect.anchoredPosition = new Vector4(0, 0);
        newsliderfill.name = "Fill";

        //Set Rect SliderArea
        buttonslidearearect.anchorMin = new Vector2(0, 0);
        buttonslidearearect.anchorMax = new Vector2(1, 1);
        buttonslidearearect.pivot = new Vector2(0.5f, 0.5f);
        buttonslidearearect.sizeDelta = new Vector2(10, 0);
        buttonslidearearect.anchoredPosition = new Vector2(10, 0);
        newsliderslidearea.name = "Handle Slide Area";

        //Set Rect Handle
        buttonhandlerect.anchorMin = new Vector2(0, 0.25f);
        buttonhandlerect.anchorMax = new Vector2(1, 0.75f);
        buttonhandlerect.pivot = new Vector2(0.5f, 0.5f);
        buttonhandlerect.sizeDelta = new Vector2(20, 0);
        buttonhandlerect.anchoredPosition = new Vector2(0, 0);
        newsliderhandleimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        newsliderslider.image = newsliderhandleimage;
        newsliderslider.fillRect = buttonfillrect;
        newsliderslider.handleRect = buttonhandlerect;
        newsliderhandle.name = "Handle";

        newsliderobj.transform.SetParent(_MainCanvas.transform);

        return newsliderobj;
    }
    GameObject HUD_Create_Bar()
    {
        GameObject newhud_text = HUD_Create_Template();

        return newhud_text;
    }
    GameObject HUD_Create_Template()
    {
        GameObject newhudobj = new GameObject();
        newhudobj.AddComponent<RectTransform>();
        newhudobj.transform.SetParent(_MainCanvas.transform);
        return newhudobj;
    }
    GameObject HUD_Create_Canvas()
    {
        GameObject canvasobj = new GameObject();
        canvasobj.name = "TestCanvas";
        Canvas canvasobj_canvas = canvasobj.AddComponent<Canvas>();
        canvasobj_canvas.worldCamera = Camera.main;
        CanvasScaler canvasscale = canvasobj.AddComponent<CanvasScaler>();
        canvasscale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasscale.referenceResolution = new Vector2(1920, 1080);
        canvasobj.AddComponent<GraphicRaycaster>();

        if (GameObject.Find("EventSystem") == null)
        {
            GameObject eventsystemobj = new GameObject();
            eventsystemobj.name = "EventSystem";
            eventsystemobj.AddComponent<EventSystem>();
            eventsystemobj.AddComponent<StandaloneInputModule>();
        }

        Canvas canvascomponent = canvasobj.GetComponent<Canvas>();
        canvascomponent.renderMode = RenderMode.ScreenSpaceCamera;

        return canvasobj;
    }

    //Home > QuickUI : HUD Set
    void HUD_Set_Rect(RectTransform rect, string anchorpos)
    {
        switch (anchorpos)
        {
            case "topleft":
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                break;
            case "topmiddle":
                rect.anchorMin = new Vector2(0.5f, 1);
                rect.anchorMax = new Vector2(0.5f, 1);
                rect.pivot = new Vector2(0.5f, 1);
                break;
            case "topright":
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                break;
            case "rightmiddle":
                rect.anchorMin = new Vector2(1, 0.5f);
                rect.anchorMax = new Vector2(1, 0.5f);
                rect.pivot = new Vector2(1, 0.5f);
                break;
            case "bottomright":
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(1, 0);
                break;
            case "bottommiddle":
                rect.anchorMin = new Vector2(0.5f, 0);
                rect.anchorMax = new Vector2(0.5f, 0);
                rect.pivot = new Vector2(0.5f, 0);
                break;
            case "bottomleft":
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(0, 0);
                break;
            case "leftmiddle":
                rect.anchorMin = new Vector2(0, 0.5f);
                rect.anchorMax = new Vector2(0, 0.5f);
                rect.pivot = new Vector2(0, 0.5f);
                break;
            case "middle":
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                break;
        }
    }
    void HUD_Set_Size(RectTransform rect, Vector2 size)
    {
        rect.sizeDelta = size;
    }
    void HUD_Set_Scale(RectTransform rect, Vector3 scale)
    {
        rect.localScale = scale;
    }
    void HUD_Set_SetOffSet(RectTransform rect, Vector3 offset)
    {
        rect.anchoredPosition = offset;
    }

    //Home > QuickUI : HUD Add
    void HUD_Add_Tab()
    {
        Tool_QuickStartUI_Tab newtab = new Tool_QuickStartUI_Tab();
        newtab.HUD_TabParent = HUD_Create_Template();

        RectTransform rect_main = _MainCanvas.GetComponent<RectTransform>();
        RectTransform rect_tab = newtab.HUD_TabParent.GetComponent<RectTransform>();

        rect_tab.sizeDelta = rect_main.sizeDelta;
        rect_tab.anchoredPosition = new Vector2(0, 0);
        rect_tab.localScale = new Vector3(1,1,1);

        newtab.HUD_TabParent.name = _HUDTab.Count.ToString();
        _HUDTab.Add(newtab);
    }

    //Home > QuickUI : HUD Profiles
    void HUD_ClearLoaded()
    {
        for (int i = 0; i < _HUDTab.Count; i++)
        {
            DestroyImmediate(_HUDTab[i].HUD_TabParent);
        }
        _HUDTab.Clear();
        _HUDTabID = 0;
    }
    void HUD_LoadProfile_Default()
    {
        HUD_Add_Tab(); //0 Home
        HUD_Add_Tab(); //1 Display
        HUD_Add_Tab(); //2 Graphics
        HUD_Add_Tab(); //3 Gameplay
        HUD_Add_Tab(); //4 Controls

        //============================================================================================= 0 Home
        Tool_QuickStartUI_Object tab_home_startbutton = new Tool_QuickStartUI_Object();
        tab_home_startbutton.HUD_Name = "Button_Start";
        tab_home_startbutton.HUD_Type = Tool_QuickStartUI_Object.HUD_Types.Button;
        tab_home_startbutton.HUD_Location = Tool_QuickStartUI_Object.HUD_Locations.BottomLeft;
        tab_home_startbutton.HUD_Offset = new Vector3(40, 450, 0);
        tab_home_startbutton.HUD_Size = new Vector2(500, 100);
        tab_home_startbutton.HUD_Scale = new Vector3(1, 1, 1);

        Tool_QuickStartUI_Object tab_home_optionsbutton = new Tool_QuickStartUI_Object();
        tab_home_optionsbutton.HUD_Name = "Button_Options";
        tab_home_optionsbutton.HUD_Type = Tool_QuickStartUI_Object.HUD_Types.Button;
        tab_home_optionsbutton.HUD_Location = Tool_QuickStartUI_Object.HUD_Locations.BottomLeft;
        tab_home_optionsbutton.HUD_Offset = new Vector3(40, 330, 0);
        tab_home_optionsbutton.HUD_Size = new Vector2(500, 100);
        tab_home_optionsbutton.HUD_Scale = new Vector3(1, 1, 1);

        Tool_QuickStartUI_Object tab_home_quitbutton = new Tool_QuickStartUI_Object();
        tab_home_quitbutton.HUD_Name = "Button_Quit";
        tab_home_quitbutton.HUD_Type = Tool_QuickStartUI_Object.HUD_Types.Button;
        tab_home_quitbutton.HUD_Location = Tool_QuickStartUI_Object.HUD_Locations.BottomLeft;
        tab_home_quitbutton.HUD_Offset = new Vector3(40, 210, 0);
        tab_home_quitbutton.HUD_Size = new Vector2(500, 100);
        tab_home_quitbutton.HUD_Scale = new Vector3(1, 1, 1);

        _HUDTab[0].HUD_TabOjects.Add(tab_home_startbutton);
        _HUDTab[0].HUD_TabOjects.Add(tab_home_optionsbutton);
        _HUDTab[0].HUD_TabOjects.Add(tab_home_quitbutton);
        //============================================================================================= 1 Display
        Tool_QuickStartUI_Object tab_display_title = new Tool_QuickStartUI_Object();
        tab_display_title.HUD_Name = "Title_Display";
        tab_display_title.HUD_Type = Tool_QuickStartUI_Object.HUD_Types.Text;
        tab_display_title.HUD_Location = Tool_QuickStartUI_Object.HUD_Locations.BottomLeft;
        tab_display_title.HUD_Offset = new Vector3(800, 800, 0);

        Tool_QuickStartUI_Object tab_display_resolution = new Tool_QuickStartUI_Object();
        tab_display_resolution.HUD_Name = "Dropdown_Resolution";
        tab_display_resolution.HUD_Type = Tool_QuickStartUI_Object.HUD_Types.Dropdown;
        tab_display_resolution.HUD_Location = Tool_QuickStartUI_Object.HUD_Locations.BottomLeft;
        tab_display_resolution.HUD_Size = new Vector2(500,60);
        tab_display_resolution.HUD_Offset = new Vector3(800, 700, 0);
        Tool_QuickStartUI_Object tab_display_resolution_text = new Tool_QuickStartUI_Object();
        tab_display_resolution_text.HUD_Name = "Text_Resolution";
        tab_display_resolution_text.HUD_Type = Tool_QuickStartUI_Object.HUD_Types.Text;
        tab_display_resolution_text.HUD_Location = Tool_QuickStartUI_Object.HUD_Locations.BottomLeft;
        tab_display_resolution_text.HUD_Offset = new Vector3(600, 700, 0);

        Tool_QuickStartUI_Object tab_display_quality = new Tool_QuickStartUI_Object();
        tab_display_quality.HUD_Name = "Dropdown_Resolution";
        tab_display_quality.HUD_Type = Tool_QuickStartUI_Object.HUD_Types.Dropdown;
        tab_display_quality.HUD_Location = Tool_QuickStartUI_Object.HUD_Locations.BottomLeft;
        tab_display_quality.HUD_Size = new Vector2(500, 60);
        tab_display_quality.HUD_Offset = new Vector3(800, 630, 0);

        Tool_QuickStartUI_Object tab_display_fullscreen = new Tool_QuickStartUI_Object();
        tab_display_fullscreen.HUD_Name = "Dropown_Windowmode";
        tab_display_fullscreen.HUD_Type = Tool_QuickStartUI_Object.HUD_Types.Dropdown;
        tab_display_fullscreen.HUD_Location = Tool_QuickStartUI_Object.HUD_Locations.BottomLeft;
        tab_display_fullscreen.HUD_Size = new Vector2(500, 60);
        tab_display_fullscreen.HUD_Offset = new Vector3(800, 560, 0);

        _HUDTab[1].HUD_TabOjects.Add(tab_display_title);
        _HUDTab[1].HUD_TabOjects.Add(tab_display_resolution);
        _HUDTab[1].HUD_TabOjects.Add(tab_display_resolution_text);
        _HUDTab[1].HUD_TabOjects.Add(tab_display_quality);
        _HUDTab[1].HUD_TabOjects.Add(tab_display_fullscreen);

        //Update
        for (int i = 0; i < _HUDTab.Count; i++)
        {
            _HUDTabID = i;
            for (int j = 0; j < _HUDTab[i].HUD_TabOjects.Count; j++)
            {
                _HUDTab[i].HUD_TabOjects[j].HUD_RectTransform = null;
                DestroyImmediate(_HUDTab[i].HUD_TabOjects[j].HUD_Object);
                HUD_Change_Type(_HUDTab[i].HUD_TabOjects[j]);
                _HUDTab[i].HUD_TabOjects[j].HUD_CheckType = _HUDTab[i].HUD_TabOjects[j].HUD_Type;
            }
        }
        _HUDTabID = 0;
    }

    //Home > QuickUI : Set Script Refs
    void Set_SettingsHandler()
    {
        if (ScriptExist("SettingsHandler"))
        {
            string UniType = "SettingsHandler";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            GameObject settingshandlerobj = new GameObject();
            settingshandlerobj.AddComponent(UnityType);

            TMP_Dropdown[] dropdowns = Resources.FindObjectsOfTypeAll<TMP_Dropdown>();

            for (int i = 0; i < dropdowns.Length; i++)
            {
                if(dropdowns[i].name == "Dropdown_Resolution")
                {
                    settingshandlerobj.GetComponent(UnityType).SendMessage("SetDropDown_Resolution", dropdowns[i]);
                }
                if (dropdowns[i].name == "Dropdown_Quality")
                {
                    settingshandlerobj.GetComponent(UnityType).SendMessage("SetDropDown_Quality", dropdowns[i]);
                }
                if (dropdowns[i].name == "Dropdown_Antialiasing")
                {
                    settingshandlerobj.GetComponent(UnityType).SendMessage("SetDropDown_AA", dropdowns[i]);
                }
                if (dropdowns[i].name == "Dropdown_TextureQuality")
                {
                    settingshandlerobj.GetComponent(UnityType).SendMessage("SetDropDown_TextureQuality", dropdowns[i]);
                }
            }

            /*
            TMP_Dropdown resolution = Resources.FindObjectsOfTypeAll<TMP_Dropdown>(); //GameObject.Find("Dropdown_Resolution").GetComponent<TMP_Dropdown>();
            TMP_Dropdown quality = GameObject.Find("").GetComponent<TMP_Dropdown>();
            TMP_Dropdown texturequality = GameObject.Find("").GetComponent<TMP_Dropdown>();
            TMP_Dropdown aa = GameObject.Find("").GetComponent<TMP_Dropdown>();
            Slider volumeslider = GameObject.Find("").GetComponent<Slider>();

            
            settingshandlerobj.GetComponent(UnityType).SendMessage("SetDropDown_Quality", quality);
            settingshandlerobj.GetComponent(UnityType).SendMessage("SetDropDown_TextureQuality", texturequality);
            settingshandlerobj.GetComponent(UnityType).SendMessage("SetDropDown_AA", aa);

            settingshandlerobj.GetComponent(UnityType).SendMessage("SetSlider_VolumeSlider", volumeslider);
            */

            settingshandlerobj.name = "SettingsHandler";
        }
    }


    //FileFinder
    void FileFinder()
    {
        _ToolState = GUILayout.Toolbar(_ToolState, new string[] { "Assets", "Scene" });

        if (_ToolState == 0)
        {
            FileFinder_Search();
            FileFinder_SearchAssets();
        }
        else
        {
            FileFinder_SceneSearch();
            _FF_Scene_InsceneInfo = EditorGUILayout.Toggle("InScene Info", _FF_Scene_InsceneInfo);
            FileFinder_Scene();
        }

        //stop focus when switching
        if (_ToolStateCheck != _ToolState)
        {
            EditorGUI.FocusTextInControl("searchproject");
            _ToolStateCheck = _ToolState;
        }
    }
    void FileFinder_Search()
    {
        _FF_Search = EditorGUILayout.TextField("Search:", _FF_Search);
        _FF_Type = EditorGUILayout.TextField("Type:", _FF_Type);
        GUILayout.Label("(" + _FF_Results + "/" + _FF_Total + ")");

        _FF_Results = 0;
        _FF_Total = 0;

        if (_FF_Search != _FF_SearchCheck)
        {
            _FF_SearchResults = System.IO.Directory.GetFiles("Assets/", "*" + _FF_Type, System.IO.SearchOption.AllDirectories);
            _FF_SearchResultsChange = _FF_SearchResults;
            _FF_SearchCheck = _FF_Search;
        }
    }
    void FileFinder_SearchAssets()
    {
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        for (int i = 0; i < _FF_SearchResults.Length; i++)
        {
            if (_FF_SearchResults[i].ToLower().Contains(_FF_Search.ToLower()))
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label(_FF_SearchResults[i], GUILayout.Width(Screen.width - 80));
                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(_FF_SearchResults[i]);
                }
                GUILayout.EndHorizontal();
                _FF_Results++;
            }
            _FF_Total++;
        }
        EditorGUILayout.EndScrollView();
    }

    void FileFinder_SceneSearch()
    {
        _FF_Scene_Search = EditorGUILayout.TextField("Search:", _FF_Scene_Search);
        GUILayout.Label("(" + _FF_Results + "/" + _FF_Total + ")");

        _FF_Results = 0;
        _FF_Total = 0;

        if (_FF_Scene_Objects.Length == 0)
            _FF_Scene_Objects = FindObjectsOfType<GameObject>();
    }
    void FileFinder_Scene()
    {
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        for (int i = 0; i < _FF_Scene_Objects.Length; i++)
        {
            if (_FF_Scene_Objects[i].name.ToLower().Contains(_FF_Scene_Search.ToLower()))
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label(_FF_Scene_Objects[i].name, GUILayout.Width(Screen.width - 80));
                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    Selection.activeObject = _FF_Scene_Objects[i];
                }
                GUILayout.EndHorizontal();
                _FF_Results++;
            }
            _FF_Total++;
        }
        EditorGUILayout.EndScrollView();
    }


    //Script To String
    void ScriptToString_Menu()
    {
        if (GUILayout.Button("Convert", GUILayout.Height(30)))
            _STS_ScriptOutput = STS_ConvertScriptToString();

        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        STS_InputOutput();
        STS_Show_Keywords();
        STS_Display_TextEditor();
        EditorGUILayout.EndScrollView();
    }
    void STS_InputOutput()
    {
        GUILayout.Space(20);
        //Input
        GUILayout.Label("Input: ", EditorStyles.boldLabel);
        _STS_ScriptInput = EditorGUILayout.TextField("", _STS_ScriptInput, GUILayout.Width(Screen.width -5));

        //Output
        GUILayout.Label("Output: ", EditorStyles.boldLabel);
        EditorGUILayout.TextField("", _STS_ScriptOutput, GUILayout.Width(Screen.width - 5));
        GUILayout.Space(20);
    }
    void STS_Show_Keywords()
    {
        //TextEditor Info
        GUILayout.Label("Use Custom Keywords to fix lines that should not be included into the commend. \n" +
            "Sometimes it leaves code after the command, you can addit it by adding a keyword below." +
            "The x on the left shows the lines that contain a comment.");

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Custom Keywords: ", EditorStyles.boldLabel, GUILayout.Width(Screen.width - 160));
        if (GUILayout.Button("Add common keywords", GUILayout.Width(150)))
        {
            STS_AddCommonKeywords();
        }
        GUILayout.EndHorizontal();

        _STS_CustomCommandCheck = EditorGUILayout.TextField("", _STS_CustomCommandCheck, GUILayout.Width(Screen.width - 5));
        if (GUILayout.Button("AddKeyword", GUILayout.Width(Screen.width - 5)))
        {
            if (_STS_CustomCommandCheck == "")
                Debug.Log("Enter a keyword");
            else
            {
                STS_Add_Keyword(_STS_CustomCommandCheck);
                _STS_CustomCommandCheck = "";
                _STS_ScriptOutput = STS_ConvertScriptToString();
            }
        }

        _STS_ToggleKeywords = EditorGUILayout.Foldout(_STS_ToggleKeywords, "Show Keywords");

        if (_STS_ToggleKeywords)
            for (int i = 0; i < _STS_CustomCommandCheckKeywords.Count; i++)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(_STS_CustomCommandCheckKeywords[i]);
                if (GUILayout.Button("Remove", GUILayout.Width(100)))
                {
                    _STS_CustomCommandCheckKeywords.Remove(_STS_CustomCommandCheckKeywords[i]);
                    _STS_CustomCommandCheck = "";
                    _STS_ScriptOutput = STS_ConvertScriptToString();
                }
                GUILayout.EndHorizontal();
            }
    }
    void STS_Display_TextEditor()
    {
        //Preview
        List<string> output = new List<string>();
        List<string> output2 = new List<string>();

        for (int i = 0; i < _STS_ScriptOutput.Length; i++)
        {
            output.Add(System.Convert.ToString(_STS_ScriptOutput[i]));
        }

        int begincalc = 0;
        int endcalc = 0;

        for (int i = 0; i < output.Count; i++)
        {
            if (i + 1 < output.Count)
            {
                if (output[i] + output[i + 1] == "\\n")
                {
                    endcalc = i;
                    string addstring = "";
                    for (int j = 0; j < endcalc - begincalc; j++)
                    {
                        addstring += output[begincalc + j];
                    }
                    addstring += output[endcalc] + output[endcalc + 1];

                    output2.Add(addstring);
                    endcalc = endcalc + 1;
                    begincalc = endcalc + 1;
                }
            }
        }

        for (int i = 0; i < output2.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (output2[i].Contains("//"))
            {
                EditorGUILayout.TextField("", "x", GUILayout.MaxWidth(15));
            }
            else
            {
                EditorGUILayout.TextField("", "", GUILayout.MaxWidth(15));
            }

            EditorGUILayout.TextField("", output2[i]);
            GUILayout.EndHorizontal();
        }
    }
    void STS_AddCommonKeywords()
    {
        STS_Add_Keyword("float");
        STS_Add_Keyword("double");
        STS_Add_Keyword("int");
        STS_Add_Keyword("void");
        STS_Add_Keyword("for");
        STS_Add_Keyword("switch");
        STS_Add_Keyword("private");
        STS_Add_Keyword("public");
        STS_Add_Keyword("[Header(");
        STS_Add_Keyword("case");
        STS_Add_Keyword("if");

        _STS_ScriptOutput = STS_ConvertScriptToString();
    }
    void STS_Add_Keyword(string keyword)
    {
        bool exist = false;
        for (int i = 0; i < _STS_CustomCommandCheckKeywords.Count; i++)
        {
            if (_STS_CustomCommandCheckKeywords[i] == keyword)
                exist = true;
        }

        if (!exist)
            _STS_CustomCommandCheckKeywords.Add(keyword);
    }
    string STS_ConvertScriptToString()
    {
        _STS_ScriptOutput = "";
        string scriptasstring = "\"";

        //Split / add to array
        List<string> textedit = new List<string>();

        for (int i = 0; i < _STS_ScriptInput.Length; i++)
        {
            textedit.Add(System.Convert.ToString(_STS_ScriptInput[i]));
        }

        bool headercheck = false;
        bool forcheck = false;
        bool commentcheck = false;

        for (int i = 0; i < textedit.Count; i++)
        {
            //Header check
            if (i + 7 < textedit.Count)
            {
                if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] + textedit[i + 6] + textedit[i + 7] == "[Header(")
                    headercheck = true;
            }

            //For check
            if (i + 2 < textedit.Count)
            {
                if (textedit[i] + textedit[i + 1] + textedit[i + 2] == "for")
                {
                    forcheck = true;
                }
            }

            //Comment check
            if (i + 1 < textedit.Count)
            {
                if (textedit[i] + textedit[i + 1] == "//" || textedit[i] + textedit[i + 1] == "/*")
                    commentcheck = true;
            }

            //Comment /* + */
            if (commentcheck)
            {
                if (textedit[i - 1] + textedit[i] == "*/")
                {
                    scriptasstring += "\\n";
                    commentcheck = false;
                }

                for (int j = 0; j < _STS_CustomCommandCheckKeywords.Count; j++)
                {
                    if (_STS_CustomCommandCheckKeywords[j].Length < textedit.Count)
                    {
                        string check = "";

                        for (int o = 0; o < _STS_CustomCommandCheckKeywords[j].Length; o++)
                        {
                            check += textedit[i + o];
                        }

                        if (check == _STS_CustomCommandCheckKeywords[j])
                        {
                            scriptasstring += "\\n";
                            commentcheck = false;
                        }
                    }
                }
            }

            scriptasstring += textedit[i];
            //Endings check
            if (i + 2 < textedit.Count)
            {
                if (textedit[i + 1] == "\"")
                {
                    scriptasstring += "\\";
                }

                if (textedit[i] == "}")
                {
                    scriptasstring += "\\n";
                }
                if (textedit[i] == ";" && !forcheck)
                {
                    scriptasstring += "\\n";
                }
                if (textedit[i] == "]" && headercheck)
                {
                    scriptasstring += "\\n";
                    headercheck = false;
                }
                if (textedit[i] == ")" && forcheck)
                {
                    scriptasstring += "\\n";
                    forcheck = false;
                }
            }
        }
        scriptasstring += "\"";

        return scriptasstring;
    }


    //MapEditor
    void MapEditor_Menu()
    {
        if (_ME_FirstLoad)
        {
            ME_Load_Prefabs();
            _ME_FirstLoad = false;
        }
        GUILayout.BeginVertical("Box");

        //Refresh/Info
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh", GUILayout.Width(80)))
        {
            ME_Load_Prefabs();
        }
        if (GUILayout.Button("Fix", GUILayout.Width(80)))
        {
            ME_FixPreview();
        }
        GUILayout.Label("Loaded objects: " + _ME_SearchResults.Length);
        GUILayout.EndHorizontal();

        //Windows
        ME_ObjectView_Header();
        ME_ObjectView_Objects();
        ME_ObjectView_Options();

        GUILayout.EndVertical();
    }
    void ME_ObjectView_Header()
    {
        GUILayout.BeginHorizontal();
        _ME_OptionsStates = GUILayout.Toolbar(_ME_OptionsStates, new string[] { "Icon", "Text" });
        _ME_ButtonSize = EditorGUILayout.Slider(_ME_ButtonSize, 0.25f, 2);
        if (!_ME_HideNames)
        {
            if (GUILayout.Button("Hide Names", GUILayout.Width(100)))
                _ME_HideNames = true;
        }
        else
        {
            if (GUILayout.Button("Show Names", GUILayout.Width(100)))
                _ME_HideNames = false;
        }
        GUILayout.EndHorizontal();
        _ME_SearchPrefab = EditorGUILayout.TextField("Search: ", _ME_SearchPrefab);
    }
    void ME_ObjectView_Objects()
    {
        Color defaultColor = GUI.backgroundColor;
        GUILayout.BeginVertical("Box");
        float calcWidth = 100 * _ME_ButtonSize;
        _ME_CollomLength = position.width / calcWidth;
        int x = 0;
        int y = 0;

        //Show/Hide Options
        if (_ME_HideOptions)
            _ME_ScrollPos1 = GUILayout.BeginScrollView(_ME_ScrollPos1, GUILayout.Width(position.width - 20), GUILayout.Height(position.height - 125));
        else
        {
            if (_ME_PlacementStates == 0)
                _ME_ScrollPos1 = GUILayout.BeginScrollView(_ME_ScrollPos1, GUILayout.Width(position.width - 20), GUILayout.Height(position.height - 266));
            else
                _ME_ScrollPos1 = GUILayout.BeginScrollView(_ME_ScrollPos1, GUILayout.Width(position.width - 20), GUILayout.Height(position.height - 285));
        }

        //Object Icons
        for (int i = 0; i < _ME_SearchResults.Length; i++)
        {
            if (_ME_Prefabs[i] != null && _ME_Prefabs[i].name.ToLower().Contains(_ME_SearchPrefab.ToLower()))
            {
                if (_ME_OptionsStates == 0) //Icons
                {
                    //Select Color
                    if (_ME_SelectedID == i) { GUI.backgroundColor = new Color(0, 1, 0); } else { GUI.backgroundColor = new Color(1, 0, 0); }

                    //Create Button
                    GUIContent content = new GUIContent();
                    content.image = _ME_PrefabIcon[i];
                    GUI.skin.button.imagePosition = ImagePosition.ImageAbove;
                    if (!_ME_HideNames)
                        content.text = _ME_Prefabs[i].name;
                    if (GUI.Button(new Rect(x * 100 * _ME_ButtonSize, y * 100 * _ME_ButtonSize, 100 * _ME_ButtonSize, 100 * _ME_ButtonSize), content))
                        if (_ME_SelectedID == i) { _ME_SelectedID = 99999999; _ME_CheckSelectedID = 99999999; DestroyImmediate(_ME_ExampleObj); } else { _ME_SelectedID = i; }

                    //Reset Button Position
                    x++;
                    if (x >= _ME_CollomLength - 1)
                    {
                        y++;
                        x = 0;
                    }
                    GUI.backgroundColor = defaultColor;
                }
                else //Text Buttons
                {
                    if (_ME_SelectedID == i) { GUI.backgroundColor = new Color(0, 1, 0); } else { GUI.backgroundColor = defaultColor; }
                    if (GUILayout.Button(_ME_Prefabs[i].name))
                        if (_ME_SelectedID == i) { _ME_SelectedID = 99999999; _ME_CheckSelectedID = 99999999; DestroyImmediate(_ME_ExampleObj); } else { _ME_SelectedID = i; }
                    GUI.backgroundColor = defaultColor;
                }
            }
        }
        if (_ME_OptionsStates == 0)
        {
            GUILayout.Space(y * 100 * _ME_ButtonSize + 100);
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
    void ME_ObjectView_Options()
    {
        GUILayout.BeginVertical("Box");
        if (!_ME_HideOptions)
        {
            //Paint Options
            GUILayout.BeginVertical("Box");
            _ME_PlacementStates = GUILayout.Toolbar(_ME_PlacementStates, new string[] { "Click", "Paint" });
            if (_ME_PlacementStates == 1)
                _ME_PaintSpeed = EditorGUILayout.FloatField("Paint Speed: ", _ME_PaintSpeed);
            //Parent Options
            GUILayout.BeginHorizontal();
            _ME_ParentObj = (GameObject)EditorGUILayout.ObjectField("Parent Object: ", _ME_ParentObj, typeof(GameObject), true);
            if (_ME_ParentObj != null)
                if (GUILayout.Button("Clean Parent"))
                    ME_CleanParent();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            //Grid Options
            GUILayout.BeginVertical("Box");
            _ME_GridSize = EditorGUILayout.Vector2Field("Grid Size: ", _ME_GridSize);
            _ME_RandomRot = EditorGUILayout.Toggle("Random Rotation: ", _ME_RandomRot);
            _ME_SnapPosActive = EditorGUILayout.Toggle("Use Grid: ", _ME_SnapPosActive);
            GUILayout.EndVertical();
        }
        //Hide/Show Options
        if (_ME_HideOptions)
        {
            if (GUILayout.Button("Show Options"))
                _ME_HideOptions = false;
        }
        else
        {
            if (GUILayout.Button("Hide Options"))
                _ME_HideOptions = true;
        }
        GUILayout.EndVertical();
    }

    //Load/Fix
    void ME_Load_Prefabs()
    {
        _ME_SearchResults = System.IO.Directory.GetFiles("Assets/", "*.prefab", System.IO.SearchOption.AllDirectories);
        _ME_Prefabs = new GameObject[_ME_SearchResults.Length];
        _ME_PrefabIcon = new Texture2D[_ME_SearchResults.Length];

        for (int i = 0; i < _ME_SearchResults.Length; i++)
        {
            UnityEngine.Object prefab = null;
            prefab = AssetDatabase.LoadAssetAtPath(_ME_SearchResults[i], typeof(GameObject));
            _ME_Prefabs[i] = prefab as GameObject;
            _ME_PrefabIcon[i] = AssetPreview.GetAssetPreview(_ME_Prefabs[i]);
        }
    }
    void ME_FixPreview()
    {
        ME_Load_Prefabs();
        _ME_SearchResults = System.IO.Directory.GetFiles("Assets/", "*.prefab", System.IO.SearchOption.AllDirectories);

        for (int i = 0; i < _ME_SearchResults.Length; i++)
        {
            if (_ME_PrefabIcon[i] == null)
                AssetDatabase.ImportAsset(_ME_SearchResults[i]);
        }
        ME_Load_Prefabs();
    }

    //Create Prefab/Clean Parent
    void ME_CreatePrefab(Vector3 createPos)
    {
        if (ME_CheckPositionEmpty(true))
        {
            GameObject createdObj = PrefabUtility.InstantiatePrefab(_ME_Prefabs[_ME_SelectedID]) as GameObject;
            createdObj.transform.position = createPos;
            createdObj.transform.localScale = new Vector3(_ME_Size, _ME_Size, _ME_Size);

            if (_ME_ParentObj == null)
            {
                _ME_ParentObj = new GameObject();
                _ME_ParentObj.name = "MapEditor_Parent";
            }

            createdObj.transform.parent = _ME_ParentObj.transform;

            //SnapPos
            if (_ME_SnapPosActive)
                createdObj.transform.position = _ME_SnapPos;
            else
                createdObj.transform.position = _ME_MousePos;

            //Rotation
            /*
            if (_ME_RandomRot)
                createdObj.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            else
                createdObj.transform.rotation = Quaternion.Euler(0, _ME_Rotation, 0);
            */

            if (_ME_RotateWithObject)
                createdObj.transform.rotation = Quaternion.Euler(_ME_HitObject.eulerAngles.x, _ME_Rotation, _ME_HitObject.eulerAngles.z);
            else
                createdObj.transform.rotation = Quaternion.Euler(0, _ME_Rotation, 0);
            //Test
        }
    }
    void ME_CleanParent()
    {
        int childAmount = _ME_ParentObj.transform.childCount;
        int childCalc = childAmount - 1;
        for (int i = 0; i < childAmount; i++)
        {
            DestroyImmediate(_ME_ParentObj.transform.GetChild(childCalc).gameObject);
            childCalc -= 1;
        }
    }
    bool ME_CheckPositionEmpty(bool checky)
    {
        if (_ME_ParentObj != null)
        {
            bool check = true;
            for (int i = 0; i < _ME_ParentObj.transform.childCount; i++)
            {
                if (checky)
                {
                    if (_ME_ParentObj.transform.GetChild(i).position.x == _ME_SnapPos.x && _ME_ParentObj.transform.GetChild(i).position.z == _ME_SnapPos.z)
                        check = false;
                }
                else
                        if (_ME_ParentObj.transform.GetChild(i).position == _ME_SnapPos)
                    check = false;
            }
            return check;
        }
        else
        {
            return true;
        }
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
        DestroyImmediate(_ME_ExampleObj);
    }

    //OnSceneGUI
    void OnSceneGUI(SceneView sceneView)
    {
        //MapEditor
        if (_WindowID == 3)
        {
            Event e = Event.current;
            Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(worldRay, out hitInfo))
            {
                //Check MousePosition
                _ME_MousePos = hitInfo.point;

                //Hit Object
                _ME_HitObject = hitInfo.transform;

                //Create Example Object
                if (_ME_SelectedID <= _ME_Prefabs.Length)
                {
                    if (_ME_CheckSelectedID != _ME_SelectedID)
                    {
                        DestroyImmediate(_ME_ExampleObj);
                        _ME_ExampleObj = Instantiate(_ME_Prefabs[_ME_SelectedID], hitInfo.point, Quaternion.identity);
                        _ME_ExampleObj.layer = LayerMask.NameToLayer("Ignore Raycast");
                        for (int i = 0; i < _ME_ExampleObj.transform.childCount; i++)
                        {
                            _ME_ExampleObj.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                            for (int o = 0; o < _ME_ExampleObj.transform.GetChild(i).childCount; o++)
                            {
                                _ME_ExampleObj.transform.GetChild(i).GetChild(o).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                            }
                        }
                        _ME_ExampleObj.name = "Example Object";
                        _ME_CheckSelectedID = _ME_SelectedID;
                    }
                }

                //Set Example Object Position + Rotation
                if (_ME_ExampleObj != null)
                {
                    //Rotate with hit object
                    //Debug.Log("Transform: X" + _ME_HitObject.eulerAngles.x.ToString() + "  Y  " + _ME_HitObject.eulerAngles.z.ToString());

                    //Rotation
                    if (_ME_RotateWithObject)
                        _ME_ExampleObj.transform.rotation = Quaternion.Euler(_ME_HitObject.eulerAngles.x, _ME_Rotation, _ME_HitObject.eulerAngles.z);
                    else
                        _ME_ExampleObj.transform.rotation = Quaternion.Euler(0, _ME_Rotation, 0);

                    _ME_ExampleObj.transform.localScale = new Vector3(_ME_Size, _ME_Size, _ME_Size);
                    if (!e.shift && !e.control)
                    {
                        if (!_ME_SnapPosActive)
                        { _ME_ExampleObj.transform.position = hitInfo.point; }
                        else
                        { _ME_ExampleObj.transform.position = _ME_SnapPos; }
                    }
                }

                //Check Buttons Pressed
                if (!Event.current.alt && _ME_SelectedID != 99999999)
                {
                    if (Event.current.type == EventType.Layout)
                        HandleUtility.AddDefaultControl(0);

                    //Mouse Button 0 Pressed
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        _ME_MouseDown = true;
                        _ME_PaintTimer = _ME_PaintSpeed;
                        if (e.mousePosition.y <= 20)
                            _ME_ClickMenu = true;
                    }

                    //Mouse Button 0 Released
                    if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                    {
                        _ME_MouseDown = false;
                        _ME_ClickMenu = false;
                    }

                    //Check Shift
                    if (e.shift)
                        _ME_ShiftDown = true;
                    else
                        _ME_ShiftDown = false;

                    //Check Ctrl
                    if (e.control)
                        _ME_CtrlDown = true;
                    else
                        _ME_CtrlDown = false;

                    if (e.shift || e.control)
                    {
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                            _ME_ClickPos = Event.current.mousePosition;
                    }

                    //Place Object
                    if (!_ME_ShiftDown && !_ME_CtrlDown && !_ME_ClickMenu)
                    {
                        if (_ME_PlacementStates == 0)
                        {
                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                                ME_CreatePrefab(hitInfo.point);
                        }
                        else
                        {
                            float timer1Final = _ME_PaintSpeed;
                            if (_ME_MouseDown)
                            {
                                _ME_PaintTimer += 1 * Time.deltaTime;
                                if (_ME_PaintTimer >= timer1Final)
                                {
                                    ME_CreatePrefab(hitInfo.point);
                                    _ME_PaintTimer = 0;
                                }
                            }
                        }
                    }
                }

                // Draw obj location
                if (_ME_SelectedID != 99999999)
                {
                    //Draw Red Cross + Sphere on object location
                    Handles.color = new Color(1, 0, 0);
                    Handles.DrawLine(new Vector3(hitInfo.point.x - 0.3f, hitInfo.point.y, hitInfo.point.z), new Vector3(hitInfo.point.x + 0.3f, hitInfo.point.y, hitInfo.point.z));
                    Handles.DrawLine(new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z - 0.3f), new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z + 0.3f));
                    if (_ME_SnapPosActive)
                    {
                        Handles.SphereHandleCap(1, new Vector3(_ME_SnapPos.x, hitInfo.point.y, _ME_SnapPos.z), Quaternion.identity, 0.1f, EventType.Repaint);
                    }
                    else
                        Handles.SphereHandleCap(1, new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z), Quaternion.identity, 0.1f, EventType.Repaint);

                    //Check Snap Position
                    if (_ME_SnapPosActive)
                    {
                        Vector2 calc = new Vector2(_ME_MousePos.x / _ME_GridSize.x, _ME_MousePos.z / _ME_GridSize.y);
                        Vector2 calc2 = new Vector2(Mathf.RoundToInt(calc.x) * _ME_GridSize.x, Mathf.RoundToInt(calc.y) * _ME_GridSize.y);

                        _ME_SnapPos = new Vector3(calc2.x, _ME_MousePos.y, calc2.y);

                        //Draw Grid
                        Handles.color = new Color(0, 1, 0);
                        float lineLength = 0;
                        if (_ME_GridSize.x > _ME_GridSize.y)
                            lineLength = _ME_GridSize.x + 1;
                        else
                            lineLength = _ME_GridSize.y + 1;

                        for (int hor = 0; hor < 3; hor++)
                        {
                            Handles.DrawLine(new Vector3(calc2.x - lineLength, hitInfo.point.y, calc2.y - _ME_GridSize.y + _ME_GridSize.y * hor), new Vector3(calc2.x + lineLength, hitInfo.point.y, calc2.y - _ME_GridSize.y + _ME_GridSize.y * hor));
                        }
                        for (int ver = 0; ver < 3; ver++)
                        {
                            Handles.DrawLine(new Vector3(calc2.x - _ME_GridSize.x + _ME_GridSize.x * ver, hitInfo.point.y, calc2.y - lineLength), new Vector3(calc2.x - _ME_GridSize.x + _ME_GridSize.x * ver, hitInfo.point.y, calc2.y + lineLength));
                        }
                    }
                }
            }
        }

        //FileFinder
        if (_FF_Scene_InsceneInfo)
        {
            Handles.color = new Color(0, 1, 0, 0.3f);
            for (int i = 0; i < _FF_Scene_Objects.Length; i++)
            {
                if (_FF_Scene_Objects[i].name.ToLower().Contains(_FF_Scene_Search.ToLower()))
                {

                    Handles.SphereHandleCap(1, _FF_Scene_Objects[i].transform.position, Quaternion.identity, 3f, EventType.Repaint);
                    Handles.Label(_FF_Scene_Objects[i].transform.position, _FF_Scene_Objects[i].name);
                }
            }
        }
    }

    //OnScene
    void OnScene(SceneView sceneView)
    {
        if (_WindowID == 3)
        {
            //InScene Option Bar
            Handles.BeginGUI();
            if (_ME_ShowOptionsInScene)
            {
                //Option Bar
                GUI.Box(new Rect(0, 0, Screen.width, 22), GUIContent.none);
                _ME_InScene_SelectedID = GUI.Toolbar(new Rect(22, 1, Screen.width / 2 - 30, 20), _ME_InScene_SelectedID, new string[] { "Settings", "Placement", "Transform", "Grid" });
                switch (_ME_InScene_SelectedID)
                {
                    case 0: //Settings
                        GUI.Label(new Rect(Screen.width / 2 - 5, 3, 50, 20), "Parent: ");
                        _ME_ParentObj = (GameObject)EditorGUI.ObjectField(new Rect(Screen.width / 2 + 50, 1, 150, 20), _ME_ParentObj, typeof(GameObject), true);
                        if (GUI.Button(new Rect(Screen.width - 110, 1, 90, 20), "Clean Parent"))
                        {
                            ME_CleanParent();
                        }
                        break;
                    case 1: //Placement
                        _ME_PlacementStates = GUI.Toolbar(new Rect(Screen.width / 2 - 5, 1, 100, 20), _ME_PlacementStates, new string[] { "Click", "Paint" });
                        _ME_PaintSpeed = EditorGUI.FloatField(new Rect(Screen.width / 2 + 185, 1, 50, 20), _ME_PaintSpeed);
                        GUI.Label(new Rect(Screen.width / 2 + 100, 3, 500, 20), "Paint speed: ");
                        break;
                    case 2: //Transform
                        _ME_Size = EditorGUI.FloatField(new Rect(Screen.width / 2 + 125, 1, 100, 20), _ME_Size);
                        break;
                    case 3: //Grid
                        GUI.Label(new Rect(Screen.width / 2 + 80, 3, 100, 20), "Grid Size: ");
                        _ME_GridSize.x = EditorGUI.FloatField(new Rect(Screen.width / 2 + 150, 1, 50, 20), _ME_GridSize.x);
                        _ME_GridSize.y = EditorGUI.FloatField(new Rect(Screen.width / 2 + 200, 1, 50, 20), _ME_GridSize.y);
                        GUI.Label(new Rect(Screen.width / 2, 3, 100, 20), "Enable: ");
                        _ME_SnapPosActive = EditorGUI.Toggle(new Rect(Screen.width / 2 + 50, 3, 20, 20), _ME_SnapPosActive);
                        break;
                }
            }

            //Hotkeys Resize / Rotate
            //Shift+MouseDown = Resize
            Vector2 prevmove = _ME_PrevMousePos - Event.current.mousePosition;
            if (_ME_ShiftDown && _ME_MouseDown)
            {
                _ME_Size = EditorGUI.Slider(new Rect(_ME_ClickPos.x - 15, _ME_ClickPos.y - 40, 50, 20), _ME_Size, 0.01f, 1000000);
                _ME_Size -= (prevmove.x + prevmove.y) * 0.05f;
                GUI.Label(new Rect(_ME_ClickPos.x - 50, _ME_ClickPos.y - 40, 500, 20), "Size: ");
            }
            //Ctrl+MouseDown = Rotate
            if (_ME_CtrlDown && _ME_MouseDown)
            {
                _ME_Rotation = EditorGUI.Slider(new Rect(_ME_ClickPos.x - 15, _ME_ClickPos.y - 40, 50, 20), _ME_Rotation, -1000000, 1000000);
                _ME_Rotation += prevmove.x + prevmove.y;
                GUI.Label(new Rect(_ME_ClickPos.x - 80, _ME_ClickPos.y - 40, 500, 20), "Rotation: ");
            }
            _ME_PrevMousePos = Event.current.mousePosition;

            //Inscene Show OptionButton
            GUI.color = new Color(1f, 1f, 1f, 1f);
            if (!_ME_ShowOptionsInScene)
            {
                if (GUI.Button(new Rect(1, 1, 20, 20), " +"))
                    _ME_ShowOptionsInScene = true;
            }
            else
            {
                if (GUI.Button(new Rect(1, 1, 20, 20), " -"))
                    _ME_ShowOptionsInScene = false;
            }
            Handles.EndGUI();
        }
    }

    //TabChange
    void ChangeTab()
    {
        if (_ME_ExampleObj != null)
            DestroyImmediate(_ME_ExampleObj);
    }
}

public class Tool_QuickStartUI_Tab
{
    public GameObject HUD_TabParent;
    public List<Tool_QuickStartUI_Object> HUD_TabOjects = new List<Tool_QuickStartUI_Object>();
}
public class Tool_QuickStartUI_Object
{
    //Object / Components
    public GameObject HUD_Object;
    public RectTransform HUD_RectTransform;

    //Settings
    public string HUD_Name;
    public Vector3 HUD_Offset;
    public Vector2 HUD_Size = new Vector2(100,25);
    public Vector3 HUD_Scale = new Vector3(1,1,1);
    public float HUD_TextFontSize = 16;

    //Other
    public bool HUD_FoldOut;

    //DropDown
    public enum HUD_Types {Text , Slider, Dropdown, Bar, Button }
    public HUD_Types HUD_Type;
    public HUD_Types HUD_CheckType;
    public enum HUD_Locations {TopLeft,TopMiddle,TopRight,LeftMiddle,RightMiddle,BottomLeft,BottomMiddle,BottomRight,Middle }
    public HUD_Locations HUD_Location;

    //Info
    public List<TextMeshProUGUI> HUD_Text = new List<TextMeshProUGUI>();
}
public class Tool_QuickStart_Script
{
    private string _Script_Name;
    private string _Script_Tag;
    private string _Script_State;
    private string _Script_Code;
    private string _Script_Path;

    public bool Exist;

    public string ScriptName { get { return _Script_Name; } }
    public string ScriptTag { get { return _Script_Tag; } }
    public string ScriptState { get { return _Script_State; } }
    public string ScriptCode { get { return _Script_Code; } }
    public string ScriptPath { get { return _Script_Path; } set { _Script_Path = value; } }

    public Tool_QuickStart_Script(string name, string tags, string state, string code)
    {
        _Script_Name = name;
        _Script_Tag = tags;
        _Script_State = state;
        _Script_Code = code;
    }
}