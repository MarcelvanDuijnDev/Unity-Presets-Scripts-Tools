using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;
using UnityEditor.PackageManager.UI;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Tool_QuickStart : EditorWindow
{
    private int _MenuID = 0;        // QuickStart/Scripts
    private int _DimensionID = 0;   // 2D/3D
    private int _WithUI = 0;
    private int _Type2DID = 0;      // Platformer/TopDown/VisualNovel
    private int _Type3DID = 0;      // FPS/ThirdPerson/TopDown/Platformer

    private bool[] _ScriptExist = new bool[50];
    private string[] _ScriptNames = new string[] {
"AudioHandler",
"BasicNavMeshAI",
"Bullet",
"DialogSystem",
"DoEvent",
"EditorWindowExample",
"EnemySpawnHandler",
"Health",
"Inventory",
"LightEffects",
"LoadScenes",
"MenuHandler",
"Movement_2D_Platformer",
"Movement_2D_TopDown",
"Movement_Camera",
"Movement_CC_Platformer",
"Movement_CC_TopDown",
"Movement_CC",
"Movement_FreeCamera",
"MVD_Namespace",
"ObjectPool",
"ObjectPoolSimple",
"OnCollision",
"ReadTwitchChat",
"SaveLoad_JSON",
"SaveLoad_XML",
"ScriptebleGameObject",
"SettingsHandler",
"Shooting",
"ShootingRayCast",
"StringFormats",
"Tool_CreateHexagonGrid",
"Tool_ScriptToString",
"Turret",
"UIEffects"
};
    private string[] _ScriptCode = new string[] // ScriptCodes Updated: 21-nov-2020
    {
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine.SceneManagement;\n using UnityEngine;\n using UnityEngine.Audio;\n  public class AudioHandler : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] private bool _RefreshSettingsOnUpdate = false;\n     [SerializeField] private AudioMixerGroup _AudioMixer = null;\n      [Header(\"Audio\")]\n     [SerializeField] private List<AudioHandler_Sound> _Sound = new List<AudioHandler_Sound>();\n      private string _CurrentScene;\n      void Start()     {         //PlayOnStart         \nfor (int i = 0; i < _Sound.Count; i++)\n         {             //AudioSource             \nif (_Sound[i].Settings.CreateAudioSource)             {                 _Sound[i].Settings.AudioSource = this.gameObject.AddComponent<AudioSource>();\n                 _Sound[i].Settings.AudioSource.outputAudioMixerGroup = _AudioMixer;\n             }\n              //AudioClip             \n_Sound[i].Settings.AudioSource.clip = _Sound[i].Settings.AudioClip;\n              //Settings             \nif (_Sound[i].AudioSettings.PlayOnStart)             {                 _Sound[i].Settings.AudioSource.playOnAwake = _Sound[i].AudioSettings.PlayOnStart;\n                 _Sound[i].Settings.AudioSource.Play();\n             }\n             if (_Sound[i].AudioEffects.FadeIn)             {                 _Sound[i].Settings.AudioSource.volume = 0;\n                 _Sound[i].AudioEffects.FadeInSpeed = _Sound[i].AudioSettings.Volume / _Sound[i].AudioEffects.FadeInDuration;\n             }\n             if (_Sound[i].AudioEffects.FadeOut)             {                 _Sound[i].AudioEffects.FadeOutSpeed = _Sound[i].AudioSettings.Volume / _Sound[i].AudioEffects.FadeOutDuration;\n             }\n         }\n          RefreshSettings();\n     }\n      void Update()     {         CheckNewScene();\n          if (_RefreshSettingsOnUpdate)             RefreshSettings();\n          for (int i = 0; i < _Sound.Count; i++)\n         {             //FadeIn             \nif (_Sound[i].AudioEffects.FadingIn)             {                 if (_Sound[i].AudioEffects.FadeIn && !_Sound[i].AudioEffects.FadeInDone)                 {                     if (_Sound[i].Settings.AudioSource.volume < _Sound[i].AudioSettings.Volume)                     {                         _Sound[i].Settings.AudioSource.volume += _Sound[i].AudioEffects.FadeInSpeed * Time.deltaTime;\n                     }\n                     else                     {                         _Sound[i].AudioEffects.FadeInDone = true;\n                         _Sound[i].Settings.AudioSource.volume = _Sound[i].AudioSettings.Volume;\n                     }\n                 }\n             }\n             //FadeOut             \nif (_Sound[i].AudioEffects.FadingOut)             {                 if (_Sound[i].AudioEffects.FadeOutAfterTime > -0.1f)                 {                     _Sound[i].AudioEffects.FadeOutAfterTime -= 1 * Time.deltaTime;\n                 }\n                 else                 {                     if (_Sound[i].AudioEffects.FadeOut && !_Sound[i].AudioEffects.FadeOutDone)                     {                         if (_Sound[i].Settings.AudioSource.volume > 0)                         {                             _Sound[i].Settings.AudioSource.volume -= _Sound[i].AudioEffects.FadeOutSpeed * Time.deltaTime;\n                         }\n                         else                         {                             _Sound[i].AudioEffects.FadeOutDone = true;\n                             _Sound[i].Settings.AudioSource.volume = 0;\n                             _Sound[i].Settings.AudioSource.Stop();\n                         }\n                     }\n                 }\n             }\n         }\n     }\n      private void CheckNewScene()     {         if (_CurrentScene != SceneManager.GetActiveScene().name)         {             _CurrentScene = SceneManager.GetActiveScene().name;\n             for (int i = 0; i < _Sound.Count; i++)\n             {                 for (int o = 0; o < _Sound[i].AudioControl.StartAudioOnScene.Count; o++)\n                 {                     if (_Sound[i].AudioControl.StartAudioOnScene[o] == _CurrentScene)                     {                         //FadeIn                         \nif (_Sound[i].AudioEffects.FadeIn)                         {                             _Sound[i].AudioEffects.FadingOut = false;\n                             _Sound[i].AudioEffects.FadeInDone = false;\n                             _Sound[i].AudioEffects.FadingIn = true;\n                         }\n                         _Sound[i].Settings.AudioSource.Play();\n                     }\n                 }\n                 for (int o = 0; o < _Sound[i].AudioControl.StopAudioOnScene.Count; o++)\n                 {                     if (_Sound[i].AudioControl.StopAudioOnScene[o] == _CurrentScene)                     {                         //FadeOut                         \nif (_Sound[i].AudioEffects.FadeOut && !_Sound[i].AudioEffects.FadingOut)                         {                             _Sound[i].AudioEffects.FadingIn = false;\n                             _Sound[i].AudioEffects.FadeOutDone = false;\n                             _Sound[i].AudioEffects.FadingOut = true;\n                         }\n                         else                             _Sound[i].Settings.AudioSource.Stop();\n                     }\n                 }\n             }\n         }\n     }\n      public void PlayTrack(string trackname)     {         for (int i = 0; i < _Sound.Count; i++)\n         {             if (_Sound[i].AudioTrackName == trackname)                 AudioHandler_PlayTrack(i);\n         }\n     }\n     private void AudioHandler_PlayTrack(int trackid)     {         _Sound[trackid].Settings.AudioSource.Play();\n     }\n     public void RefreshSettings()     {         for (int i = 0; i < _Sound.Count; i++)\n         {             //SetClip             \nif (_Sound[i].Settings.AudioSource.clip != _Sound[i].Settings.AudioClip)                 _Sound[i].Settings.AudioSource.clip = _Sound[i].Settings.AudioClip;\n             //SetEffects             \nif (!_Sound[i].AudioEffects.FadeIn || _Sound[i].AudioEffects.FadeIn && _Sound[i].AudioEffects.FadeInDone)                 _Sound[i].Settings.AudioSource.volume = _Sound[i].AudioSettings.Volume;\n             _Sound[i].Settings.AudioSource.loop = _Sound[i].AudioSettings.Loop;\n         }\n     }\n      public void SetAudioSource(string trackname, AudioSource audiosource)     {         for (int i = 0; i < _Sound.Count; i++)\n         {             if (_Sound[i].AudioTrackName == trackname)                 _Sound[i].Settings.AudioSource = audiosource;\n         }\n     }\n }\n  [System.Serializable] public class AudioHandler_Sound {     public string AudioTrackName;\n     public AudioHandler_Settings Settings;\n     public AudioHandler_AudioSettings AudioSettings;\n     public AudioHandler_Control AudioControl;\n     public AudioHandler_Effects AudioEffects;\n }\n  [System.Serializable] public class AudioHandler_Settings {     [Header(\"AudioClip\")]\n     public AudioClip AudioClip;\n      [Header(\"AudioSource\")]\n     public AudioSource AudioSource;\n     public bool CreateAudioSource;\n }\n  [System.Serializable] public class AudioHandler_AudioSettings {     [Header(\"AudioSettings\")]\n     [Range(0, 1)] public float Volume = 1;\n     public bool Loop;\n     public bool PlayOnStart;\n }\n  [System.Serializable] public class AudioHandler_Control {     [Header(\"Enable/Disable Song\")]\n     public List<string> StartAudioOnScene = new List<string>();\n     public List<string> StopAudioOnScene = new List<string>();\n     public bool StopOnNextScene;\n     [HideInInspector] public int SceneEnabled;\n }\n  [System.Serializable] public class AudioHandler_Effects {     [Header(\"FadeIn\")]\n     public bool FadeIn;\n     public float FadeInDuration;\n     [HideInInspector] public float FadeInSpeed;\n     [HideInInspector] public bool FadeInDone;\n     [HideInInspector] public bool FadingIn;\n     [Header(\"FadeOut\")]\n     public bool FadeOut;\n     public float FadeOutAfterTime;\n     public float FadeOutDuration;\n     [HideInInspector] public float FadeOutSpeed;\n     [HideInInspector] public bool FadeOutDone;\n     [HideInInspector] public bool FadingOut;\n }",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.AI;\n  public class BasicNavMeshAI : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] private Transform _Target = null;     [SerializeField] private float _Speed = 2;      private NavMeshAgent _Agent;      private void Awake()\n     {         if (_Target == null)         {             try             {                 _Target = GameObject.Find(\"Player\").transform;             }\n             catch             {                 Debug.Log(\"No Target\")\n;\n             }\n         }\n          _Agent = GetComponent<NavMeshAgent>();\n         _Agent.speed = _Speed;\n     }\n      private void Update()     {         if (_Target != null)         {             _Agent.SetDestination(_Target.position);\n         }\n     }\n }\n ",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Bullet : MonoBehaviour {     [SerializeField] private float _Speed = 5;\n     [SerializeField] private float _Damage = 25;\n      void FixedUpdate()     {         transform.Translate(Vector3.forward * _Speed * Time.deltaTime)\n;\n     }\n      private void OnTriggerEnter(Collider other)     {         if(other.tag == \"ExampleTag\")         {             other.GetComponent<Health>().DoDamage(_Damage);\n             gameObject.SetActive(false);\n         }\n     }\n }",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class DialogSystem : MonoBehaviour {       void Start()     {              }\n      void Update()     {              }\n }\n ",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine.Events;\n using UnityEngine;\n using UnityEngine.SceneManagement;\n  public class DoEvent : MonoBehaviour {     [SerializeField] private UnityEvent _Event = null;\n     [SerializeField] private bool _OnStart = false;\n     [SerializeField] private bool _OnUpdate = false;\n     [SerializeField] private bool _OnButtonPressed = false;\n      void Start()     {         if (_OnStart)             DoEvents();\n     }\n      void Update()     {         if (_OnUpdate)             DoEvents();\n          if (_OnButtonPressed)             if (Input.anyKey)                 DoEvents();\n     }\n      private void DoEvents()     {         _Event.Invoke();\n     }\n      public void SetGameobject_InActive(GameObject targetobject)     {         targetobject.SetActive(false);\n     }\n      public void SetGameobject_Active(GameObject targetobject)     {         targetobject.SetActive(true);\n     }\n      public void SetGameObject_Negative(GameObject targetobject)     {         if (targetobject.activeSelf)             targetobject.SetActive(false);\n         else             targetobject.SetActive(true);\n     }\n      public void LoadScene(int sceneid)     {         SceneManager.LoadScene(sceneid);\n     }\n     public void LoadScene(string scenename)     {         SceneManager.LoadScene(scenename);\n     }\n     public void Quit()     {         Application.Quit();\n     }\n }",
"using UnityEngine;\n using UnityEditor;\n using System.Collections;\n  class EditorWindowExample : EditorWindow {     string examplestring = \"example\";\n     bool examplebool = false;\n      [MenuItem(\"Tools/EditorWindowExample\")]     public static void ShowWindow()     {         EditorWindow.GetWindow(typeof(EditorWindowExample));\n     }\n      void OnGUI()     {         GUILayout.Label(\"Example Title\", EditorStyles.boldLabel);\n         examplestring = EditorGUILayout.TextField(\"Example string field\", examplestring);\n         examplebool = EditorGUILayout.Toggle(\"Example bool field\", examplebool);\n     }\n }",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class EnemySpawnHandler : MonoBehaviour {     private enum Options {Endless, Waves}\n      [Header(\"Settings\")]\n     [SerializeField] private Options _Option = Options.Endless;\n     [SerializeField] private int _Seed = 0;\n     [SerializeField] private bool _SetRandomSeed = true;\n      [Header(\"Object Pool\")]\n     [SerializeField] private ObjectPool _ObjectPool = null;\n      [Header(\"Enemies\")]\n     [SerializeField] private EnemySpawnHandler_Enemy[] _Enemies = null;\n      [Header(\"SpawnLocations\")]\n     [SerializeField] private Transform[] _SpawnLocations = null;      [Header(\"Settings - Endless\")\n]\n     [SerializeField] private float _SpawnRate = 5;\n // Seconds between spawning     \n[SerializeField] private float _SpawnRateEncrease = 0.05f;\n // Decrease time between spawning per sec     \n[SerializeField] private bool _RandomEnemy = true;\n     [SerializeField] private bool _RandomSpawn = true;\n      [Header(\"Settings - Waves\")]\n     [SerializeField] private EnemySpawnHandler_WaveSettings _Waves = null;\n      private float _Timer = 0;\n     private int _CurrentWave = 0;\n     private int _CheckWave = 999;\n     private float _TimerBetweenWaves = 0;\n     private float _SpawnSpeed = 0;\n      private void Start()     {         if (_SetRandomSeed)             Random.InitState(Random.Range(0, 10000));\n         else             Random.InitState(_Seed);\n          if (_Waves.WaveOption == EnemySpawnHandler_WaveSettings.WaveOptions.Generate)             GenerateWaves();\n         if (_Waves.WaveOption == EnemySpawnHandler_WaveSettings.WaveOptions.Endless)         {             _Waves.WaveAmount = 1;\n             GenerateWaves();\n             GenerateWaves();\n         }\n     }\n      void Update()     {         _Timer += 1 * Time.deltaTime;\n          switch (_Option)         {             case Options.Endless:                 Update_Endless();\n                 break;\n             case Options.Waves:                 Update_Waves();\n                 break;\n         }\n     }\n      //Update     \nprivate void Update_Endless()     {         if (_Timer >= _SpawnRate)         {             int randomenemyid = 0;\n             int randomspawnid = 0;\n             if (_RandomEnemy)                  randomenemyid = Random.Range(0, _Enemies.Length);\n             if (_RandomSpawn)                 randomspawnid = Random.Range(0, _SpawnLocations.Length);\n             Spawn(randomenemyid, randomspawnid);\n             _Timer = 0;\n         }\n         _SpawnRate -= _SpawnRateEncrease * Time.deltaTime;\n     }\n     private void Update_Waves()     {         if (_CurrentWave < _Waves.WaveAmount)         {             if (_CheckWave != _CurrentWave)             {                 //Get info / time between                 \n_TimerBetweenWaves += 1 * Time.deltaTime;\n                 if (_TimerBetweenWaves >= _Waves.TimeBetweenWaves)                 {                     _TimerBetweenWaves = 0;\n                     _CheckWave = _CurrentWave;\n                     _SpawnSpeed = _Waves.Waves[_CurrentWave].SpawnDuration / _Waves.Waves[_CurrentWave].TotalEnemies;\n                     if (_Waves.WaveOption == EnemySpawnHandler_WaveSettings.WaveOptions.Endless)                         GenerateWaves();\n                 }\n             }\n             else             {                 //Spawn                 \nif (_Waves.Waves[_CurrentWave].TotalEnemies > 0)                 {                     if (_Timer > _SpawnSpeed)                     {                         bool spawncheck = false;\n                         while (!spawncheck)                         {                             int spawnid = Random.Range(0, _Enemies.Length);\n                             if (_Waves.Waves[_CurrentWave].EnemyID[spawnid] > 0)                             {                                 Spawn(spawnid, Random.Range(0, _SpawnLocations.Length));\n                                 _Waves.Waves[_CheckWave].EnemyID[spawnid]--;\n                                 _Waves.Waves[_CurrentWave].TotalEnemies--;\n                                 spawncheck = true;\n                             }\n                         }\n                         _Timer = 0;\n                     }\n                 }\n                 else                 {                     _CurrentWave++;\n                 }\n             }\n         }\n     }\n      //Generate Waves     \nprivate void GenerateWaves()     {         int enemytypes = _Enemies.Length;\n         for (int i = 0; i < _Waves.WaveAmount; i++)\n         {             EnemySpawnHandler_Wave newwave = new EnemySpawnHandler_Wave();\n             int enemyamount = Mathf.RoundToInt(_Waves.EnemyAmount * ((_Waves.EnemyIncreaseAmount * i) + 1));\n              //Set enemy amount             \nnewwave.EnemyID = new int[enemytypes];\n             int checkenemyamount = 0;\n             newwave.TotalEnemies = enemyamount;\n              while (checkenemyamount < enemyamount)             {                 for (int j = 0; j < enemytypes; j++)\n                 {                     if (_Enemies[j].StartWave <= i)                     {                         int addamount = 0;\n                         if (enemyamount < 2)                             addamount = Random.Range(0, enemyamount);\n                         else                             addamount = Random.Range(0, Mathf.RoundToInt(enemyamount*0.5f));\n                          if (enemyamount > checkenemyamount + addamount)                         {                             newwave.EnemyID[j] += addamount;\n                             checkenemyamount += addamount;\n                         }\n                         else                         {                             newwave.EnemyID[j] += enemyamount - checkenemyamount;\n                             checkenemyamount = enemyamount;\n                             continue;\n                         }\n                     }\n                 }\n             }\n             _Waves.Waves.Add(newwave);\n         }\n     }\n      public void Spawn(int enemyid, int spawnid)     {         GameObject obj = _ObjectPool.GetObjectPrefabName(_Enemies[enemyid].EnemyPrefab.name, false);\n         obj.transform.position = _SpawnLocations[spawnid].position;         obj.SetActive(true)\n;\n     }\n }\n  [System.Serializable] public class EnemySpawnHandler_Enemy {     public string EnemyName;\n     public GameObject EnemyPrefab;\n      [Header(\"Settings\")]\n     public int StartWave;\n }\n  [System.Serializable] public class EnemySpawnHandler_WaveSettings {     public enum WaveOptions {Endless, Manually, Generate}\n     public WaveOptions WaveOption;\n      [Header(\"Endless\")]\n     public float EnemyIncreaseAmount;\n      [Header(\"Manual\")]\n     public List<EnemySpawnHandler_Wave> Waves;\n      [Header(\"Generate\")]\n     public int WaveAmount;\n     public int EnemyAmount;\n      [Header(\"Other\")]\n     public float TimeBetweenWaves;\n }\n  [System.Serializable] public class EnemySpawnHandler_Wave {     public int[] EnemyID;\n     public float SpawnDuration = 5;\n      [HideInInspector] public int TotalEnemies;\n }",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Health : MonoBehaviour {     [SerializeField] private float _MaxHealth = 100;\n      private float _CurrentHealth;\n      private void OnEnable()     {         _CurrentHealth = _MaxHealth;\n     }\n      public void DoDamage(float damageamount)     {         _CurrentHealth -= damageamount;\n         if(_CurrentHealth <= 0)         {             _CurrentHealth = 0;\n             gameObject.SetActive(false);\n         }\n     }\n      public float GetCurrentHealth()     {         return _CurrentHealth;\n     }\n     public float GetMaxHealth()     {         return GetMaxHealth();\n     }\n }\n ",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.UI;\n  public class Inventory : MonoBehaviour {     [Header(\"Inventory\")]\n     [SerializeField] private int[] _Inventory;\n     [SerializeField] private Image[] _InventoryVisuals;\n      [Header(\"Items\")]\n     [SerializeField] private List<InventoryItems> _Items = new List<InventoryItems>();\n      private void Start()     {         _Inventory = new int[_InventoryVisuals.Length];\n     }\n      public void AddItem(GameObject addobj)     {         for (int i = 0; i < _Inventory.Length; i++)\n         {             if(_Inventory[i] == 0)             {                 for (int j = 0; j < _Items.Count; j++)\n                 {                     if(addobj == _Items[j].Item_Prefab)                     {                         _Inventory[i] = j + 1;\n                     }\n                 }\n             }\n         }\n     }\n }\n   [System.Serializable] public class InventoryItems {     public GameObject Item_Prefab;\n     public Sprite Item_Sprite;\n }",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class LightEffects : MonoBehaviour {     public enum LightEffectOptions { Flickering, Off, On }\n;\n      [Header(\"Settings\")]\n     [SerializeField] private LightEffectOptions _LightEffectOption = LightEffectOptions.Flickering;\n     [SerializeField] private Vector2 _MinMaxIncrease = new Vector2(0.8f, 1.2f);\n     [Range(0.01f, 100)] [SerializeField] private float _EffectStrength = 50;\n      Queue<float> _LightFlickerQ;\n     private float _LastSum = 0;\n     private Light _Light;\n     private float _LightIntensity = 0;\n      public void Reset()     {         if (_LightEffectOption == LightEffectOptions.Flickering)         {             _LightFlickerQ.Clear();\n             _LastSum = 0;\n         }\n     }\n      void Start()     {         _Light = GetComponent<Light>();\n         _LightIntensity = _Light.intensity;\n         _LightFlickerQ = new Queue<float>(Mathf.RoundToInt(_EffectStrength));\n     }\n      void Update()     {         switch(_LightEffectOption)         {             case LightEffectOptions.Flickering:                 while (_LightFlickerQ.Count >= _EffectStrength)                     _LastSum -= _LightFlickerQ.Dequeue();\n                  float newVal = Random.Range(_LightIntensity * _MinMaxIncrease.x, _LightIntensity * _MinMaxIncrease.y);\n                 _LightFlickerQ.Enqueue(newVal);\n                 _LastSum += newVal;\n                 _Light.intensity = _LastSum / (float)_LightFlickerQ.Count;\n                 break;\n             case LightEffectOptions.Off:                 _Light.intensity = 0;\n                 break;\n             case LightEffectOptions.On:                 _Light.intensity = _LightIntensity = _MinMaxIncrease.x;\n                 break;\n         }\n      }\n      public void SetEffect(LightEffectOptions options)     {         _LightEffectOption = options;\n     }\n }",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.SceneManagement;\n  public class LoadScenes : MonoBehaviour {     public void Action_LoadScene(int sceneid)     {         SceneManager.LoadScene(sceneid);\n     }\n     public void Action_LoadScene(string scenename)     {         SceneManager.LoadScene(scenename);\n     }\n      public void Action_ReloadScene()     {         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);\n     }\n      public void Action_QuitApplication()     {         Application.Quit();\n     }\n }\n ",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.SceneManagement;\n  public class MenuHandler : MonoBehaviour {     public void LoadScene(int sceneid)     {         SceneManager.LoadScene(sceneid);\n     }\n      public void LoadScene(string scenename)     {         SceneManager.LoadScene(scenename);\n     }\n      public int Get_CurrentSceneID()     {         return SceneManager.GetActiveScene().buildIndex;\n     }\n      public string Get_CurrentSceneName()     {         return SceneManager.GetActiveScene().name;\n     }\n      public void QuitGame()     {         Application.Quit();\n     }\n }\n ",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  [RequireComponent(typeof(Rigidbody2D))] public class Movement_2D_Platformer : MonoBehaviour {     [Header(\"Settings\")\n]\n     [SerializeField] private float _NormalSpeed = 5;\n     [SerializeField] private float _SprintSpeed = 8;\n     [SerializeField] private float _JumpSpeed = 300;\n     [SerializeField] private float _GroundCheck = 0.6f;\n     [Header(\"Set ground layer\")]\n     [SerializeField] private LayerMask _GroundMask = ~1;\n      private float _Speed = 0;\n     private Rigidbody2D _RB;\n      void Start()     {         //Get Rigidbody / Lock z rotation         \n_RB = GetComponent<Rigidbody2D>();\n         _RB.constraints = RigidbodyConstraints2D.FreezeRotation;\n     }\n      void Update()     {         //Sprint         \nif (Input.GetKey(KeyCode.LeftShift))             _Speed = _SprintSpeed;\n         else             _Speed = _NormalSpeed;\n          //Jumping         \nif (Input.GetButtonDown(\"Jump\") && IsGrounded())             _RB.AddForce(new Vector2(0, _JumpSpeed));\n          //Apply Movement         \n_RB.velocity = new Vector2(Input.GetAxis(\"Horizontal\") * _Speed, _RB.velocity.y);\n     }\n      bool IsGrounded()     {         RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _GroundCheck, _GroundMask)\n;\n         if (hit.collider != null)         {             return true;\n         }\n         return false;\n     }\n }\n ",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  [RequireComponent(typeof(Rigidbody2D))] public class Movement_2D_TopDown : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] private float _NormalSpeed = 5;\n     [SerializeField] private float _SprintSpeed = 8;\n      private float _Speed = 0;\n     private Rigidbody2D _RB;\n      void Start()     {         //Get Rigidbody / Lock z rotation         \n_RB = GetComponent<Rigidbody2D>();\n         _RB.constraints = RigidbodyConstraints2D.FreezeRotation;\n         _RB.gravityScale = 0;\n     }\n      void Update()     {         //Sprint         \nif (Input.GetKey(KeyCode.LeftShift))             _Speed = _SprintSpeed;\n         else             _Speed = _NormalSpeed;\n          //Apply Movement         \n_RB.velocity = new Vector2(Input.GetAxis(\"Horizontal\") * _Speed, Input.GetAxis(\"Vertical\") * _Speed);\n     }\n }\n ",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Movement_Camera : MonoBehaviour {     private enum CameraOptionsPos { None, Follow }\n     private enum CameraOptionsRot { None, Follow }\n      [Header(\"Options\")]\n     [SerializeField] private CameraOptionsPos _CameraOptionPos = CameraOptionsPos.Follow;\n     [SerializeField] private CameraOptionsRot _CameraOptionRot = CameraOptionsRot.Follow;\n     [Header(\"Settings - Position\")]\n     [SerializeField] private Vector3 _OffsetPosition = new Vector3(0,12,-4);\n     [SerializeField] private bool _UseOffsetYAsDefaultHeight = true;\n     [Header(\"Settings - Rotation\")]\n     [SerializeField] private Vector3 _OffsetRotation = Vector3.zero;\n     [Header(\"Settings\")]\n     [SerializeField] private float _Speed = 1000;\n      [Header(\"Other\")]\n     [SerializeField] private Transform _Target = null;      private Vector3 _TargetPosition;     private float _ScreenShakeDuration;     private float _ScreenShakeIntensity;      void Update()\n     {         if(Input.GetKeyDown(KeyCode.J))         {             Effect_ScreenShake(3, 0.5f);\n         }\n          //Update Target Location         \nif (_UseOffsetYAsDefaultHeight)             _TargetPosition = new Vector3(_Target.transform.position.x + _OffsetPosition.x, _OffsetPosition.y, _Target.transform.position.z + _OffsetPosition.z)\n;\n         else             _TargetPosition = new Vector3(_Target.transform.position.x + _OffsetPosition.x, _Target.transform.position.y + _OffsetPosition.y, _Target.transform.position.z + _OffsetPosition.z)\n;\n          // Movement         \nswitch (_CameraOptionPos)         {             case CameraOptionsPos.Follow:                 if (_UseOffsetYAsDefaultHeight)                     transform.position = Vector3.MoveTowards(transform.position, _TargetPosition, _Speed * Time.deltaTime)\n;\n                 else                     transform.position = _TargetPosition;                 break;         }\n          //ScreenShake         \nif(_ScreenShakeDuration > 0)\n         {             transform.localPosition = new Vector3(_TargetPosition.x + Random.insideUnitSphere.x * _ScreenShakeIntensity, _TargetPosition.y + Random.insideUnitSphere.y * _ScreenShakeIntensity, _TargetPosition.z)\n;\n             _ScreenShakeDuration -= 1 * Time.deltaTime;\n         }\n         else         {             // Rotation             \nswitch (_CameraOptionRot)             {                 case CameraOptionsRot.Follow:                     Vector3 rpos = _Target.position - transform.position;                     Quaternion lookrotation = Quaternion.LookRotation(rpos, Vector3.up)\n;\n                     transform.eulerAngles = new Vector3(lookrotation.eulerAngles.x + _OffsetRotation.x, lookrotation.eulerAngles.y + _OffsetRotation.y, lookrotation.eulerAngles.z + _OffsetRotation.z)\n;\n                     break;\n             }\n         }\n     }\n      //Effects     \npublic void Effect_ScreenShake(float duration, float intesity)     {         _ScreenShakeDuration = duration;\n         _ScreenShakeIntensity = intesity;\n     }\n      //Set     \npublic void Set_CameraTarget(GameObject targetobj)     {         _Target = targetobj.transform;     }\n     public void Set_OffSet(Vector3 offset)\n     {         _OffsetPosition = offset;\n     }\n }\n ",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  [RequireComponent(typeof(CharacterController))] public class Movement_CC_Platformer : MonoBehaviour {     [Header(\"Settings\")\n]\n     [SerializeField] private float _NormalSpeed = 5, _SprintSpeed = 8;\n     [SerializeField] private float _JumpSpeed = 5;\n     [SerializeField] private float _Gravity = 20;\n     [SerializeField] private bool _ZMovementActive = false;\n          private Vector3 _MoveDirection = Vector3.zero;\n     private float _Speed;\n     private CharacterController _CC;\n      void Start()     {         _CC = GetComponent<CharacterController>();\n     }\n      void Update()     {         //Movement         \nif (_CC.isGrounded)         {             float verticalmovement = 0;\n             if (_ZMovementActive)                 verticalmovement = Input.GetAxis(\"Vertical\");\n              _MoveDirection = new Vector3(Input.GetAxis(\"Horizontal\"), 0, verticalmovement);\n             _MoveDirection = transform.TransformDirection(_MoveDirection)\n;\n             _MoveDirection *= _Speed;\n             if (Input.GetButton(\"Jump\"))                 _MoveDirection.y = _JumpSpeed;\n         }\n          //Sprint         \nif (Input.GetKey(KeyCode.LeftShift))             _Speed = _SprintSpeed;\n         else             _Speed = _NormalSpeed;\n          //Apply Movement         \n_MoveDirection.y -= _Gravity * Time.deltaTime;\n         _CC.Move(_MoveDirection * Time.deltaTime);\n     }\n }\n ",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  [RequireComponent(typeof(CharacterController))] public class Movement_CC_TopDown : MonoBehaviour {     //Movement     \n[Header(\"Settings Camera\")]\n     [SerializeField] private Camera _Camera;\n     [Header(\"Settings\")]\n     [SerializeField] private float _NormalSpeed = 5;\n     [SerializeField] private float _SprintSpeed = 8;\n     [SerializeField] private float _JumpSpeed = 5;\n     [SerializeField] private float _Gravity = 20;\n     [SerializeField] private bool _MovementRelativeToRotation = false;\n      private float _Speed = 0;\n     private Vector3 _MoveDirection = Vector3.zero;\n     private CharacterController _CC;\n      void Start()     {         _CC = GetComponent<CharacterController>();\n     }\n      void Update()     {         //Movement         \nif (_CC.isGrounded)         {             _MoveDirection = new Vector3(Input.GetAxis(\"Horizontal\"), 0, Input.GetAxis(\"Vertical\"));\n             if (_MovementRelativeToRotation)                 _MoveDirection = transform.TransformDirection(_MoveDirection)\n;\n             _MoveDirection *= _Speed;\n             if (Input.GetButton(\"Jump\"))                 _MoveDirection.y = _JumpSpeed;\n         }\n          _MoveDirection.y -= _Gravity * Time.deltaTime;\n         _CC.Move(_MoveDirection * Time.deltaTime);\n          //Sprint         \nif (Input.GetKey(KeyCode.LeftShift))             _Speed = _SprintSpeed;\n         else             _Speed = _NormalSpeed;\n          Ray cameraRay = _Camera.ScreenPointToRay(Input.mousePosition);\n         Plane groundPlane = new Plane(Vector3.up, Vector3.zero);\n         float rayLength;\n         if (groundPlane.Raycast(cameraRay, out rayLength))         {             Vector3 pointToLook = cameraRay.GetPoint(rayLength);\n             transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z)\n);\n         }\n     }\n      public void SetCamera(Camera cameraobj)     {         _Camera = cameraobj;\n     }\n }\n ",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  [RequireComponent(typeof(CharacterController))] public class Movement_CC : MonoBehaviour {     //Movement     \n[SerializeField] private float _NormalSpeed = 5, _SprintSpeed = 8;\n     [SerializeField] private float _JumpSpeed = 5;\n     [SerializeField] private float _Gravity = 20;\n     private Vector3 _MoveDirection = Vector3.zero;\n     //Look around     \npublic float _CameraSensitivity = 1;\n     [SerializeField] private Transform _Head = null;     private float _RotationX = 90.0f;     private float _RotationY = 0.0f;     private float _Speed;      private CharacterController _CC;     private bool _LockRotation;      void Start()\n     {         Cursor.lockState = CursorLockMode.Locked;\n         Cursor.visible = false;\n         _CC = GetComponent<CharacterController>();\n         if (_Head == null)             _Head = transform.GetChild(0)\n.transform;     }\n      void Update()\n     {         //Look around         \nif (!_LockRotation)         {             _RotationX += Input.GetAxis(\"Mouse X\") * _CameraSensitivity;\n             _RotationY += Input.GetAxis(\"Mouse Y\") * _CameraSensitivity;\n             _RotationY = Mathf.Clamp(_RotationY, -90, 90);\n              transform.localRotation = Quaternion.AngleAxis(_RotationX, Vector3.up)\n;\n             _Head.transform.localRotation = Quaternion.AngleAxis(_RotationY, Vector3.left)\n;\n         }\n          //Movement         \nif (_CC.isGrounded)         {             _MoveDirection = new Vector3(Input.GetAxis(\"Horizontal\"), 0, Input.GetAxis(\"Vertical\"));\n             _MoveDirection = transform.TransformDirection(_MoveDirection)\n;\n             _MoveDirection *= _Speed;\n             if (Input.GetButton(\"Jump\"))                 _MoveDirection.y = _JumpSpeed;\n         }\n          //Sprint         \nif (Input.GetKey(KeyCode.LeftShift))             _Speed = _SprintSpeed;\n         else             _Speed = _NormalSpeed;\n          //Apply Movement         \n_MoveDirection.y -= _Gravity * Time.deltaTime;\n         _CC.Move(_MoveDirection * Time.deltaTime);\n     }\n      public void LockRotation(bool state)     {         _LockRotation = state;\n     }\n }",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Movement_FreeCamera : MonoBehaviour {     [SerializeField] private float _Speed = 5;\n     [SerializeField] private float _SprintSpeed = 8;\n      private float _CurrentSpeed;\n      void Start()     {         Cursor.visible = false;\n         Cursor.lockState = CursorLockMode.Locked;\n     }\n      void Update()     {         if (Input.GetKey(KeyCode.LeftShift))             _CurrentSpeed = _SprintSpeed;\n         else             _CurrentSpeed = _Speed;\n          float xas = Input.GetAxis(\"Horizontal\");\n         float zas = Input.GetAxis(\"Vertical\");\n          transform.Translate(new Vector3(xas,0, zas)\n * _CurrentSpeed * Time.deltaTime);\n          float mousex = Input.GetAxis(\"Mouse X\");\n         float mousey = Input.GetAxis(\"Mouse Y\");\n         transform.eulerAngles += new Vector3(-mousey, mousex, 0)\n;\n     }\n }\n ",
"",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class ObjectPool : MonoBehaviour {     [SerializeField] private ObjectPool_Pool[] _ObjectPools = null;\n     private List<Transform> _Parents = new List<Transform>()\n;\n      private void Awake()     {         GameObject emptyobject = GameObject.CreatePrimitive(PrimitiveType.Cube);\n         Destroy(emptyobject.GetComponent<MeshRenderer>());\n         Destroy(emptyobject.GetComponent<BoxCollider>());\n          for (int i = 0; i < _ObjectPools.Length; i++)\n         {             //Create parent             \nGameObject poolparent = Instantiate(emptyobject, transform.position, Quaternion.identity)\n;\n             Destroy(poolparent.GetComponent<MeshRenderer>());\n             Destroy(poolparent.GetComponent<BoxCollider>());\n              //Set parent             \npoolparent.transform.parent = transform;             poolparent.transform.name = \"Pool_\" + _ObjectPools[i]._Name;             _Parents.Add(poolparent.transform)\n;\n              //Create objects             \nfor (int o = 0; o < _ObjectPools[i]._Amount; o++)\n             {                 GameObject obj = (GameObject)Instantiate(_ObjectPools[i]._Prefab);\n                 obj.transform.parent = poolparent.transform;                 obj.transform.position = new Vector2(9999, 9999)\n;\n                 obj.SetActive(false);\n                 _ObjectPools[i]._Objects.Add(obj);\n             }\n         }\n         Destroy(emptyobject);\n     }\n      //GetObject     \npublic GameObject GetObject(string objname, bool setactive)     {         int id = FindObjectPoolID(objname, false);\n         return GetObject(id, setactive);\n     }\n     public GameObject GetObject(GameObject obj, bool setactive)     {         int id = FindObjectPoolID(obj);\n         return GetObject(id, setactive);\n     }\n     public GameObject GetObjectPrefabName(string prefabname, bool setactive)     {         int id = FindObjectPoolID(prefabname, true);\n         return GetObject(id, setactive);\n     }\n      public GameObject GetObject(int id, bool setactive)     {         GameObject freeObject = null;\n         bool checkfreeobj = false;\n          for (int i = 0; i < _ObjectPools[id]._Objects.Count; i++)\n         {             if (!_ObjectPools[id]._Objects[i].activeInHierarchy)             {                 _ObjectPools[id]._Objects[i].transform.position = new Vector3(999, 999, 999)\n;\n                 _ObjectPools[id]._Objects[i].SetActive(setactive);\n                 freeObject = _ObjectPools[id]._Objects[i];\n                 return freeObject;\n             }\n         }\n          if (!checkfreeobj)         {             _ObjectPools[id]._Objects.Clear();\n             freeObject = (GameObject)Instantiate(_ObjectPools[id]._Prefab, new Vector3(999,999,999), Quaternion.identity);\n             freeObject.transform.parent = _Parents[id];             freeObject.SetActive(setactive)\n;\n             _ObjectPools[id]._Objects.Add(freeObject);\n             return freeObject;\n         }\n          Debug.Log(\"No Object Found\");\n         return null;\n     }\n      public List<GameObject> GetAllObjects(GameObject objtype)     {         int id = FindObjectPoolID(objtype);\n         return _ObjectPools[id]._Objects;\n     }\n      private int FindObjectPoolID(GameObject obj)     {         int id = 0;\n         for (int i = 0; i < _ObjectPools.Length; i++)\n         {             if (obj == _ObjectPools[i]._Prefab)             {                 id = i;\n             }\n         }\n         return id;\n     }\n     private int FindObjectPoolID(string objname, bool isprefab)     {         for (int i = 0; i < _ObjectPools.Length; i++)\n         {             if (isprefab)                 if (objname == _ObjectPools[i]._Prefab.name)                 {                     return i;\n                 }\n                 else             if (objname == _ObjectPools[i]._Name)                 {                     return i;\n                 }\n         }\n         Debug.Log(objname + \" Not Found\");\n         return 0;\n     }\n }\n  [System.Serializable] public class ObjectPool_Pool {     public string _Name;\n     public GameObject _Prefab;\n     public int _Amount;\n     [HideInInspector] public List<GameObject> _Objects;\n }",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class ObjectPoolSimple : MonoBehaviour {     public GameObject prefabGameObject;\n     public int pooledAmount;\n      [HideInInspector] public List<GameObject> objects;\n      void Awake()     {         for (int i = 0; i < pooledAmount; i++)\n         {             GameObject obj = (GameObject)Instantiate(prefabGameObject);\n             obj.transform.parent = gameObject.transform;             obj.SetActive(false)\n;\n             objects.Add(obj);\n         }\n     }\n }\n   /* Use Pool          \n[SerializeField]private ObjectPoolSimple _ObjectPool;\n      private void Spawn() {         for (int i = 0; i < _ObjectPool.objects.Count; i++)\n {             if (!_ObjectPool.objects[i].activeInHierarchy) {                 _ObjectPool.objects[i].transform.position = new Vector3(0,0,0)\n;\n                 _ObjectPool.objects[i].transform.rotation = Quaternion.Euler(0, 0, 0)\n;\n                 _ObjectPool.objects[i].SetActive(true);\n                 break;\n             }\n         }\n     }\n */ ",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.Events;\n  public class OnCollision : MonoBehaviour {     private enum Options {OnTriggerEnter, OnTriggerExit, OnTriggerStay, OnCollisionEnter, OnCollisionExit, OnCollisionStay}\n;\n     [SerializeField] private LayerMask _LayerMask = ~0;\n     [SerializeField] private Options _Option = Options.OnTriggerEnter;\n     [SerializeField] private string _Tag = \"\";\n     [SerializeField] private UnityEvent _Event = null;\n      private bool _HasTag;\n      private void Start()     {         if (_Tag != \"\" || _Tag != null)             _HasTag = true;\n     }\n      private void Action(Collider other)     {         if (_HasTag)             if (other.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)                 _Event.Invoke();\n     }\n     private void Action(Collision other)     {         if (_HasTag)             if (other.gameObject.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)                 _Event.Invoke();\n     }\n      private void OnTriggerEnter(Collider other)     {         if (_Option == Options.OnTriggerEnter)             Action(other);\n     }\n     private void OnTriggerExit(Collider other)     {         if (_Option == Options.OnTriggerExit)             Action(other);\n     }\n     private void OnTriggerStay(Collider other)     {         if (_Option == Options.OnTriggerStay)             Action(other);\n     }\n     void OnCollisionEnter(Collision other)     {         if (_Option == Options.OnCollisionEnter)             Action(other);\n     }\n     void OnCollisionExit(Collision other)     {         if (_Option == Options.OnCollisionExit)             Action(other);\n     }\n     void OnCollisionStay(Collision other)     {         if (_Option == Options.OnCollisionStay)             Action(other);\n     }\n }",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using System;\n using System.ComponentModel;\n using System.Net.Sockets;\n using System.IO;\n  public class ReadTwitchChat : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] private float _RefreshConnectionTimer = 60;\n     private float _Timer;\n      [Header(\"Twitch\")]\n     private TcpClient twitchClient;\n     private StreamReader reader;\n     private StreamWriter writer;\n      [SerializeField] private string _Username = \"\";\n //Twitch user name     \n[SerializeField] private string _OauthToken = \"\";\n //Get token from https://twitchapps.com/tmi     \n[SerializeField] private string _Channelname = \"\";\n //Twitch channel name      \nvoid Start()     {         Connect();\n     }\n      void Update()     {         //Check connection         \nif (!twitchClient.Connected)             Connect();\n          _Timer -= 1 * Time.deltaTime;\n         if (_Timer <= 0)         {             Connect();\n             _Timer = _RefreshConnectionTimer;\n         }\n          ReadChat();\n     }\n      private void Connect()     {         twitchClient = new TcpClient(\"irc.chat.twitch.tv\", 6667);\n         reader = new StreamReader(twitchClient.GetStream());\n         writer = new StreamWriter(twitchClient.GetStream());\n          writer.WriteLine(\"PASS \" + _OauthToken);\n         writer.WriteLine(\"NICK \" + _Username);\n         writer.WriteLine(\"USER \" + _Username + \" 8 * :\" + _Username);\n         writer.WriteLine(\"JOIN #\" + _Channelname);\n          writer.Flush();\n     }\n      private void ReadChat()     {         if (twitchClient.Available > 0)         {             var message = reader.ReadLine();\n              if (message.Contains(\"PRIVMSG\"))             {                 //Split                 \nvar splitPoint = message.IndexOf(\"!\", 1);\n                 var chatName = message.Substring(0, splitPoint);\n                  //Name                 \nchatName = chatName.Substring(1);\n                  //Message                 \nsplitPoint = message.IndexOf(\":\", 1);\n                 message = message.Substring(splitPoint + 1);\n                 print(string.Format(\"{0}\n: {1}\\n\", chatName, message));\n                  if (message.ToLower().Contains(\"example\"))                 {                     Debug.Log(\"<color=green>\" + chatName + \" has used the command example </color>\");\n                 }\n             }\n         }\n     }\n  }",
"using System.Collections.Generic;\n using System.IO;\n using UnityEngine;\n  public class SaveLoad_JSON : MonoBehaviour {     private Json_SaveData _SaveData = new Json_SaveData();\n      void Start()     {         LoadData();\n     }\n      public void SaveData()     {         string jsonData = JsonUtility.ToJson(_SaveData, true);\n         File.WriteAllText(Application.persistentDataPath + \"/SaveData.json\", jsonData);\n     }\n     public void LoadData()     {         try         {             string dataAsJson = File.ReadAllText(Application.persistentDataPath + \"/SaveData.json\");\n             _SaveData = JsonUtility.FromJson<Json_SaveData>(dataAsJson);\n         }\n         catch         {             SaveData();\n         }\n     }\n     public Json_SaveData GetSaveData()     {         return _SaveData;\n     }\n     public void CreateNewSave()     {         Json_ExampleData newsave = new Json_ExampleData();\n         newsave.exampleValue = 10;\n         _SaveData.saveData.Add(newsave);\n     }\n }\n  [System.Serializable] public class Json_SaveData {     public List <Json_ExampleData> saveData = new List<Json_ExampleData>();\n }\n [System.Serializable] public class Json_ExampleData {     public float exampleValue = 0;\n }",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using System.Xml.Serialization;\n using System.IO;\n  public class SaveLoad_XML : MonoBehaviour {     private XML_SaveData _SaveData = new XML_SaveData();\n      void Start()     {         LoadData();\n     }\n      public void SaveData()     {         XmlSerializer serializer = new XmlSerializer(typeof(XML_SaveData));\n          using (FileStream stream = new FileStream(Application.persistentDataPath + \"/SaveData.xml\", FileMode.Create))         {             serializer.Serialize(stream, _SaveData);\n         }\n     }\n      public void LoadData()     {         try         {             XmlSerializer serializer = new XmlSerializer(typeof(XML_SaveData));\n              using (FileStream stream = new FileStream(Application.persistentDataPath + \"/SaveData.xml\", FileMode.Open))             {                 _SaveData = serializer.Deserialize(stream) as XML_SaveData;\n             }\n         }\n         catch         {             SaveData();\n         }\n     }\n      public XML_SaveData GetSaveData()     {         return _SaveData;\n     }\n     public void CreateNewSave()     {         XML_ExampleData newsave = new XML_ExampleData();\n         newsave.exampleValue = 10;\n         _SaveData.saveData.Add(newsave);\n     }\n }\n  [System.Serializable] public class XML_SaveData {     public List<XML_ExampleData> saveData = new List<XML_ExampleData>();\n }\n [System.Serializable] public class XML_ExampleData {     public float exampleValue = 0;\n }",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  [CreateAssetMenu(fileName = \"Example\", menuName = \"SO/ExampleSO\", order = 1)] public class ScriptebleGameObject : ScriptableObject {     public string examplestring;\n     public int exampleint;\n }",
"",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Shooting : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] ObjectPool _ObjectPool = null;\n     [SerializeField] private GameObject _BulletPrefab = null;\n     [SerializeField] private GameObject _ShootPoint = null;\n      [Header(\"Semi\")]\n     [SerializeField] private int _SemiAutomaticBulletAmount = 3;\n     [SerializeField] private float _SemiShootSpeed = 0.2f;\n     [Header(\"Automatic\")]\n     [SerializeField] private float _SecondsBetweenShots = 0.5f;\n      private enum ShootModes { SingleShot, SemiAutomatic, Automatic }\n     [SerializeField] private ShootModes _ShootMode = ShootModes.SingleShot;\n      private bool _CheckSingleShot;\n     private float _Timer;\n     private bool _LockShooting;\n      void Update()     {         if (Input.GetMouseButton(0))         {             switch (_ShootMode)             {                 case ShootModes.SingleShot:                     if (!_CheckSingleShot)                         Shoot();\n                     _CheckSingleShot = true;\n                     break;\n                 case ShootModes.SemiAutomatic:                     if (!_CheckSingleShot && !_LockShooting)                         StartCoroutine(SemiShot());\n                     _CheckSingleShot = true;\n                     break;\n                 case ShootModes.Automatic:                     _Timer += 1 * Time.deltaTime;\n                     if (_Timer >= _SecondsBetweenShots)                     {                         Shoot();\n                         _Timer = 0;\n                     }\n                     break;\n             }\n         }\n         if (Input.GetMouseButtonUp(0))         {             _CheckSingleShot = false;\n         }\n     }\n      IEnumerator SemiShot()     {         _LockShooting = true;\n         for (int i = 0; i < _SemiAutomaticBulletAmount; i++)\n         {             Shoot();\n             yield return new WaitForSeconds(_SemiShootSpeed);\n         }\n         _LockShooting = false;\n     }\n      void Shoot()     {        GameObject bullet = _ObjectPool.GetObject(_BulletPrefab, true);\n         bullet.SetActive(true);\n         bullet.transform.position = _ShootPoint.transform.position;         bullet.transform.rotation = _ShootPoint.transform.rotation;     }\n }\n ",
"using System.Collections;\n using System.Collections.Generic;\n using System.Threading;\n using UnityEngine;\n  public class ShootingRayCast : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] private float _Damage = 20;\n     [SerializeField] private float _ShootDistance = 50;\n     [SerializeField] private string _EnemyTag = \"Enemy\";\n      [Header(\"Semi\")]\n     [SerializeField] private int _SemiAutomaticBulletAmount = 3;\n     [SerializeField] private float _SemiShootSpeed = 0.2f;\n     [Header(\"Automatic\")]\n     [SerializeField] private float _SecondsBetweenShots = 0.5f;\n      private enum ShootModes {SingleShot, SemiAutomatic, Automatic }\n     [SerializeField] private ShootModes _ShootMode = ShootModes.SingleShot;\n      private bool _CheckSingleShot;\n     private float _Timer;\n     private bool _LockShooting;\n      void Update()     {         if (Input.GetMouseButton(0))         {             switch (_ShootMode)             {                 case ShootModes.SingleShot:                     if (!_CheckSingleShot)                         Shoot();\n                     _CheckSingleShot = true;\n                     break;\n                 case ShootModes.SemiAutomatic:                     if (!_CheckSingleShot && !_LockShooting)                         StartCoroutine(SemiShot());\n                     _CheckSingleShot = true;\n                     break;\n                 case ShootModes.Automatic:                     _Timer += 1 * Time.deltaTime;\n                     if(_Timer >= _SecondsBetweenShots)                     {                         Shoot();\n                         _Timer = 0;\n                     }\n                     break;\n             }\n         }\n         if(Input.GetMouseButtonUp(0))         {             _CheckSingleShot = false;\n         }\n     }\n      IEnumerator SemiShot()     {         _LockShooting = true;\n         for (int i = 0; i < _SemiAutomaticBulletAmount; i++)\n         {             Shoot();\n             yield return new WaitForSeconds(_SemiShootSpeed);\n         }\n         _LockShooting = false;\n     }\n      void Shoot()     {         RaycastHit hit;\n         if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward)\n, out hit, _ShootDistance))             if (hit.transform.tag == _EnemyTag)\n             {                 hit.transform.GetComponent<Health>()\n.DoDamage(_Damage);\n             }\n     }\n }",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using TMPro;\n  public class StringFormats : MonoBehaviour {     private enum FormatOptions {DigitalTime }\n;\n     [SerializeField] private FormatOptions _FormatOption = FormatOptions.DigitalTime;\n     [SerializeField] private TextMeshProUGUI _ExampleText = null;\n      private float _Timer;\n      void Update()     {         _Timer += 1 * Time.deltaTime;\n          switch (_FormatOption)         {             case FormatOptions.DigitalTime:                 _ExampleText.text = string.Format(\"{0:00}\n:{1:00}\n:{2:00}\\n\", Mathf.Floor(_Timer / 3600), Mathf.Floor((_Timer / 60) % 60), _Timer % 60);\n                 break;\n}\n     }\n }\n ",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEditor;\n  public class Tool_CreateHexagonGrid : EditorWindow {     private GameObject _CenterObj;\n     private List<GameObject> _ObjSaved = new List<GameObject>();\n     private int _TotalObjects = 100;\n      //Hex     \nprivate int _HexLengthX = 10, _HexLengthZ = 10;\n     private float _HexSize = 1;\n     private float _DistanceBetween = 1;\n      private bool _Center = true;\n     private bool _Invert = false;\n      [MenuItem(\"Tools/CreateHexagonGrid\")]     static void Init()     {         Tool_CreateHexagonGrid window = (Tool_CreateHexagonGrid)EditorWindow.GetWindow(typeof(Tool_CreateHexagonGrid));\n         window.Show();\n     }\n      void OnGUI()     {          GUILayout.BeginVertical(\"Box\");\n         _CenterObj = (GameObject)EditorGUILayout.ObjectField(\"Center Object\", _CenterObj, typeof(GameObject), true);\n         GUILayout.EndVertical();\n          GUILayout.BeginVertical(\"Box\");\n         _HexSize = EditorGUILayout.FloatField(\"Size: \", _HexSize);\n         _HexLengthX = EditorGUILayout.IntField(\"Collom: \", _HexLengthX);\n         _HexLengthZ = EditorGUILayout.IntField(\"Row: \", _HexLengthZ);\n          GUILayout.BeginHorizontal(\"Box\");\n         if (GUILayout.Button(\"Calculate Total Objects\"))             _TotalObjects = _HexLengthX * _HexLengthZ;\n         EditorGUILayout.LabelField(\"Total: \" + _TotalObjects.ToString());\n         GUILayout.EndHorizontal();\n          _Center = EditorGUILayout.Toggle(\"Center\", _Center);\n         _Invert = EditorGUILayout.Toggle(\"Invert: \", _Invert);\n         _DistanceBetween = EditorGUILayout.FloatField(\"Distance Between: \", _DistanceBetween);\n         GUILayout.EndVertical();\n          GUILayout.BeginVertical(\"Box\");\n         if (GUILayout.Button(\"Create\"))         {             if (_CenterObj != null)             {                 if (_ObjSaved.Count > 0)                 {                     for (int i = 0; i < _ObjSaved.Count; i++)\n                     {                         DestroyImmediate(_ObjSaved[i]);\n                     }\n                     _ObjSaved.Clear();\n                 }\n                  Vector3 objPos = _CenterObj.transform.position;                 CreateHexagon(new Vector3(_HexLengthX, 0, _HexLengthZ)\n);\n                 SetParent();\n             }\n             else             {                 Debug.Log(\"Center Object not selected!\");\n             }\n         }\n          if (GUILayout.Button(\"Destroy\"))         {             if (_CenterObj != null)             {                 for (int i = 0; i < _ObjSaved.Count; i++)\n                 {                     DestroyImmediate(_ObjSaved[i]);\n                 }\n                 _ObjSaved.Clear();\n                   int childs = _CenterObj.transform.childCount;                 for (int i = childs -1; i >= 0; i--)\n                 {                     DestroyImmediate(_CenterObj.transform.GetChild(i)\n.gameObject);\n                 }\n             }\n             else             {                 Debug.Log(\"Center Object not selected!\");\n             }\n     }\n          if (GUILayout.Button(\"Confirm\"))         {             _ObjSaved.Clear();\n         }\n         GUILayout.EndVertical();\n     }\n      void CreateHexagon(Vector3 dimentsions)     {         Vector3 objPos = _CenterObj.transform.position;         if (_Center && !_Invert)\n         {             objPos.x -= dimentsions.x * 0.5f * 1.7321f * _HexSize;\n             objPos.z -= dimentsions.z * 0.5f * -1.5f * _HexSize;\n         }\n         if (_Center && _Invert)         {             objPos.x -= dimentsions.x * 0.5f * 1.7321f * _HexSize;\n             objPos.z += dimentsions.z * 0.5f * -1.5f * _HexSize;\n         }\n          for (int xas = 0; xas < dimentsions.x; xas++)\n         {             CreateHax(new Vector3(objPos.x + 1.7321f  * _HexSize * _DistanceBetween * xas, objPos.y, objPos.z));\n             for (int zas = 1; zas < dimentsions.z; zas++)\n             {                 float offset = 0;\n                 if (zas % 2 == 1)                 {                     offset = 0.86605f * _HexSize * _DistanceBetween;\n                 }\n                 else                 {                     offset = 0;\n                 }\n                 if (!_Invert)                 {                     CreateHax(new Vector3(objPos.x + 1.7321f * _HexSize * _DistanceBetween * xas - offset, objPos.y, objPos.z + -1.5f * _HexSize * _DistanceBetween * zas));\n                 }\n                 else                 {                     CreateHax(new Vector3(objPos.x + 1.7321f * _HexSize * _DistanceBetween * xas - offset, objPos.y, objPos.z + +1.5f * _HexSize * _DistanceBetween * zas));\n                 }\n             }\n         }\n     }\n     void CreateHax(Vector3 positions)     {         Vector3 objPos = _CenterObj.transform.position;          GameObject gridObj = GameObject.CreatePrimitive(PrimitiveType.Cube)\n;\n         gridObj.transform.position = new Vector3(positions.x, positions.y, positions.z)\n;\n          DestroyImmediate(gridObj.GetComponent<BoxCollider>());\n          float size = _HexSize;\n         float width = Mathf.Sqrt(3) * size;\n         float height = size * 2f;\n         Mesh mesh = new Mesh();\n         Vector3[] vertices = new Vector3[7];\n          for (int i = 0; i < 6; i++)\n         {             float angle_deg = 60 * i - 30;\n             float angle_rad = Mathf.Deg2Rad * angle_deg;\n              vertices[i + 1] = new Vector3(size * Mathf.Cos(angle_rad), 0f, size * Mathf.Sin(angle_rad));\n         }\n         mesh.vertices = vertices;\n          mesh.triangles = new int[]         {             2,1,0,             3,2,0,             4,3,0,             5,4,0,             6,5,0,             1,6,0         }\n;\n          Vector2[] uv = new Vector2[7];\n         for (int i = 0; i < 7; i++)\n         {             uv[i] = new Vector2(                 (vertices[i].x + -width * .5f) * .5f / size,                 (vertices[i].z + -height * .5f) * .5f / size);\n         }\n          mesh.uv = uv;\n         gridObj.GetComponent<MeshFilter>().sharedMesh = mesh;\n          _ObjSaved.Add(gridObj);\n     }\n      void SetParent()     {         for (int i = 0; i < _ObjSaved.Count; i++)\n         {             _ObjSaved[i].transform.parent = _CenterObj.transform;         }\n     }\n }",
"using System.Collections;\n using System.Collections.Generic;\n using System.Linq;\n using UnityEditor;\n using UnityEngine;\n  public class Tool_ScriptToString : EditorWindow {     string _ScriptInput = \"\";\n     string _ScriptOutput = \"\";\n      List<string> _CustomCommandCheckKeywords = new List<string>();\n     string _CustomCommandCheck;\n      private Vector2 _ScrollPos = new Vector2();\n      [MenuItem(\"Tools/Convert Script to String\")]     public static void ShowWindow()     {         EditorWindow.GetWindow(typeof(Tool_ScriptToString));\n     }\n      void OnGUI()     {         //Convert         \nif (GUILayout.Button(\"Convert\", GUILayout.Height(30)))         {             _ScriptOutput = ConvertScriptToString();\n         }\n          GUILayout.Space(20);\n          //Input         \nGUILayout.Label(\"Input: \", EditorStyles.boldLabel);\n         _ScriptInput = EditorGUILayout.TextField(\"\", _ScriptInput);\n          //Output         \nGUILayout.Label(\"Output: \", EditorStyles.boldLabel);\n         EditorGUILayout.TextField(\"\", _ScriptOutput);\n          GUILayout.Space(20);\n          GUILayout.Label(\"Use Custom Keywords to fix lines that should not be included into the commend. \n\" +             \"The x on the left shows the lines that contain a comment.\");\n          GUILayout.Space(20);\n          //Other Settings         \nGUILayout.Label(\"Custom Keywords: \", EditorStyles.boldLabel);\n         _CustomCommandCheck = EditorGUILayout.TextField(\"\", _CustomCommandCheck);\n         if (GUILayout.Button(\"AddKeyword\"))         {             _CustomCommandCheckKeywords.Add(_CustomCommandCheck);\n             _CustomCommandCheck = \"\";\n             _ScriptOutput = ConvertScriptToString();\n         }\n         for (int i = 0; i < _CustomCommandCheckKeywords.Count; i++)\n         {             GUILayout.BeginHorizontal(\"box\");\n             GUILayout.Label(_CustomCommandCheckKeywords[i]);\n             if (GUILayout.Button(\"Remove\", GUILayout.Width(100)))             {                 _CustomCommandCheckKeywords.RemoveAt(i);\n             }\n             GUILayout.EndHorizontal();\n         }\n          //Preview         \nList<string> output = new List<string>();\n         List<string> output2 = new List<string>();\n          for (int i = 0; i < _ScriptOutput.Length; i++)\n         {             output.Add(System.Convert.ToString(_ScriptOutput[i]));\n         }\n          int begincalc = 0;\n         int endcalc = 0;\n          for (int i = 0; i < output.Count; i++)\n         {             if(i + 1 < output.Count)             {                 if(output[i] + output[i + 1] == \"\\n\")                 {                     endcalc = i;\n                     string addstring = \"\";\n                     for (int j = 0; j < endcalc - begincalc; j++)\n                     {                         addstring += output[begincalc + j];\n                     }\n                     addstring += output[endcalc] + output[endcalc + 1];\n                      output2.Add(addstring);\n                     endcalc = endcalc + 1;\n                     begincalc = endcalc + 1;\n                 }\n             }\n         }\n          _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);\n          for (int i = 0; i < output2.Count; i++)\n         {             GUILayout.BeginHorizontal();\n             if(output2[i].Contains(\"//\"))             {                 Editor\nGUILayout.TextField(\"\", \"x\", GUILayout.MaxWidth(15));\n             }\n             else             {                 EditorGUILayout.TextField(\"\", \"\", GUILayout.MaxWidth(15));\n             }\n              EditorGUILayout.TextField(\"\", output2[i]);\n             GUILayout.EndHorizontal();\n         }\n                  EditorGUILayout.EndScrollView();\n     }\n      private string ConvertScriptToString()     {         _ScriptOutput = \"\";\n         string scriptasstring = \"\";\n          //Split / add to array         \nList<string> textedit = new List<string>();\n          for (int i = 0; i < _ScriptInput.Length; i++)\n         {             textedit.Add(System.Convert.ToString(_ScriptInput[i]));\n         }\n          bool headercheck = false;\n         bool forcheck = false;         bool commentcheck = false;          for (int i = 0; i < textedit.Count; i++)\n         {             //Header check             \nif (i + 7 < textedit.Count)             {                 if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] + textedit[i + 6] + textedit[i + 7] == \"[Header(\")                     headercheck = true;\n             }\n              //For check             \nif(i + 2 < textedit.Count)             {                 if(textedit[i]\n + textedit[i+1] + textedit[i + 2] == \"for\")\n                 {                     forcheck = true;                 }\n             }\n              //Comment check             \nif (i + 1 < textedit.Count)\n             {                 if (textedit[i] + textedit[i + 1] == \"//\" || textedit[i] + textedit[i + 1] == \"/*\")                     commentcheck = true;\n             }\n              //Comment /* + *\n/             if (commentcheck)             {                 if (textedit[i - 1] + textedit[i] == \"*/\")                 {                     scriptasstring += \"\\n\";\n                     commentcheck = false;\n                 }\n                  if (i + 6 < textedit.Count)                 {                     //\nif                     if (textedit[i] + textedit[i + 1] == \"if\")                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                     //\nfor                     if (textedit[i] + textedit[i + 1] + textedit[i + 2] == \"for\")\n                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                     //\nswitch                     if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] == \"switch\")                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                     //\ncase                     if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] == \"case\")                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                     //\npublic                     if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] == \"public\")                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                     //\nprivate                     if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] + textedit[i + 4] + textedit[i + 5] + textedit[i + 6] == \"private\")                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                     //\nvoid                     if (textedit[i] + textedit[i + 1] + textedit[i + 2] + textedit[i + 3] == \"void\")                     {                         scriptasstring += \"\\n\";\n                         commentcheck = false;\n                     }\n                 }\n                  for (int j = 0; j < _CustomCommandCheckKeywords.Count; j++)\n                 {                     if(_CustomCommandCheckKeywords[j].Length < textedit.Count)                     {                         string check = \"\";\n                          for (int o = 0; o < _CustomCommandCheckKeywords[j].Length; o++)\n                         {                             check += textedit[i + o];\n                         }\n                                                  if(check == _CustomCommandCheckKeywords[j])                         {                             scriptasstring += \"\\n\";\n                             commentcheck = false;\n                         }\n                     }\n                 }\n             }\n              scriptasstring += textedit[i];\n              //Endings check             \nif (i + 1 < textedit.Count)             {                 if (textedit[i + 1] == \"\")                 {                     scriptasstring += \"\\\";\n                 }\n                  if (textedit[i] == \"}\\n\")                 {    scriptasstring += \"\\n\";\n                 }\n                 if (textedit[i] == \";\\n\" && !forcheck)\n                 {        scriptasstring += \"\\n\";\n                 }\n                 if(textedit[i] == \"]\" && headercheck)                 {                     scriptasstring += \"\\n\";\n                     headercheck = false;\n                 }\n                 if (textedit[i] == \")\" && forcheck)\n                 {                     scriptasstring += \"\\n\";\n                     forcheck = false;                 }\n             }\n         }\n          scriptasstring += \"\";         return scriptasstring;     }\n }\n ",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n  public class Turret : MonoBehaviour {     [Header(\"Settings\")]\n     [SerializeField] private Vector2 _MinMaxRange = Vector2.zero;\n     [SerializeField] private float _SecondsBetweenShots = 2;\n     [SerializeField] private float _Damage = 25;\n     [SerializeField] private GameObject _ShootPart = null;\n     [SerializeField] private string _Tag = \"Enemy\";\n          private float _Timer;\n     private GameObject _Target;\n      void Update()     {         if (_Target != null)         {             _ShootPart.transform.LookAt(_Target.transform.position)\n;\n             _Timer += 1 * Time.deltaTime;\n             if (_Timer >= _SecondsBetweenShots)             {                 _Target.GetComponent<Health>().DoDamage(_Damage);\n                 _Timer = 0;\n             }\n         }\n         else         {             _ShootPart.transform.rotation = Quaternion.Euler(90, 0, 0)\n;\n         }\n          _Target = FindEnemy();\n     }\n      public GameObject FindEnemy()     {         GameObject[] m_Targets = GameObject.FindGameObjectsWithTag(_Tag);\n         GameObject closest = null;\n         float distance = Mathf.Infinity;\n         Vector3 position = transform.position;          _MinMaxRange.x = _MinMaxRange.x * _MinMaxRange.x;         _MinMaxRange.y = _MinMaxRange.y * _MinMaxRange.y;         foreach (GameObject target in m_Targets)\n         {             Vector3 diff = target.transform.position - position;             float curDistance = diff.sqrMagnitude;             if (curDistance < distance && curDistance >= _MinMaxRange.x && curDistance <= _MinMaxRange.y)\n             {                 closest = target;\n                 distance = curDistance;\n             }\n         }\n         return closest;\n     }\n }",
"using System.Collections;\n using System.Collections.Generic;\n using UnityEngine;\n using UnityEngine.EventSystems;\n  public class UIEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {     private enum UIEffectOptions { Grow, Shrink }\n     [SerializeField] private UIEffectOptions _UIEffect = UIEffectOptions.Grow;\n     [SerializeField] private Vector3 _MinDefaultMaxSize = new Vector3(0.9f,1f,1.1f);\n     [SerializeField] private float _IncreaseSpeed = 1;\n      private Vector3 _OriginalSize;\n     private bool _MouseOver;\n      void Start()     {         _OriginalSize = transform.localScale;     }\n      void Update()\n     {         switch (_UIEffect)         {             case UIEffectOptions.Grow:                 if (_MouseOver)                 {                     if (transform.localScale.y < _MinDefaultMaxSize.z)\n                         transform.localScale += new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed)\n * Time.deltaTime;\n                 }\n                 else                     if (transform.localScale.y > _OriginalSize.y)\n                     transform.localScale -= new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed)\n * Time.deltaTime;\n                 else                     transform.localScale = new Vector3(_OriginalSize.y, _OriginalSize.z, _OriginalSize.z)\n;\n                 break;\n             case UIEffectOptions.Shrink:                 if (_MouseOver)                 {                     if (transform.localScale.y > _MinDefaultMaxSize.x)\n                         transform.localScale -= new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed)\n * Time.deltaTime;\n                 }\n                 else                    if (transform.localScale.y < _OriginalSize.x)\n                     transform.localScale += new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed)\n * Time.deltaTime;\n                 else                     transform.localScale = new Vector3(_OriginalSize.x, _OriginalSize.y, _OriginalSize.z)\n;\n                 break;\n         }\n     }\n      public void OnPointerEnter(PointerEventData eventData)     {         _MouseOver = true;\n     }\n      public void OnPointerExit(PointerEventData eventData)     {         _MouseOver = false;\n     }\n }"
    };
    private string[] _ScriptTags = new string[] {
"Audio",
"AI",
"Shooting",
"Dialog",
"Event",
"Editor_Example",
"Enemy_Handler",
"Health_Shooting",
"Inventory",
"Light_Effects",
"Loading",
"Menu_Scene_Handler",
"Movement_2D",
"Movement_2D",
"Movement_Camera",
"Movement_CC_CharacterController",
"Movement_CC_CharacterController",
"Movement_CC",
"Movement_Camera",
"MVD_Namespace",
"ObjectPool",
"ObjectPool",
"OnCollision",
"Networking",
"Save_Load",
"Save_Load",
"ScriptebleGameObject",
"SettingsHandler",
"Shooting",
"Shooting_RayCast",
"String",
"Tool_Editor",
"Tool_Editor",
"Turret_Shooting",
"UI_Effects"
    };

    private int _CreateSceneOptions = 0;
    private Vector2 _ScrollPos = new Vector2();

    private string _Search_Script = "";
    private string _Search_Tag = "";

    //UI
    int _Options_Type;
    int _Options_Style;
    GameObject _CreatedCanvas;

    [MenuItem("Tools/Tool_QuickStart")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Tool_QuickStart));
    }

    void OnGUI()
    {
        //Menu Type
        _MenuID = GUILayout.Toolbar(_MenuID, new string[] { "QuickStart", "Scripts", "QuickUI" });

        if (_MenuID == 0)
            Menu_QuickStart();
        else if (_MenuID == 1)
            Menu_Scripts();
        else
            Menu_UI();
    }

    //UI
    void Menu_UI()
    {
        _Options_Type = GUILayout.Toolbar(_Options_Type, new string[] { "All", "Menu", "HUD" });

        if (_Options_Type == 0)
        {
            _Options_Style = GUILayout.Toolbar(_Options_Style, new string[] { "Default" });//, "CSGO", "Overwatch", "Minecraft", "RocketLeague" });
        }
        else
        {
            _Options_Style = GUILayout.Toolbar(_Options_Style, new string[] { "Default" });//, "CSGO", "Overwatch", "Minecraft", "RocketLeague" });
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create"))
            CreateUI();
        if (GUILayout.Button("Delete"))
            if (_CreatedCanvas != null)
            {
                DestroyImmediate(_CreatedCanvas);
                _CreatedCanvas = null;

            }
        GUILayout.EndHorizontal();

        GUILayout.Label("Info:");
        _CreatedCanvas = (GameObject)EditorGUILayout.ObjectField("Created object", _CreatedCanvas, typeof(GameObject), true);
    }

    //Menu QuickStart
    void Menu_QuickStart()
    {
        //Dimension
        _DimensionID = GUILayout.Toolbar(_DimensionID, new string[] { "2D", "3D" });
        _WithUI = GUILayout.Toolbar(_WithUI, new string[] { "No UI", "UI All", "UI Menu", "UI HUD" });

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
                ScriptStatus("Health");
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
                ScriptStatus("Health");
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
                ScriptStatus("Health");
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
                ScriptStatus("Health");
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


    //UI
    void CreateUI()
    {
        switch (_Options_Style)
        {
            case 0:
                CreateUI_Default();
                //CreateUI_HUD();
                break;
        }

        Set_SettingsHandler();
    }

    void CreateUIOptions()
    {
        switch(_Options_Style)
        {
            case 0: //All

                break;
            case 1: //Menu

                break;
            case 2: //HUD

                break;
        }
    }

    void CreateUI_Default()
    {
        //Add canvas
        GameObject canvasobj = CreateCanvas();

        //Add setting tabs
        GameObject tab_display = Create_Tab(canvasobj, "Display");
        GameObject tab_graphics = Create_Tab(canvasobj, "Graphics");
        GameObject tab_gameplay = Create_Tab(canvasobj, "Gameplay");
        GameObject tab_controls = Create_Tab(canvasobj, "Controls");
        GameObject tab_keybinding = Create_Tab(canvasobj, "Keybinding");

        //Add Buttons
        GameObject main = new GameObject();
        RectTransform mainrect = main.AddComponent<RectTransform>();
        SetRect(mainrect, "bottomleft");
        GameObject button_start = Create_Button(main, "Button_Start", "Start", new Vector2(40, 450), new Vector2(700, 100), 30, 60, "bottomleft");
        GameObject button_options = Create_Button(main, "Button_Options", "Options", new Vector2(40, 330), new Vector2(700, 100), 30, 60, "bottomleft");
        GameObject button_quit = Create_Button(main, "Button_Quit", "Quit", new Vector2(40, 210), new Vector2(700, 100), 30, 60, "bottomleft");
        main.name = "Main";
        main.transform.SetParent(canvasobj.transform);

        _CreatedCanvas = canvasobj;
    }
    void CreateUI_HUD()
    {
        GameObject hudobj = new GameObject();
        RectTransform hudobjrect = hudobj.AddComponent<RectTransform>();
        SetRect(hudobjrect, "bottomleft");
        hudobj.name = "HUD";


    }
    void CreateUI_PauzeMenu()
    {

    }

    GameObject CreateCanvas()
    {
        GameObject canvasobj = new GameObject();
        canvasobj.name = "TestCanvas";
        canvasobj.AddComponent<Canvas>();
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
    GameObject Create_Button(GameObject parentobj, string name, string buttontext, Vector2 pos, Vector2 size, float textoffset, float textsize, string anchorpos)
    {
        GameObject buttontemplate = new GameObject();
        RectTransform buttontransform = buttontemplate.AddComponent<RectTransform>();

        buttontransform.sizeDelta = size;
        buttontransform.anchoredPosition = pos;

        SetRect(buttontransform, anchorpos);

        buttontemplate.AddComponent<CanvasRenderer>();
        Image buttonimage = buttontemplate.AddComponent<Image>();
        buttonimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        buttonimage.type = Image.Type.Sliced;
        Button buttonbutton = buttontemplate.AddComponent<Button>();
        buttonbutton.targetGraphic = buttonimage;
        buttontemplate.name = name;

        GameObject buttontextemplate = new GameObject();
        RectTransform buttontextrect = buttontextemplate.AddComponent<RectTransform>();

        buttontextrect.anchoredPosition = new Vector2(pos.x + textoffset, pos.y);
        buttontextrect.sizeDelta = size;
        buttontextrect.pivot = new Vector2(0, 0);

        TextMeshProUGUI buttontexttmpro = buttontextemplate.AddComponent<TextMeshProUGUI>();
        buttontexttmpro.text = buttontext;
        buttontexttmpro.fontSize = textsize;
        buttontexttmpro.alignment = TextAlignmentOptions.MidlineLeft;
        buttontexttmpro.color = Color.black;

        buttontextemplate.name = name + "text";



        buttontextemplate.transform.SetParent(buttontemplate.transform);

        buttontemplate.transform.SetParent(parentobj.transform);

        return buttontemplate;
    }
    GameObject Create_Dropdown(GameObject parentobj, string name, string buttontext, Vector2 pos, Vector2 size, float textoffset, float textsize, string anchorpos)
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
        dropdownobjrect.sizeDelta = size;
        dropdownobjrect.anchoredPosition = pos;
        Image dropdownimage = dropdownobj.AddComponent<Image>();
        dropdownimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        dropdownimage.type = Image.Type.Sliced;
        TMP_Dropdown dropdowntmp = dropdownobj.AddComponent<TMP_Dropdown>();
        SetRect(dropdownobjrect, anchorpos);
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
        dropdown_labelrect.sizeDelta = new Vector2(25, 6);
        dropdown_labelrect.anchoredPosition = new Vector4(10, 7);

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
        dropdown_viewportrect.sizeDelta = new Vector2(18, 15); //NotDy
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
        dropdown_itemrect.sizeDelta = new Vector2(0, size.y);
        dropdown_itemrect.anchoredPosition = new Vector4(0, -15); //NotDy

        //Set Rect Item Background
        dropdown_item_backgroundrect.anchorMin = new Vector2(0,0);
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
        dropdownobj.transform.SetParent(parentobj.transform);

        dropdowntmp.captionText = dropdown_labeltext;

        return dropdownobj;
    }
    GameObject Create_Text(GameObject parentobj, string name, Vector2 pos, Vector2 size, string textcontent, float fontsize, Color textcolor)
    {
        GameObject newtextobj = new GameObject();
        RectTransform buttontextrect = newtextobj.AddComponent<RectTransform>();
        newtextobj.name = name;
        buttontextrect.sizeDelta = size;
        buttontextrect.anchoredPosition = pos;
        SetRect(buttontextrect, "bottomleft");

        TextMeshProUGUI newtext = newtextobj.AddComponent<TextMeshProUGUI>();
        newtext.text = textcontent;
        newtext.fontSize = fontsize;
        newtext.alignment = TextAlignmentOptions.MidlineLeft;
        newtext.color = textcolor;

        newtext.transform.SetParent(parentobj.transform);
        return newtextobj;
    }
    GameObject Create_Slider(GameObject parentobj, string name, string buttontext, Vector2 pos, Vector2 size, float textoffset, float textsize, string anchorpos)
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
        RectTransform buttonobjrect = newsliderobj.AddComponent<RectTransform>();
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
        buttonobjrect.sizeDelta = size;
        buttonobjrect.anchoredPosition = pos;
        SetRect(buttonobjrect, anchorpos);
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

        newsliderobj.transform.SetParent(parentobj.transform);

        return newsliderobj;
    }
    GameObject Create_Tab(GameObject parentobj, string name)
    {
        GameObject tab_new = new GameObject();
        RectTransform tab_newrect = tab_new.AddComponent<RectTransform>();
        SetRect(tab_newrect, "bottomleft");
        tab_new.name = name;

        GameObject textobj = Create_Text(tab_new, "Title_" + name, new Vector2(800, 800), new Vector2(1000, 200), name, 100, Color.white);

        switch (name)
        {
            case "Display":
                GameObject button_Resolution = Create_Dropdown(tab_new, "Dropdown_Resolution", "Resolution", new Vector2(800, 700), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_Fullscreen = Create_Button(tab_new, "Button_Fullscreen", "Fullscreen", new Vector2(800, 630), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_Quality = Create_Dropdown(tab_new, "Dropdown_Quality", "Quality", new Vector2(800, 560), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_VSync = Create_Button(tab_new, "Button_VSync", "VSync", new Vector2(800, 490), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_MaxFPS = Create_Button(tab_new, "Button_MaxFPS", "MaxFPS", new Vector2(800, 420), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_Gamma = Create_Button(tab_new, "Button_Gamma", "Gamma", new Vector2(800, 350), new Vector2(500, 60), 10, 40, "bottomleft");
                // Resolution
                // Fullscreen
                // Quality
                // V-Sync
                // Max fps
                // Gamma
                break;
            case "Graphics":
                GameObject button_Antialiasing = Create_Dropdown(tab_new, "Dropdown_Antialiasing", "Antialiasing", new Vector2(800, 700), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_Shadows = Create_Button(tab_new, "Button_Shadows", "Shadows", new Vector2(800, 630), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_ViewDistance = Create_Button(tab_new, "Button_ViewDistance", "ViewDistance", new Vector2(800, 560), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_TextureQuality = Create_Button(tab_new, "Dropdown_TextureQuality", "TextureQuality", new Vector2(800, 490), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_ViolageDistance = Create_Button(tab_new, "Button_ViolageDistance", "ViolageDistance", new Vector2(800, 420), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_ViolageDensity = Create_Button(tab_new, "Button_ViolageDensity", "ViolageDensity", new Vector2(800, 350), new Vector2(500, 60), 10, 40, "bottomleft");
                // Antialiasing
                // Shadows
                // ViewDistance
                // TextureQuality
                // ViolageDistance
                // ViolageDensity
                break;
            case "Gameplay":
                GameObject button_SoundEffects = Create_Button(tab_new, "Button_SoundEffects", "SoundEffects", new Vector2(800, 700), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_Music = Create_Slider(tab_new, "Slider_Music", "Music", new Vector2(800, 630), new Vector2(500, 60), 10, 40, "bottomleft");
                // SoundEffects
                // Music
                break;
            case "Controls":

                break;
            case "Keybinding":

                break;
        }


        tab_new.transform.SetParent(parentobj.transform);
        tab_new.SetActive(false);
        return tab_new;
    }

    void SetRect(RectTransform rect, string anchorpos)
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
}