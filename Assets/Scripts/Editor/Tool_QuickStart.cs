using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Tool_QuickStart : EditorWindow
{
    //Version
    string _Version = "V1.2.0";

    //Navigation Tool
    int _MenuID = 0;        // QuickStart/Scripts/QuickUI/Scene
    int _DimensionID = 0;   // 2D/3D
    int _Type2DID = 0;      // Platformer/TopDown/VisualNovel
    int _Type3DID = 0;      // FPS/ThirdPerson/TopDown/Platformer
    bool _SelectWindow = false;

    //Navigation Tool Windows
    int _WindowID = 0;      // Default/UpdateLog/FileFinder/ScriptToString/MapEditor
    string[] _WindowNames = new string[] {"Home","Update Log","FileFinder","ScriptToString","MapEditor" };

    //Scripts
    Tool_QuickStart_Script[] QuickStart_Scripts = new Tool_QuickStart_Script[] {
        //                              NAME                      TAGS                           STATE          DESCRIPTION/CODE
        new Tool_QuickStart_Script("AudioHandler",              "Audio_Handler",                "stable",           "Play an audiotrack from any script as long as this script exists in the scene\n\nCompatible Scripts:\nAudioZone.cs",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine.SceneManagement;\nusing UnityEngine;\nusing UnityEngine.Audio;\n\npublic class AudioHandler : MonoBehaviour\n{\n    [Header(\"Settings\")]\n    [Tooltip(\"Only used for testing disable it for the final build to improve performance\")]\n    [SerializeField] private bool _RefreshSettingsOnUpdate = false;\n\n    [Header(\"AudioMixer/Audio\")]\n    [SerializeField] private AudioMixerGroup _AudioMixer = null;\n    [SerializeField] private List<AudioHandler_Sound> _Sound = new List<AudioHandler_Sound>();\n\n    private string _CurrentScene;\n\n    //You can call AudioHandler.AUDIO from every script as long as you have the script in the scene.\n    public static AudioHandler AUDIO;\n\n    void Awake()\n    {\n        AUDIO = this;\n\n        //PlayOnStart\n        for (int i = 0; i < _Sound.Count; i++)\n        {\n            //AudioSource\n            if (_Sound[i].Settings.CreateAudioSource)\n            {\n                //3D Space\n                if (_Sound[i].Audio3D.Enable3DAudio)\n                {\n                    //Create new object\n                    GameObject audiopos = new GameObject(\"Audio_\" + _Sound[i].AudioTrackName);\n\n                    //Set audiopos position\n                    if (_Sound[i].Audio3D.SpatialTransform != null)\n                        audiopos.transform.position = _Sound[i].Audio3D.SpatialTransform.position;\n                    else\n                        audiopos.transform.position = _Sound[i].Audio3D.SpatialPosition;\n                    audiopos.transform.parent = this.gameObject.transform;\n\n                    //Add AudioSource to audioposition\n                    _Sound[i].Settings.AudioSource = audiopos.AddComponent<AudioSource>();\n                }\n                else\n                    _Sound[i].Settings.AudioSource = this.gameObject.AddComponent<AudioSource>();\n\n                //SetVolume\n                _Sound[i].Settings.AudioSource.volume = _Sound[i].AudioSettings.Volume;\n\n                //AudioMixer\n                _Sound[i].Settings.AudioSource.outputAudioMixerGroup = _AudioMixer;\n\n                //AudioGroup\n                if (_Sound[i].Settings.AudioGroup != null)\n                    _Sound[i].Settings.AudioSource.outputAudioMixerGroup = _Sound[i].Settings.AudioGroup;\n            }\n\n            //3D Space Settings\n            if (_Sound[i].Audio3D.Enable3DAudio)\n            {\n                _Sound[i].Settings.AudioSource.spatialBlend = 1;\n            }\n\n            //AudioClip\n            _Sound[i].Settings.AudioSource.clip = _Sound[i].Settings.AudioClip;\n\n            //Settings\n            if (!_Sound[i].AudioSettings.PlayOnStart_DiplicateOnly)\n            {\n                if (_Sound[i].AudioSettings.PlayOnStart)\n                {\n                    _Sound[i].Settings.AudioSource.playOnAwake = _Sound[i].AudioSettings.PlayOnStart;\n                    _Sound[i].Settings.AudioSource.Play();\n                }\n                if (_Sound[i].AudioEffects.FadeIn)\n                {\n                    _Sound[i].Settings.AudioSource.volume = 1;\n                    _Sound[i].AudioEffects.FadeInSpeed = _Sound[i].AudioSettings.Volume / _Sound[i].AudioEffects.FadeInDuration;\n                }\n                if (_Sound[i].AudioEffects.FadeOut)\n                {\n                    _Sound[i].AudioEffects.FadeOutSpeed = _Sound[i].AudioSettings.Volume / _Sound[i].AudioEffects.FadeOutDuration;\n                }\n            }\n        }\n\n        RefreshSettings();\n    }\n\n    void Update()\n    {\n        CheckNewScene();\n\n        if (_RefreshSettingsOnUpdate)\n            RefreshSettings();\n\n        for (int i = 0; i < _Sound.Count; i++)\n        {\n            //FadeIn\n            if (_Sound[i].AudioEffects.FadingIn)\n            {\n                if (_Sound[i].AudioEffects.FadeIn && !_Sound[i].AudioEffects.FadeInDone)\n                {\n                    if (_Sound[i].Settings.AudioSource.volume < _Sound[i].AudioSettings.Volume)\n                    {\n                        _Sound[i].Settings.AudioSource.volume += _Sound[i].AudioEffects.FadeInSpeed * Time.deltaTime;\n                    }\n                    else\n                    {\n                        _Sound[i].AudioEffects.FadeInDone = true;\n                        _Sound[i].Settings.AudioSource.volume = _Sound[i].AudioSettings.Volume;\n                    }\n                }\n            }\n            //FadeOut\n            if (_Sound[i].AudioEffects.FadingOut)\n            {\n                if (_Sound[i].AudioEffects.FadeOutAfterTime > -0.1f)\n                {\n                    _Sound[i].AudioEffects.FadeOutAfterTime -= 1 * Time.deltaTime;\n                }\n                else\n                {\n                    if (_Sound[i].AudioEffects.FadeOut && !_Sound[i].AudioEffects.FadeOutDone)\n                    {\n                        if (_Sound[i].Settings.AudioSource.volume > 0)\n                        {\n                            _Sound[i].Settings.AudioSource.volume -= _Sound[i].AudioEffects.FadeOutSpeed * Time.deltaTime;\n                        }\n                        else\n                        {\n                            _Sound[i].AudioEffects.FadeOutDone = true;\n                            _Sound[i].Settings.AudioSource.volume = 0;\n                            _Sound[i].Settings.AudioSource.Stop();\n                        }\n                    }\n                }\n            }\n        }\n    }\n\n    private void CheckNewScene()\n    {\n        if (_CurrentScene != SceneManager.GetActiveScene().name)\n        {\n            _CurrentScene = SceneManager.GetActiveScene().name;\n            for (int i = 0; i < _Sound.Count; i++)\n            {\n                for (int o = 0; o < _Sound[i].AudioControl.StartAudioOnScene.Count; o++)\n                {\n                    if (_Sound[i].AudioControl.StartAudioOnScene[o] == _CurrentScene)\n                    {\n                        //FadeIn\n                        if (_Sound[i].AudioEffects.FadeIn)\n                        {\n                            _Sound[i].AudioEffects.FadingOut = false;\n                            _Sound[i].AudioEffects.FadeInDone = false;\n                            _Sound[i].AudioEffects.FadingIn = true;\n                        }\n                        _Sound[i].Settings.AudioSource.Play();\n                    }\n                }\n                for (int o = 0; o < _Sound[i].AudioControl.StopAudioOnScene.Count; o++)\n                {\n                    if (_Sound[i].AudioControl.StopAudioOnScene[o] == _CurrentScene)\n                    {\n                        //FadeOut\n                        if (_Sound[i].AudioEffects.FadeOut && !_Sound[i].AudioEffects.FadingOut)\n                        {\n                            _Sound[i].AudioEffects.FadingIn = false;\n                            _Sound[i].AudioEffects.FadeOutDone = false;\n                            _Sound[i].AudioEffects.FadingOut = true;\n                        }\n                        else\n                            _Sound[i].Settings.AudioSource.Stop();\n                    }\n                }\n            }\n        }\n    }\n    private void AudioHandler_PlayTrack(int trackid)\n    {\n        _Sound[trackid].Settings.AudioSource.Play();\n    }\n\n    /// <summary>Plays the audiotrack.</summary>\n    public void PlayTrack(string trackname)\n    {\n        for (int i = 0; i < _Sound.Count; i++)\n        {\n            if (_Sound[i].AudioTrackName == trackname)\n                AudioHandler_PlayTrack(i);\n        }\n    }\n    public void PlayTrack(int trackid)\n    {\n        AudioHandler_PlayTrack(trackid);\n    }\n\n    /// <summary>Plays the audiotrack if it's not playing yet.</summary>\n    public void StartTrack(string trackname)\n    {\n        for (int i = 0; i < _Sound.Count; i++)\n        {\n            if (_Sound[i].AudioTrackName == trackname)\n                if (!_Sound[i].Settings.AudioSource.isPlaying)\n                    AudioHandler_PlayTrack(i);\n        }\n    }\n    public void StartTrack(int trackid)\n    {\n        if (!_Sound[trackid].Settings.AudioSource.isPlaying)\n            AudioHandler_PlayTrack(trackid);\n    }\n\n    /// <summary>Stops the audiotrack.</summary>\n    public void StopTrack(string trackname)\n    {\n        for (int i = 0; i < _Sound.Count; i++)\n        {\n            if (_Sound[i].AudioTrackName == trackname)\n                _Sound[i].Settings.AudioSource.Stop();\n        }\n    }\n    public void StopTrack(int trackid)\n    {\n        _Sound[trackid].Settings.AudioSource.Stop();\n    }\n\n    /// <summary>Returns audio file name.</summary>\n    public string Get_Track_AudioFileName(string trackname)\n    {\n        for (int i = 0; i < _Sound.Count; i++)\n        {\n            if (_Sound[i].AudioTrackName == trackname)\n                return _Sound[i].Settings.AudioClip.name;\n        }\n        return \"No AudioClip detected\";\n    }\n    public string Get_Track_AudioFileName(int trackid)\n    {\n        return _Sound[trackid].Settings.AudioClip.name;\n    }\n\n    /// <summary>Set audiosource.</summary>\n    public void SetAudioSource(string trackname, AudioSource audiosource)\n    {\n        for (int i = 0; i < _Sound.Count; i++)\n        {\n            if (_Sound[i].AudioTrackName == trackname)\n                _Sound[i].Settings.AudioSource = audiosource;\n        }\n    }\n    public void SetAudioSource(int trackid, AudioSource audiosource)\n    {\n        _Sound[trackid].Settings.AudioSource = audiosource;\n    }\n\n    /// <summary>Set track volume.</summary>\n    public void SetTrackVolume(string trackname, float volume, bool checkmaxvolume)\n    {\n        for (int i = 0; i < _Sound.Count; i++)\n        {\n            if (_Sound[i].AudioTrackName == trackname)\n            {\n                if (!checkmaxvolume)\n                    _Sound[i].AudioSettings.Volume = volume;\n                else\n                    if (volume >= _Sound[i].AudioSettings.MaxVolume)\n                    _Sound[i].AudioSettings.Volume = _Sound[i].AudioSettings.MaxVolume;\n                else\n                    _Sound[i].AudioSettings.Volume = volume;\n                break;\n            }\n        }\n    }\n    public void SetTrackVolume(int trackid, float volume, bool checkmaxvolume)\n    {\n        if (!checkmaxvolume)\n            _Sound[trackid].AudioSettings.Volume = volume;\n        else if (volume >= _Sound[trackid].AudioSettings.MaxVolume)\n            _Sound[trackid].AudioSettings.Volume = _Sound[trackid].AudioSettings.MaxVolume;\n        else\n            _Sound[trackid].AudioSettings.Volume = volume;\n    }\n\n    /// <summary>Returns track id.</summary>\n    public int Get_Track_ID(string trackname)\n    {\n        for (int i = 0; i < _Sound.Count; i++)\n        {\n            if (_Sound[i].AudioTrackName == trackname)\n                return i;\n        }\n        return -1;\n    }\n\n    /// <summary>Refresh settings.</summary>\n    public void RefreshSettings()\n    {\n        for (int i = 0; i < _Sound.Count; i++)\n        {\n            //SetClip\n            if (_Sound[i].Settings.AudioSource.clip != _Sound[i].Settings.AudioClip)\n                _Sound[i].Settings.AudioSource.clip = _Sound[i].Settings.AudioClip;\n            //SetEffects\n            if (!_Sound[i].AudioEffects.FadeIn || _Sound[i].AudioEffects.FadeIn && _Sound[i].AudioEffects.FadeInDone)\n                _Sound[i].Settings.AudioSource.volume = _Sound[i].AudioSettings.Volume;\n            _Sound[i].Settings.AudioSource.loop = _Sound[i].AudioSettings.Loop;\n        }\n    }\n\n    /// <summary>Duplicate AudioTrack.</summary>\n    public string DuplicateAudioTrack(string trackname)\n    {\n        int audioid = Get_Track_ID(trackname);\n        if (audioid == -1)\n            return null;\n\n        AudioHandler_Sound newsound = new AudioHandler_Sound();\n        GameObject newaudiopos = new GameObject();\n\n        newsound.AudioTrackName = \"Audio_\" + _Sound[audioid].AudioTrackName;\n\n        //Settings\n        newsound.Settings = new AudioHandler_Settings();\n        newsound.Settings.AudioClip = _Sound[audioid].Settings.AudioClip;\n        newsound.Settings.AudioGroup = _Sound[audioid].Settings.AudioGroup;\n        newsound.Settings.AudioSource = newaudiopos.AddComponent<AudioSource>();\n        newsound.Settings.CreateAudioSource = _Sound[audioid].Settings.CreateAudioSource;\n\n        //Control\n        newsound.AudioControl = new AudioHandler_Control();\n        newsound.AudioControl.SceneEnabled = _Sound[audioid].AudioControl.SceneEnabled;\n        newsound.AudioControl.StartAudioOnScene = _Sound[audioid].AudioControl.StartAudioOnScene;\n        newsound.AudioControl.StopAudioOnScene = _Sound[audioid].AudioControl.StopAudioOnScene;\n        newsound.AudioControl.StopOnNextScene = _Sound[audioid].AudioControl.StopOnNextScene;\n\n        //Audio3D\n        newsound.Audio3D = new AudioHandler_3DAudio();\n        newsound.Audio3D.Enable3DAudio = _Sound[audioid].Audio3D.Enable3DAudio;\n        newsound.Audio3D.SpatialPosition = _Sound[audioid].Audio3D.SpatialPosition;\n        newsound.Audio3D.SpatialTransform = _Sound[audioid].Audio3D.SpatialTransform;\n\n        //AudioSettings\n        newsound.AudioSettings = new AudioHandler_AudioSettings();\n        newsound.AudioSettings.Loop = _Sound[audioid].AudioSettings.Loop;\n        newsound.AudioSettings.MaxVolume = _Sound[audioid].AudioSettings.MaxVolume;\n        newsound.AudioSettings.PlayOnStart = _Sound[audioid].AudioSettings.PlayOnStart;\n        newsound.AudioSettings.Volume = _Sound[audioid].AudioSettings.Volume;\n\n        //AudioEffect\n        newsound.AudioEffects = new AudioHandler_Effects();\n        newsound.AudioEffects.FadeIn = _Sound[audioid].AudioEffects.FadeIn;\n        newsound.AudioEffects.FadeInDone = _Sound[audioid].AudioEffects.FadeInDone;\n        newsound.AudioEffects.FadeInDuration = _Sound[audioid].AudioEffects.FadeInDuration;\n        newsound.AudioEffects.FadeInSpeed = _Sound[audioid].AudioEffects.FadeInSpeed;\n        newsound.AudioEffects.FadeOut = _Sound[audioid].AudioEffects.FadeOut;\n        newsound.AudioEffects.FadeOutAfterTime = _Sound[audioid].AudioEffects.FadeOutAfterTime;\n        newsound.AudioEffects.FadeOutDone = _Sound[audioid].AudioEffects.FadeOutDone;\n        newsound.AudioEffects.FadeOutDuration = _Sound[audioid].AudioEffects.FadeOutDuration;\n        newsound.AudioEffects.FadeOutSpeed = _Sound[audioid].AudioEffects.FadeOutSpeed;\n        newsound.AudioEffects.FadingIn = _Sound[audioid].AudioEffects.FadingIn;\n        newsound.AudioEffects.FadingOut = _Sound[audioid].AudioEffects.FadingOut;\n\n        newsound.AudioTrackName += \"_\" + _Sound.Count.ToString();\n\n        //Activate Settings\n        newsound.Settings.AudioSource.loop = newsound.AudioSettings.Loop;\n        newsound.Settings.AudioSource.volume = newsound.AudioSettings.Volume;\n        if (newsound.Audio3D.Enable3DAudio)\n            newsound.Settings.AudioSource.spatialBlend = 1;\n        if (newsound.AudioSettings.PlayOnStart)\n            newsound.Settings.AudioSource.Play();\n\n        //Create new object\n        newaudiopos.transform.name = newsound.AudioTrackName;\n\n        //Audio Source Settings\n        newsound.Settings.AudioSource.clip = newsound.Settings.AudioClip;\n        newsound.Settings.AudioSource.outputAudioMixerGroup = newsound.Settings.AudioGroup;\n\n        //Position\n        if (newsound.Audio3D.SpatialTransform != null)\n            ChangeAudioPosition(newsound.AudioTrackName, newsound.Audio3D.SpatialTransform.position);\n        else\n            ChangeAudioPosition(newsound.AudioTrackName, newsound.Audio3D.SpatialPosition);\n\n        //PlayOnStart\n        if (newsound.AudioSettings.PlayOnStart)\n            newsound.Settings.AudioSource.Play();\n\n        //Apply\n        newaudiopos.transform.parent = this.transform;\n        _Sound.Add(newsound);\n        return newsound.AudioTrackName;\n    }\n\n    /// <summary>Change AudioSource Position.</summary>\n    public void ChangeAudioPosition(string trackname, Vector3 newpos)\n    {\n        int audioid = Get_Track_ID(trackname);\n        if (audioid != -1)\n            _Sound[audioid].Settings.AudioSource.transform.position = newpos;\n    }\n}\n\n[System.Serializable]\npublic class AudioHandler_Sound\n{\n    public string AudioTrackName;\n    public AudioHandler_Settings Settings;\n    public AudioHandler_AudioSettings AudioSettings;\n    public AudioHandler_3DAudio Audio3D;\n    public AudioHandler_Control AudioControl;\n    public AudioHandler_Effects AudioEffects;\n}\n\n[System.Serializable]\npublic class AudioHandler_Settings\n{\n    [Header(\"AudioClip/AudioMixerGroup\")]\n    public AudioClip AudioClip;\n    public AudioMixerGroup AudioGroup;\n\n    [Header(\"AudioSource\")]\n    public AudioSource AudioSource;\n    public bool CreateAudioSource;\n}\n\n[System.Serializable]\npublic class AudioHandler_AudioSettings\n{\n    [Header(\"AudioSettings\")]\n    [Range(0, 1)] public float Volume = 1;\n    [Range(0, 1)] public float MaxVolume = 1;\n    public bool Loop;\n    public bool PlayOnStart;\n    public bool PlayOnStart_DiplicateOnly;\n}\n\n[System.Serializable]\npublic class AudioHandler_Control\n{\n    [Header(\"Enable/Disable Song\")]\n    public List<string> StartAudioOnScene = new List<string>();\n    public List<string> StopAudioOnScene = new List<string>();\n    public bool StopOnNextScene;\n    [HideInInspector] public int SceneEnabled;\n}\n\n[System.Serializable]\npublic class AudioHandler_Effects\n{\n    [Header(\"FadeIn\")]\n    public bool FadeIn;\n    public float FadeInDuration;\n    [HideInInspector] public float FadeInSpeed;\n    [HideInInspector] public bool FadeInDone;\n    [HideInInspector] public bool FadingIn;\n    [Header(\"FadeOut\")]\n    public bool FadeOut;\n    public float FadeOutAfterTime;\n    public float FadeOutDuration;\n    [HideInInspector] public float FadeOutSpeed;\n    [HideInInspector] public bool FadeOutDone;\n    [HideInInspector] public bool FadingOut;\n}\n\n[System.Serializable]\npublic class AudioHandler_3DAudio\n{\n    [Header(\"3D Space / (0,0,0)+null = this object position\")]\n    public bool Enable3DAudio;\n    public Vector3 SpatialPosition;\n    public Transform SpatialTransform;\n}\n"),
        new Tool_QuickStart_Script("AudioZone",                 "Audio_Zone_Handler",           "stable",           "!This script needs AudioHandler.cs to work!\n\nUse AudioZone.cs to place/play audio in 3d space\n\nCompatible Scripts:\nAudioZoneEditor.cs",        "using UnityEngine;\n\npublic class AudioZone : MonoBehaviour\n{\n    private enum Options { SetVolume, VolumeOnDistance };\n    [Header(\"Type\")]\n    [SerializeField] private Options _Option = Options.SetVolume;\n\n    [Header(\"Target\")]\n    [SerializeField] private Transform _ZoneEffector = null;\n\n    [Header(\"Settings - Zone\")]\n    [SerializeField] private string _AudioTrackName = \"\";\n    [SerializeField] private float _Volume = 1;\n    public float Range = 10;\n    [Tooltip(\"1 = volume from 0 to max based on how close the effector is to the center.\")]\n    [SerializeField] private float _IncreaseMultiplier = 1;\n\n    [Header(\"3D Audio\")]\n    [SerializeField] private bool _Use3DAudio = true;\n    [SerializeField] private bool _UseThisPos = true;\n    [SerializeField] private bool _CreateNewAudioSource = true;\n\n    // Check effector leaving bounds\n    private bool _EffectorInBounds;\n\n    // Optimization (This way the AudioHandler doesn't have too loop trough the available audiotracks)\n    private int _AudioTrackID;\n\n    // Max distance\n    private float _MaxDistance;\n\n    void Start()\n    {\n        if (AudioHandler.AUDIO != null)\n        {\n            //3D Audio\n            if (_Use3DAudio)\n            {\n                if (_CreateNewAudioSource)\n                    _AudioTrackName = AudioHandler.AUDIO.DuplicateAudioTrack(_AudioTrackName);\n\n                if (_UseThisPos)\n                    AudioHandler.AUDIO.ChangeAudioPosition(_AudioTrackName, transform.position);\n            }\n\n            if (_ZoneEffector == null)\n            {\n                try { \n                    _ZoneEffector = GameObject.FindObjectsOfType<AudioListener>()[0].gameObject.transform; \n                }\n                catch { Debug.Log(\"No AudioListener Found In The Scene\"); }\n            }\n\n            // Get TrackID\n            _AudioTrackID = AudioHandler.AUDIO.Get_Track_ID(_AudioTrackName);\n            if (_AudioTrackID == -1)\n                Debug.Log(\"AudioZone: Track(\" + _AudioTrackName + \") Does not Exist\");\n\n            // Set max distance\n            _MaxDistance = Range;\n        }\n        else\n        {\n            _AudioTrackID = -1;\n            Debug.Log(\"AudioZone: AudioHandler does not exist in scene\");\n        }\n    }\n\n    void Update()\n    {\n        if (_AudioTrackID == -1)\n            return;\n        if (Vector3.Distance(transform.position,_ZoneEffector.position) <= _MaxDistance)\n        {\n            switch (_Option)\n            {\n                case Options.SetVolume:\n                    AudioHandler.AUDIO.SetTrackVolume(_AudioTrackID, _Volume, true);\n                    break;\n                case Options.VolumeOnDistance:\n                    float distance = Vector3.Distance(transform.position, _ZoneEffector.position);\n                    float newvolume = (1 - (distance / _MaxDistance)) * _Volume * _IncreaseMultiplier;\n                    AudioHandler.AUDIO.SetTrackVolume(_AudioTrackID, newvolume, true);\n                    break;\n            }\n\n            // Check Effector OnExit\n            if (!_EffectorInBounds)\n                _EffectorInBounds = true;\n        }\n        else\n        {\n            // Effector OnExit\n            if (_EffectorInBounds)\n            {\n                AudioHandler.AUDIO.SetTrackVolume(_AudioTrackID, 0, true);\n                _EffectorInBounds = false;\n            }\n        }\n    }\n\n    public void PlayTrack()\n    {\n        AudioHandler.AUDIO.PlayTrack(_AudioTrackName);\n    }\n    public void StartTrack()\n    {\n        AudioHandler.AUDIO.StartTrack(_AudioTrackName);\n    }\n\n    private void OnDrawGizmos()\n    {\n        Gizmos.color = new Vector4(0, 1f, 0, 0.1f);\n        Gizmos.DrawSphere(transform.position, Range);\n    }\n}\n"),
        new Tool_QuickStart_Script("AudioZoneEditor",           "Audio_Zone_Handler_Editor",    "stable",           "!Editor script for AudioZone.cs!\n\nEdit the audiozone's easily in the editorscene",        "using UnityEditor;\nusing UnityEditor.IMGUI.Controls;\nusing UnityEngine;\n\n[CustomEditor(typeof(AudioZone)), CanEditMultipleObjects]\npublic class AudioZoneEditor : Editor\n{\n    private SphereBoundsHandle _BoundsHandle = new SphereBoundsHandle();\n\n    protected virtual void OnSceneGUI()\n    {\n        AudioZone audiozone = (AudioZone)target;\n\n        _BoundsHandle.center = audiozone.transform.position;\n        _BoundsHandle.radius = audiozone.Range;\n\n        EditorGUI.BeginChangeCheck();\n        _BoundsHandle.DrawHandle();\n        if (EditorGUI.EndChangeCheck())\n        {\n            Undo.RecordObject(audiozone, \"Change Bounds\");\n\n            BoundingSphere newBounds = new BoundingSphere();\n            newBounds.position = audiozone.transform.position;\n            newBounds.radius = _BoundsHandle.radius;\n\n            audiozone.Range = _BoundsHandle.radius;\n        }\n\n    }\n}\n"),
        new Tool_QuickStart_Script("BasicNavMeshAI",            "AI_NavMesh",                   "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEngine.AI;\n\npublic class BasicNavMeshAI : MonoBehaviour\n{\n    [Header(\"Settings\")]\n    [SerializeField] private Transform _Target = null;\n    [SerializeField] private float _Speed = 2;\n\n    private NavMeshAgent _Agent;\n\n    private void Awake()\n    {\n        if (_Target == null)\n        {\n            try\n            {\n                _Target = GameObject.Find(\"Player\").transform;\n            }\n            catch\n            {\n                Debug.Log(\"No Target\");\n            }\n        }\n\n        _Agent = GetComponent<NavMeshAgent>();\n        _Agent.speed = _Speed;\n    }\n\n    private void Update()\n    {\n        if (_Target != null)\n        {\n            _Agent.SetDestination(_Target.position);\n        }\n    }\n}\n"),
        new Tool_QuickStart_Script("Bullet",                    "Shooting_Bullet",              "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Bullet : MonoBehaviour\n{\n    [SerializeField] private float _Speed = 5;\n    [SerializeField] private float _Damage = 25;\n\n    void FixedUpdate()\n    {\n        transform.Translate(Vector3.forward * _Speed * Time.deltaTime);\n    }\n\n    private void OnTriggerEnter(Collider other)\n    {\n        if(other.tag == \"Enemy\")\n        {\n            other.GetComponent<Health>().DoDamage(_Damage);\n            gameObject.SetActive(false);\n        }\n        if(other.tag == \"Wall\")\n        {\n            gameObject.SetActive(false);\n        }\n    }\n}\n"),
        new Tool_QuickStart_Script("CarArcade",                 "Car_Drive_Vehicle",            "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class CarArcade : MonoBehaviour\n{\n    [Header(\"Settings\")]\n    [SerializeField] private float _ForwardAccel = 8;\n    [SerializeField] private float _ReverseAccel = 4;\n    [SerializeField] private float _TurnStrength = 180;\n    [SerializeField] private float _GravityForce = 15;\n\n    [Header(\"GroundCheck\")]\n    [SerializeField] private LayerMask _GroundMask = ~0;\n    [SerializeField] private float _GroundCheckLength = 0.5f;\n\n    [Header(\"RigidBody\")]\n    [SerializeField] private Rigidbody _RB = null;\n\n    private float _SpeedInput;\n    private float _TurnInput;\n    private bool _Grounded;\n\n    void Start() => _RB.transform.parent = null;\n\n    void Update()\n    {\n        _SpeedInput = 0;\n        if(Input.GetAxis(\"Vertical\") > 0)\n            _SpeedInput = Input.GetAxis(\"Vertical\") * _ForwardAccel * 1000;\n        else if(Input.GetAxis(\"Vertical\") < 0)\n            _SpeedInput = Input.GetAxis(\"Vertical\") * _ReverseAccel * 1000;\n\n        _TurnInput = Input.GetAxis(\"Horizontal\");\n\n        if(_Grounded)\n        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, _TurnInput * _TurnStrength * Time.deltaTime, 0));\n\n        transform.position = _RB.transform.position;\n    }\n\n    private void FixedUpdate()\n    {\n        _Grounded = GroundCheck();\n\n        if (_Grounded)\n        {\n            if (Mathf.Abs(_SpeedInput) > 0)\n                _RB.AddForce(transform.forward * _SpeedInput);\n        }\n        else\n            _RB.AddForce(Vector3.up * -_GravityForce * 100);\n    }\n\n    private bool GroundCheck()\n    {\n        _Grounded = false;\n        RaycastHit hit;\n\n        if(Physics.Raycast(transform.position, -transform.up, out hit, _GroundCheckLength, _GroundMask))\n        {\n            _Grounded = true;\n            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;\n        }\n\n        return _Grounded;\n    }\n}\n"),
        new Tool_QuickStart_Script("CarRealistic",              "Car_Drive_Vehicle",            "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class CarRealistic : MonoBehaviour\n{\n    [Header(\"Motor\")]\n    [SerializeField] private List<AxleInfo> axleInfos = null;\n    [SerializeField] private float maxMotorTorque = 1000;\n\n    [Header(\"Steering\")]\n    [SerializeField] private float maxSteeringAngle = 50;\n\n    public void FixedUpdate()\n    {\n        float motor = maxMotorTorque * Input.GetAxis(\"Vertical\");\n        float steering = maxSteeringAngle * Input.GetAxis(\"Horizontal\");\n\n        foreach (AxleInfo axleInfo in axleInfos)\n        {\n            if (axleInfo.steering)\n            {\n                axleInfo.leftWheel.steerAngle = steering;\n                axleInfo.rightWheel.steerAngle = steering;\n            }\n            if (axleInfo.motor)\n            {\n                axleInfo.leftWheel.motorTorque = motor;\n                axleInfo.rightWheel.motorTorque = motor;\n            }\n        }\n    }\n\n}\n\n[System.Serializable]\npublic class AxleInfo\n{\n    public WheelCollider leftWheel;\n    public WheelCollider rightWheel;\n    public bool motor;\n    public bool steering;\n}\n"),
        new Tool_QuickStart_Script("DebugCommandBase",          "Debug_Console",                "stable",           "",        "using System;\nusing System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class DebugCommandBase\n{\n    private string _CommandID;\n    private string _CommandDescription;\n    private string _CommandFormat;\n\n    public string CommandID { get { return _CommandID; } }\n    public string CommandDescription { get { return _CommandDescription; } }\n    public string CommandFormat { get { return _CommandFormat; } }\n\n    public DebugCommandBase(string id, string description, string format)\n    {\n        _CommandID = id;\n        _CommandDescription = description;\n        _CommandFormat = format;\n    }\n}\n\npublic class DebugCommand : DebugCommandBase\n{\n    private Action command;\n\n    public DebugCommand(string id, string description, string format, Action command) : base (id, description, format)\n    {\n        this.command = command;\n    }\n\n    public void Invoke()\n    {\n        command.Invoke();\n    }\n}\n\npublic class DebugCommand<T1> : DebugCommandBase\n{\n    private Action<T1> command;\n\n    public DebugCommand(string id, string description, string format, Action<T1> command) : base (id, description, format)\n    {\n        this.command = command;\n    }\n\n    public void Invoke(T1 value)\n    {\n        command.Invoke(value);\n    }\n}\n"),
        new Tool_QuickStart_Script("DebugConsole",              "Debug_Console",                "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class DebugConsole : MonoBehaviour\n{\n    private bool _ShowConsole;\n    private bool _ShowHelp;\n    private string _Input;\n    private Vector2 _Scroll;\n\n    public static DebugCommand TEST;\n    public static DebugCommand HELP;\n    public static DebugCommand HIDEHELP;\n    public static DebugCommand<float> SETVALUE;\n\n    public List<object> commandList;\n\n    private void Awake()\n    {\n        HELP = new DebugCommand(\"help\", \"Shows a list of commands\", \"help\", () =>\n        {\n            _ShowHelp = !_ShowHelp;\n        });\n\n        HIDEHELP = new DebugCommand(\"hidehelp\", \"hide help info\", \"hidehelp\", () =>\n        {\n            _ShowHelp = false;\n        });\n\n        TEST = new DebugCommand(\"test\", \"example command\", \"test\", () =>\n        {\n            Debug.Log(\"test command triggered\");\n        });\n\n        SETVALUE = new DebugCommand<float>(\"setvalue\", \"example set value\", \"setvalue <value>\", (x) =>\n        {\n            Debug.Log(\"Value added: \" + x.ToString());\n        });\n\n        commandList = new List<object>\n        {         \n            HELP,\n            HIDEHELP,\n            TEST,\n            SETVALUE\n        };\n    }\n\n    private void OnGUI()\n    {\n        //Check input\n        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F1)\n        {\n            _ShowConsole = !_ShowConsole;\n        }\n\n        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return && _ShowConsole)\n        {\n            HandleInput();\n            _Input = \"\";\n        }\n\n        //Console active\n        if (!_ShowConsole) return;\n\n        GUI.FocusControl(\"FOCUS\");\n        \n        float y = 0f;\n\n        if(_ShowHelp)\n        {\n            GUI.Box(new Rect(0, y, Screen.width, 100), \"\");\n            Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);\n            _Scroll = GUI.BeginScrollView(new Rect(0, y + 5, Screen.width, 90), _Scroll, viewport);\n\n            for (int i=0; i<commandList.Count; i++)\n            {\n                DebugCommandBase command = commandList[i] as DebugCommandBase;\n                string label = $\"{command.CommandFormat} - {command.CommandDescription}\";\n                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);\n                GUI.Label(labelRect, label);\n            }\n\n            GUI.EndScrollView();\n            y += 100;\n        }\n\n        GUI.Box(new Rect(0, y, Screen.width, 30), \"\");\n\n        GUI.backgroundColor = new Color(0,0,0,0);\n        GUI.SetNextControlName(\"FOCUS\");\n        _Input = GUI.TextField(new Rect(10, y + 5, Screen.width - 20, 20), _Input);\n    }\n\n    private void HandleInput()\n    {\n        string[] properties = _Input.Split(' ');\n\n        for(int i=0; i < commandList.Count; i++)\n        {\n            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;\n\n            if (_Input.Contains(commandBase.CommandID))\n            {\n                if (commandList[i] as DebugCommand != null)\n                    (commandList[i] as DebugCommand).Invoke();\n                else if (commandList[i] as DebugCommand<int> != null && properties.Length > 1)\n                    if (CheckInput(properties[1]))\n                        (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));\n            }\n        }\n    }\n\n    private bool CheckInput(string str)\n    {\n        foreach (char c in str)\n        {\n            if (c < '0' || c > '9')\n                return false;\n        }\n        return true;\n    }\n}\n"),
        new Tool_QuickStart_Script("DialogSystem",              "Dialog",                       "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEngine.UI;\nusing TMPro;\n\npublic class DialogSystem : MonoBehaviour\n{\n    [Header(\"Ref\")]\n    [SerializeField] private TextMeshProUGUI _DialogText;\n    [SerializeField] private Button _NextButton;\n\n    [Header(\"Ref Options\")]\n    [SerializeField] private List<TextMeshProUGUI> _OptionsText = new List<TextMeshProUGUI>();\n    [SerializeField] private List<GameObject> _OptionsButtons = new List<GameObject>();\n\n    [Header(\"Dialog\")]\n    public DialogSystem_File _Dialog;\n\n    //Private variables\n    private Vector2Int _CurrentID = new Vector2Int(0, 0);\n    private bool _Finished;\n\n    void Update()\n    {\n        _DialogText.text = _Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Dialog;\n\n        // Option Buttons \n        for (int i = 0; i < _OptionsButtons.Count; i++)\n        {\n            if (_Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options.Count != 0)\n            {\n                if (i < _Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options.Count)\n                {\n                    _OptionsButtons[i].SetActive(true);\n                    _OptionsText[i].text = _Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options[i].OptionInfo;\n                }\n                else\n                    _OptionsButtons[i].SetActive(false);\n            }\n            else\n                _OptionsButtons[i].SetActive(false);\n        }\n\n        // NextButton\n        if (_Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options != null)\n        {\n            if (_Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options.Count == 0)\n                _NextButton.gameObject.SetActive(true);\n            else\n                _NextButton.gameObject.SetActive(false);\n        }\n        else\n            _NextButton.gameObject.SetActive(false);\n\n        //Done\n        if(_Finished)\n        {\n            _NextButton.gameObject.SetActive(false);\n            for (int i = 0; i < _OptionsButtons.Count; i++)\n            {\n                _OptionsButtons[i].SetActive(false);\n            }\n            _DialogText.text = \"Finished\";\n        }\n    }\n\n    public void ButtonInput(int id)\n    {\n        for (int i = 0; i < _Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options[id].Options.Count; i++)\n        {\n            switch (_Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options[id].Options[i].Option)\n            {\n                case DialogSystem_DialogOption.Options.GOTO:\n                    _CurrentID = _Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options[id].Options[i].NextID;\n                    break;\n                case DialogSystem_DialogOption.Options.NEXT:\n                    _CurrentID.y++;\n                    break;\n                case DialogSystem_DialogOption.Options.FINISHED:\n                    _Finished = true;\n                    break;\n            }\n        }\n    }\n\n    public void Next()\n    {\n        _CurrentID.y++;\n    }\n}\n\n[System.Serializable]\npublic class DialogSystem_File\n{\n    public string DialogNameID;\n    public List<DialogSystem_DialogTree> DialogTree = new List<DialogSystem_DialogTree>();\n}\n\n[System.Serializable]\npublic class DialogSystem_DialogTree\n{\n    public string DialogTreeInfo = \"\";\n    public List<DialogSystem_Dialog> Dialog = new List<DialogSystem_Dialog>();\n}\n\n[System.Serializable]\npublic class DialogSystem_Dialog\n{\n    public string Dialog = \"\";\n    public List<DialogSystem_DialogOptions> Options = new List<DialogSystem_DialogOptions>();\n}\n\n[System.Serializable]\npublic class DialogSystem_DialogOptions\n{\n    public string OptionInfo = \"\";\n    public List<DialogSystem_DialogOption> Options = new List<DialogSystem_DialogOption>();\n\n    [HideInInspector] public bool OptionToggle = false;\n}\n\n[System.Serializable]\npublic class DialogSystem_DialogOption\n{\n    //Options\n    public enum Options {GOTO, NEXT, FINISHED};\n    public Options Option;\n\n    //EventData\n    public Vector2Int NextID = new Vector2Int();\n}\n"),
        new Tool_QuickStart_Script("DialogSystemEditor",        "Dialog_Editor",                "stable",           "",        "using UnityEngine;\nusing UnityEditor;\nusing System.IO;\nusing System.Collections;\nusing System.Collections.Generic;\n\nclass DialogSystemEditor : EditorWindow\n{\n    DialogSystem _Dialog;\n    string _DialogLoad;\n\n    Vector2 _ScrollPos_TimeLine = new Vector2();\n    Vector2 _ScrollPos_Editor = new Vector2();\n\n    Vector2Int _Selected = new Vector2Int();\n\n    bool _SearchScript = false;\n\n    [MenuItem(\"Tools/DialogSystem Editor\")]\n    public static void ShowWindow()\n    {\n        EditorWindow.GetWindow(typeof(DialogSystemEditor));\n    }\n\n    void OnGUI()\n    {\n        if (!_SearchScript)\n            _Dialog = GameObject.FindObjectOfType<DialogSystem>();\n\n        GUILayout.Label(\"Dialog Editor\", EditorStyles.boldLabel);\n        _Dialog = EditorGUILayout.ObjectField(_Dialog, typeof(DialogSystem), true) as DialogSystem;\n\n        if (_Dialog == null)\n            return;\n        _Dialog._Dialog.DialogNameID = EditorGUILayout.TextField(\"DialogName\",_Dialog._Dialog.DialogNameID);\n\n        EditorGUILayout.BeginHorizontal(\"box\");\n        if (_Dialog != null)\n        {\n            //Editor\n            Editor();\n\n            //TimeLine\n            TimeLine();\n        }\n        EditorGUILayout.EndHorizontal();\n    }\n\n    void Editor()\n    {\n        EditorGUILayout.BeginVertical(\"box\");\n\n        //Edit\n        _ScrollPos_Editor = EditorGUILayout.BeginScrollView(_ScrollPos_Editor, GUILayout.Width(325));\n        if (_Selected.x >= 0)\n        {\n            if (_Selected.x < _Dialog._Dialog.DialogTree.Count)\n            {\n                if (_Selected.y < _Dialog._Dialog.DialogTree[_Selected.x].Dialog.Count)\n                {\n                    //Dialog\n                    GUILayout.Label(\"Selected   \" + \"ID:\" + _Selected.x.ToString() + \",\" + _Selected.y.ToString());\n                    _Dialog._Dialog.DialogTree[_Selected.x].DialogTreeInfo = EditorGUILayout.TextField(\"Row Info:\", _Dialog._Dialog.DialogTree[_Selected.x].DialogTreeInfo);\n                    _Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Dialog = EditorGUILayout.TextArea(_Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Dialog, GUILayout.Height(200), GUILayout.Width(300));\n\n                    //Dialog Options\n                    GUILayout.Label(\"Options\");\n                    EditorGUILayout.BeginVertical(\"box\");\n\n                    int optionscount = 0;\n                    for (int i = 0; i < _Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options.Count; i++)\n                    {\n                        optionscount++;\n                        //Toggle\n                        _Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].OptionToggle = EditorGUILayout.Foldout(_Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].OptionToggle, \"(\" + optionscount.ToString() + \") \" + _Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].OptionInfo);\n                        \n                        //Options\n                        if (_Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].OptionToggle)\n                        {\n                            //Option Dialog\n                            GUILayout.Label(\"Option Dialog:\");\n                            _Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].OptionInfo = EditorGUILayout.TextArea(_Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].OptionInfo, GUILayout.Height(100), GUILayout.Width(300));\n\n                            //Display options\n                            EditorGUILayout.BeginVertical(\"box\");\n                            for (int o = 0; o < _Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].Options.Count; o++)\n                            {\n                                //Option dropdown/Remove Event\n                                EditorGUILayout.BeginHorizontal();\n                                _Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].Options[o].Option = (DialogSystem_DialogOption.Options)EditorGUILayout.EnumPopup(_Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].Options[o].Option);\n                                if (GUILayout.Button(\"-\", GUILayout.Width(20)))\n                                {\n                                    _Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].Options.RemoveAt(o);\n                                    break;\n                                }\n                                EditorGUILayout.EndHorizontal();\n\n                                //Options\n                                switch (_Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].Options[o].Option)\n                                {\n                                    case DialogSystem_DialogOption.Options.GOTO:\n                                        _Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].Options[o].NextID = EditorGUILayout.Vector2IntField(\"Next ID\", _Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].Options[o].NextID);\n                                        break;\n                                }\n\n                            }\n                            if (GUILayout.Button(\"Add Event\"))\n                            {\n                                DialogSystem_DialogOption newoption = new DialogSystem_DialogOption();\n                                _Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options[i].Options.Add(newoption);\n                            }\n                            EditorGUILayout.EndVertical();\n                        }\n                    }\n                    if(GUILayout.Button(\"Add Option Dialog\"))\n                    {\n                        DialogSystem_DialogOptions newoption = new DialogSystem_DialogOptions();\n                        newoption.OptionInfo = \"new option\";\n                        _Dialog._Dialog.DialogTree[_Selected.x].Dialog[_Selected.y].Options.Add(newoption);\n                    }\n                    EditorGUILayout.EndVertical();\n                }\n                else\n                {\n                    GUILayout.Label(\"Selected\");\n                    GUILayout.Label(\"ID: --\");\n                    GUILayout.Label(\"Press a button on \\nthe timeline to select!\");\n                }\n            }\n        }\n        else\n        {\n            GUILayout.Label(\"Selected\");\n            GUILayout.Label(\"ID: --\");\n            GUILayout.Label(\"Press a button on \\nthe timeline to select!\");\n        }\n\n        //SaveLoad\n        if(GUILayout.Button(\"Save\"))\n        {\n            SaveData();\n        }\n\n        _DialogLoad = EditorGUILayout.TextField(\"Dialog Path:\", _DialogLoad);\n        if (GUILayout.Button(\"Load\"))\n        {\n            LoadData();\n        }\n\n        EditorGUILayout.EndScrollView();\n        EditorGUILayout.EndVertical();\n    }\n\n    void TimeLine()\n    {\n        EditorGUILayout.BeginVertical();\n        _ScrollPos_TimeLine = EditorGUILayout.BeginScrollView(_ScrollPos_TimeLine);\n        for (int i = 0; i < _Dialog._Dialog.DialogTree.Count; i++)\n        {\n            EditorGUILayout.BeginHorizontal(\"box\");\n\n            // Row Options\n            EditorGUILayout.BeginVertical();\n\n            // ID/Remove\n            EditorGUILayout.BeginHorizontal();\n            if (GUILayout.Button(\"-\", GUILayout.Width(20)))\n            {\n                _Dialog._Dialog.DialogTree.RemoveAt(i);\n                if(_Selected.x > _Dialog._Dialog.DialogTree.Count-1)\n                    _Selected.x--;\n                break;\n            }\n            EditorGUILayout.EndHorizontal();\n\n            // Add Dialog to timeline\n            if (GUILayout.Button(\"+\", GUILayout.Width(20)))\n            {\n                DialogSystem_Dialog newdialog = new DialogSystem_Dialog();\n                newdialog.Dialog = \"dialogtext\";\n                _Dialog._Dialog.DialogTree[i].Dialog.Add(newdialog);\n            }\n\n            EditorGUILayout.EndVertical();\n\n            //TimeLineButtons\n            for (int j = 0; j < 100; j++)\n            {\n                EditorGUILayout.BeginVertical();\n                GUILayout.Label(j.ToString());\n\n                if (j < _Dialog._Dialog.DialogTree[i].Dialog.Count)\n                {\n                    //if (GUILayout.Button(\"(\" + _Dialog.Dialog[i].DialogTree[j].Options.Count.ToString() + \") \" + _Dialog.Dialog[i].DialogTree[j].Dialog, GUILayout.Width(100), GUILayout.Height(30)))\n                    if (GUILayout.Button(j.ToString() + \"    (\" + _Dialog._Dialog.DialogTree[i].Dialog[j].Options.Count.ToString() + \") \", GUILayout.Width(100), GUILayout.Height(30)))\n                    {\n                        _Selected = new Vector2Int(i, j);\n                    }\n                }\n\n                EditorGUILayout.EndVertical();\n            }\n            EditorGUILayout.EndHorizontal();\n        }\n\n        // Add row\n        if (GUILayout.Button(\"Add Dialog Tree\", GUILayout.Width(100), GUILayout.Height(50)))\n        {\n            DialogSystem_DialogTree newdialogtree = new DialogSystem_DialogTree();\n            DialogSystem_Dialog newdialog = new DialogSystem_Dialog();\n            newdialog.Dialog = \"dialogtext\";\n            newdialogtree.Dialog.Add(newdialog);\n            _Dialog._Dialog.DialogTree.Add(newdialogtree);\n        }\n\n        EditorGUILayout.EndScrollView();\n        EditorGUILayout.EndVertical();\n    }\n\n    //SaveLoad\n    public void SaveData()\n    {\n        string jsonData = JsonUtility.ToJson(_Dialog._Dialog, true);\n        File.WriteAllText(Application.dataPath + \"/\" + _Dialog._Dialog.DialogNameID + \".json\", jsonData);\n    }\n    public void LoadData()\n    {\n        try\n        {\n            string dataAsJson = File.ReadAllText(Application.dataPath + \"/\" + _DialogLoad + \".json\");\n            _Dialog._Dialog = JsonUtility.FromJson<DialogSystem_File>(dataAsJson);\n        }\n        catch\n        {\n            SaveData();\n        }\n    }\n}\n"),
        new Tool_QuickStart_Script("Disable",                   "Practical_Disable",            "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Disable : MonoBehaviour {\n\n    [SerializeField] private GameObject _Object;\n\n    public void DisableObject(float seconds) {\n        StartCoroutine(StartDisable(seconds));\n    }\n\n    private IEnumerator StartDisable(float seconds)\n    {\n        yield return new WaitForSeconds(seconds);\n        _Object.SetActive(false);\n    }\n}\n"),
        new Tool_QuickStart_Script("DoEvent",                   "Practical_Event_UnityEvent",   "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine.Events;\nusing UnityEngine;\nusing UnityEngine.SceneManagement;\n\npublic class DoEvent : MonoBehaviour\n{\n    [SerializeField] private UnityEvent _Event = null;\n    [SerializeField] private bool _OnStart = false;\n    [SerializeField] private bool _OnUpdate = false;\n    [SerializeField] private bool _OnButtonPressed = false;\n\n    void Start()\n    {\n        if (_OnStart)\n            DoEvents();\n    }\n\n    void Update()\n    {\n        if (_OnUpdate)\n            DoEvents();\n\n        if (_OnButtonPressed)\n            if (Input.anyKey)\n                DoEvents();\n    }\n\n    private void DoEvents()\n    {\n        _Event.Invoke();\n    }\n\n    public void SetGameobject_InActive(GameObject targetobject)\n    {\n        targetobject.SetActive(false);\n    }\n\n    public void SetGameobject_Active(GameObject targetobject)\n    {\n        targetobject.SetActive(true);\n    }\n\n    public void SetGameObject_Negative(GameObject targetobject)\n    {\n        if (targetobject.activeSelf)\n            targetobject.SetActive(false);\n        else\n            targetobject.SetActive(true);\n    }\n\n    public void LoadScene(int sceneid)\n    {\n        SceneManager.LoadScene(sceneid);\n    }\n    public void LoadScene(string scenename)\n    {\n        SceneManager.LoadScene(scenename);\n    }\n    public void Quit()\n    {\n        Application.Quit();\n    }\n}\n"),
        new Tool_QuickStart_Script("DontDestroy",               "Practical",                    "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class DontDestroy : MonoBehaviour\n{\n    void Start()\n    {\n        DontDestroyOnLoad(this.gameObject);\n    }\n}\n"),
        new Tool_QuickStart_Script("EditorWindowExample",       "Editor_Window",                "stable",           "",        "using UnityEngine;\nusing UnityEditor;\nusing System.Collections;\n\nclass EditorWindowExample : EditorWindow\n{\n    string examplestring = \"example\";\n    bool examplebool = false;\n\n    [MenuItem(\"Tools/EditorWindowExample\")]\n    public static void ShowWindow()\n    {\n        EditorWindow.GetWindow(typeof(EditorWindowExample));\n    }\n\n    void OnGUI()\n    {\n        GUILayout.Label(\"Example Title\", EditorStyles.boldLabel);\n        examplestring = EditorGUILayout.TextField(\"Example string field\", examplestring);\n        examplebool = EditorGUILayout.Toggle(\"Example bool field\", examplebool);\n    }\n}\n"),
        new Tool_QuickStart_Script("EnemySpawnHandler",         "Enemy_Spawn_Handler",          "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class EnemySpawnHandler : MonoBehaviour\n{\n    private enum Options {Endless, Waves}\n\n    [Header(\"Settings\")]\n    [SerializeField] private Options _Option = Options.Endless;\n    [SerializeField] private int _Seed = 0;\n    [SerializeField] private bool _SetRandomSeed = true;\n\n    [Header(\"Object Pool\")]\n    [SerializeField] private ObjectPool _ObjectPool = null;\n\n    [Header(\"Enemies\")]\n    [SerializeField] private EnemySpawnHandler_Enemy[] _Enemies = null;\n\n    [Header(\"SpawnLocations\")]\n    [SerializeField] private Transform[] _SpawnLocations = null;\n\n    [Header(\"Settings - Endless\")]\n    [SerializeField] private float _SpawnRate = 5; // Seconds between spawning\n    [SerializeField] private float _SpawnRateEncrease = 0.05f; // Decrease time between spawning per sec\n    [SerializeField] private bool _RandomEnemy = true;\n    [SerializeField] private bool _RandomSpawn = true;\n\n    [Header(\"Settings - Waves\")]\n    [SerializeField] private EnemySpawnHandler_WaveSettings _Waves = null;\n    [SerializeField] private bool _WaitForAllEnemiesKilled = true;\n\n    private float _Timer = 0;\n    private int _CurrentWave = 0;\n    private int _CheckWave = 999;\n    private float _TimerBetweenWaves = 0;\n    private float _SpawnSpeed = 0;\n\n    private int _EnemiesAlive = 0;\n\n    private void Start()\n    {\n        if (_SetRandomSeed)\n            Random.InitState(Random.Range(0, 10000));\n        else\n            Random.InitState(_Seed);\n\n        if (_Waves.WaveOption == EnemySpawnHandler_WaveSettings.WaveOptions.Generate)\n            GenerateWaves();\n        if (_Waves.WaveOption == EnemySpawnHandler_WaveSettings.WaveOptions.Endless)\n        {\n            _Waves.WaveAmount = 1;\n            GenerateWaves();\n            GenerateWaves(1);\n        }\n    }\n\n    void Update()\n    {\n        _Timer += 1 * Time.deltaTime;\n\n        switch (_Option)\n        {\n            case Options.Endless:\n                Update_Endless();\n                break;\n            case Options.Waves:\n                Update_Waves();\n                break;\n        }\n    }\n\n    //Update\n    private void Update_Endless()\n    {\n        if (_Timer >= _SpawnRate)\n        {\n            int randomenemyid = 0;\n            int randomspawnid = 0;\n            if (_RandomEnemy)\n                 randomenemyid = Random.Range(0, _Enemies.Length);\n            if (_RandomSpawn)\n                randomspawnid = Random.Range(0, _SpawnLocations.Length);\n            Spawn(randomenemyid, randomspawnid);\n            _Timer = 0;\n        }\n        _SpawnRate -= _SpawnRateEncrease * Time.deltaTime;\n    }\n    private void Update_Waves()\n    {\n        if (_CurrentWave < _Waves.Waves.Count)\n        {\n            if (_CheckWave != _CurrentWave)\n            {\n                if (_WaitForAllEnemiesKilled)\n                {\n                    EnemiesAlive();\n\n                    if (_EnemiesAlive == 0)\n                        _TimerBetweenWaves += 1 * Time.deltaTime;\n                }\n                else\n                    _TimerBetweenWaves += 1 * Time.deltaTime;\n\n                if (_TimerBetweenWaves >= _Waves.TimeBetweenWaves)\n                {\n                    _TimerBetweenWaves = 0;\n                    _CheckWave = _CurrentWave;\n                    _SpawnSpeed = _Waves.Waves[_CurrentWave].SpawnDuration / _Waves.Waves[_CurrentWave].TotalEnemies;\n                    if (_Waves.WaveOption == EnemySpawnHandler_WaveSettings.WaveOptions.Endless)\n                        GenerateWaves(_CurrentWave+2);\n                }\n            }\n            else\n            {\n                //Spawn\n                if (_Waves.Waves[_CurrentWave].TotalEnemies > 0)\n                {\n                    if (_Timer > _SpawnSpeed)\n                    {\n                        bool spawncheck = false;\n                        while (!spawncheck)\n                        {\n                            int spawnid = Random.Range(0, _Enemies.Length);\n                            if (_Waves.Waves[_CurrentWave].EnemyID[spawnid] > 0)\n                            {\n                                Spawn(spawnid, Random.Range(0, _SpawnLocations.Length));\n                                _Waves.Waves[_CheckWave].EnemyID[spawnid]--;\n                                _Waves.Waves[_CurrentWave].TotalEnemies--;\n                                spawncheck = true;\n                            }\n                        }\n                        _Timer = 0;\n                    }\n                }\n                else\n                {\n                    _CurrentWave++;\n                }\n            }\n        }\n    }\n\n    //Generate Waves\n    private void GenerateWaves(int waveid = 0)\n    {\n        int enemytypes = _Enemies.Length;\n        for (int i = 0; i < _Waves.WaveAmount; i++)\n        {\n            EnemySpawnHandler_Wave newwave = new EnemySpawnHandler_Wave();\n            int enemyamount = 0;\n\n            if (waveid == 0)\n                enemyamount = Mathf.RoundToInt(_Waves.EnemyAmount * ((_Waves.EnemyIncreaseAmount * i) + 1));\n            else\n                enemyamount = Mathf.RoundToInt(_Waves.EnemyAmount * ((_Waves.EnemyIncreaseAmount * waveid) + 1));\n\n            //Set enemy amount\n            newwave.EnemyID = new int[enemytypes];\n            int checkenemyamount = 0;\n            newwave.TotalEnemies = enemyamount;\n\n            while (checkenemyamount < enemyamount)\n            {\n                for (int j = 0; j < enemytypes; j++)\n                {\n                    if (_Enemies[j].StartWave <= i)\n                    {\n                        int addamount = 0;\n                        if (enemyamount < 2)\n                            addamount = Random.Range(0, enemyamount);\n                        else\n                            addamount = Random.Range(0, Mathf.RoundToInt(enemyamount*0.5f));\n\n                        if (enemyamount > checkenemyamount + addamount)\n                        {\n                            newwave.EnemyID[j] += addamount;\n                            checkenemyamount += addamount;\n                        }\n                        else\n                        {\n                            newwave.EnemyID[j] += enemyamount - checkenemyamount;\n                            checkenemyamount = enemyamount;\n                            continue;\n                        }\n                    }\n                }\n            }\n            _Waves.Waves.Add(newwave);\n        }\n    }\n\n    public void Spawn(int enemyid, int spawnid)\n    {\n        GameObject obj = _ObjectPool.GetObjectPrefabName(_Enemies[enemyid].EnemyPrefab.name, false);\n        obj.transform.position = _SpawnLocations[spawnid].position;\n        obj.SetActive(true);\n    }\n    private void EnemiesAlive()\n    {\n        _EnemiesAlive = GameObject.FindGameObjectsWithTag(\"Enemy\").Length;\n    }\n}\n\n[System.Serializable]\npublic class EnemySpawnHandler_Enemy\n{\n    public string EnemyName;\n    public GameObject EnemyPrefab;\n\n    [Header(\"Settings\")]\n    public int StartWave;\n}\n\n[System.Serializable]\npublic class EnemySpawnHandler_WaveSettings\n{\n    public enum WaveOptions {Endless, Manually, Generate}\n    public WaveOptions WaveOption;\n\n    [Header(\"Endless\")]\n    public float EnemyIncreaseAmount;\n\n    [Header(\"Manual\")]\n    public List<EnemySpawnHandler_Wave> Waves;\n\n    [Header(\"Generate\")]\n    public int WaveAmount;\n    public int EnemyAmount;\n\n    [Header(\"Other\")]\n    public float TimeBetweenWaves;\n}\n\n[System.Serializable]\npublic class EnemySpawnHandler_Wave\n{\n    public int[] EnemyID;\n    public float SpawnDuration = 5;\n\n    [HideInInspector] public int TotalEnemies;\n}\n"),
        new Tool_QuickStart_Script("FadeInOut",                 "Practical",                    "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine.UI;\nusing UnityEngine;\n\npublic class FadeInOut : MonoBehaviour\n{\n    private enum Fade { In, Out }\n    [SerializeField] private Fade _FadeOption = Fade.In;\n    [SerializeField] private float _Duration = 0;\n\n    [SerializeField] private Image _Image = null;\n\n    private float _ChangeSpeed;\n    private Color _Color;\n\n    void Start()\n    {\n        if (_Image == null)\n            _Image = GetComponent<Image>();\n\n        if (_FadeOption == Fade.In)\n            _Color = new Color(_Image.color.r, _Image.color.g, _Image.color.b, 0);\n        else\n            _Color = new Color(_Image.color.r, _Image.color.g, _Image.color.b, 1);\n\n        _ChangeSpeed = 1 / _Duration;\n    }\n\n    void Update()\n    {\n        if (_FadeOption == Fade.In && _Color.a < 1)\n        {\n            _Color.a += _ChangeSpeed * Time.deltaTime;\n        }\n        if (_FadeOption == Fade.Out && _Color.a > 0)\n        {\n            _Color.a -= _ChangeSpeed * Time.deltaTime;\n        }\n\n        _Image.color = _Color;\n    }\n}\n"),
        new Tool_QuickStart_Script("Health",                    "Health",                       "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEngine.Events;\n\npublic class Health : MonoBehaviour\n{\n    [SerializeField] private float _MaxHealth = 100;\n\n    [SerializeField] private UnityEvent _OnDeath;\n\n    private float _CurrentHealth;\n\n    private void OnEnable()\n    {\n        _CurrentHealth = _MaxHealth;\n    }\n\n    public void DoDamage(float damageamount)\n    {\n        _CurrentHealth -= damageamount;\n        if(_CurrentHealth <= 0)\n        {\n            _CurrentHealth = 0;\n            _OnDeath.Invoke();\n            gameObject.SetActive(false);\n        }\n    }\n\n    public float GetCurrentHealth()\n    {\n        return _CurrentHealth;\n    }\n    public float GetMaxHealth()\n    {\n        return GetMaxHealth();\n    }\n}\n"),
        new Tool_QuickStart_Script("Interactable",              "Interaction",                  "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEngine.Events;\nusing UnityEngine.UI;\n\npublic class Interactable : MonoBehaviour\n{\n    public enum InteractableType { Move, Door, SetLight, SetLightNegative, Lever, Button, Item, UIButton }\n    public InteractableType _Type;\n\n    private enum AxisOptions { x, y, z }\n    [SerializeField] private AxisOptions _AxisOption = AxisOptions.x;\n\n    [SerializeField] private bool _InvertMouse = false;\n\n    [Header(\"Type - Light\")]\n    [SerializeField] private GameObject _Light = null;\n    [SerializeField] private bool _Light_StartOff = false;\n    [Header(\"Type - Lever/Door\")]\n    [SerializeField] private Transform _LeverRotationPoint = null;\n    [SerializeField] private Vector2 _LeverMinMaxRotation = Vector2.zero;\n    [SerializeField] private float _CompleteDeathZone = 0;\n    [Header(\"Type - Button\")]\n    [SerializeField] private float _ButtonPressDepth = 0;\n    private bool _ButtonPressed;\n    [Header(\"Type - Item\")]\n    [SerializeField] private string _ItemInfo = \"\";\n    [Header(\"Speed\")]\n    [SerializeField] private float _Speed = 1;\n\n    [Header(\"OnHigh\")]\n    [SerializeField] private UnityEvent _OnHighEvent = null;\n    [Header(\"OnLow\")]\n    [SerializeField] private UnityEvent _OnLowEvent = null;\n    [Header(\"OnNeutral\")]\n    [SerializeField] private UnityEvent _OnNeutral = null;\n    [Header(\"Trigger\")]\n    [SerializeField] private UnityEvent _OnTrigger = null;\n\n\n    private Vector3 velocity = Vector3.zero;\n    private Rigidbody _RB;\n    private Vector3 _DefaultLocalPosition;\n    private Vector3 _DefaultRotation;\n    private bool _MovingBack;\n\n    private void Start()\n    {\n        _DefaultLocalPosition = transform.localPosition;\n        _DefaultRotation = transform.eulerAngles;\n        _RB = GetComponent<Rigidbody>();\n        if (_Type == InteractableType.SetLight || _Type == InteractableType.SetLightNegative)\n        {\n            if (_Light_StartOff)\n                _Light.SetActive(false);\n            else\n                _Light.SetActive(true);\n        }\n    }\n\n    private void Update()\n    {\n        if (_Type == InteractableType.Button)\n        {\n            UpdateButton();\n        }\n\n        if (_MovingBack)\n        {\n            transform.eulerAngles = _DefaultRotation;\n            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _DefaultLocalPosition, 10 * Time.deltaTime);\n            if (transform.localPosition == _DefaultLocalPosition)\n                _MovingBack = false;\n        }\n    }\n\n    public InteractableType Type()\n    {\n        return _Type;\n    }\n\n    public void GotoPickupPoint(Transform point)\n    {\n        _RB.velocity = Vector3.zero;\n        transform.position = Vector3.SmoothDamp(transform.position, point.position, ref velocity, 0.2f);\n        transform.rotation = Quaternion.RotateTowards(transform.rotation, point.rotation, 5f);\n    }\n    public void SetVelocity(Vector3 velocity)\n    {\n        _RB.velocity = velocity;\n    }\n    public void TrowObject(Transform transformtrow)\n    {\n        _RB.AddForce(transformtrow.forward * 5000);\n    }\n    public void OpenDoor()\n    {\n        float mouseY = Input.GetAxis(\"Mouse Y\");\n        float angle = 0;\n        switch (_AxisOption)\n        {\n            case AxisOptions.x:\n                angle = _LeverRotationPoint.localEulerAngles.x;\n                break;\n            case AxisOptions.y:\n                angle = _LeverRotationPoint.localEulerAngles.y;\n                break;\n            case AxisOptions.z:\n                angle = _LeverRotationPoint.localEulerAngles.z;\n                break;\n        }\n        angle = (angle > 180) ? angle - 360 : angle;\n\n        HandleRotation(_LeverRotationPoint, new Vector2(0, mouseY), _LeverMinMaxRotation, 1.2f, angle);\n    }\n    public void MoveLever()\n    {\n        float mouseY = Input.GetAxis(\"Mouse Y\");\n        float angle = 0;\n        switch (_AxisOption)\n        {\n            case AxisOptions.x:\n                angle = _LeverRotationPoint.localEulerAngles.x;\n                break;\n            case AxisOptions.y:\n                angle = _LeverRotationPoint.localEulerAngles.y;\n                break;\n            case AxisOptions.z:\n                angle = _LeverRotationPoint.localEulerAngles.z;\n                break;\n        }\n        angle = (angle > 180) ? angle - 360 : angle;\n\n        HandleRotation(_LeverRotationPoint, new Vector2(0, mouseY), _LeverMinMaxRotation, 1.2f, angle);\n\n        //Check\n        if (angle < _LeverMinMaxRotation.x + _CompleteDeathZone)\n        {\n            _OnLowEvent.Invoke();\n        }\n        if (angle > _LeverMinMaxRotation.y - _CompleteDeathZone)\n        {\n            _OnHighEvent.Invoke();\n        }\n        if (angle > _LeverMinMaxRotation.x + _CompleteDeathZone && angle < _LeverMinMaxRotation.y - _CompleteDeathZone)\n        {\n            _OnNeutral.Invoke();\n        }\n    }\n    public void PressButton(bool option)\n    {\n        _ButtonPressed = true;\n    }\n    public void PressButtonNegative()\n    {\n        _ButtonPressed = !_ButtonPressed;\n    }\n    public void SetLight(bool option)\n    {\n        _Light.SetActive(option);\n    }\n    public void SetLightNegative()\n    {\n        if (_Light.activeSelf)\n            _Light.SetActive(false);\n        else\n            _Light.SetActive(true);\n    }\n    public void ReturnToDefaultPos()\n    {\n        _MovingBack = true;\n    }\n    public string GetItemInfo()\n    {\n        return _ItemInfo;\n    }\n    public void PressUIButton()\n    {\n        _OnTrigger.Invoke();\n    }\n    private void HandleRotation(Transform effectedtransform, Vector2 mousemovement, Vector2 minmaxangle, float speed, float angle)\n    {\n        if (_InvertMouse)\n        {\n            mousemovement.x = mousemovement.x * -2;\n            mousemovement.y = mousemovement.y * -2;\n        }\n\n        switch (_AxisOption)\n        {\n            case AxisOptions.x:\n                effectedtransform.localEulerAngles += new Vector3((mousemovement.x + mousemovement.y) * speed, 0, 0);\n\n                if (angle < minmaxangle.x)\n                    effectedtransform.localEulerAngles = new Vector3(minmaxangle.x + 0.5f, 0, 0);\n                if (angle > minmaxangle.y)\n                    effectedtransform.localEulerAngles = new Vector3(minmaxangle.y - 0.5f, 0, 0);\n                break;\n            case AxisOptions.y:\n                effectedtransform.localEulerAngles += new Vector3(0, (mousemovement.x + mousemovement.y) * speed, 0);\n\n                if (angle < minmaxangle.x)\n                    effectedtransform.localEulerAngles = new Vector3(0, minmaxangle.x + 0.5f, 0);\n                if (angle > minmaxangle.y)\n                    effectedtransform.localEulerAngles = new Vector3(0, minmaxangle.y - 0.5f, 0);\n                break;\n            case AxisOptions.z:\n                effectedtransform.localEulerAngles += new Vector3(0, 0, (mousemovement.x + mousemovement.y) * speed);\n\n                if (angle < minmaxangle.x)\n                    effectedtransform.localEulerAngles = new Vector3(0, 0, minmaxangle.x + 0.5f);\n                if (angle > minmaxangle.y)\n                    effectedtransform.localEulerAngles = new Vector3(0, 0, minmaxangle.y - 0.5f);\n                break;\n        }\n    }\n\n    private void UpdateButton()\n    {\n        switch (_AxisOption)\n        {\n            case AxisOptions.x:\n                if (_ButtonPressed)\n                {\n                    if (transform.localPosition.x > _DefaultLocalPosition.x - _ButtonPressDepth)\n                        transform.localPosition -= new Vector3(_Speed, 0, 0) * Time.deltaTime;\n                    else\n                    {\n                        transform.localPosition = new Vector3(_DefaultLocalPosition.x - _ButtonPressDepth - 0.001f, transform.localPosition.y, transform.localPosition.z);\n                        _OnLowEvent.Invoke();\n                    }\n                }\n                else\n                {\n                    if (transform.localPosition.x < _DefaultLocalPosition.x + _ButtonPressDepth)\n                        transform.localPosition += new Vector3(_Speed, 0, 0) * Time.deltaTime;\n                    else\n                    {\n                        transform.localPosition = new Vector3(_DefaultLocalPosition.x + _ButtonPressDepth, transform.localPosition.y, transform.localPosition.z);\n                        _OnHighEvent.Invoke();\n                    }\n\n                }\n                break;\n            case AxisOptions.y:\n                if (_ButtonPressed)\n                {\n                    if (transform.localPosition.y > _DefaultLocalPosition.y - _ButtonPressDepth)\n                        transform.localPosition -= new Vector3(0, _Speed, 0) * Time.deltaTime;\n                    else\n                    {\n                        transform.localPosition = new Vector3(_DefaultLocalPosition.x, _DefaultLocalPosition.y - _ButtonPressDepth - 0.001f, _DefaultLocalPosition.z);\n                        _OnLowEvent.Invoke();\n                    }\n                }\n                else\n                {\n                    if (transform.localPosition.y < _DefaultLocalPosition.y)\n                        transform.localPosition += new Vector3(0, _Speed, 0) * Time.deltaTime;\n                    else\n                    {\n                        transform.localPosition = _DefaultLocalPosition;\n                        _OnHighEvent.Invoke();\n                    }\n                }\n                break;\n            case AxisOptions.z:\n                if (_ButtonPressed)\n                {\n                    if (transform.localPosition.z > _DefaultLocalPosition.z - _ButtonPressDepth)\n                        transform.localPosition -= new Vector3(0, 0, _Speed) * Time.deltaTime;\n                    else\n                    {\n                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _DefaultLocalPosition.z - _ButtonPressDepth - 0.001f);\n                        _OnLowEvent.Invoke();\n                    }\n                }\n                else\n                {\n                    if (transform.localPosition.z < _DefaultLocalPosition.z + _ButtonPressDepth)\n                        transform.localPosition += new Vector3(0, 0, _Speed) * Time.deltaTime;\n                    else\n                    {\n                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _DefaultLocalPosition.z + _ButtonPressDepth);\n                        _OnHighEvent.Invoke();\n                    }\n                }\n                break;\n        }\n    }\n}\n"),
        new Tool_QuickStart_Script("InteractionHandler",        "Interaction_Handler",          "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEngine.UI;\nusing TMPro;\n\npublic class InteractionHandler : MonoBehaviour\n{\n    [SerializeField] private Image _Cursor = null;\n    [SerializeField] private LayerMask _LayerMask = 0;\n    [SerializeField] private Transform _Head = null;\n    [Header(\"Pickup\")]\n    [SerializeField] private GameObject _PickupPoint = null;\n    [SerializeField] private Vector2 _PickupMinMaxRange = Vector2.zero;\n    [SerializeField] private float _Range = 0;\n    [Header(\"Item\")]\n    [SerializeField] private Transform _ItemPreviewPoint = null;\n    [SerializeField] private TextMeshProUGUI _ItemInfoText = null;\n\n    private string _ItemInfo;\n\n    private Vector3 _PickupPointPosition;\n    private Vector3 _CalcVelocity;\n    private Vector3 _PrevPosition;\n\n    private GameObject _ActiveObject;\n    private GameObject _CheckObject;\n    private Interactable _Interactable;\n\n    private bool _Interacting;\n    private bool _Previewing;\n\n    private Movement_CC_FirstPerson _CCS; //Script that handles rotation\n\n    void Start()\n    {\n        _CCS = GetComponent<Movement_CC_FirstPerson>();\n        _PickupPointPosition.z = _PickupMinMaxRange.x;\n    }\n\n    void Update()\n    {\n        if (!_Interacting)\n        {\n            RaycastHit hit;\n\n            if (Physics.Raycast(_Head.position, _Head.TransformDirection(Vector3.forward), out hit, _Range, _LayerMask))\n            {\n                Debug.DrawRay(_Head.position, _Head.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);\n\n                _ActiveObject = hit.transform.gameObject;\n\n                _Cursor.color = Color.white;\n            }\n            else\n            {\n                Debug.DrawRay(_Head.position, _Head.TransformDirection(Vector3.forward) * _Range, Color.white);\n                _Cursor.color = Color.red;\n\n                _ActiveObject = null;\n                _CheckObject = null;\n            }\n\n            if (_ActiveObject != _CheckObject)\n            {\n                _Interactable = _ActiveObject.GetComponent<Interactable>();\n                _CheckObject = _ActiveObject;\n            }\n        }\n\n        if(_ActiveObject != null && _Interactable != null)\n        {\n            if (_Interactable._Type != Interactable.InteractableType.Item)\n            {\n                //OnDown\n                if (Input.GetMouseButtonDown(0))\n                    OnDown();\n\n                if (_Interacting)\n                {\n                    //OnUp\n                    if (Input.GetMouseButtonUp(0))\n                        OnUp();\n\n                    //OnActive\n                    OnActive();\n                }\n            }\n            else\n            {\n                if (!_Previewing)\n                {\n                    //Start Preview\n                    if (Input.GetKeyDown(KeyCode.E))\n                    {\n                        _ItemInfo = _Interactable.GetItemInfo();\n                        _CCS.LockRotation(true);\n                        _Previewing = true;\n                    }\n                }\n                else\n                {\n                    _ActiveObject.transform.position = _ItemPreviewPoint.position;\n                    _Interactable.gameObject.transform.eulerAngles += new Vector3(-Input.GetAxis(\"Mouse Y\"), Input.GetAxis(\"Mouse X\"), 0);\n\n                    //Reset Preview\n                    if (Input.GetKeyDown(KeyCode.E))\n                    {\n                        _ItemInfo = \"\";\n                        _CCS.LockRotation(false);\n                        _Interactable.ReturnToDefaultPos();\n                        _Previewing = false;\n                    }\n                }\n            }\n        }\n\n        _ItemInfoText.text = _ItemInfo;\n    }\n\n    void FixedUpdate()\n    {\n        if (_Interacting)\n        {\n            OnActiveFixed();\n            OnActiveFixed();\n        }\n    }\n\n    private void OnUp()\n    {\n        _Interacting = false;\n        switch (_Interactable._Type)\n        {\n            case Interactable.InteractableType.Lever:\n                _CCS.LockRotation(false);\n                break;\n            case Interactable.InteractableType.Door:\n                _CCS.LockRotation(false);\n                break;\n            case Interactable.InteractableType.Move:\n                _Interactable.SetVelocity(_CalcVelocity);\n                break;\n        }\n    }\n    private void OnDown()\n    {\n        _Interacting = true;\n\n        //OnClick\n        switch (_Interactable._Type)\n        {\n            case Interactable.InteractableType.SetLight:\n                _Interactable.SetLight(true);\n                break;\n            case Interactable.InteractableType.SetLightNegative:\n                _Interactable.SetLightNegative();\n                break;\n            case Interactable.InteractableType.Move:\n                _PickupPoint.transform.rotation = _ActiveObject.transform.rotation;\n                _PickupPointPosition.z = Vector3.Distance(_Head.position, _ActiveObject.transform.position);\n                break;\n            case Interactable.InteractableType.Lever:\n                _CCS.LockRotation(true);\n                _PickupPointPosition.z = Vector3.Distance(_Head.position, _ActiveObject.transform.position);\n                break;\n            case Interactable.InteractableType.Door:\n                _CCS.LockRotation(true);\n                break;\n            case Interactable.InteractableType.Button:\n                _Interactable.PressButtonNegative();\n                break;\n            case Interactable.InteractableType.UIButton:\n                _Interactable.PressUIButton();\n                break;\n        }\n    }\n    private void OnActive()\n    {\n        switch (_Interactable._Type)\n        {\n            case Interactable.InteractableType.Move:\n                if (_PickupPointPosition.z < _PickupMinMaxRange.y && Input.mouseScrollDelta.y > 0)\n                    _PickupPointPosition.z += Input.mouseScrollDelta.y * 0.5f;\n                if (_PickupPointPosition.z > _PickupMinMaxRange.x && Input.mouseScrollDelta.y < 0)\n                    _PickupPointPosition.z += Input.mouseScrollDelta.y * 0.5f;\n\n                if(Input.GetMouseButtonDown(1))\n                {\n                    _Interactable.TrowObject(_Head.transform);\n                    OnUp();\n                }\n                break;\n            case Interactable.InteractableType.Door:\n                _Interactable.OpenDoor();\n                break;\n            case Interactable.InteractableType.Lever:\n                _Interactable.MoveLever();\n                break;\n        }\n\n        if (Vector3.Distance(_Head.transform.position, _ActiveObject.transform.position) > _Range)\n        {\n            _Interacting = false;\n            OnUp();\n        }\n    }\n\n    private void OnActiveFixed()\n    {\n        switch (_Interactable._Type)\n        {\n            case Interactable.InteractableType.Move:\n                _Interactable.GotoPickupPoint(_PickupPoint.transform);\n\n                _PickupPoint.transform.localPosition = _PickupPointPosition;\n\n                _CalcVelocity = (_ActiveObject.transform.position - _PrevPosition) / Time.deltaTime;\n                _PrevPosition = _ActiveObject.transform.position;\n                break;\n        }\n    }\n}\n"),
        //new Tool_QuickStart_Script("InteractWithPhysics",       "Interact_Physics",             "wip",            "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class InteractWithPhysics : MonoBehaviour\n{\n\n    void OnControllerColliderHit(ControllerColliderHit hit)\n    {\n        Rigidbody body = hit.collider.attachedRigidbody;\n        if (body != null && !body.isKinematic)\n             body.velocity += hit.controller.velocity;\n    }\n}\n"),
        //new Tool_QuickStart_Script("Inventory",                 "Inventory",                    "wip",            "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Inventory : MonoBehaviour\n{\n    // Start is called before the first frame update\n    void Start()\n    {\n        \n    }\n\n    // Update is called once per frame\n    void Update()\n    {\n        \n    }\n}\n"),
        new Tool_QuickStart_Script("LightEffects",              "Light_Effect",                 "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class LightEffects : MonoBehaviour\n{\n    public enum LightEffectOptions { Flickering, Off, On };\n\n    [Header(\"Settings\")]\n    [SerializeField] private LightEffectOptions _LightEffectOption = LightEffectOptions.Flickering;\n    [SerializeField] private Vector2 _MinMaxIncrease = new Vector2(0.8f, 1.2f);\n    [Range(0.01f, 100)] [SerializeField] private float _EffectStrength = 50;\n\n    Queue<float> _LightFlickerQ;\n    private float _LastSum = 0;\n    private Light _Light;\n    private float _LightIntensity = 0;\n\n    public void Reset()\n    {\n        if (_LightEffectOption == LightEffectOptions.Flickering)\n        {\n            _LightFlickerQ.Clear();\n            _LastSum = 0;\n        }\n    }\n\n    void Start()\n    {\n        _Light = GetComponent<Light>();\n        _LightIntensity = _Light.intensity;\n        _LightFlickerQ = new Queue<float>(Mathf.RoundToInt(_EffectStrength));\n    }\n\n    void Update()\n    {\n        switch(_LightEffectOption)\n        {\n            case LightEffectOptions.Flickering:\n                while (_LightFlickerQ.Count >= _EffectStrength)\n                    _LastSum -= _LightFlickerQ.Dequeue();\n\n                float newVal = Random.Range(_LightIntensity * _MinMaxIncrease.x, _LightIntensity * _MinMaxIncrease.y);\n                _LightFlickerQ.Enqueue(newVal);\n                _LastSum += newVal;\n                _Light.intensity = _LastSum / (float)_LightFlickerQ.Count;\n                break;\n            case LightEffectOptions.Off:\n                _Light.intensity = 0;\n                break;\n            case LightEffectOptions.On:\n                _Light.intensity = _LightIntensity = _MinMaxIncrease.x;\n                break;\n        }\n\n    }\n\n    public void SetEffect(LightEffectOptions options)\n    {\n        _LightEffectOption = options;\n    }\n}\n"),
        new Tool_QuickStart_Script("LoadScenes",                "Load_Scenes",                  "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEngine.SceneManagement;\n\npublic class LoadScenes : MonoBehaviour\n{\n    public void Action_LoadScene(int sceneid)\n    {\n        SceneManager.LoadScene(sceneid);\n    }\n    public void Action_LoadScene(string scenename)\n    {\n        SceneManager.LoadScene(scenename);\n    }\n\n    public void Action_ReloadScene()\n    {\n        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);\n    }\n\n    public void Action_QuitApplication()\n    {\n        Application.Quit();\n    }\n}\n"),
        new Tool_QuickStart_Script("MenuHandler",               "Menu_Handler",                 "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEngine.SceneManagement;\n\npublic class MenuHandler : MonoBehaviour\n{\n    public void LoadScene(int sceneid)\n    {\n        SceneManager.LoadScene(sceneid);\n    }\n\n    public void LoadScene(string scenename)\n    {\n        SceneManager.LoadScene(scenename);\n    }\n\n    public int Get_CurrentSceneID()\n    {\n        return SceneManager.GetActiveScene().buildIndex;\n    }\n\n    public string Get_CurrentSceneName()\n    {\n        return SceneManager.GetActiveScene().name;\n    }\n\n    public void QuitGame()\n    {\n        Application.Quit();\n    }\n}\n"),
        new Tool_QuickStart_Script("Movement_2D_Platformer",    "Movement_2D",                  "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\n[RequireComponent(typeof(Rigidbody2D))]\npublic class Movement_2D_Platformer : MonoBehaviour\n{\n    [Header(\"Settings\")]\n    [SerializeField] private float _NormalSpeed = 5;\n    [SerializeField] private float _SprintSpeed = 8;\n    [SerializeField] private float _JumpSpeed = 300;\n    [SerializeField] private float _GroundCheck = 0.6f;\n    [Header(\"Set ground layer\")]\n    [SerializeField] private LayerMask _GroundMask = ~1;\n\n    private float _Speed = 0;\n    private Rigidbody2D _RB;\n\n    void Start()\n    {\n        //Get Rigidbody / Lock z rotation\n        _RB = GetComponent<Rigidbody2D>();\n        _RB.constraints = RigidbodyConstraints2D.FreezeRotation;\n    }\n\n    void Update()\n    {\n        //Sprint\n        if (Input.GetKey(KeyCode.LeftShift))\n            _Speed = _SprintSpeed;\n        else\n            _Speed = _NormalSpeed;\n\n        //Jumping\n        if (Input.GetButtonDown(\"Jump\") && IsGrounded())\n            _RB.AddForce(new Vector2(0, _JumpSpeed));\n\n        //Apply Movement\n        _RB.velocity = new Vector2(Input.GetAxis(\"Horizontal\") * _Speed, _RB.velocity.y);\n    }\n\n    bool IsGrounded()\n    {\n        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _GroundCheck, _GroundMask);\n        if (hit.collider != null)\n        {\n            return true;\n        }\n        return false;\n    }\n}\n"),
        new Tool_QuickStart_Script("Movement_2D_TopDown",       "Movement_2D",                  "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\n[RequireComponent(typeof(Rigidbody2D))]\npublic class Movement_2D_TopDown : MonoBehaviour\n{\n    [Header(\"Settings\")]\n    [SerializeField] private float _NormalSpeed = 5;\n    [SerializeField] private float _SprintSpeed = 8;\n\n    private float _Speed = 0;\n    private Rigidbody2D _RB;\n\n    void Start()\n    {\n        //Get Rigidbody / Lock z rotation\n        _RB = GetComponent<Rigidbody2D>();\n        _RB.constraints = RigidbodyConstraints2D.FreezeRotation;\n        _RB.gravityScale = 0;\n    }\n\n    void Update()\n    {\n        //Sprint\n        if (Input.GetKey(KeyCode.LeftShift))\n            _Speed = _SprintSpeed;\n        else\n            _Speed = _NormalSpeed;\n\n        //Apply Movement\n        _RB.velocity = new Vector2(Input.GetAxis(\"Horizontal\") * _Speed, Input.GetAxis(\"Vertical\") * _Speed);\n    }\n}\n"),
        new Tool_QuickStart_Script("Movement_Camera",           "Movement_3D_Camera",           "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Movement_Camera : MonoBehaviour\n{\n    private enum CameraOptionsPos { None, Follow }\n    private enum CameraOptionsRot { None, Follow }\n\n    [Header(\"Options\")]\n    [SerializeField] private CameraOptionsPos _CameraOptionPos = CameraOptionsPos.Follow;\n    [SerializeField] private CameraOptionsRot _CameraOptionRot = CameraOptionsRot.Follow;\n\n    [Header(\"Settings - Position\")]\n    [SerializeField] private Vector3 _OffsetPosition = new Vector3(0,12,-4);\n\n    [Header(\"Settings - Rotation\")]\n    [SerializeField] private Vector3 _OffsetRotation = Vector3.zero;\n\n    [Header(\"Settings\")]\n    [SerializeField] private float _Speed = 1000;\n\n    [Header(\"Other\")]\n    [SerializeField] private Transform _Target = null;\n    [SerializeField] private bool _LockAxis_X = false;\n    [SerializeField] private bool _LockAxis_Y = false;\n    [SerializeField] private bool _LockAxis_Z = false;\n\n    private Vector3 _TargetPosition;\n    private float _ScreenShakeDuration;\n    private float _ScreenShakeIntensity;\n\n    void Update()\n    {\n        if(Input.GetKeyDown(KeyCode.J))\n        {\n            Effect_ScreenShake(3, 0.5f);\n        }\n\n        //Update Target Location\n        float x_axis = _Target.transform.position.x + _OffsetPosition.x;\n        float y_axis = _Target.transform.position.y + _OffsetPosition.y;\n        float z_axis = _Target.transform.position.z + _OffsetPosition.z;\n\n        if (_LockAxis_X)\n            x_axis = _OffsetPosition.x;\n        if (_LockAxis_Y)\n            y_axis = _OffsetPosition.y;\n        if (_LockAxis_Z)\n            z_axis = _OffsetPosition.z;\n\n        _TargetPosition = new Vector3(x_axis, y_axis, z_axis);\n\n        // Movement\n        switch (_CameraOptionPos)\n        {\n            case CameraOptionsPos.Follow:\n                transform.position = Vector3.MoveTowards(transform.position, _TargetPosition, _Speed * Time.deltaTime);\n                break;\n        }\n\n        //ScreenShake\n        if(_ScreenShakeDuration > 0)\n        {\n            transform.localPosition = new Vector3(_TargetPosition.x + Random.insideUnitSphere.x * _ScreenShakeIntensity, _TargetPosition.y + Random.insideUnitSphere.y * _ScreenShakeIntensity, _TargetPosition.z);\n            _ScreenShakeDuration -= 1 * Time.deltaTime;\n        }\n        else\n        {\n            // Rotation\n            switch (_CameraOptionRot)\n            {\n                case CameraOptionsRot.Follow:\n                    Vector3 rpos = _Target.position - transform.position;\n                    Quaternion lookrotation = Quaternion.LookRotation(rpos, Vector3.up);\n                    transform.eulerAngles = new Vector3(lookrotation.eulerAngles.x + _OffsetRotation.x, lookrotation.eulerAngles.y + _OffsetRotation.y, lookrotation.eulerAngles.z + _OffsetRotation.z);\n                    break;\n            }\n        }\n    }\n\n    //Effects\n    public void Effect_ScreenShake(float duration, float intesity)\n    {\n        _ScreenShakeDuration = duration;\n        _ScreenShakeIntensity = intesity;\n    }\n\n    //Set\n    public void Set_CameraTarget(GameObject targetobj)\n    {\n        _Target = targetobj.transform;\n    }\n    public void Set_OffSet(Vector3 offset)\n    {\n        _OffsetPosition = offset;\n    }\n}\n"),
        new Tool_QuickStart_Script("Movement_CC_FirstPerson",   "Movement_3D",                  "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\n[RequireComponent(typeof(CharacterController))]\npublic class Movement_CC_FirstPerson : MonoBehaviour\n{\n    //Movement\n    [SerializeField] private float _NormalSpeed = 5, _SprintSpeed = 8;\n    [SerializeField] private float _JumpSpeed = 5;\n    [SerializeField] private float _Gravity = 20;\n    private Vector3 _MoveDirection = Vector3.zero;\n    //Look around\n    public float _CameraSensitivity = 1;\n    [SerializeField] private Transform _Head = null;\n    private float _RotationX = 90.0f;\n    private float _RotationY = 0.0f;\n    private float _Speed;\n\n    private CharacterController _CC;\n    private bool _LockRotation;\n\n    void Start()\n    {\n        Cursor.lockState = CursorLockMode.Locked;\n        Cursor.visible = false;\n        _CC = GetComponent<CharacterController>();\n        if (_Head == null)\n            _Head = transform.GetChild(0).transform;\n    }\n\n    void Update()\n    {\n        //Look around\n        if (!_LockRotation)\n        {\n            _RotationX += Input.GetAxis(\"Mouse X\") * _CameraSensitivity;\n            _RotationY += Input.GetAxis(\"Mouse Y\") * _CameraSensitivity;\n            _RotationY = Mathf.Clamp(_RotationY, -90, 90);\n\n            transform.localRotation = Quaternion.AngleAxis(_RotationX, Vector3.up);\n            _Head.transform.localRotation = Quaternion.AngleAxis(_RotationY, Vector3.left);\n        }\n\n        //Movement\n        if (_CC.isGrounded)\n        {\n            _MoveDirection = new Vector3(Input.GetAxis(\"Horizontal\"), 0, Input.GetAxis(\"Vertical\"));\n            _MoveDirection = transform.TransformDirection(_MoveDirection);\n            _MoveDirection *= _Speed;\n            if (Input.GetButton(\"Jump\"))\n                _MoveDirection.y = _JumpSpeed;\n        }\n\n        //Sprint\n        if (Input.GetKey(KeyCode.LeftShift))\n            _Speed = _SprintSpeed;\n        else\n            _Speed = _NormalSpeed;\n\n        //Apply Movement\n        _MoveDirection.y -= _Gravity * Time.deltaTime;\n        _CC.Move(_MoveDirection * Time.deltaTime);\n    }\n\n    public void LockRotation(bool state)\n    {\n        _LockRotation = state;\n    }\n}\n"),
        new Tool_QuickStart_Script("Movement_CC_Platformer",    "Movement_3D",                  "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\n[RequireComponent(typeof(CharacterController))]\npublic class Movement_CC_Platformer : MonoBehaviour\n{\n    [Header(\"Settings\")]\n    [SerializeField] private float _NormalSpeed = 5, _SprintSpeed = 8;\n    [SerializeField] private float _JumpSpeed = 5;\n    [SerializeField] private float _Gravity = 20;\n    [SerializeField] private bool _ZMovementActive = false;\n    \n    private Vector3 _MoveDirection = Vector3.zero;\n    private float _Speed;\n    private CharacterController _CC;\n\n    void Start()\n    {\n        _CC = GetComponent<CharacterController>();\n    }\n\n    void Update()\n    {\n        //Movement\n        if (_CC.isGrounded)\n        {\n            float verticalmovement = 0;\n            if (_ZMovementActive)\n                verticalmovement = Input.GetAxis(\"Vertical\");\n\n            _MoveDirection = new Vector3(Input.GetAxis(\"Horizontal\"), 0, verticalmovement);\n            _MoveDirection = transform.TransformDirection(_MoveDirection);\n            _MoveDirection *= _Speed;\n            if (Input.GetButton(\"Jump\"))\n                _MoveDirection.y = _JumpSpeed;\n        }\n\n        //Sprint\n        if (Input.GetKey(KeyCode.LeftShift))\n            _Speed = _SprintSpeed;\n        else\n            _Speed = _NormalSpeed;\n\n        //Apply Movement\n        _MoveDirection.y -= _Gravity * Time.deltaTime;\n        _CC.Move(_MoveDirection * Time.deltaTime);\n    }\n}\n"),
        new Tool_QuickStart_Script("Movement_CC_TopDown",       "Movement_3D",                  "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\n[RequireComponent(typeof(CharacterController))]\npublic class Movement_CC_TopDown : MonoBehaviour\n{\n    //Movement\n    [Header(\"Settings Camera\")]\n    [SerializeField] private Camera _Camera;\n    [Header(\"Settings\")]\n    [SerializeField] private float _NormalSpeed = 5;\n    [SerializeField] private float _SprintSpeed = 8;\n    [SerializeField] private float _JumpSpeed = 5;\n    [SerializeField] private float _Gravity = 20;\n    [SerializeField] private bool _MovementRelativeToRotation = false;\n\n    private float _Speed = 0;\n    private Vector3 _MoveDirection = Vector3.zero;\n    private CharacterController _CC;\n\n    void Start()\n    {\n        _CC = GetComponent<CharacterController>();\n    }\n\n    void Update()\n    {\n        //Movement\n        if (_CC.isGrounded)\n        {\n            _MoveDirection = new Vector3(Input.GetAxis(\"Horizontal\"), 0, Input.GetAxis(\"Vertical\"));\n            if (_MovementRelativeToRotation)\n                _MoveDirection = transform.TransformDirection(_MoveDirection);\n            _MoveDirection *= _Speed;\n            if (Input.GetButton(\"Jump\"))\n                _MoveDirection.y = _JumpSpeed;\n        }\n\n        _MoveDirection.y -= _Gravity * Time.deltaTime;\n        _CC.Move(_MoveDirection * Time.deltaTime);\n\n        //Sprint\n        if (Input.GetKey(KeyCode.LeftShift))\n            _Speed = _SprintSpeed;\n        else\n            _Speed = _NormalSpeed;\n\n        Ray cameraRay = _Camera.ScreenPointToRay(Input.mousePosition);\n        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);\n        float rayLength;\n        if (groundPlane.Raycast(cameraRay, out rayLength))\n        {\n            Vector3 pointToLook = cameraRay.GetPoint(rayLength);\n            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));\n        }\n    }\n\n    public void SetCamera(Camera cameraobj)\n    {\n        _Camera = cameraobj;\n    }\n}\n"),
        new Tool_QuickStart_Script("Movement_FreeCamera",       "Movement_3D_Camera",           "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Movement_FreeCamera : MonoBehaviour\n{\n    [SerializeField] private float _Speed = 5;\n    [SerializeField] private float _SprintSpeed = 8;\n\n    private float _CurrentSpeed;\n\n    void Start()\n    {\n        Cursor.visible = false;\n        Cursor.lockState = CursorLockMode.Locked;\n    }\n\n    void Update()\n    {\n        if (Input.GetKey(KeyCode.LeftShift))\n            _CurrentSpeed = _SprintSpeed;\n        else\n            _CurrentSpeed = _Speed;\n\n        float xas = Input.GetAxis(\"Horizontal\");\n        float zas = Input.GetAxis(\"Vertical\");\n\n        transform.Translate(new Vector3(xas,0, zas) * _CurrentSpeed * Time.deltaTime);\n\n        float mousex = Input.GetAxis(\"Mouse X\");\n        float mousey = Input.GetAxis(\"Mouse Y\");\n        transform.eulerAngles += new Vector3(-mousey, mousex, 0);\n    }\n}\n"),
        new Tool_QuickStart_Script("Movement_RB_FirstPerson",   "Movement_3D",                  "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\n[RequireComponent(typeof(Rigidbody))]\npublic class Movement_RB_FirstPerson : MonoBehaviour\n{\n    [Header(\"Set Refference\")]\n    [SerializeField] private Transform _Head = null;\n\n    [Header(\"Settings\")]\n    [SerializeField] private float _MovementSpeed = 5;\n    [SerializeField] private float _JumpSpeed = 5;\n    [SerializeField] private float _CameraSensitivity = 1;\n\n    private Vector2 _LookRot = new Vector2(90,0);\n    private Rigidbody _RB;\n    private bool _Grounded;\n\n    void Start()\n    {\n        Cursor.lockState = CursorLockMode.Locked;\n        Cursor.visible = false;\n\n        _RB = GetComponent<Rigidbody>();\n    }\n\n    void Update()\n    {\n        //Check Grounded\n        _Grounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), 0.4f);\n\n        //Movement\n        float x = Input.GetAxisRaw(\"Horizontal\") * _MovementSpeed;\n        float y = Input.GetAxisRaw(\"Vertical\") * _MovementSpeed;\n\n        //Jump\n        if (Input.GetKeyDown(KeyCode.Space) && _Grounded)\n            _RB.velocity = new Vector3(_RB.velocity.x, _JumpSpeed, _RB.velocity.z);\n\n        //Apply Movement\n        Vector3 move = transform.right * x + transform.forward * y;\n        _RB.velocity = new Vector3(move.x, _RB.velocity.y, move.z);\n\n        //Look around\n        _LookRot.x += Input.GetAxis(\"Mouse X\") * _CameraSensitivity;\n        _LookRot.y += Input.GetAxis(\"Mouse Y\") * _CameraSensitivity;\n        _LookRot.y = Mathf.Clamp(_LookRot.y, -90, 90);\n\n        transform.localRotation = Quaternion.AngleAxis(_LookRot.x, Vector3.up);\n        _Head.transform.localRotation = Quaternion.AngleAxis(_LookRot.y, Vector3.left);\n    }\n}\n"),
        //new Tool_QuickStart_Script("MVD_Namespace",             "",                             "wip",            "",        "using UnityEngine;\nusing System.Collections;\n\nnamespace MVD\n{\n    public class mvd : MonoBehaviour\n    {\n\n        //Movement\n        public static void Movement_2D_Platformer(float speed, float shiftspeed, Rigidbody rigidbody, float jumpspeed, float groundcheck, LayerMask groundlayer, Transform playertransform)\n        {\n            float currentspeed;\n\n            //Sprint\n            if (Input.GetKey(KeyCode.LeftShift))\n                currentspeed = shiftspeed;\n            else\n                currentspeed = speed;\n\n            //Jumping\n            if (Input.GetButtonDown(\"Jump\") && IsGrounded(groundcheck, groundlayer, playertransform))\n                rigidbody.AddForce(new Vector2(0, jumpspeed));\n\n            //Apply Movement\n            rigidbody.velocity = new Vector2(Input.GetAxis(\"Horizontal\") * currentspeed, rigidbody.velocity.y);\n        }\n        static bool IsGrounded(float groundcheck, LayerMask groundlayer, Transform playertransform)\n        {\n            RaycastHit2D hit = Physics2D.Raycast(playertransform.position, Vector2.down, groundcheck, groundlayer);\n            if (hit.collider != null)\n            {\n                return true;\n            }\n            return false;\n        }\n        public static void Movement_2D_TopDown(float speed, float shiftspeed, Rigidbody rigidbody)\n        {\n            float currentspeed;\n\n            //Sprint\n            if (Input.GetKey(KeyCode.LeftShift))\n                currentspeed = shiftspeed;\n            else\n                currentspeed = speed;\n\n            //Apply Movement\n            rigidbody.velocity = new Vector2(Input.GetAxis(\"Horizontal\") * currentspeed, Input.GetAxis(\"Vertical\") * currentspeed);\n\n        }\n        public static void Movement_FreeCamera(float speed, float shiftspeed, Transform cameraobj)\n        {\n            Cursor.visible = false;\n            Cursor.lockState = CursorLockMode.Locked;\n\n            float _CurrentSpeed;\n\n            if (Input.GetKey(KeyCode.LeftShift))\n                _CurrentSpeed = shiftspeed;\n            else\n                _CurrentSpeed = speed;\n\n            float xas = Input.GetAxis(\"Horizontal\");\n            float zas = Input.GetAxis(\"Vertical\");\n\n            cameraobj.Translate(new Vector3(xas, 0, zas) * _CurrentSpeed * Time.deltaTime);\n\n            float mousex = Input.GetAxis(\"Mouse X\");\n            float mousey = Input.GetAxis(\"Mouse Y\");\n            cameraobj.transform.eulerAngles += new Vector3(-mousey, mousex, 0);\n        }\n\n\n        \n    }\n}\n"),
        new Tool_QuickStart_Script("ObjectPool",                "ObjectPool",                   "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class ObjectPool : MonoBehaviour\n{\n    [SerializeField] private ObjectPool_Pool[] _ObjectPools = null;\n    private List<Transform> _Parents = new List<Transform>();\n\n    private void Awake()\n    {\n        GameObject emptyobject = GameObject.CreatePrimitive(PrimitiveType.Cube);\n        Destroy(emptyobject.GetComponent<MeshRenderer>());\n        Destroy(emptyobject.GetComponent<BoxCollider>());\n\n        for (int i = 0; i < _ObjectPools.Length; i++)\n        {\n            //Create parent\n            GameObject poolparent = Instantiate(emptyobject, transform.position, Quaternion.identity);\n            Destroy(poolparent.GetComponent<MeshRenderer>());\n            Destroy(poolparent.GetComponent<BoxCollider>());\n\n            //Set parent\n            poolparent.transform.parent = transform;\n            poolparent.transform.name = \"Pool_\" + _ObjectPools[i]._Name;\n            _Parents.Add(poolparent.transform);\n\n            //Create objects\n            for (int o = 0; o < _ObjectPools[i]._Amount; o++)\n            {\n                GameObject obj = (GameObject)Instantiate(_ObjectPools[i]._Prefab);\n                obj.transform.parent = poolparent.transform;\n                obj.transform.position = new Vector2(9999, 9999);\n                obj.SetActive(false);\n                _ObjectPools[i]._Objects.Add(obj);\n            }\n        }\n        Destroy(emptyobject);\n    }\n\n    //GetObject\n    public GameObject GetObject(string objname)\n    {\n        int id = FindObjectPoolID(objname, false);\n        return GetObject(id, true);\n    }\n    public GameObject GetObject(GameObject obj)\n    {\n        int id = FindObjectPoolID(obj);\n        return GetObject(id, true);\n    }\n    public GameObject GetObjectPrefabName(string prefabname)\n    {\n        int id = FindObjectPoolID(prefabname, true);\n        return GetObject(id, true);\n    }\n\n    //GetObject/setactive\n    public GameObject GetObject(string objname, bool setactive)\n    {\n        int id = FindObjectPoolID(objname, false);\n        return GetObject(id, setactive);\n    }\n    public GameObject GetObject(GameObject obj, bool setactive)\n    {\n        int id = FindObjectPoolID(obj);\n        return GetObject(id, setactive);\n    }\n    public GameObject GetObjectPrefabName(string prefabname, bool setactive)\n    {\n        int id = FindObjectPoolID(prefabname, true);\n        return GetObject(id, setactive);\n    }\n\n    public GameObject GetObject(int id, bool setactive)\n    {\n        GameObject freeObject = null;\n        bool checkfreeobj = false;\n\n        for (int i = 0; i < _ObjectPools[id]._Objects.Count; i++)\n        {\n            if (!_ObjectPools[id]._Objects[i].activeInHierarchy)\n            {\n                _ObjectPools[id]._Objects[i].transform.position = new Vector3(999, 999, 999);\n                _ObjectPools[id]._Objects[i].SetActive(setactive);\n                freeObject = _ObjectPools[id]._Objects[i];\n                return freeObject;\n            }\n        }\n\n        if (!checkfreeobj)\n        {\n            _ObjectPools[id]._Objects.Clear();\n            freeObject = (GameObject)Instantiate(_ObjectPools[id]._Prefab, new Vector3(999,999,999), Quaternion.identity);\n            freeObject.transform.parent = _Parents[id];\n            freeObject.SetActive(setactive);\n            _ObjectPools[id]._Objects.Add(freeObject);\n            return freeObject;\n        }\n\n        Debug.Log(\"No Object Found\");\n        return null;\n    }\n\n    public List<GameObject> GetAllObjects(GameObject objtype)\n    {\n        int id = FindObjectPoolID(objtype);\n        return _ObjectPools[id]._Objects;\n    }\n\n    private int FindObjectPoolID(GameObject obj)\n    {\n        int id = 0;\n        for (int i = 0; i < _ObjectPools.Length; i++)\n        {\n            if (obj == _ObjectPools[i]._Prefab)\n            {\n                id = i;\n            }\n        }\n        return id;\n    }\n    private int FindObjectPoolID(string objname, bool isprefab)\n    {\n        for (int i = 0; i < _ObjectPools.Length; i++)\n        {\n            if (isprefab)\n                if (objname == _ObjectPools[i]._Prefab.name)\n                {\n                    return i;\n                }\n                else\n            if (objname == _ObjectPools[i]._Name)\n                {\n                    return i;\n                }\n        }\n        Debug.Log(objname + \" Not Found\");\n        return 0;\n    }\n}\n\n[System.Serializable]\npublic class ObjectPool_Pool\n{\n    public string _Name;\n    public GameObject _Prefab;\n    public int _Amount;\n    [HideInInspector] public List<GameObject> _Objects;\n}\n"),
        new Tool_QuickStart_Script("ObjectPoolSimple",          "ObjectPool",                   "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class ObjectPoolSimple : MonoBehaviour\n{\n    public GameObject prefabGameObject;\n    public int pooledAmount;\n\n    [HideInInspector] public List<GameObject> objects;\n\n    void Awake()\n    {\n        for (int i = 0; i < pooledAmount; i++)\n        {\n            GameObject obj = (GameObject)Instantiate(prefabGameObject);\n            obj.transform.parent = gameObject.transform;\n            obj.SetActive(false);\n            objects.Add(obj);\n        }\n    }\n}\n\n\n/* Use Pool\n    \n    [SerializeField]private ObjectPoolSimple _ObjectPool;\n\n    private void Spawn() {\n        for (int i = 0; i < _ObjectPool.objects.Count; i++) {\n            if (!_ObjectPool.objects[i].activeInHierarchy) {\n                _ObjectPool.objects[i].transform.position = new Vector3(0,0,0);\n                _ObjectPool.objects[i].transform.rotation = Quaternion.Euler(0, 0, 0);\n                _ObjectPool.objects[i].SetActive(true);\n                break;\n            }\n        }\n    }\n*/\n"),
        new Tool_QuickStart_Script("OnCollision",               "Collision_Practical",          "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEngine.Events;\n\npublic class OnCollision : MonoBehaviour\n{\n    private enum Options {OnTriggerEnter, OnTriggerExit, OnTriggerStay, OnCollisionEnter, OnCollisionExit, OnCollisionStay, OnAll};\n    [SerializeField] private LayerMask _LayerMask = ~0;\n    [SerializeField] private Options _Option = Options.OnAll;\n    [SerializeField] private string _Tag = \"\";\n    [SerializeField] private UnityEvent _Event = null;\n\n    private bool _HasTag;\n\n    private void Start()\n    {\n        if (_Tag != \"\" || _Tag != null)\n            _HasTag = true;\n    }\n\n    private void Action(Collider other)\n    {\n        if (_HasTag)\n            if (other.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)\n                _Event.Invoke();\n    }\n    private void Action(Collision other)\n    {\n        if (_HasTag)\n            if (other.gameObject.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)\n                _Event.Invoke();\n    }\n\n    private void OnTriggerEnter(Collider other)\n    {\n        if (_Option == Options.OnTriggerEnter || _Option == Options.OnAll)\n            Action(other);\n    }\n    private void OnTriggerExit(Collider other)\n    {\n        if (_Option == Options.OnTriggerExit || _Option == Options.OnAll)\n            Action(other);\n    }\n    private void OnTriggerStay(Collider other)\n    {\n        if (_Option == Options.OnTriggerStay || _Option == Options.OnAll)\n            Action(other);\n    }\n    private void OnCollisionEnter(Collision other)\n    {\n        if (_Option == Options.OnCollisionEnter || _Option == Options.OnAll)\n            Action(other);\n    }\n    private void OnCollisionExit(Collision other)\n    {\n        if (_Option == Options.OnCollisionExit || _Option == Options.OnAll)\n            Action(other);\n    }\n    private void OnCollisionStay(Collision other)\n    {\n        if (_Option == Options.OnCollisionStay || _Option == Options.OnAll)\n            Action(other);\n    }\n}\n"),
        new Tool_QuickStart_Script("OnCollision2D",             "Collision_2D_Practical",       "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEngine.Events;\n\npublic class OnCollision2D : MonoBehaviour\n{\n    private enum Options {OnTriggerEnter, OnTriggerExit, OnTriggerStay, OnCollisionEnter, OnCollisionExit, OnCollisionStay, OnAll};\n    [SerializeField] private LayerMask _LayerMask = ~0;\n    [SerializeField] private Options _Option = Options.OnAll;\n    [SerializeField] private string _Tag = \"\";\n    [SerializeField] private UnityEvent _Event = null;\n\n    private bool _HasTag;\n\n    private void Start()\n    {\n        if (_Tag != \"\" || _Tag != null)\n            _HasTag = true;\n    }\n\n    private void Action(Collider2D other)\n    {\n        if (_HasTag)\n            if (other.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)\n                _Event.Invoke();\n    }\n    private void Action(Collision2D other)\n    {\n        if (_HasTag)\n            if (other.gameObject.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)\n                _Event.Invoke();\n    }\n\n    private void OnTriggerEnter2D(Collider2D other)\n    {\n        if (_Option == Options.OnTriggerEnter || _Option == Options.OnAll)\n            Action(other);\n    }\n    private void OnTriggerExit2D(Collider2D other)\n    {\n        if (_Option == Options.OnTriggerExit || _Option == Options.OnAll)\n            Action(other);\n    }\n    private void OnTriggerStay2D(Collider2D other)\n    {\n        if (_Option == Options.OnTriggerStay || _Option == Options.OnAll)\n            Action(other);\n    }\n    private void OnCollisionEnter2D(Collision2D other)\n    {\n        if (_Option == Options.OnCollisionEnter || _Option == Options.OnAll)\n            Action(other);\n    }\n    private void OnCollisionExit2D(Collision2D other)\n    {\n        if (_Option == Options.OnCollisionExit || _Option == Options.OnAll)\n            Action(other);\n    }\n    private void OnCollisionStay2D(Collision2D other)\n    {\n        if (_Option == Options.OnCollisionStay || _Option == Options.OnAll)\n            Action(other);\n    }\n}\n"),
        new Tool_QuickStart_Script("PauseMenu",                 "Menu_Practical",               "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEngine.SceneManagement;\n\npublic class PauseMenu : MonoBehaviour\n{\n    [SerializeField] private GameObject _PauseMenu;\n\n    void Update()\n    {\n        if(Input.GetKeyDown(KeyCode.Escape))\n        {\n            _PauseMenu.SetActive(!_PauseMenu.activeSelf);\n\n            if (_PauseMenu.activeSelf)\n                Time.timeScale = 0;\n            else\n                Time.timeScale = 1;\n        }\n    }\n\n    public void LoadScene(int sceneid)\n    {\n        SceneManager.LoadScene(sceneid);\n        Time.timeScale = 1;\n    }\n\n    public void LoadScene(string scenename)\n    {\n        SceneManager.LoadScene(scenename);\n        Time.timeScale = 1;\n    }\n\n    public void Resume()\n    {\n        _PauseMenu.SetActive(false);\n        Time.timeScale = 1;\n    }\n}\n"),
        new Tool_QuickStart_Script("PosToPos",                  "Practical",                    "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class PosToPos : MonoBehaviour\n{\n    [SerializeField] private Transform _GotoPosition = null;\n    [SerializeField] private float _Speed = 0;\n\n    private bool _Activated;\n\n    void Update()\n    {\n        if (_Activated)\n            transform.position = Vector3.MoveTowards(transform.position, _GotoPosition.position, _Speed);\n    }\n\n    public void StartMoving()\n    {\n        _Activated = true;\n    }\n}\n"),
        //new Tool_QuickStart_Script("Read_ExcelFile",            "Read_Excel_File",              "wip",            "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Read_ExcelFile : MonoBehaviour\n{\n    // Start is called before the first frame update\n    void Start()\n    {\n        \n    }\n\n    // Update is called once per frame\n    void Update()\n    {\n        \n    }\n}\n"),
        new Tool_QuickStart_Script("ReadAudioFile",             "Read_File_Audio",              "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class ReadAudioFile : MonoBehaviour\n{\n    [Header(\"Settings\")]\n    private AudioSource _AudioScource = null;\n    private bool _PlayOnStart = true;\n\n    [Header(\"Info\")]\n    public float[] Samples = new float[512];\n    public float[] FreqBand = new float[8];\n    public float[] BandBuffer = new float[8];\n    public float[] BufferDecrease = new float[8];\n\n    void Start()\n    {\n        if (_AudioScource == null)\n            _AudioScource = GetComponent<AudioSource>();\n\n        if (_PlayOnStart)\n            _AudioScource.Play();\n    }\n\n    void Update()\n    {\n        GetSpectrumAudioSource();\n        MakeFrequencyBands();\n        UpdateBandBuffer();\n    }\n\n    void GetSpectrumAudioSource()\n    {\n        _AudioScource.GetSpectrumData(Samples, 0, FFTWindow.Blackman);\n    }\n\n    void UpdateBandBuffer()\n    {\n        for (int i = 0; i < 8; i++)\n        {\n            if (FreqBand[i] > BandBuffer[i])\n            {\n                BandBuffer[i] = FreqBand[i];\n                BufferDecrease[i] = 0.005f;\n            }\n            if (FreqBand[i] < BandBuffer[i])\n            {\n                BandBuffer[i] -= BufferDecrease[i];\n                BufferDecrease[i] *= 1.2f;\n            }\n        }\n    }\n\n    void MakeFrequencyBands()\n    {\n        float average = 0;\n        int count = 0;\n\n        for (int i = 0; i < 8; i++)\n        {\n            int sampleCount = (int)Mathf.Pow(2, i) * 2;\n\n            if (i == 7)\n                sampleCount += 2;\n\n            for (int j = 0; j < sampleCount; j++)\n            {\n                average += Samples[count] * (count + 1);\n                count++;\n            }\n\n            average /= count;\n            FreqBand[i] = average * 10;\n        }\n    }\n}\n"),
        new Tool_QuickStart_Script("ReadTwitchChat",            "Networking",                   "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing System;\nusing System.ComponentModel;\nusing System.Net.Sockets;\nusing System.IO;\n\npublic class ReadTwitchChat : MonoBehaviour\n{\n    [Header(\"Settings\")]\n    [SerializeField] private float _RefreshConnectionTimer = 60;\n    private float _Timer;\n\n    [Header(\"Twitch\")]\n    private TcpClient twitchClient;\n    private StreamReader reader;\n    private StreamWriter writer;\n\n    [SerializeField] private string _Username = \"\"; //Twitch user name\n    [SerializeField] private string _OauthToken = \"\"; //Get token from https://twitchapps.com/tmi\n    [SerializeField] private string _Channelname = \"\"; //Twitch channel name\n\n    void Start()\n    {\n        Connect();\n    }\n\n    void Update()\n    {\n        //Check connection\n        if (!twitchClient.Connected)\n            Connect();\n\n        _Timer -= 1 * Time.deltaTime;\n        if (_Timer <= 0)\n        {\n            Connect();\n            _Timer = _RefreshConnectionTimer;\n        }\n\n        ReadChat();\n    }\n\n    private void Connect()\n    {\n        twitchClient = new TcpClient(\"irc.chat.twitch.tv\", 6667);\n        reader = new StreamReader(twitchClient.GetStream());\n        writer = new StreamWriter(twitchClient.GetStream());\n\n        writer.WriteLine(\"PASS \" + _OauthToken);\n        writer.WriteLine(\"NICK \" + _Username);\n        writer.WriteLine(\"USER \" + _Username + \" 8 * :\" + _Username);\n        writer.WriteLine(\"JOIN #\" + _Channelname);\n\n        writer.Flush();\n    }\n\n    private void ReadChat()\n    {\n        if (twitchClient.Available > 0)\n        {\n            var message = reader.ReadLine();\n\n            if (message.Contains(\"PRIVMSG\"))\n            {\n                //Split\n                var splitPoint = message.IndexOf(\"!\", 1);\n                var chatName = message.Substring(0, splitPoint);\n\n                //Name\n                chatName = chatName.Substring(1);\n\n                //Message\n                splitPoint = message.IndexOf(\":\", 1);\n                message = message.Substring(splitPoint + 1);\n                print(string.Format(\"{0}: {1}\", chatName, message));\n\n                if (message.ToLower().Contains(\"example\"))\n                {\n                    Debug.Log(\"<color=green>\" + chatName + \" has used the command example </color>\");\n                }\n            }\n        }\n    }\n\n}\n"),
        new Tool_QuickStart_Script("ReadWrite_TextFile",        "Read_Write_File",              "stable",           "",        "using UnityEngine;\nusing System.IO;\n\npublic class ReadWrite_TextFile : MonoBehaviour\n{\n    [SerializeField] private string _Path = \"\";\n    [SerializeField] private string _FileName = \"ExampleTextFile\";\n\n    [Header(\"Example\")]\n    [SerializeField] private string _Message = \"Test Message\";\n\n    void Start()\n    {\n        if (_Path == \"\")\n        {\n            _Path = \"Assets/\" + _FileName;\n        }\n\n        WriteTextFile();\n        ReadTextFile();\n    }\n\n    public void ReadTextFile()\n    {\n        StreamReader reader = new StreamReader(_Path + \".txt\");\n        Debug.Log(\"Read Result: \" + reader.ReadToEnd());\n        reader.Close();\n    }\n\n    public void WriteTextFile()\n    {\n        StreamWriter writer = new StreamWriter(_Path + \".txt\", true);\n        writer.WriteLine(_Message);\n        writer.Close();\n        Debug.Log(\"Write Complete\");\n    }\n}\n"),
        new Tool_QuickStart_Script("Rotation",                  "Practical",                    "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Rotation : MonoBehaviour\n{\n    [SerializeField] private Vector3 _RotationSpeed = Vector3.zero;\n\n    void Update()\n    {\n        transform.Rotate(new Vector3(_RotationSpeed.x, _RotationSpeed.y, _RotationSpeed.z) * Time.deltaTime);\n    }\n}\n"),
        new Tool_QuickStart_Script("SaveLoad_JSON",             "Json_Save_Load",               "stable",           "",        "using System.Collections.Generic;\nusing System.IO;\nusing UnityEngine;\n\npublic class SaveLoad_JSON : MonoBehaviour\n{\n    private Json_SaveData _SaveData = new Json_SaveData();\n\n    void Start()\n    {\n        LoadData();\n    }\n\n    public void SaveData()\n    {\n        string jsonData = JsonUtility.ToJson(_SaveData, true);\n        File.WriteAllText(Application.persistentDataPath + \"/SaveData.json\", jsonData);\n    }\n    public void LoadData()\n    {\n        try\n        {\n            string dataAsJson = File.ReadAllText(Application.persistentDataPath + \"/SaveData.json\");\n            _SaveData = JsonUtility.FromJson<Json_SaveData>(dataAsJson);\n        }\n        catch\n        {\n            SaveData();\n        }\n    }\n    public Json_SaveData GetSaveData()\n    {\n        return _SaveData;\n    }\n    public void CreateNewSave()\n    {\n        Json_ExampleData newsave = new Json_ExampleData();\n        newsave.exampleValue = 10;\n        _SaveData.saveData.Add(newsave);\n    }\n}\n\n[System.Serializable]\npublic class Json_SaveData\n{\n    public List <Json_ExampleData> saveData = new List<Json_ExampleData>();\n}\n[System.Serializable]\npublic class Json_ExampleData\n{\n    public float exampleValue = 0;\n}\n"),
        new Tool_QuickStart_Script("SaveLoad_XML",              "XML_Save_Load",                "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing System.Xml.Serialization;\nusing System.IO;\n\npublic class SaveLoad_XML : MonoBehaviour\n{\n    private XML_SaveData _SaveData = new XML_SaveData();\n\n    void Start()\n    {\n        LoadData();\n    }\n\n    public void SaveData()\n    {\n        XmlSerializer serializer = new XmlSerializer(typeof(XML_SaveData));\n\n        using (FileStream stream = new FileStream(Application.persistentDataPath + \"/SaveData.xml\", FileMode.Create))\n        {\n            serializer.Serialize(stream, _SaveData);\n        }\n    }\n\n    public void LoadData()\n    {\n        try\n        {\n            XmlSerializer serializer = new XmlSerializer(typeof(XML_SaveData));\n\n            using (FileStream stream = new FileStream(Application.persistentDataPath + \"/SaveData.xml\", FileMode.Open))\n            {\n                _SaveData = serializer.Deserialize(stream) as XML_SaveData;\n            }\n        }\n        catch\n        {\n            SaveData();\n        }\n    }\n\n    public XML_SaveData GetSaveData()\n    {\n        return _SaveData;\n    }\n    public void CreateNewSave()\n    {\n        XML_ExampleData newsave = new XML_ExampleData();\n        newsave.exampleValue = 10;\n        _SaveData.saveData.Add(newsave);\n    }\n}\n\n[System.Serializable]\npublic class XML_SaveData\n{\n    public List<XML_ExampleData> saveData = new List<XML_ExampleData>();\n}\n[System.Serializable]\npublic class XML_ExampleData\n{\n    public float exampleValue = 0;\n}\n"),
        new Tool_QuickStart_Script("ScriptebleGameObject",      "SO_ScriptebleGameObject",      "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\n[CreateAssetMenu(fileName = \"Example\", menuName = \"SO/ExampleSO\", order = 1)]\npublic class ScriptebleGameObject : ScriptableObject\n{\n    public string examplestring;\n    public int exampleint;\n}\n"),
        //new Tool_QuickStart_Script("SettingsHandler",           "Settings_Handler",             "wip",            "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing TMPro;\nusing UnityEngine.Audio;\nusing UnityEngine.UI;\nusing System;\n\npublic class SettingsHandler : MonoBehaviour\n{\n    [SerializeField] private AudioMixer _AudioMixer = null;\n    [SerializeField] private float _Current_Volume = 1;\n\n\n    [SerializeField] private TMP_Dropdown _Dropdown_Resolution = null;\n    [SerializeField] private TMP_Dropdown _Dropdown_Quality = null;\n    [SerializeField] private TMP_Dropdown _Dropdown_Texture = null;\n    [SerializeField] private TMP_Dropdown _Dropdown_AA = null;\n    [SerializeField] private Slider _Slider_Volume = null;\n\n    [SerializeField] private Resolution[] _Resolutions = null;\n\n    private void Start()\n    {\n        _Resolutions = Screen.resolutions;\n        _Dropdown_Resolution.ClearOptions();\n        List<string> options = new List<string>();\n\n        int currentresid = 0;\n        for (int i = 0; i < _Resolutions.Length; i++)\n        {\n            string option = _Resolutions[i].width + \" x \" + _Resolutions[i].height;\n            options.Add(option);\n\n            if (_Resolutions[i].width == Screen.currentResolution.width && _Resolutions[i].height == Screen.currentResolution.height)\n                currentresid = i;\n        }\n\n        _Dropdown_Resolution.AddOptions(options);\n        _Dropdown_Resolution.value = currentresid;\n        _Dropdown_Resolution.RefreshShownValue();\n    }\n\n    // [Display]\n    //Resolution\n    public void Set_Resolution(int resid)\n    {\n        Resolution resolution = _Resolutions[resid];\n        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);\n    }\n\n    //FullScreen\n    public void Set_FullScreen(bool isFullscreen)\n    {\n        Screen.fullScreen = isFullscreen;\n    }\n\n    //Quiality\n    public void Set_Quality(int qualityid)\n    {\n        if (qualityid != 6) // if the user is not using \n                               //any of the presets\n            QualitySettings.SetQualityLevel(qualityid);\n        switch (qualityid)\n        {\n            case 0: // quality level - very low\n                _Dropdown_Texture.value = 3;\n                _Dropdown_AA.value = 0;\n                break;\n            case 1: // quality level - low\n                _Dropdown_Texture.value = 2;\n                _Dropdown_AA.value = 0;\n                break;\n            case 2: // quality level - medium\n                _Dropdown_Texture.value = 1;\n                _Dropdown_AA.value = 0;\n                break;\n            case 3: // quality level - high\n                _Dropdown_Texture.value = 0;\n                _Dropdown_AA.value = 0;\n                break;\n            case 4: // quality level - very high\n                _Dropdown_Texture.value = 0;\n                _Dropdown_AA.value = 1;\n                break;\n            case 5: // quality level - ultra\n                _Dropdown_Texture.value = 0;\n                _Dropdown_AA.value = 2;\n                break;\n        }\n\n        _Dropdown_Quality.value = qualityid;\n    }\n\n    //Vsync\n    //MaxFOS\n    //Gama\n\n    // [Grapics]\n    //Antialiasing\n    public void SetAntiAliasing(int aaid)\n    {\n        QualitySettings.antiAliasing = aaid;\n        _Dropdown_Quality.value = 6;\n    }\n\n    //Shadows\n    //ViewDistance\n    //TextureQuality\n    public void Set_TextureQuality(int textureid)\n    {\n        QualitySettings.masterTextureLimit = textureid;\n        _Dropdown_Quality.value = 6;\n    }\n\n    //ViolageDistance\n    //ViolageDensity\n\n    // [Gameplay]\n    //SoundAll\n    public void Set_Volume(float volume)\n    {\n        _AudioMixer.SetFloat(\"Volume\", volume);\n        _Current_Volume = volume;\n    }\n\n    //SoundEffects\n    //Music\n\n    // Quit / Save / Load\n    public void ExitGame()\n    {\n        Application.Quit();\n    }\n    public void SaveSettings()\n    {\n        PlayerPrefs.SetInt(\"QualitySettingPreference\",\n                   _Dropdown_Quality.value);\n        PlayerPrefs.SetInt(\"ResolutionPreference\",\n                   _Dropdown_Resolution.value);\n        PlayerPrefs.SetInt(\"TextureQualityPreference\",\n                   _Dropdown_Texture.value);\n        PlayerPrefs.SetInt(\"AntiAliasingPreference\",\n                   _Dropdown_AA.value);\n        PlayerPrefs.SetInt(\"FullscreenPreference\",\n                   Convert.ToInt32(Screen.fullScreen));\n        PlayerPrefs.SetFloat(\"VolumePreference\",\n                   _Current_Volume);\n    }\n    public void LoadSettings(int currentResolutionIndex)\n    {\n        if (PlayerPrefs.HasKey(\"QualitySettingPreference\"))\n            _Dropdown_Quality.value =\n                         PlayerPrefs.GetInt(\"QualitySettingPreference\");\n        else\n            _Dropdown_Quality.value = 3;\n        if (PlayerPrefs.HasKey(\"ResolutionPreference\"))\n            _Dropdown_Resolution.value =\n                         PlayerPrefs.GetInt(\"ResolutionPreference\");\n        else\n            _Dropdown_Resolution.value = currentResolutionIndex;\n        if (PlayerPrefs.HasKey(\"TextureQualityPreference\"))\n            _Dropdown_Texture.value =\n                         PlayerPrefs.GetInt(\"TextureQualityPreference\");\n        else\n            _Dropdown_Texture.value = 0;\n        if (PlayerPrefs.HasKey(\"AntiAliasingPreference\"))\n            _Dropdown_AA.value =\n                         PlayerPrefs.GetInt(\"AntiAliasingPreference\");\n        else\n            _Dropdown_AA.value = 1;\n        if (PlayerPrefs.HasKey(\"FullscreenPreference\"))\n            Screen.fullScreen =\n            Convert.ToBoolean(PlayerPrefs.GetInt(\"FullscreenPreference\"));\n        else\n            Screen.fullScreen = true;\n        if (PlayerPrefs.HasKey(\"VolumePreference\"))\n            _Slider_Volume.value =\n                        PlayerPrefs.GetFloat(\"VolumePreference\");\n        else\n            _Slider_Volume.value =\n                        PlayerPrefs.GetFloat(\"VolumePreference\");\n    }\n\n\n    //Set\n    public void SetDropDown_Resolution(TMP_Dropdown resolutions)\n    {\n        _Dropdown_Resolution = resolutions;\n    }\n    public void SetDropDown_Quality(TMP_Dropdown quality)\n    {\n        _Dropdown_Quality = quality;\n    }\n    public void SetDropDown_TextureQuality(TMP_Dropdown texturequality)\n    {\n        _Dropdown_Texture = texturequality;\n    }\n    public void SetDropDown_AA(TMP_Dropdown aa)\n    {\n        _Dropdown_AA = aa;\n    }\n    public void SetSlider_VolumeSlider(Slider volumeslider)\n    {\n        _Slider_Volume = volumeslider;\n    }\n}\n"),
        new Tool_QuickStart_Script("Shooting",                  "Shooting",                     "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Shooting : MonoBehaviour\n{\n    [Header(\"Settings\")]\n    [SerializeField] ObjectPool _ObjectPool = null;\n    [SerializeField] private GameObject _BulletPrefab = null;\n    [SerializeField] private GameObject _ShootPoint = null;\n\n    [Header(\"Semi\")]\n    [SerializeField] private int _SemiAutomaticBulletAmount = 3;\n    [SerializeField] private float _SemiShootSpeed = 0.2f;\n    [Header(\"Automatic\")]\n    [SerializeField] private float _SecondsBetweenShots = 0.5f;\n\n    private enum ShootModes { SingleShot, SemiAutomatic, Automatic }\n    [SerializeField] private ShootModes _ShootMode = ShootModes.SingleShot;\n\n    private bool _CheckSingleShot;\n    private float _Timer;\n    private bool _LockShooting;\n\n    void Update()\n    {\n        if (Input.GetMouseButton(0))\n        {\n            switch (_ShootMode)\n            {\n                case ShootModes.SingleShot:\n                    if (!_CheckSingleShot)\n                        Shoot();\n                    _CheckSingleShot = true;\n                    break;\n                case ShootModes.SemiAutomatic:\n                    if (!_CheckSingleShot && !_LockShooting)\n                        StartCoroutine(SemiShot());\n                    _CheckSingleShot = true;\n                    break;\n                case ShootModes.Automatic:\n                    _Timer += 1 * Time.deltaTime;\n                    if (_Timer >= _SecondsBetweenShots)\n                    {\n                        Shoot();\n                        _Timer = 0;\n                    }\n                    break;\n            }\n        }\n        if (Input.GetMouseButtonUp(0))\n        {\n            _CheckSingleShot = false;\n        }\n    }\n\n    IEnumerator SemiShot()\n    {\n        _LockShooting = true;\n        for (int i = 0; i < _SemiAutomaticBulletAmount; i++)\n        {\n            Shoot();\n            yield return new WaitForSeconds(_SemiShootSpeed);\n        }\n        _LockShooting = false;\n    }\n\n    void Shoot()\n    {\n       GameObject bullet = _ObjectPool.GetObject(_BulletPrefab, true);\n        bullet.SetActive(true);\n        bullet.transform.position = _ShootPoint.transform.position;\n        bullet.transform.rotation = _ShootPoint.transform.rotation;\n    }\n}\n"),
        new Tool_QuickStart_Script("ShootingRayCast",           "Shooting",                     "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing System.Threading;\nusing UnityEngine;\n\npublic class ShootingRayCast : MonoBehaviour\n{\n    [Header(\"Settings\")]\n    [SerializeField] private float _Damage = 20;\n    [SerializeField] private float _ShootDistance = 50;\n    [SerializeField] private string _EnemyTag = \"Enemy\";\n\n    [Header(\"Semi\")]\n    [SerializeField] private int _SemiAutomaticBulletAmount = 3;\n    [SerializeField] private float _SemiShootSpeed = 0.2f;\n    [Header(\"Automatic\")]\n    [SerializeField] private float _SecondsBetweenShots = 0.5f;\n\n    private enum ShootModes {SingleShot, SemiAutomatic, Automatic }\n    [SerializeField] private ShootModes _ShootMode = ShootModes.SingleShot;\n\n    private bool _CheckSingleShot;\n    private float _Timer;\n    private bool _LockShooting;\n\n    void Update()\n    {\n        if (Input.GetMouseButton(0))\n        {\n            switch (_ShootMode)\n            {\n                case ShootModes.SingleShot:\n                    if (!_CheckSingleShot)\n                        Shoot();\n                    _CheckSingleShot = true;\n                    break;\n                case ShootModes.SemiAutomatic:\n                    if (!_CheckSingleShot && !_LockShooting)\n                        StartCoroutine(SemiShot());\n                    _CheckSingleShot = true;\n                    break;\n                case ShootModes.Automatic:\n                    _Timer += 1 * Time.deltaTime;\n                    if(_Timer >= _SecondsBetweenShots)\n                    {\n                        Shoot();\n                        _Timer = 0;\n                    }\n                    break;\n            }\n        }\n        if(Input.GetMouseButtonUp(0))\n        {\n            _CheckSingleShot = false;\n        }\n    }\n\n    IEnumerator SemiShot()\n    {\n        _LockShooting = true;\n        for (int i = 0; i < _SemiAutomaticBulletAmount; i++)\n        {\n            Shoot();\n            yield return new WaitForSeconds(_SemiShootSpeed);\n        }\n        _LockShooting = false;\n    }\n\n    void Shoot()\n    {\n        RaycastHit hit;\n        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _ShootDistance))\n            if (hit.transform.tag == _EnemyTag)\n            {\n                hit.transform.GetComponent<Health>().DoDamage(_Damage);\n            }\n    }\n}\n"),
        //new Tool_QuickStart_Script("SpaceShip",                 "Vehicle_Movement",             "wip",            "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class SpaceShip : MonoBehaviour\n{\n    // Start is called before the first frame update\n    void Start()\n    {\n        \n    }\n\n    // Update is called once per frame\n    void Update()\n    {\n        \n    }\n}\n"),
        new Tool_QuickStart_Script("StringFormats",             "String_Format",                "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing TMPro;\n\npublic class StringFormats : MonoBehaviour\n{\n    private enum FormatOptions {DigitalTime };\n    [SerializeField] private FormatOptions _FormatOption = FormatOptions.DigitalTime;\n    [SerializeField] private TextMeshProUGUI _ExampleText = null;\n\n    private float _Timer;\n\n    void Update()\n    {\n        _Timer += 1 * Time.deltaTime;\n\n        switch (_FormatOption)\n        {\n            case FormatOptions.DigitalTime:\n                _ExampleText.text = string.Format(\"{0:00}:{1:00}:{2:00}\", Mathf.Floor(_Timer / 3600), Mathf.Floor((_Timer / 60) % 60), _Timer % 60);\n                break;\n        }\n    }\n}\n"),
        //new Tool_QuickStart_Script("Tool_Commands",             "Tool_Commands",                "wip",            "",        "using UnityEditor.SceneManagement;\nusing System.Collections.Generic;\nusing System.Collections;\nusing UnityEditor;\nusing UnityEngine;\nusing System.IO;\nusing UnityEngine.Events;\n\npublic class Tool_Commands : EditorWindow\n{\n    //WIP\n\n    //Commands\n    private List<string> _ConsoleCommands = new List<string>();\n    private List<string> _ConsoleCommandsResults = new List<string>();\n    private Tool_CustomCommands _Commands = new Tool_CustomCommands(); \n\n    //Search\n    private string _Search = \"\";\n    private string _InfoText = \"\";\n    private string _CommandUsing = \"\";\n    private int _ResultID = 0;\n    private bool _Loaded = false;\n\n    //Menu\n    private string[] _MenuOptions = new string[] {\"Commands\", \"Create Command\", \"Menu\" };\n    private int _MenuSelected = 0;\n\n    private bool _EditMode;\n    private int _CommandID;\n\n    //Create Command\n    private string[] _NewCommandTypes = new string[] {\"Search\", \"Load\", \"Create\" };\n    private string[] _NewCommandType_Search = new string[] {\"Scene\", \"Folder\", \"Objects\" };\n    private string[] _NewCommandType_Load = new string[] { \"Scene\", \"Folder\", \"Objects\" };\n    private string[] _NewCommandType_Create = new string[] { \"Scene\", \"Object\" };\n    private int _NewCommandTypesChosen = 0;\n    private int _NewCommandTypeOptionChosen = 0;\n    private string _NewCommandText = \"\";\n    private int _Type;\n\n    [MenuItem(\"Tools/Commands wip\")]\n    static void Init()\n    {\n        Tool_Commands window = EditorWindow.GetWindow(typeof(Tool_Commands), false, \"Commands\") as Tool_Commands;\n        window.minSize = new Vector2(550, 750);\n        window.maxSize = new Vector2(550, 750);\n        window.Show();\n        window.minSize = new Vector2(50, 50);\n        window.maxSize = new Vector2(9999999, 9999999);\n    }\n\n    //Window / Menu\n    void OnGUI()\n    {   \n        if (!_Loaded)\n        {\n            JsonLoad();\n            _Loaded = true;\n        }\n\n        Menu();\n        InfoBox();\n    }\n    private void Menu()\n    {\n        HandleInput();\n        _MenuSelected = GUILayout.Toolbar(_MenuSelected, _MenuOptions);\n\n        switch (_MenuSelected)\n        {\n            case 0:\n                Search();\n                ShowConsoleCommands();\n                break;\n            case 1:\n                CreateCommands();\n                break;\n            case 2:\n                MenuVieuw();\n                break;\n        }\n    }\n\n    //Input / Search\n    private void HandleInput()\n    {\n        Event e = Event.current;\n        switch (e.type)\n        {\n            case EventType.KeyDown:\n                {\n                    if (Event.current.keyCode == (KeyCode.Return) || Event.current.keyCode == (KeyCode.KeypadEnter))\n                    {\n                        DoCommand();\n                        GUI.FocusControl(\"SearchField\");\n                        GUIUtility.ExitGUI();\n                    }\n                    if (Event.current.keyCode == (KeyCode.DownArrow))\n                    {\n                        if (_ResultID == _ConsoleCommandsResults.Count - 1)\n                            _ResultID = 0;\n                        else\n                            _ResultID++;\n                    }\n                    if (Event.current.keyCode == (KeyCode.UpArrow))\n                    {\n                        if (_ResultID == 0)\n                            _ResultID = _ConsoleCommandsResults.Count - 1;\n                        else\n                            _ResultID--;\n                    }\n                }\n                break;\n        }\n        if (Event.current.keyCode == (KeyCode.Escape))\n        {\n            this.Close();\n        }\n        Repaint();\n    }\n    private void Search()\n    {\n        //Input\n        GUILayout.Label(\"Search: \");\n        GUI.FocusControl(\"SearchField\");\n        GUI.SetNextControlName(\"SearchField\");\n        _Search = GUI.TextField(new Rect(5, 40, 500, 20), _Search.ToLower());\n        GUILayout.Space(20);\n    }\n\n    //Search/Show Commands\n    private void ShowConsoleCommands()\n    {\n        _CommandUsing = \"\";\n        _ConsoleCommandsResults.Clear();\n        GUILayout.BeginVertical(\"Box\");\n        int resultamount = 0;\n        string resultcommand = \"\";\n        for (int i = 0; i < _ConsoleCommands.Count; i++)\n        {\n            if (_ConsoleCommands[i].Contains(_Search))\n            {\n                _ConsoleCommandsResults.Add(_ConsoleCommands[i]);\n                resultamount++;\n                resultcommand = _ConsoleCommands[i];\n            }\n        }\n\n        if (resultamount > 1)\n        {\n            for (int i = 0; i < _ConsoleCommandsResults.Count; i++)\n            {\n                GUILayout.BeginHorizontal();\n                if (_ConsoleCommandsResults[i] == _Search)\n                {\n                    _ResultID = 0;\n                    GUILayout.Label(\">\" + _ConsoleCommandsResults[i]);\n                    _CommandUsing = _ConsoleCommandsResults[i];\n                }\n                else\n                {\n                    if (i == _ResultID)\n                    {\n                        GUILayout.Label(\">\" + _ConsoleCommandsResults[i]);\n                        _CommandUsing = _ConsoleCommandsResults[i];\n                    }\n                    else\n                    {\n                        GUILayout.Label(_ConsoleCommandsResults[i]);\n                    }\n                }\n                //GUILayout.Label(_ConsoleCommandsInfoResults[i]);\n                GUILayout.EndHorizontal();\n            }\n        }\n        else\n        {\n            GUILayout.Label(\">\" + resultcommand);\n            _CommandUsing = resultcommand;\n        }\n        if (_ResultID < 0)\n            _ResultID = 0;\n        if (_ResultID >= _ConsoleCommandsResults.Count - 1)\n            _ResultID = _ConsoleCommandsResults.Count - 1;\n        GUILayout.EndVertical();\n    }\n    private void DoCommand()\n    {\n        //GetCommand\n        if (_NewCommandTypes[_Commands._ToolCommands[_ResultID]._Type] == \"Search\")\n            DoCommand_Search();\n        if (_NewCommandTypes[_Commands._ToolCommands[_ResultID]._Type] == \"Load\")\n            DoCommand_Load();\n        if (_NewCommandTypes[_Commands._ToolCommands[_ResultID]._Type] == \"Create\")\n            DoCommand_Create();\n    }\n    private void DoCommand_Search()\n    {\n        if(_Commands._ToolCommands[_ResultID]._CommandData[0] == \"Objects\")\n        {\n            FindObjects(_Commands._ToolCommands[_ResultID]._CommandData[1]);\n        }\n        if (_Commands._ToolCommands[_ResultID]._CommandData[0] == \"Folder\")\n        {\n            //FindObjects(_Commands._ToolCommands[_ResultID]._CommandData[1]);\n            string relativePath = _Commands._ToolCommands[_ResultID]._CommandData[1].Substring(_Commands._ToolCommands[_ResultID]._CommandData[1].IndexOf(\"Assets/\"));\n            Selection.activeObject = AssetDatabase.LoadAssetAtPath(relativePath, typeof(Object));\n        }\n    }\n    private void DoCommand_Load()\n    {\n\n    }\n    private void DoCommand_Create()\n    {\n\n    }\n\n    //Info Box\n    private void InfoBox()\n    {\n        if (_InfoText != \"\")\n        {\n            GUILayout.BeginVertical(\"Box\");\n            GUILayout.Label(_InfoText);\n            GUILayout.EndVertical();\n        }\n    }\n\n    //Create Commands\n    private void CreateCommands()\n    {\n        GUILayout.BeginVertical(\"box\");\n        _NewCommandText = EditorGUILayout.TextField(\"New Command: \",_NewCommandText);\n        //GetType\n        _NewCommandTypesChosen = GUILayout.Toolbar(_NewCommandTypesChosen, _NewCommandTypes);\n        switch(_NewCommandTypesChosen)\n        {\n            case 0: //Seach\n                _NewCommandTypeOptionChosen = GUILayout.Toolbar(_NewCommandTypeOptionChosen, _NewCommandType_Search);\n                if(_NewCommandTypeOptionChosen >= _NewCommandType_Search.Length)\n                    _NewCommandTypeOptionChosen--;\n                break;\n            case 1: //Load\n                _NewCommandTypeOptionChosen = GUILayout.Toolbar(_NewCommandTypeOptionChosen, _NewCommandType_Load);\n                if (_NewCommandTypeOptionChosen >= _NewCommandType_Load.Length)\n                    _NewCommandTypeOptionChosen--;\n                break;\n            case 2: //Create\n                _NewCommandTypeOptionChosen = GUILayout.Toolbar(_NewCommandTypeOptionChosen, _NewCommandType_Create);\n                if (_NewCommandTypeOptionChosen >= _NewCommandType_Create.Length)\n                    _NewCommandTypeOptionChosen--;\n                break;\n        }\n\n        if (GUILayout.Button(\"AddCommand\"))\n        {\n            //Create New Command\n            Tool_Command newcommand = new Tool_Command();\n            newcommand._TypeOption = _NewCommandTypeOptionChosen;\n\n            switch (_NewCommandTypesChosen)\n            {\n                case 0: //Seach\n                    newcommand._CommandData.Add(_NewCommandType_Search[_NewCommandTypeOptionChosen]);\n                    newcommand._CommandData.Add(\"Object Name\");\n                    break;\n                case 1: //Load\n                    newcommand._CommandData.Add(_NewCommandType_Load[_NewCommandTypeOptionChosen]);\n                    break;\n                case 2: //Create\n                    newcommand._CommandData.Add(_NewCommandType_Create[_NewCommandTypeOptionChosen]);\n                    break;\n            }\n\n            newcommand._CommandName = _NewCommandText;\n            newcommand._Type = _NewCommandTypesChosen;\n            _Commands._ToolCommands.Add(newcommand);\n\n            _NewCommandText = \"\";\n            JsonUpdate();\n\n            //Edit Command\n            _EditMode = true;\n            _CommandID = _Commands._ToolCommands.Count -1;\n            EditCommand();\n        }\n        GUILayout.EndVertical();\n\n        if (_EditMode)\n            EditCommand();\n\n        CommandEditList();\n    }\n    private void EditCommand()\n    {\n        GUILayout.BeginVertical(\"box\");\n        GUILayout.Label(\"Editing Command: \" + _Commands._ToolCommands[_CommandID]._CommandName, EditorStyles.boldLabel);\n        GUILayout.BeginVertical();\n        if(_NewCommandTypes[_Commands._ToolCommands[_CommandID]._Type] == \"Search\")\n        {\n            if(_NewCommandType_Search[_Commands._ToolCommands[_CommandID]._TypeOption] == \"Objects\")\n            {\n                _Commands._ToolCommands[_CommandID]._CommandData[1] = EditorGUILayout.TextField(\"Name Gameobject:\",_Commands._ToolCommands[_CommandID]._CommandData[1]);\n            }\n            if (_NewCommandType_Search[_Commands._ToolCommands[_CommandID]._TypeOption] == \"Folder\")\n            {\n                if (GUILayout.Button(\"Get Path\"))\n                    _Commands._ToolCommands[_CommandID]._CommandData[1] = EditorUtility.OpenFilePanel(\"FolderPath: \", _Commands._ToolCommands[_CommandID]._CommandData[1], \"\");\n            }\n        }\n        if (GUILayout.Button(\"Save\"))\n        {\n            JsonUpdate();\n            _EditMode = false;\n        }\n        GUILayout.EndVertical();\n        GUILayout.EndVertical();\n    }\n    private void CommandEditList()\n    {\n        GUILayout.BeginVertical(\"box\");\n        GUILayout.BeginHorizontal(\"box\");\n\n        GUILayout.BeginHorizontal(\"box\");\n        GUILayout.Label(\"Command Name\");\n        GUILayout.EndHorizontal();\n\n        GUILayout.BeginHorizontal(\"box\");\n        GUILayout.Label(\"Edit\");\n        GUILayout.EndHorizontal();\n\n        GUILayout.BeginHorizontal(\"box\");\n        GUILayout.Label(\"Delete\");\n        GUILayout.EndHorizontal();\n\n        GUILayout.BeginHorizontal(\"box\");\n        GUILayout.Label(\"Add to menu\");\n        GUILayout.EndHorizontal();\n\n        GUILayout.EndHorizontal();\n\n\n\n        for (int i = 0; i < _Commands._ToolCommands.Count; i++)\n        {\n            GUILayout.BeginHorizontal(\"box\");\n            GUILayout.Label(_Commands._ToolCommands[i]._CommandName);\n            GUILayout.Label(\"Type: \" + _NewCommandTypes[_Commands._ToolCommands[i]._Type] + \" \" + _Commands._ToolCommands[i]._CommandData[0]);\n            if (GUILayout.Button(\"Edit\"))\n            {\n                _CommandID = i;\n                _EditMode = true;\n            }\n            if (GUILayout.Button(\"Delete\"))\n            {\n                _Commands._ToolCommands.Remove(_Commands._ToolCommands[i]);\n                JsonUpdate();\n            }\n            if (_Commands._ToolCommands[i]._ToMenu)\n            {\n                if (GUILayout.Button(\"+\", GUILayout.Width(20)))\n                {\n                    _Commands._ToolCommands[i]._ToMenu = !_Commands._ToolCommands[i]._ToMenu;\n                    JsonUpdate();\n                }\n            }\n            else\n            {\n                if (GUILayout.Button(\"-\", GUILayout.Width(20)))\n                {\n                    _Commands._ToolCommands[i]._ToMenu = !_Commands._ToolCommands[i]._ToMenu;\n                    JsonUpdate();\n                }\n            }\n            GUILayout.EndHorizontal();\n        }\n        GUILayout.EndVertical();\n    }\n\n    //Menu\n    private void MenuVieuw()\n    {\n        GUILayout.BeginVertical(\"box\");\n        for (int i = 0; i < _Commands._ToolCommands.Count; i++)\n        {\n            if(_Commands._ToolCommands[i]._ToMenu)\n            {\n                if(GUILayout.Button(_Commands._ToolCommands[i]._CommandName))\n                {\n                    _ResultID = i;\n                    DoCommand();\n                }\n            }\n        }\n        GUILayout.EndVertical();\n    }\n\n    //Save/Load\n    private void JsonLoad()\n    {\n        try\n        {\n            _Commands._ToolCommands.Clear();\n            _ConsoleCommands.Clear();\n            string dataPath = \"Assets/Commands.CommandData\";\n            string dataAsJson = File.ReadAllText(dataPath);\n            _Commands = JsonUtility.FromJson<Tool_CustomCommands>(dataAsJson);\n\n            for (int i = 0; i < _Commands._ToolCommands.Count; i++)\n            {\n                _ConsoleCommands.Add(_Commands._ToolCommands[i]._CommandName);\n            }\n        }\n        catch\n        {\n            JsonSave();\n            JsonLoad();\n        }\n    }\n    private void JsonSave()\n    {\n        string json = JsonUtility.ToJson(_Commands);\n        File.WriteAllText(\"Assets/Commands.CommandData\", json.ToString());\n    }\n    private void JsonUpdate()\n    {\n        JsonSave();\n        JsonLoad();\n    }\n\n    //Find Objects\n    private void FindObjects(string searchtext)\n    {\n        //GameObject[] objects = new GameObject[0];\n\n        Selection.activeGameObject = GameObject.Find(searchtext);\n    }\n\n}\n\n[System.Serializable]\npublic class Tool_CustomCommands\n{\n    public List<Tool_Command> _ToolCommands = new List<Tool_Command>();\n}\n\n[System.Serializable]\npublic class Tool_Command\n{\n    public string _CommandName = \"\";\n    public int _Type = 0;\n    public int _TypeOption = 0;\n    public string _CommandInfo = \"\";\n    public List<string> _CommandData = new List<string>();\n    public bool _ToMenu = false;\n}\n"),
        new Tool_QuickStart_Script("Tool_CreateHexagonMesh",    "Tool_Editor",                  "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEditor;\n\npublic class Tool_CreateHexagonMesh : EditorWindow\n{\n    private GameObject _CenterObj;\n    private List<GameObject> _ObjSaved = new List<GameObject>();\n    private int _TotalObjects = 100;\n\n    //Hex\n    private int _HexLengthX = 10, _HexLengthZ = 10;\n    private float _HexSize = 1;\n    private float _DistanceBetween = 1;\n\n    private bool _Center = true;\n    private bool _Invert = false;\n\n    [MenuItem(\"Tools/CreateHexagonGrid\")]\n    static void Init()\n    {\n        Tool_CreateHexagonMesh window = (Tool_CreateHexagonMesh)EditorWindow.GetWindow(typeof(Tool_CreateHexagonMesh));\n        window.Show();\n    }\n\n    void OnGUI()\n    { \n        GUILayout.BeginVertical(\"Box\");\n        _CenterObj = (GameObject)EditorGUILayout.ObjectField(\"Center Object\", _CenterObj, typeof(GameObject), true);\n        GUILayout.EndVertical();\n\n        GUILayout.BeginVertical(\"Box\");\n        _HexSize = EditorGUILayout.FloatField(\"Size: \", _HexSize);\n        _HexLengthX = EditorGUILayout.IntField(\"Collom: \", _HexLengthX);\n        _HexLengthZ = EditorGUILayout.IntField(\"Row: \", _HexLengthZ);\n\n        GUILayout.BeginHorizontal(\"Box\");\n        if (GUILayout.Button(\"Calculate Total Objects\"))\n            _TotalObjects = _HexLengthX * _HexLengthZ;\n        EditorGUILayout.LabelField(\"Total: \" + _TotalObjects.ToString());\n        GUILayout.EndHorizontal();\n\n        _Center = EditorGUILayout.Toggle(\"Center\", _Center);\n        _Invert = EditorGUILayout.Toggle(\"Invert: \", _Invert);\n        _DistanceBetween = EditorGUILayout.FloatField(\"Distance Between: \", _DistanceBetween);\n        GUILayout.EndVertical();\n\n        GUILayout.BeginVertical(\"Box\");\n        if (GUILayout.Button(\"Create\"))\n        {\n            if (_CenterObj != null)\n            {\n                if (_ObjSaved.Count > 0)\n                {\n                    for (int i = 0; i < _ObjSaved.Count; i++)\n                    {\n                        DestroyImmediate(_ObjSaved[i]);\n                    }\n                    _ObjSaved.Clear();\n                }\n\n                Vector3 objPos = _CenterObj.transform.position;\n                CreateHexagon(new Vector3(_HexLengthX, 0, _HexLengthZ));\n                SetParent();\n            }\n            else\n            {\n                Debug.Log(\"Center Object not selected!\");\n            }\n        }\n\n        if (GUILayout.Button(\"Destroy\"))\n        {\n            if (_CenterObj != null)\n            {\n                for (int i = 0; i < _ObjSaved.Count; i++)\n                {\n                    DestroyImmediate(_ObjSaved[i]);\n                }\n                _ObjSaved.Clear();\n\n\n                int childs = _CenterObj.transform.childCount;\n                for (int i = childs -1; i >= 0; i--)\n                {\n                    DestroyImmediate(_CenterObj.transform.GetChild(i).gameObject);\n                }\n            }\n            else\n            {\n                Debug.Log(\"Center Object not selected!\");\n            }\n    }\n\n        if (GUILayout.Button(\"Confirm\"))\n        {\n            _ObjSaved.Clear();\n        }\n        GUILayout.EndVertical();\n    }\n\n    void CreateHexagon(Vector3 dimentsions)\n    {\n        Vector3 objPos = _CenterObj.transform.position;\n        if (_Center && !_Invert)\n        {\n            objPos.x -= dimentsions.x * 0.5f * 1.7321f * _HexSize;\n            objPos.z -= dimentsions.z * 0.5f * -1.5f * _HexSize;\n        }\n        if (_Center && _Invert)\n        {\n            objPos.x -= dimentsions.x * 0.5f * 1.7321f * _HexSize;\n            objPos.z += dimentsions.z * 0.5f * -1.5f * _HexSize;\n        }\n\n        for (int xas = 0; xas < dimentsions.x; xas++)\n        {\n            CreateHax(new Vector3(objPos.x + 1.7321f  * _HexSize * _DistanceBetween * xas, objPos.y, objPos.z));\n            for (int zas = 1; zas < dimentsions.z; zas++)\n            {\n                float offset = 0;\n                if (zas % 2 == 1)\n                {\n                    offset = 0.86605f * _HexSize * _DistanceBetween;\n                }\n                else\n                {\n                    offset = 0;\n                }\n                if (!_Invert)\n                {\n                    CreateHax(new Vector3(objPos.x + 1.7321f * _HexSize * _DistanceBetween * xas - offset, objPos.y, objPos.z + -1.5f * _HexSize * _DistanceBetween * zas));\n                }\n                else\n                {\n                    CreateHax(new Vector3(objPos.x + 1.7321f * _HexSize * _DistanceBetween * xas - offset, objPos.y, objPos.z + +1.5f * _HexSize * _DistanceBetween * zas));\n                }\n            }\n        }\n    }\n    void CreateHax(Vector3 positions)\n    {\n        Vector3 objPos = _CenterObj.transform.position;\n\n        GameObject gridObj = GameObject.CreatePrimitive(PrimitiveType.Cube);\n        gridObj.transform.position = new Vector3(positions.x, positions.y, positions.z);\n\n        DestroyImmediate(gridObj.GetComponent<BoxCollider>());\n\n        float size = _HexSize;\n        float width = Mathf.Sqrt(3) * size;\n        float height = size * 2f;\n        Mesh mesh = new Mesh();\n        Vector3[] vertices = new Vector3[7];\n\n        for (int i = 0; i < 6; i++)\n        {\n            float angle_deg = 60 * i - 30;\n            float angle_rad = Mathf.Deg2Rad * angle_deg;\n\n            vertices[i + 1] = new Vector3(size * Mathf.Cos(angle_rad), 0f, size * Mathf.Sin(angle_rad));\n        }\n        mesh.vertices = vertices;\n\n        mesh.triangles = new int[]\n        {\n            2,1,0,\n            3,2,0,\n            4,3,0,\n            5,4,0,\n            6,5,0,\n            1,6,0\n        };\n\n        Vector2[] uv = new Vector2[7];\n        for (int i = 0; i < 7; i++)\n        {\n            uv[i] = new Vector2(\n                (vertices[i].x + -width * .5f) * .5f / size,\n                (vertices[i].z + -height * .5f) * .5f / size);\n        }\n\n        mesh.uv = uv;\n        gridObj.GetComponent<MeshFilter>().sharedMesh = mesh;\n\n        _ObjSaved.Add(gridObj);\n    }\n\n    void SetParent()\n    {\n        for (int i = 0; i < _ObjSaved.Count; i++)\n        {\n            _ObjSaved[i].transform.parent = _CenterObj.transform;\n        }\n    }\n}\n"),
        new Tool_QuickStart_Script("Tool_FileFinder",           "Tool_File_Finder",             "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEditor;\nusing UnityEngine;\n\npublic class Tool_FileFinder : EditorWindow\n{\n    //Tool State / Scrollpos\n    int _ToolState = 0;\n    int _ToolStateCheck = 1;\n    Vector2 _ScrollPos = new Vector2();\n\n    //Project\n    string _Project_Type = \"\";\n    string _Project_Search = \"\";\n    string _Project_SearchCheck = \"a\";\n    bool _Project_ExcludeMeta = true;\n    int _Project_Results = 0;\n    int _Project_Total = 0;\n\n    //Project > Results\n    string[] _SearchResults = new string[0];\n    string[] _SearchResultsChange = new string[0];\n\n    //Scene\n    string _Scene_Search = \"\";\n    bool _Scene_InsceneInfo = true;\n\n    //Scene > Results\n    bool[] _Scene_Objects_Toggle = new bool[0];\n    GameObject[] _Scene_Objects = new GameObject[0];\n\n    //GetWindow\n    [MenuItem(\"Tools/Tool_FileFinder\")]\n    public static void ShowWindow()\n    {\n        EditorWindow.GetWindow(typeof(Tool_FileFinder));\n    }\n\n    //Menu/HomePage\n    void OnGUI()\n    {\n        _ToolState = GUILayout.Toolbar(_ToolState, new string[] { \"Assets\", \"Scene\" });\n\n        if (_ToolState == 0)\n        {\n            FileFinder_Search();\n            FileFinder_SearchProject();\n        }\n        else\n        {\n            FileFinder_SceneSearch();\n            _Scene_InsceneInfo = EditorGUILayout.Toggle(\"InScene Info\", _Scene_InsceneInfo);\n            FileFinder_Scene();\n        }\n\n        //stop focus when switching\n        if(_ToolStateCheck != _ToolState)\n        {\n            EditorGUI.FocusTextInControl(\"searchproject\");\n            _ToolStateCheck = _ToolState;\n        } \n    }\n\n    //Project\n    void FileFinder_Search()\n    {\n        _Project_Search = EditorGUILayout.TextField(\"Search:\", _Project_Search);\n        _Project_Type = EditorGUILayout.TextField(\"Type:\", _Project_Type);\n        _Project_ExcludeMeta = EditorGUILayout.Toggle(\"Exlude Meta:\", _Project_ExcludeMeta);\n        GUILayout.Label(\"(\" + _Project_Results + \"/\" + _Project_Total + \")\");\n\n        _Project_Results = 0;\n        _Project_Total = 0;\n\n        if (_Project_Search != _Project_SearchCheck)\n        {\n            _SearchResults = System.IO.Directory.GetFiles(\"Assets/\", \"*\" + _Project_Type, System.IO.SearchOption.AllDirectories);\n            _SearchResultsChange = _SearchResults;\n            _Project_SearchCheck = _Project_Search;\n        }\n    }\n    void FileFinder_SearchProject()\n    {\n        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);\n        for (int i = 0; i < _SearchResults.Length; i++)\n        {\n            if (_SearchResults[i].ToLower().Contains(_Project_Search.ToLower()))\n            {\n                if(_Project_ExcludeMeta)\n                {\n                    if (!_SearchResults[i].ToLower().Contains(\".meta\"))\n                        FileFinder_SearchProject_Results(i);\n                }\n                else\n                    FileFinder_SearchProject_Results(i);\n            }\n            _Project_Total++;\n        }\n        EditorGUILayout.EndScrollView();\n    }\n    void FileFinder_SearchProject_Results(int id)\n    {\n        GUILayout.BeginHorizontal(\"Box\");\n        GUILayout.Label(_SearchResults[id], GUILayout.Width(Screen.width - 80));\n        if (GUILayout.Button(\"Select\", GUILayout.Width(50)))\n        {\n            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(_SearchResults[id]);\n        }\n        GUILayout.EndHorizontal();\n        _Project_Results++;\n    }\n\n    //Scene\n    void FileFinder_SceneSearch()\n    {\n        _Scene_Search = EditorGUILayout.TextField(\"Search:\", _Scene_Search);\n        GUILayout.Label(\"(\" + _Project_Results + \"/\" + _Project_Total + \")\");\n\n        if (GUILayout.Button(\"Refresh\"))\n        {\n            _Scene_Objects = new GameObject[0];\n        }\n\n        _Project_Results = 0;\n        _Project_Total = 0;\n\n        if (_Scene_Objects.Length == 0)\n        {\n            _Scene_Objects = FindObjectsOfType<GameObject>();\n            _Scene_Objects_Toggle = new bool[_Scene_Objects.Length];\n        }\n    }\n    void FileFinder_Scene()\n    {\n        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);\n        try\n        {\n            for (int i = 0; i < _Scene_Objects.Length; i++)\n            {\n                if (_Scene_Objects[i].name.ToLower().Contains(_Scene_Search.ToLower()))\n                {\n                    GUILayout.BeginHorizontal(\"Box\");\n                    _Scene_Objects_Toggle[i] = EditorGUILayout.Foldout(_Scene_Objects_Toggle[i], \"\");\n\n                    GUILayout.Label(_Scene_Objects[i].name, GUILayout.Width(Screen.width - 80));\n                    if (GUILayout.Button(\"Select\", GUILayout.Width(50)))\n                    {\n                        Selection.activeObject = _Scene_Objects[i];\n                    }\n\n                    if (_Scene_Objects_Toggle[i])\n                    {\n                        GUILayout.EndHorizontal();\n                        GUILayout.BeginVertical(\"box\");\n                        _Scene_Objects[i].name = EditorGUILayout.TextField(\"Name:\", _Scene_Objects[i].name);\n                        _Scene_Objects[i].transform.position = EditorGUILayout.Vector3Field(\"Position:\", _Scene_Objects[i].transform.position);\n                        _Scene_Objects[i].transform.eulerAngles = EditorGUILayout.Vector3Field(\"Rotation:\", _Scene_Objects[i].transform.eulerAngles);\n                        GUILayout.EndVertical();\n                        GUILayout.BeginHorizontal();\n                    }\n\n                    GUILayout.EndHorizontal();\n                    _Project_Results++;\n                }\n                _Project_Total++;\n            }\n        }\n        catch\n        {\n            _Scene_Objects = new GameObject[0];\n        }\n        EditorGUILayout.EndScrollView();\n    }\n\n    //wip\n    void FileFinder_NameChange()\n    {\n        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);\n        for (int i = 0; i < _SearchResults.Length; i++)\n        {\n            if (_SearchResults[i].ToLower().Contains(_Project_Search.ToLower()))\n            {\n                GUILayout.BeginHorizontal(\"Box\");\n                _SearchResultsChange[i] = EditorGUILayout.TextField(\"Object Name: \", _SearchResultsChange[i]);\n                if (GUILayout.Button(\"Save\", GUILayout.Width(50)))\n                {\n                    _SearchResults[i] = _SearchResultsChange[i];\n                    Debug.Log(_SearchResults[i] + \" to > \" + _SearchResultsChange[i]);\n                }\n                if (GUILayout.Button(\"Revert\", GUILayout.Width(50)))\n                {\n                    _SearchResultsChange[i] = _SearchResults[i];\n                    Debug.Log(_SearchResultsChange[i] + \" to > \" + _SearchResults[i]);\n                }\n                if (GUILayout.Button(\"Select\", GUILayout.Width(50)))\n                {\n                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(_SearchResults[i]);\n                }\n                GUILayout.EndHorizontal();\n                _Project_Results++;\n            }\n            _Project_Total++;\n        }\n        EditorGUILayout.EndScrollView();\n    }\n\n    //Enable/Disable\n    void OnEnable()\n    {\n        SceneView.duringSceneGui += this.OnSceneGUI;\n    }\n    void OnDisable()\n    {\n        SceneView.duringSceneGui -= this.OnSceneGUI;\n    }\n\n    //OnSceneGUI\n    void OnSceneGUI(SceneView sceneView)\n    {\n        try\n        {\n            if (_Scene_InsceneInfo)\n            {\n                Handles.color = new Color(0, 1, 0, 0.1f);\n                for (int i = 0; i < _Scene_Objects.Length; i++)\n                {\n                    if (_Scene_Objects[i].name.ToLower().Contains(_Scene_Search.ToLower()))\n                    {\n                        Handles.SphereHandleCap(1, _Scene_Objects[i].transform.position, Quaternion.identity, 3f, EventType.Repaint);\n                        Handles.Label(_Scene_Objects[i].transform.position, _Scene_Objects[i].name);\n                    }\n                }\n            }\n        }\n        catch { }\n    }\n}\n"),
        new Tool_QuickStart_Script("Tool_MapEditor",            "Tool_Map_Editor",              "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEditor;\nusing UnityEditorInternal;\nusing System.IO;\n\npublic class Tool_MapEditor : EditorWindow\n{\n    #region Array Icons\n    //Prefab Array\n    private GameObject[] _Prefabs = new GameObject[0];\n    private string[] _SearchResults = new string[0];\n\n    //Array Options\n    private string _SearchPrefab = \"\";\n    private bool _HideNames = true;\n    private float _ButtonSize = 1, _CollomLength = 4;\n\n    //Array Selection\n    private int _SelectedID = 99999999, _CheckSelectedID = 999999999;\n    #endregion\n    #region Options\n    //Options\n    private bool _HideOptions = true;\n    private int _OptionsStates = 0, _PlacementStates = 0;\n\n    //Placement Option\n    private float _PaintSpeed = 1, _PaintTimer = 0;\n    private bool _SnapPosActive = false;\n\n    //Onscene Options\n    private bool _ShowOptionsInScene;\n    private int _InScene_SelectedID;\n    #endregion\n    #region Transform\n    //Position\n    private Vector3 _MousePos, _SnapPos, _ObjectPos;\n    private Vector2 _GridSize = new Vector2(1, 1);\n\n    //Rotation/Size\n    private float _Rotation, _Size = 1;\n    private bool _RandomRot = false;\n    private Vector2 _PrevMousePos = new Vector3(0, 0, 0);\n    #endregion\n    #region Check\n    //Check Buttons Event\n    private bool _MouseDown, _ShiftDown, _CtrlDown, _ClickMenu;\n    #endregion\n    #region Other\n    //Placement\n    private GameObject _ParentObj, _ExampleObj;\n\n    //Other\n    private Vector2 _ScrollPos1, _ClickPos;\n    private Texture2D[] _PrefabIcon = new Texture2D[0];\n    #endregion\n\n    //Start Window\n    [MenuItem(\"Tools/Map Editor  %m\")]\n    static void Init()\n    {\n        Tool_MapEditor window = EditorWindow.GetWindow(typeof(Tool_MapEditor), false, \"Tool_MapEditor\") as Tool_MapEditor;\n        window.Show();\n    }\n\n    //Load Objects\n    private void Awake()\n    {\n        Load_Prefabs();\n        Load_Prefabs();\n    }\n\n    //Enable/Disable\n    void OnEnable()\n    {\n        SceneView.duringSceneGui += this.OnSceneGUI;\n        SceneView.duringSceneGui += this.OnScene;\n    }\n    void OnDisable()\n    {\n        SceneView.duringSceneGui -= this.OnSceneGUI;\n        SceneView.duringSceneGui -= this.OnScene;\n        DestroyImmediate(_ExampleObj);\n    }\n\n    //OnGUI ObjectView\n    void OnGUI()\n    {\n        GUILayout.BeginVertical(\"Box\");\n\n        //Refresh/Info\n        GUILayout.BeginHorizontal();\n        if (GUILayout.Button(\"Refresh\", GUILayout.Width(80)))\n        {\n            FixPreview();\n            Load_Prefabs();\n        }\n        GUILayout.Label(\"Loaded objects: \" + _SearchResults.Length);\n        GUILayout.EndHorizontal();\n\n        //Windows\n        ObjectView_Header();\n        ObjectView_Objects();\n        ObjectView_Options();\n\n        GUILayout.EndVertical();\n    }\n    private void ObjectView_Header()\n    {\n        GUILayout.BeginHorizontal();\n        _OptionsStates = GUILayout.Toolbar(_OptionsStates, new string[] { \"Icon\", \"Text\" });\n        _ButtonSize = EditorGUILayout.Slider(_ButtonSize, 0.25f, 2);\n        if (!_HideNames)\n        {\n            if (GUILayout.Button(\"Hide Names\", GUILayout.Width(100)))\n                _HideNames = true;\n        }\n        else\n        {\n            if (GUILayout.Button(\"Show Names\", GUILayout.Width(100)))\n                _HideNames = false;\n        }\n        GUILayout.EndHorizontal();\n        _SearchPrefab = EditorGUILayout.TextField(\"Search: \", _SearchPrefab);\n    }\n    private void ObjectView_Objects()\n    {\n        Color defaultColor = GUI.backgroundColor;\n        GUILayout.BeginVertical(\"Box\");\n        float calcWidth = 100 * _ButtonSize;\n        _CollomLength = position.width / calcWidth;\n        int x = 0;\n        int y = 0;\n\n        //Show/Hide Options\n        if (_HideOptions)\n            _ScrollPos1 = GUILayout.BeginScrollView(_ScrollPos1, GUILayout.Width(position.width - 20), GUILayout.Height(position.height - 109));\n        else\n        {\n            if (_PlacementStates == 0)\n                _ScrollPos1 = GUILayout.BeginScrollView(_ScrollPos1, GUILayout.Width(position.width - 20), GUILayout.Height(position.height - 235));\n            else\n                _ScrollPos1 = GUILayout.BeginScrollView(_ScrollPos1, GUILayout.Width(position.width - 20), GUILayout.Height(position.height - 253));\n        }\n\n        //Object Icons\n        for (int i = 0; i < _SearchResults.Length; i++)\n        {\n            if (_Prefabs[i] != null && _Prefabs[i].name.ToLower().Contains(_SearchPrefab.ToLower()))\n            {\n                if (_OptionsStates == 0) //Icons\n                {\n                    //Select Color\n                    if (_SelectedID == i) { GUI.backgroundColor = new Color(0, 1, 0); } else { GUI.backgroundColor = new Color(1, 0, 0); }\n\n                    //Create Button\n                    GUIContent content = new GUIContent();\n                    content.image = _PrefabIcon[i];\n                    GUI.skin.button.imagePosition = ImagePosition.ImageAbove;\n                    if (!_HideNames)\n                        content.text = _Prefabs[i].name;\n                    if (GUI.Button(new Rect(x * 100 * _ButtonSize, y * 100 * _ButtonSize, 100 * _ButtonSize, 100 * _ButtonSize), content))\n                        if (_SelectedID == i) { _SelectedID = 99999999; _CheckSelectedID = 99999999; DestroyImmediate(_ExampleObj); } else { _SelectedID = i; }\n\n                    //Reset Button Position\n                    x++;\n                    if (x >= _CollomLength - 1)\n                    {\n                        y++;\n                        x = 0;\n                    }\n                    GUI.backgroundColor = defaultColor;\n                }\n                else //Text Buttons\n                {\n                    if (_SelectedID == i) { GUI.backgroundColor = new Color(0, 1, 0); } else { GUI.backgroundColor = defaultColor; }\n                    if (GUILayout.Button(_Prefabs[i].name))\n                        if (_SelectedID == i) { _SelectedID = 99999999; _CheckSelectedID = 99999999; DestroyImmediate(_ExampleObj); } else { _SelectedID = i; }\n                    GUI.backgroundColor = defaultColor;\n                }\n            }\n        }\n        if (_OptionsStates == 0)\n        {\n            GUILayout.Space(y * 100 * _ButtonSize + 100);\n        }\n        GUILayout.EndScrollView();\n        GUILayout.EndVertical();\n    }\n    private void ObjectView_Options()\n    {\n        GUILayout.BeginVertical(\"Box\");\n        if (!_HideOptions)\n        {\n            //Paint Options\n            GUILayout.BeginVertical(\"Box\");\n            _PlacementStates = GUILayout.Toolbar(_PlacementStates, new string[] { \"Click\", \"Paint\" });\n            if (_PlacementStates == 1)\n                _PaintSpeed = EditorGUILayout.FloatField(\"Paint Speed: \", _PaintSpeed);\n            //Parent Options\n            GUILayout.BeginHorizontal();\n            _ParentObj = (GameObject)EditorGUILayout.ObjectField(\"Parent Object: \", _ParentObj, typeof(GameObject), true);\n            if (_ParentObj != null)\n                if (GUILayout.Button(\"Clean Parent\"))\n                    CleanParent();\n            GUILayout.EndHorizontal();\n            GUILayout.EndVertical();\n\n            //Grid Options\n            GUILayout.BeginVertical(\"Box\");\n            _GridSize = EditorGUILayout.Vector2Field(\"Grid Size: \", _GridSize);\n            _RandomRot = EditorGUILayout.Toggle(\"Random Rotation: \", _RandomRot);\n            _SnapPosActive = EditorGUILayout.Toggle(\"Use Grid: \", _SnapPosActive);\n            GUILayout.EndVertical();\n        }\n        //Hide/Show Options\n        if (_HideOptions)\n        {\n            if (GUILayout.Button(\"Show Options\"))\n                _HideOptions = false;\n        }\n        else\n        {\n            if (GUILayout.Button(\"Hide Options\"))\n                _HideOptions = true;\n        }\n        GUILayout.EndVertical();\n    }\n\n    //OnSceneGUI\n    void OnSceneGUI(SceneView sceneView)\n    {\n        Event e = Event.current;\n        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);\n        RaycastHit hitInfo;\n\n        if (Physics.Raycast(worldRay, out hitInfo))\n        {\n            //Check MousePosition\n            _MousePos = hitInfo.point;\n\n            //Create Example Object\n            if (_SelectedID <= _Prefabs.Length)\n            {\n                if (_CheckSelectedID != _SelectedID)\n                {\n                    DestroyImmediate(_ExampleObj);\n                    _ExampleObj = Instantiate(_Prefabs[_SelectedID], hitInfo.point, Quaternion.identity);\n                    _ExampleObj.layer = LayerMask.NameToLayer(\"Ignore Raycast\");\n                    for (int i = 0; i < _ExampleObj.transform.childCount; i++)\n                    {\n                        _ExampleObj.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(\"Ignore Raycast\");\n                        for (int o = 0; o < _ExampleObj.transform.GetChild(i).childCount; o++)\n                        {\n                            _ExampleObj.transform.GetChild(i).GetChild(o).gameObject.layer = LayerMask.NameToLayer(\"Ignore Raycast\");\n                        }\n                    }\n                    _ExampleObj.name = \"Example Object\";\n                    _CheckSelectedID = _SelectedID;\n                }\n            }\n\n            //Set Example Object Position + Rotation\n            if (_ExampleObj != null)\n            {\n                _ExampleObj.transform.rotation = Quaternion.Euler(0, _Rotation, 0);\n                _ExampleObj.transform.localScale = new Vector3(_Size, _Size, _Size);\n                if (!e.shift && !e.control)\n                {\n                    if (!_SnapPosActive)\n                    { _ExampleObj.transform.position = hitInfo.point; }\n                    else\n                    { _ExampleObj.transform.position = _SnapPos; }\n                }\n            }\n\n            //Check Buttons Pressed\n            if (!Event.current.alt && _SelectedID != 99999999)\n            {\n                if (Event.current.type == EventType.Layout)\n                    HandleUtility.AddDefaultControl(0);\n\n                //Mouse Button 0 Pressed\n                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)\n                {\n                    _MouseDown = true;\n                    _PaintTimer = _PaintSpeed;\n                    if (e.mousePosition.y <= 20)\n                        _ClickMenu = true;\n                }\n\n                //Mouse Button 0 Released\n                if (Event.current.type == EventType.MouseUp && Event.current.button == 0)\n                {\n                    _MouseDown = false;\n                    _ClickMenu = false;\n                }\n\n                //Check Shift\n                if (e.shift)\n                    _ShiftDown = true;\n                else\n                    _ShiftDown = false;\n\n                //Check Ctrl\n                if (e.control)\n                    _CtrlDown = true;\n                else\n                    _CtrlDown = false;\n\n                if (e.shift || e.control)\n                {\n                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)\n                        _ClickPos = Event.current.mousePosition;\n                }\n\n                //Place Object\n                if (!_ShiftDown && !_CtrlDown && !_ClickMenu)\n                {\n                    if (_PlacementStates == 0)\n                    {\n                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)\n                            CreatePrefab(hitInfo.point);\n                    }\n                    else\n                    {\n                        float timer1Final = _PaintSpeed;\n                        if (_MouseDown)\n                        {\n                            _PaintTimer += 1 * Time.deltaTime;\n                            if (_PaintTimer >= timer1Final)\n                            {\n                                CreatePrefab(hitInfo.point);\n                                _PaintTimer = 0;\n                            }\n                        }\n                    }\n                }\n            }\n\n            // Draw obj location\n            if (_SelectedID != 99999999)\n            {\n                //Draw Red Cross + Sphere on object location\n                Handles.color = new Color(1, 0, 0);\n                Handles.DrawLine(new Vector3(hitInfo.point.x - 0.3f, hitInfo.point.y, hitInfo.point.z), new Vector3(hitInfo.point.x + 0.3f, hitInfo.point.y, hitInfo.point.z));\n                Handles.DrawLine(new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z - 0.3f), new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z + 0.3f));\n                if (_SnapPosActive)\n                {\n                    Handles.SphereHandleCap(1, new Vector3(_SnapPos.x, hitInfo.point.y, _SnapPos.z), Quaternion.identity, 0.1f, EventType.Repaint);\n                }\n                else\n                    Handles.SphereHandleCap(1, new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z), Quaternion.identity, 0.1f, EventType.Repaint);\n\n                //Check Snap Position\n                if (_SnapPosActive)\n                {\n                    Vector2 calc = new Vector2(_MousePos.x / _GridSize.x, _MousePos.z / _GridSize.y);\n                    Vector2 calc2 = new Vector2(Mathf.RoundToInt(calc.x) * _GridSize.x, Mathf.RoundToInt(calc.y) * _GridSize.y);\n\n                    _SnapPos = new Vector3(calc2.x, _MousePos.y, calc2.y);\n\n                    //Draw Grid\n                    Handles.color = new Color(0, 1, 0);\n                    float lineLength = 0;\n                    if (_GridSize.x > _GridSize.y)\n                        lineLength = _GridSize.x + 1;\n                    else\n                        lineLength = _GridSize.y + 1;\n\n                    for (int hor = 0; hor < 3; hor++)\n                    {\n                        Handles.DrawLine(new Vector3(calc2.x - lineLength, hitInfo.point.y, calc2.y - _GridSize.y + _GridSize.y * hor), new Vector3(calc2.x + lineLength, hitInfo.point.y, calc2.y - _GridSize.y + _GridSize.y * hor));\n                    }\n                    for (int ver = 0; ver < 3; ver++)\n                    {\n                        Handles.DrawLine(new Vector3(calc2.x - _GridSize.x + _GridSize.x * ver, hitInfo.point.y, calc2.y - lineLength), new Vector3(calc2.x - _GridSize.x + _GridSize.x * ver, hitInfo.point.y, calc2.y + lineLength));\n                    }\n                }\n            }\n        }\n    }\n\n    //OnScene\n    void OnScene(SceneView sceneView)\n    {\n        //InScene Option Bar\n        Handles.BeginGUI();\n        if (_ShowOptionsInScene)\n        {\n            //Option Bar\n            GUI.Box(new Rect(0, 0, Screen.width, 22), GUIContent.none);\n            _InScene_SelectedID = GUI.Toolbar(new Rect(22, 1, Screen.width / 2 - 30, 20), _InScene_SelectedID, new string[] { \"Settings\", \"Placement\", \"Transform\", \"Grid\" });\n            switch (_InScene_SelectedID)\n            {\n                case 0: //Settings\n                    GUI.Label(new Rect(Screen.width / 2 - 5, 3, 50, 20), \"Parent: \");\n                    _ParentObj = (GameObject)EditorGUI.ObjectField(new Rect(Screen.width / 2 + 50, 1, 150, 20), _ParentObj, typeof(GameObject), true);\n                    if (GUI.Button(new Rect(Screen.width - 110, 1, 90, 20), \"Clean Parent\"))\n                    {\n                        CleanParent();\n                    }\n                    break;\n                case 1: //Placement\n                    _PlacementStates = GUI.Toolbar(new Rect(Screen.width / 2 - 5, 1, 100, 20), _PlacementStates, new string[] { \"Click\", \"Paint\" });\n                    _PaintSpeed = EditorGUI.FloatField(new Rect(Screen.width / 2 + 185, 1, 50, 20), _PaintSpeed);\n                    GUI.Label(new Rect(Screen.width / 2 + 100, 3, 500, 20), \"Paint speed: \");\n                    break;\n                case 2: //Transform\n                    _Size = EditorGUI.FloatField(new Rect(Screen.width / 2 + 125, 1, 100, 20), _Size);\n                    break;\n                case 3: //Grid\n                    GUI.Label(new Rect(Screen.width / 2 + 80, 3, 100, 20), \"Grid Size: \");\n                    _GridSize.x = EditorGUI.FloatField(new Rect(Screen.width / 2 + 150, 1, 50, 20), _GridSize.x);\n                    _GridSize.y = EditorGUI.FloatField(new Rect(Screen.width / 2 + 200, 1, 50, 20), _GridSize.y);\n                    GUI.Label(new Rect(Screen.width / 2, 3, 100, 20), \"Enable: \");\n                    _SnapPosActive = EditorGUI.Toggle(new Rect(Screen.width / 2 + 50, 3, 20, 20), _SnapPosActive);\n                    break;\n            }\n        }\n\n        //Hotkeys Resize / Rotate\n        //Shift+MouseDown = Resize\n        Vector2 prevmove = _PrevMousePos - Event.current.mousePosition;\n        if (_ShiftDown && _MouseDown)\n        {\n            _Size = EditorGUI.Slider(new Rect(_ClickPos.x - 15, _ClickPos.y - 40, 50, 20), _Size, 0.01f, 1000000);\n            _Size -= (prevmove.x + prevmove.y) * 0.05f;\n            GUI.Label(new Rect(_ClickPos.x - 50, _ClickPos.y - 40, 500, 20), \"Size: \");\n        }\n        //Ctrl+MouseDown = Rotate\n        if (_CtrlDown && _MouseDown)\n        {\n            _Rotation = EditorGUI.Slider(new Rect(_ClickPos.x - 15, _ClickPos.y - 40, 50, 20), _Rotation, -1000000, 1000000);\n            _Rotation += prevmove.x + prevmove.y;\n            GUI.Label(new Rect(_ClickPos.x - 80, _ClickPos.y - 40, 500, 20), \"Rotation: \");\n        }\n        _PrevMousePos = Event.current.mousePosition;\n\n        //Inscene Show OptionButton\n        GUI.color = new Color(1f, 1f, 1f, 1f);\n        if (!_ShowOptionsInScene)\n        {\n            if (GUI.Button(new Rect(1, 1, 20, 20), \" +\"))\n                _ShowOptionsInScene = true;\n        }\n        else\n        {\n            if (GUI.Button(new Rect(1, 1, 20, 20), \" -\"))\n                _ShowOptionsInScene = false;\n        }\n        Handles.EndGUI();\n    }\n\n    //Load/Fix\n    void Load_Prefabs()\n    {\n        _SearchResults = System.IO.Directory.GetFiles(\"Assets/\", \"*.prefab\", System.IO.SearchOption.AllDirectories);\n        _Prefabs = new GameObject[_SearchResults.Length];\n        _PrefabIcon = new Texture2D[_SearchResults.Length];\n\n        for (int i = 0; i < _SearchResults.Length; i++)\n        {\n            Object prefab = null;\n            prefab = AssetDatabase.LoadAssetAtPath(_SearchResults[i], typeof(GameObject));\n            _Prefabs[i] = prefab as GameObject;\n            _PrefabIcon[i] = AssetPreview.GetAssetPreview(_Prefabs[i]);\n        }\n    }\n    void FixPreview()\n    {\n        Load_Prefabs();\n        _SearchResults = System.IO.Directory.GetFiles(\"Assets/\", \"*.prefab\", System.IO.SearchOption.AllDirectories);\n\n        for (int i = 0; i < _SearchResults.Length; i++)\n        {\n            if (_PrefabIcon[i] == null)\n                AssetDatabase.ImportAsset(_SearchResults[i]);\n        }\n        Load_Prefabs();\n    }\n\n    //Create Prefab/Clean Parent\n    void CreatePrefab(Vector3 createPos)\n    {\n        if (CheckPositionEmpty(true))\n        {\n            GameObject createdObj = PrefabUtility.InstantiatePrefab(_Prefabs[_SelectedID]) as GameObject;\n            createdObj.transform.position = createPos;\n            createdObj.transform.localScale = new Vector3(_Size, _Size, _Size);\n\n            if (_ParentObj == null)\n            {\n                _ParentObj = new GameObject();\n                _ParentObj.name = \"MapEditor_Parent\";\n            }\n\n            createdObj.transform.parent = _ParentObj.transform;\n            if (_SnapPosActive)\n                createdObj.transform.position = _SnapPos;\n            else\n                createdObj.transform.position = _MousePos;\n            if (_RandomRot)\n                createdObj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);\n            else\n                createdObj.transform.rotation = Quaternion.Euler(0, _Rotation, 0);\n        }\n    }\n    void CleanParent()\n    {\n        int childAmount = _ParentObj.transform.childCount;\n        int childCalc = childAmount - 1;\n        for (int i = 0; i < childAmount; i++)\n        {\n            DestroyImmediate(_ParentObj.transform.GetChild(childCalc).gameObject);\n            childCalc -= 1;\n        }\n    }\n    bool CheckPositionEmpty(bool checky)\n    {\n        if (_ParentObj != null)\n        {\n            bool check = true;\n            for (int i = 0; i < _ParentObj.transform.childCount; i++)\n            {\n                if (checky)\n                {\n                    if (_ParentObj.transform.GetChild(i).position.x == _SnapPos.x && _ParentObj.transform.GetChild(i).position.z == _SnapPos.z)\n                        check = false;\n                }\n                else\n                        if (_ParentObj.transform.GetChild(i).position == _SnapPos)\n                    check = false;\n            }\n            return check;\n        }\n        else\n        {\n            return true;\n        }\n    }\n}\n"),
        //new Tool_QuickStart_Script("Tool_Math",                 "Tool_Math",                    "wip",            "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEditor;\n\npublic class Tool_Math : EditorWindow\n{\n    string _Search = \"\";\n\n    int _Selected = 0;\n    int _CheckSelected = 1;\n    string[] _Options = new string[]\n    {\n        \"sin\", \"cos\", \"tan\",\n    };\n    string[] _Results = new string[]\n    {\n        \"Mathf.Sin(float);\",\n        \"Mathf.Cos(float);\",\n        \"Mathf.Tan(float);\",\n    };\n\n    string _Result;\n    private Vector2 _ScrollPos = new Vector2();\n\n    [MenuItem(\"Tools/Tool_Math\")]\n    public static void ShowWindow()\n    {\n        EditorWindow.GetWindow(typeof(Tool_Math));\n    }\n\n    void OnGUI()\n    {\n        //Result\n        GUILayout.Label(\"Result:\");\n        EditorGUILayout.TextField(\"\", _Result);\n\n        //Check Selected\n        if (_Selected != _CheckSelected)\n        {\n            _Result = _Results[_Selected];\n            _CheckSelected = _Selected;\n        }\n\n        //Seach\n        GUILayout.Space(20);\n        GUILayout.Label(\"Search Formula:\");\n        _Search = EditorGUILayout.TextField(\"\", _Search);\n\n        //Search Results\n        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);\n        for (int i = 0; i < _Results.Length; i++)\n        {\n            if (_Options[i].Contains(_Search))\n                if (GUILayout.Button(_Options[i]))\n                    _Selected = i;\n        }\n        EditorGUILayout.EndScrollView();\n    }\n}\n"),
        new Tool_QuickStart_Script("Tool_ScriptToString",       "Tool_Editor",                  "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing System.Linq;\nusing System.IO;\nusing UnityEditor;\nusing UnityEngine;\n\npublic class Tool_ScriptToString : EditorWindow\n{\n    MonoScript _InputScript;\n    string _ScriptOutput = \"\";\n\n    private Vector2 _ScrollPos = new Vector2();\n\n    [MenuItem(\"Tools/Convert Script to String\")]\n    public static void ShowWindow()\n    {\n        EditorWindow.GetWindow(typeof(Tool_ScriptToString));\n    }\n\n    void OnGUI()\n    {\n        if (GUILayout.Button(\"Convert\", GUILayout.Height(30)))\n            if(_InputScript != null)\n            _ScriptOutput = ConvertScriptToString();\n\n        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);\n        Display_InputOutput();\n        Display_StringExample();\n        EditorGUILayout.EndScrollView();\n    }\n\n    private void Display_InputOutput()\n    {\n        GUILayout.Space(20);\n        //Input\n        GUILayout.Label(\"Input: \", EditorStyles.boldLabel);\n        _InputScript = EditorGUILayout.ObjectField(_InputScript, typeof(MonoScript), false) as MonoScript;\n\n        //Output\n        GUILayout.Label(\"Output: \", EditorStyles.boldLabel);\n        EditorGUILayout.TextField(\"\", _ScriptOutput);\n        GUILayout.Space(20);\n    }\n\n    private void Display_StringExample()\n    {\n        //Preview\n        List<string> output = new List<string>();\n        List<string> output2 = new List<string>();\n\n        for (int i = 0; i < _ScriptOutput.Length; i++)\n        {\n            output.Add(System.Convert.ToString(_ScriptOutput[i]));\n        }\n\n        int begincalc = 0;\n        int endcalc = 0;\n\n        for (int i = 0; i < output.Count; i++)\n        {\n            if (i + 1 < output.Count)\n            {\n                if (output[i] + output[i + 1] == \"\\\\n\")\n                {\n                    endcalc = i;\n                    string addstring = \"\";\n                    for (int j = 0; j < endcalc - begincalc; j++)\n                    {\n                        addstring += output[begincalc + j];\n                    }\n                    addstring += output[endcalc] + output[endcalc + 1];\n\n                    output2.Add(addstring);\n                    endcalc = endcalc + 1;\n                    begincalc = endcalc + 1;\n                }\n            }\n        }\n\n        for (int i = 0; i < output2.Count; i++)\n        {\n            GUILayout.BeginHorizontal();\n            if (output2[i].Contains(\"//\"))\n            {\n                EditorGUILayout.TextField(\"\", \"x\", GUILayout.MaxWidth(15));\n            }\n            else\n            {\n                EditorGUILayout.TextField(\"\", \"\", GUILayout.MaxWidth(15));\n            }\n\n            EditorGUILayout.TextField(\"\", output2[i]);\n            GUILayout.EndHorizontal();\n        }\n    }\n\n    private string ConvertScriptToString()\n    {\n        string newstring = \"\\\"\";\n        string path = GetPath();\n        string[] readText = File.ReadAllLines(path);\n\n        for (int i = 0; i < readText.Length; i++)\n        {\n            string newline = \"\";\n            for (int j = 0; j < readText[i].Length; j++)\n            {\n                if(System.Convert.ToString(readText[i][j]) == \"\\\"\")\n                    newline += \"\\\\\";\n                newline += System.Convert.ToString(readText[i][j]);\n            }\n            readText[i] = newline + \"\\\\n\";\n            newstring += readText[i];\n        }\n\n        newstring += \"\\\"\";\n\n        return newstring;\n    }\n\n    private string GetPath()\n    {\n        string[] filepaths = System.IO.Directory.GetFiles(\"Assets/\", \"*.cs\", System.IO.SearchOption.AllDirectories);\n        for (int i = 0; i < filepaths.Length; i++)\n        {\n            if (filepaths[i].Contains(_InputScript.name + \".cs\"))\n            {\n                return filepaths[i];\n            }\n        }\n        return \"\";\n    }\n}\n"),
        new Tool_QuickStart_Script("Turret",                    "Turret_Shooting",              "stable",           "",        "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class Turret : MonoBehaviour\n{\n    [Header(\"Settings\")]\n    [SerializeField] private Vector2 _MinMaxRange = Vector2.zero;\n    [SerializeField] private float _SecondsBetweenShots = 2;\n    [SerializeField] private float _Damage = 25;\n    [SerializeField] private GameObject _ShootPart = null;\n    [SerializeField] private string _Tag = \"Enemy\";\n    \n    private float _Timer;\n    private GameObject _Target;\n\n    void Update()\n    {\n        if (_Target != null)\n        {\n            _ShootPart.transform.LookAt(_Target.transform.position);\n            _Timer += 1 * Time.deltaTime;\n            if (_Timer >= _SecondsBetweenShots)\n            {\n                _Target.GetComponent<Health>().DoDamage(_Damage);\n                _Timer = 0;\n            }\n        }\n        else\n        {\n            _ShootPart.transform.rotation = Quaternion.Euler(90, 0, 0);\n        }\n\n        _Target = FindEnemy();\n    }\n\n    public GameObject FindEnemy()\n    {\n        GameObject[] m_Targets = GameObject.FindGameObjectsWithTag(_Tag);\n        GameObject closest = null;\n        float distance = Mathf.Infinity;\n        Vector3 position = transform.position;\n\n        _MinMaxRange.x = _MinMaxRange.x * _MinMaxRange.x;\n        _MinMaxRange.y = _MinMaxRange.y * _MinMaxRange.y;\n        foreach (GameObject target in m_Targets)\n        {\n            Vector3 diff = target.transform.position - position;\n            float curDistance = diff.sqrMagnitude;\n            if (curDistance < distance && curDistance >= _MinMaxRange.x && curDistance <= _MinMaxRange.y)\n            {\n                closest = target;\n                distance = curDistance;\n            }\n        }\n        return closest;\n    }\n}\n"),
        new Tool_QuickStart_Script("UIEffects",                 "UI_Effect",                    "stable",           "",        "using UnityEngine;\nusing UnityEngine.EventSystems;\n\npublic class UIEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler\n{\n    private enum UIEffectOptions { Grow, Shrink }\n    [Header(\"Effects\")]\n    [SerializeField] private UIEffectOptions _UIEffect = UIEffectOptions.Grow;\n\n    [Header(\"Scaling Options\")]\n    [SerializeField] private bool _RelativeToOriginalSize = true;\n    [SerializeField] private float _IncreaseSpeed = 1;\n\n    [Header(\"Minimal Size:\")]\n    [SerializeField] private float _MinimalSize = 0.9f;\n\n    [Header(\"Maximal Size:\")]\n    [SerializeField] private float _MaximalSize = 1.1f;\n\n    private Vector3 _OriginalSize;\n    private bool _MouseOver;\n\n    void Start()\n    {\n        _OriginalSize = transform.localScale;\n\n        if (_RelativeToOriginalSize)\n        {\n            _MinimalSize = _OriginalSize.y * _MinimalSize;\n            _MaximalSize = _OriginalSize.y * _MaximalSize;\n            _IncreaseSpeed = _IncreaseSpeed * ((_OriginalSize.x + _OriginalSize.y + _OriginalSize.z) / 3);\n        }\n    }\n\n    void Update()\n    {\n        switch (_UIEffect)\n        {\n            case UIEffectOptions.Grow:\n                if (_MouseOver)\n                {\n                    if (transform.localScale.y < _MaximalSize)\n                        transform.localScale += new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;\n                }\n                else\n                    if (transform.localScale.y > _OriginalSize.y)\n                    transform.localScale -= new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;\n                else\n                    transform.localScale = new Vector3(_OriginalSize.x, _OriginalSize.y, _OriginalSize.z);\n                break;\n            case UIEffectOptions.Shrink:\n                if (_MouseOver)\n                {\n                    if (transform.localScale.y > _MinimalSize)\n                        transform.localScale -= new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;\n                }\n                else\n                   if (transform.localScale.y < _OriginalSize.y)\n                    transform.localScale += new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;\n                else\n                    transform.localScale = new Vector3(_OriginalSize.x, _OriginalSize.y, _OriginalSize.z);\n                break;\n        }\n    }\n\n    public void OnPointerEnter(PointerEventData eventData)\n    {\n        _MouseOver = true;\n    }\n\n    public void OnPointerExit(PointerEventData eventData)\n    {\n        _MouseOver = false;\n    }\n}\n"
            )};
    bool[] _AddMultipleScripts = new bool[0];
    bool _AddMultipleScriptsActive = false;
    int _AddMultipleScripts_SelectAmount = 0;
    int _ShowScriptDescription = -1;

    //Settings
    int _CreateSceneOptions = 0;
    Vector2 _ScrollPos = new Vector2();
    bool _StartUpSearch = false;
    bool[] _UpdateLogFoldout = new bool[2];

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
    enum HUDProfiles { BasicStartMenu, AdvanceStartMenu_wip };
    HUDProfiles _HUD_Profiles;

    //Other
    GameObject _MainCanvas;
    RectTransform _MainCanvasRect;
    Vector2 _CheckMainCanvasRectSize;

    //Tool Mode
    int _ToolState = 0;
    int _ToolStateCheck = 1;

    //Scene Window
    bool[] _SceneStructure = new bool[0];
    string[] _SceneStructureOptions = new string[] {
    "Essentials",
    "Canvas",
    "Map",
    "Lighting",
    "Other"};
    List<Tool_QuickStart_SceneOrganizer> _SceneObjects = new List<Tool_QuickStart_SceneOrganizer>();
    List<Tool_QuickStart_SceneOrganizer> _SceneObjects_Snapshot = new List<Tool_QuickStart_SceneOrganizer>();
    List<Tool_QuickStart_SceneOrganizer_GameObjectProfile_All> _AllSceneObjects = new List<Tool_QuickStart_SceneOrganizer_GameObjectProfile_All>();
    bool _SceneObjects_ShowSnapshot = true;
    bool _SceneObjects_Show_SceneStructure = false;
    bool _SceneObjects_Show_SceneAllObject = false;
    bool _SceneObjects_Show_Scripts = false;
    string _SceneObjects_Search = "";
    string _SceneObjects_SearchScript = "";
    bool _SceneObjects_Filter_HasScript = false;
    Vector2 _Scene_Scroll;

    //FileFinder (FF) ----------------------------------------------
    #region FileFinder
    string _FF_Type = "";
    string _FF_TypeCheck = "";
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
    #endregion


    //Script To String (STS) ----------------------------------------------
    #region Script To String
    MonoScript _STS_InputScript = null;
    string _STS_ScriptOutput = "";
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
        GUILayout.Label(_Version);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("=", GUILayout.Width(20)))
        {
            _WindowID = 0;
            _SelectWindow = !_SelectWindow;
        }
        if (GUILayout.Button("?", GUILayout.Width(20)))
        {
            if (_WindowID == 1)
                _WindowID = 0;
            else
                _WindowID = 1;
            _SelectWindow = false;
        }
        if (_SelectWindow)
        {
            GUILayout.Label("Tool Navigation");
            GUILayout.EndHorizontal();

            _Search_Window = EditorGUILayout.TextField("Search: ", _Search_Window);

            for (int i = 2; i < _WindowNames.Length; i++)
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
                    _MenuID = GUILayout.Toolbar(_MenuID, new string[] { "QuickStart", "Scripts", "QuickUI (wip)", "Scene (wip)" });
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
                        case 3: //Scene
                            Menu_Scene();
                            break;
                    }
                    break;
                case 1: //UpdateLog
                    GUILayout.EndHorizontal();
                    UpdateLog();
                    break;
                case 2: //FileFinder
                    GUILayout.EndHorizontal();
                    FileFinder();
                    break;
                case 3: //ScriptToString
                    GUILayout.EndHorizontal();
                    ScriptToString_Menu();
                    break;
                case 4: //MapEditor
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
                _Type2DID = GUILayout.Toolbar(_Type2DID, new string[] { "Platformer", "TopDown", "VisualNovel (wip)" });
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
                break;
            case 1: //TopDown
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("Movement_2D_TopDown");
                ScriptStatus("Movement_Camera");
                GUILayout.Label("Extra", EditorStyles.boldLabel);
                break;
            case 2: //VisualNovel
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("DialogSystem");
                ScriptStatus("DialogSystemEditor");
                GUILayout.Label("Extra", EditorStyles.boldLabel);
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
                GUILayout.Label("Recommended", EditorStyles.boldLabel);
                ScriptStatus("ObjectPool");
                GUILayout.Label("Shooter", EditorStyles.boldLabel);
                ScriptStatus("Health");
                ScriptStatus("Bullet");
                GUILayout.Label("Interactions", EditorStyles.boldLabel);
                ScriptStatus("InteractionHandler");
                ScriptStatus("Interactable");
                break;
            case 1: //ThirdPerson
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("Movement_CC_FirstPerson");
                GUILayout.Label("Extra", EditorStyles.boldLabel);
                ScriptStatus("Health");
                break;
            case 2: //TopDown
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("Movement_CC_TopDown");
                GUILayout.Label("Extra", EditorStyles.boldLabel);
                ScriptStatus("Health");
                break;
            case 3: //Platformer
                GUILayout.Label("Essential", EditorStyles.boldLabel);
                ScriptStatus("Movement_CC_Platformer");
                GUILayout.Label("Extra", EditorStyles.boldLabel);
                ScriptStatus("Health");
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
                case 2: //VisualNovel
                    CreateObjects_2D_VisualNovel(player, groundCube);
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

        GameObject objpool = null;

        //Setup Scripts
        if (ScriptExist("Health"))
        {
            string UniType = "Health";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            playerobj.AddComponent(UnityType);
        }
        if (ScriptExist("ObjectPool"))
        {
            objpool = new GameObject();
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
    void CreateObjects_2D_VisualNovel(GameObject playerobj, GameObject groundobj)
    {
        DestroyImmediate(playerobj);
        DestroyImmediate(groundobj);

        //Create DialogSystem
        GameObject dialogsystemobj = new GameObject();
        dialogsystemobj.name = "DialogSystem";

        if (ScriptExist("DialogSystem"))
        {
            string UniType = "DialogSystem";
            Type UnityType = Type.GetType(UniType + ", Assembly-CSharp");
            dialogsystemobj.AddComponent(UnityType);
        }


        //Create Canvas
        GameObject visualnovel_canvas = HUD_Create_Canvas();
        visualnovel_canvas.name = "Canvas_VisualNovel";

        //Dialogtext
        GameObject visualnovel_text = HUD_Create_Text();


        //Add to canvas
        visualnovel_text.transform.parent = visualnovel_canvas.transform;


    }


    //Home > Scripts
    void Menu_Scripts()
    {
        //Refresh
        if (GUILayout.Button("Refresh"))
            SearchScripts();

        //MultiSelect
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Add Multiple: ", GUILayout.Width(75));
        _AddMultipleScriptsActive = EditorGUILayout.Toggle(_AddMultipleScriptsActive, GUILayout.Width(70));

        //MultiSelect Buttons
        if (_AddMultipleScriptsActive)
        {
            //Add Selected
            if (GUILayout.Button("Add Selected", GUILayout.Width(100)))
                for (int i = 0; i < _AddMultipleScripts.Length; i++)
                    if (_AddMultipleScripts[i])
                        AddScript(i);
            
            //Select All
            if(GUILayout.Button("Selelct All", GUILayout.Width(100)))
                for (int i = 0; i < _AddMultipleScripts.Length; i++)
                    _AddMultipleScripts[i] = true;

            //DeSelect
            if (GUILayout.Button("DeSelect (" + _AddMultipleScripts_SelectAmount.ToString() + ")", GUILayout.Width(100)))
                for (int i = 0; i < _AddMultipleScripts.Length; i++)
                    _AddMultipleScripts[i] = false;
        }
        EditorGUILayout.EndHorizontal();

        //Search Options
        _Search_Script = EditorGUILayout.TextField("Search: ", _Search_Script);
        _Search_Tag = EditorGUILayout.TextField("SearchTag: ", _Search_Tag);
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);

        //Quickstart Scripts
        if (!_AddMultipleScriptsActive)
            _Search_QuickStartScripts_Toggle = EditorGUILayout.Foldout(_Search_QuickStartScripts_Toggle, "QuickStart" + "     ||     Results(" + _Search_Results.ToString() + "/" + QuickStart_Scripts.Length.ToString() + ")   ||   In Project: " + _Search_InProject_Results.ToString());
        else
            _Search_QuickStartScripts_Toggle = EditorGUILayout.Foldout(_Search_QuickStartScripts_Toggle, "QuickStart" + "     ||     Results(" + _Search_Results.ToString() + "/" + QuickStart_Scripts.Length.ToString() + ")   ||   In Project: " + _Search_InProject_Results.ToString() + "   ||   Selected: " + _AddMultipleScripts_SelectAmount.ToString());
        if (_Search_QuickStartScripts_Toggle)
        {
            _Search_Results = 0;
            _Search_InProject_Results = 0;
            for (int i = 0; i < QuickStart_Scripts.Length; i++)
            {
                //Scripts
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
                        {
                            if (_AddMultipleScriptsActive)
                            {
                                if (_AddMultipleScripts[i])
                                    GUI.backgroundColor = new Color(0.5f,0.7f,0);
                                else
                                    GUI.backgroundColor = new Color(1, 0, 0);
                            }
                            else
                                GUI.backgroundColor = new Color(1, 0, 0);
                        }

                        //Script
                        Rect testrect = EditorGUILayout.BeginHorizontal();

                        //Update Faster
                        Repaint();

                        if (Event.current.mousePosition.y >= testrect.y && Event.current.mousePosition.y <= testrect.y + testrect.height)
                        {
                            GUI.backgroundColor = new Color(0.2f, 0.8f, 0);
                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                            {
                                if (_AddMultipleScriptsActive)
                                    _AddMultipleScripts[i] = !_AddMultipleScripts[i];
                                else
                                {
                                    if(_ShowScriptDescription == i)
                                        _ShowScriptDescription = -1;
                                    else
                                        _ShowScriptDescription = i;
                                }
                            }
                        }

                        EditorGUILayout.BeginHorizontal("Box");
                        if (Screen.width <= 325)
                        {
                            if (_AddMultipleScriptsActive)
                                EditorGUILayout.LabelField(QuickStart_Scripts[i].ScriptName + ".cs", EditorStyles.boldLabel, GUILayout.Width(Screen.width - 150));
                            else
                                EditorGUILayout.LabelField(QuickStart_Scripts[i].ScriptName + ".cs", EditorStyles.boldLabel, GUILayout.Width(Screen.width - 135));
                        }
                        else
                        {
                            if (_AddMultipleScriptsActive)
                                EditorGUILayout.LabelField(QuickStart_Scripts[i].ScriptName + ".cs", EditorStyles.boldLabel, GUILayout.Width(Screen.width - 205));
                            else
                                EditorGUILayout.LabelField(QuickStart_Scripts[i].ScriptName + ".cs", EditorStyles.boldLabel, GUILayout.Width(Screen.width - 190));
                        }

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

                        //Add Multiple
                        if (_AddMultipleScriptsActive)
                        {
                            EditorGUI.BeginDisabledGroup(QuickStart_Scripts[i].Exist);
                            _AddMultipleScripts[i] = EditorGUILayout.Toggle(_AddMultipleScripts[i]);
                            EditorGUI.EndDisabledGroup();
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndHorizontal();

                        
                        //Description
                        if (!_AddMultipleScriptsActive && _ShowScriptDescription == i)
                        { 
                            GUI.backgroundColor = Color.white;
                            if (QuickStart_Scripts[i].ScriptDescription != "")
                                GUILayout.Label(QuickStart_Scripts[i].ScriptDescription, EditorStyles.helpBox);
                            else
                                GUILayout.Label("No Info", EditorStyles.helpBox);
                        }
                        
                    }
                }

                //SelectAmount
                _AddMultipleScripts_SelectAmount = 0;
                for (int j = 0; j < _AddMultipleScripts.Length; j++)
                {
                    if (_AddMultipleScripts[j])
                        _AddMultipleScripts_SelectAmount++;
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
            { 
                GUI.backgroundColor = new Color(0, 1, 0); 
                
            }
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
        GUI.backgroundColor = Color.white;
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
                string[] scriptpathsplit = search_results[o].Split(new Char[] { '/', '\\' },
                                 StringSplitOptions.RemoveEmptyEntries);

                if (scriptpathsplit[scriptpathsplit.Length-1].ToLower() == QuickStart_Scripts[i].ScriptName.ToLower() + ".cs")
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

        //Set 
        if (_AddMultipleScripts.Length == 0)
            _AddMultipleScripts = new bool[QuickStart_Scripts.Length];

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
            if(GUILayout.Button("DeSelect"))
            {
                _HUDTab.Clear();
                _CheckMainCanvasRectSize = Vector2.zero;
                _MainCanvasRect = null;
                _MainCanvas = null;
            }

            if (GUILayout.Button("Delete Canvas"))
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
                case HUDProfiles.BasicStartMenu:
                    HUD_LoadProfile_BasicStartMenu();
                    break;
                case HUDProfiles.AdvanceStartMenu_wip:
                    HUD_LoadProfile_AdvancedMenu();
                    break;
            }
        }
        GUILayout.EndHorizontal();
    }
    void HUD_Editor_Obj()
    {
        for (int i = 0; i < _HUDTab[_HUDTabID].HUD_TabOjects.Count; i++)
        {
            if (GUILayout.Button(_HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_Name))
                _HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_FoldOut = !_HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_FoldOut;

            if (_HUDTab[_HUDTabID].HUD_TabOjects[i].HUD_FoldOut)
            {
                GUILayout.BeginVertical("box");
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

                //Type
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
                GUILayout.EndVertical();
            }
        }
    }
    void HUD_Editor_CanvasOptions()
    {
        if (_MainCanvas != null)
        {
            //LiveEdit
            if (_HUD_EnableLiveEdit)
            {
                if (GUILayout.Button("Create", GUILayout.Height(30)))
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
    void HUDEditorRefresh()
    {
        for (int o = 0; o < _HUDTab.Count; o++)
        {
            for (int i = 0; i < _HUDTab[o].HUD_TabOjects.Count; i++)
            {
                if (_HUDTab[o].HUD_TabOjects[i].HUD_Object != null)
                {
                    //Update HUD
                    HUD_Change_Position(_HUDTab[o].HUD_TabOjects[i]);
                    HUD_Set_Size(_HUDTab[o].HUD_TabOjects[i].HUD_RectTransform, _HUDTab[o].HUD_TabOjects[i].HUD_Size);
                    HUD_Set_Scale(_HUDTab[o].HUD_TabOjects[i].HUD_RectTransform, _HUDTab[o].HUD_TabOjects[i].HUD_Scale);
                    HUD_Set_SetOffSet(_HUDTab[o].HUD_TabOjects[i].HUD_RectTransform, _HUDTab[o].HUD_TabOjects[i].HUD_Offset);
                    _HUDTab[o].HUD_TabOjects[i].HUD_Object.name = _HUDTab[o].HUD_TabOjects[i].HUD_Name;

                    HUD_TextSize(_HUDTab[o].HUD_TabOjects[i]);

                    //Update canvas size / tab size
                    if (_MainCanvasRect == null)
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
                    for (int j = 0; j < _HUDTab[o].HUD_TabOjects.Count; j++)
                    {
                        if (_HUDTab[o].HUD_TabOjects[j].HUD_Type == Tool_QuickStartUI_Object.HUD_Types.Button)
                        {
                            for (int p = 0; p < _HUDTab[o].HUD_TabOjects[j].HUD_Text.Count; p++)
                            {
                                _HUDTab[o].HUD_TabOjects[j].HUD_Text[p].rectTransform.sizeDelta = _HUDTab[o].HUD_TabOjects[j].HUD_Size;
                            }
                        }
                    }
                }
            }
        }
    }

    //Home > QuickUI : HUD Edit
    void HUD_Change_Position(Tool_QuickStartUI_Object obj)
    {
        obj.HUD_RectTransform.position = _MainCanvas.transform.position;
        switch (obj.HUD_Location)
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

        rect_tab.anchorMin = new Vector2(0, 1);
        rect_tab.anchorMax = new Vector2(0, 1);
        rect_tab.pivot = new Vector2(0, 1);

        rect_tab.sizeDelta = rect_main.sizeDelta;
        rect_tab.localScale = new Vector3(1, 1, 1);
        rect_tab.position = _MainCanvas.transform.position;
        rect_tab.anchoredPosition = new Vector3(0, 0, 0);

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
    void HUD_LoadProfile_Refresh()
    {
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

        HUDEditorRefresh();
    }
    void HUD_LoadProfile_AdvancedMenu()
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

        HUD_LoadProfile_Refresh();
    }
    void HUD_LoadProfile_BasicStartMenu()
    {
        HUD_Add_Tab();
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

        HUD_LoadProfile_Refresh();
    }
    void HUD_LoadProfile_Settings()
    {

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


    //Home > Scene : Menu
    void Menu_Scene()
    {
        //Scene Structure
        _SceneObjects_Show_SceneStructure = EditorGUILayout.Foldout(_SceneObjects_Show_SceneStructure, "Scene Structure (wip):");
        if (_SceneObjects_Show_SceneStructure)
        {
            for (int i = 0; i < _SceneStructure.Length; i++)
            {
                _SceneStructure[i] = EditorGUILayout.Toggle(_SceneStructureOptions[i], _SceneStructure[i]);
            }

            if (GUILayout.Button("Add Scene Structure"))
            {
                if (_SceneStructure[0])
                    if (GameObject.Find("----- Essentials -----") == null)
                    {
                        GameObject newobject_essentials = new GameObject();
                        newobject_essentials.name = "----- Essentials -----";
                    }

                if (_SceneStructure[1])
                    if (GameObject.Find("----- Canvas -----") == null)
                    {
                        GameObject newobject_canvas = new GameObject();
                        newobject_canvas.name = "----- Canvas -----";
                    }

                if (_SceneStructure[2])
                    if (GameObject.Find("----- Map -----") == null)
                    {
                        GameObject newobject_map = new GameObject();
                        newobject_map.name = "----- Map -----";
                    }

                if (_SceneStructure[3])
                    if (GameObject.Find("----- Lighting -----") == null)
                    {
                        GameObject newobject_lighting = new GameObject();
                        newobject_lighting.name = "----- Lighting -----";
                    }

                if (_SceneStructure[4])
                    if (GameObject.Find("----- Other -----") == null)
                    {
                        GameObject newobject_other = new GameObject();
                        newobject_other.name = "----- Other -----";
                    }
            }

            //Organize
            Menu_Scene_Organizer();
        }

        //All Objects
        _SceneObjects_Show_SceneAllObject = EditorGUILayout.Foldout(_SceneObjects_Show_SceneAllObject, "Scene All Objects (wip):");
        if (_SceneObjects_Show_SceneAllObject)
            Menu_Scene_ObjectFiltered();
    }
    void Menu_Scene_Organizer()
    {
        GUILayout.Label("Organize");
        if (GUILayout.Button("Create SnapShot (wip)"))
        {
            _SceneObjects_Snapshot.Clear();

            //Get Root Objects
            List<GameObject> rootobjects = new List<GameObject>();
            Scene scene = SceneManager.GetActiveScene();
            scene.GetRootGameObjects(rootobjects);

            //Add Root Objects To SnapShot Profile
            for (int i = 0; i < rootobjects.Count; ++i)
            {
                GameObject gameObject = rootobjects[i];
                Tool_QuickStart_SceneOrganizer parentobject = new Tool_QuickStart_SceneOrganizer();
                parentobject.ParentObject = rootobjects[i];
                _SceneObjects_Snapshot.Add(parentobject);
            }

            //Get Child Object
            for (int i = 0; i < _SceneObjects_Snapshot.Count; i++)
            {
                for (int j = 0; j < _SceneObjects_Snapshot[i].ParentObject.transform.childCount; j++)
                {
                    Tool_QuickStart_SceneOrganizer_GameObjectProfile newchildobj = new Tool_QuickStart_SceneOrganizer_GameObjectProfile();
                    newchildobj.ScriptAmount = _SceneObjects_Snapshot[i].ParentObject.transform.GetChild(j).GetComponents<MonoBehaviour>().Length;
                    newchildobj.ChildObject = _SceneObjects_Snapshot[i].ParentObject.transform.GetChild(j).gameObject;

                    _SceneObjects_Snapshot[i].ChildObjects.Add(newchildobj);
                }
            }
        }

        //SnapShot
        _SceneObjects_ShowSnapshot = EditorGUILayout.Foldout(_SceneObjects_ShowSnapshot, "SnapShot");
        if (_SceneObjects_ShowSnapshot)
        {
            for (int i = 0; i < _SceneObjects_Snapshot.Count; i++)
            {
                GUILayout.Label(_SceneObjects_Snapshot[i].ParentObject.name);

                for (int j = 0; j < _SceneObjects_Snapshot[i].ChildObjects.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    //Object
                    GUILayout.Label(_SceneObjects_Snapshot[i].ChildObjects[j].ChildObject.name);

                    //HasScripts
                    GUILayout.Label("Scripts: " + _SceneObjects_Snapshot[i].ChildObjects[j].ScriptAmount.ToString());

                    //Up Down List
                    bool check1 = false;
                    bool check2 = false;
                    if (j == 0)
                        check1 = true;
                    if (j == _SceneObjects_Snapshot[i].ChildObjects.Count - 1)
                        check2 = true;
                    EditorGUI.BeginDisabledGroup(check1);
                    if (GUILayout.Button("/\\", GUILayout.Width(20)))
                    {
                        GameObject oldobj = _SceneObjects_Snapshot[i].ChildObjects[j - 1].ChildObject;
                        GameObject newobj = _SceneObjects_Snapshot[i].ChildObjects[j].ChildObject;
                        _SceneObjects_Snapshot[i].ChildObjects[j - 1].ChildObject = newobj;
                        _SceneObjects_Snapshot[i].ChildObjects[j].ChildObject = oldobj;
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.BeginDisabledGroup(check2);
                    if (GUILayout.Button("\\/", GUILayout.Width(20)))
                    {
                        GameObject oldobj = _SceneObjects_Snapshot[i].ChildObjects[j + 1].ChildObject;
                        GameObject newobj = _SceneObjects_Snapshot[i].ChildObjects[j].ChildObject;
                        _SceneObjects_Snapshot[i].ChildObjects[j + 1].ChildObject = newobj;
                        _SceneObjects_Snapshot[i].ChildObjects[j].ChildObject = oldobj;
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        //Apply
        if (GUILayout.Button("Apply"))
        {
            for (int i = 0; i < _SceneObjects_Snapshot.Count; i++)
            {
                for (int j = 0; j < _SceneObjects_Snapshot[i].ChildObjects.Count; j++)
                {
                    _SceneObjects_Snapshot[i].ChildObjects[j].ChildObject.transform.parent = null;
                    _SceneObjects_Snapshot[i].ChildObjects[j].ChildObject.transform.parent = _SceneObjects_Snapshot[i].ParentObject.transform;
                }
            }
        }
    }
    void Menu_Scene_ObjectFiltered()
    {
        //Scan Scene
        if(GUILayout.Button("Scan Scene"))
        {
            _AllSceneObjects.Clear();
            GameObject[] allobjects = GameObject.FindObjectsOfType<GameObject>(true);
            for (int i = 0; i < allobjects.Length; i++)
            {
                Tool_QuickStart_SceneOrganizer_GameObjectProfile_All newobj = new Tool_QuickStart_SceneOrganizer_GameObjectProfile_All();
                newobj.Scripts = new List<string>();
                newobj.ChildObject = allobjects[i];

                //Get Script Names
                MonoBehaviour[] scripts = allobjects[i].GetComponents<MonoBehaviour>();
                List<String> scriptnames = new List<string>();
                foreach(MonoBehaviour mb in scripts)
                {
                    scriptnames.Add(mb.GetType().Name);
                }

                //Add Script Names To Object Profiles
                for (int j = 0; j < scriptnames.Count; j++)
                {
                    newobj.Scripts.Add(scriptnames[j]);
                }

                _AllSceneObjects.Add(newobj);
            }
        }

        //Search
        EditorGUILayout.BeginHorizontal("box");
        _SceneObjects_Search = EditorGUILayout.TextField("Search: ", _SceneObjects_Search);
        EditorGUILayout.EndHorizontal();

        //Search Script
        EditorGUILayout.BeginHorizontal("box");
        _SceneObjects_SearchScript = EditorGUILayout.TextField("Search Script: ", _SceneObjects_SearchScript);
        _SceneObjects_Filter_HasScript = EditorGUILayout.Toggle("Has Scripts", _SceneObjects_Filter_HasScript);
        _SceneObjects_Show_Scripts = EditorGUILayout.Toggle("Show Scripts", _SceneObjects_Show_Scripts);
        EditorGUILayout.EndHorizontal();

        //Loop Trough Objects/Scripts
        _Scene_Scroll = EditorGUILayout.BeginScrollView(_Scene_Scroll);
        for (int i = 0; i < _AllSceneObjects.Count; i++)
        {
            if (_AllSceneObjects[i].ChildObject.name.ToLower().Contains(_SceneObjects_Search.ToLower()))
            {
                if (_SceneObjects_Filter_HasScript || _SceneObjects_SearchScript != "")
                    Scene_ObjectFilter_SearchScript(i);
                else
                    Scene_ObjectFilter_Search(i);
            }
        }
        EditorGUILayout.EndScrollView();
    }
    void Scene_ObjectFilter_Search(int i)
    {
        if (_SceneObjects_Show_Scripts)
        {
            if (_AllSceneObjects[i].Scripts.Count > 0)
            {
                EditorGUILayout.BeginVertical("box");
                GUILayout.Label(i.ToString() + "  -  " + _AllSceneObjects[i].ChildObject.name + "  -  " + "Scripts: " + _AllSceneObjects[i].Scripts.Count.ToString());
                for (int j = 0; j < _AllSceneObjects[i].Scripts.Count; j++)
                {
                    GUILayout.Label("> " + _AllSceneObjects[i].Scripts[j] + ".cs");
                }
                EditorGUILayout.EndVertical();
            }
            else
                GUILayout.Label(i.ToString() + "  -  " + _AllSceneObjects[i].ChildObject.name + "  -  " + "Scripts: 0");
        }
        else
        {
            if (_AllSceneObjects[i].Scripts.Count > 0)
                GUILayout.Label(i.ToString() + "  -  " + _AllSceneObjects[i].ChildObject.name + "  -  " + "Scripts: " + _AllSceneObjects[i].Scripts.Count.ToString());
            else
                GUILayout.Label(i.ToString() + "  -  " + _AllSceneObjects[i].ChildObject.name + "  -  " + "Scripts: 0");
        }

    }
    void Scene_ObjectFilter_SearchScript(int i)
    {
        if (_AllSceneObjects[i].Scripts.Count > 0)
        {
            if (_SceneObjects_SearchScript != "")
            {
                bool check = false;
                for (int j = 0; j < _AllSceneObjects[i].Scripts.Count; j++)
                {
                    if (_AllSceneObjects[i].Scripts[j].ToLower().Contains(_SceneObjects_SearchScript.ToLower()))
                        check = true;
                }
                if (check)
                {
                    EditorGUILayout.BeginVertical("box");
                    GUILayout.Label(i.ToString() + "  -  " + _AllSceneObjects[i].ChildObject.name + "  -  " + "Scripts: " + _AllSceneObjects[i].Scripts.Count);
                    for (int j = 0; j < _AllSceneObjects[i].Scripts.Count; j++)
                    {
                        GUILayout.Label("> " + _AllSceneObjects[i].Scripts[j] + ".cs");
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                if (_SceneObjects_Show_Scripts)
                {
                    EditorGUILayout.BeginVertical("box");
                    GUILayout.Label(i.ToString() + "  -  " + _AllSceneObjects[i].ChildObject.name + "  -  " + "Scripts: " + _AllSceneObjects[i].Scripts.Count);
                    for (int j = 0; j < _AllSceneObjects[i].Scripts.Count; j++)
                    {
                        GUILayout.Label("> " + _AllSceneObjects[i].Scripts[j] + ".cs");
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                    GUILayout.Label(i.ToString() + "  -  " + _AllSceneObjects[i].ChildObject.name + "  -  " + "Scripts: " + _AllSceneObjects[i].Scripts.Count);

            }
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

        if (_FF_Search != _FF_SearchCheck || _FF_Type != _FF_TypeCheck)
        {
            _FF_SearchResults = System.IO.Directory.GetFiles("Assets/", "*" + _FF_Type, System.IO.SearchOption.AllDirectories);
            _FF_SearchCheck = _FF_Search;
            _FF_TypeCheck = _FF_Type;
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
        STS_StringExample();
        EditorGUILayout.EndScrollView();
    }
    void STS_InputOutput()
    {
        GUILayout.Space(20);
        //Input
        GUILayout.Label("Input: ", EditorStyles.boldLabel);
        _STS_InputScript = EditorGUILayout.ObjectField(_STS_InputScript, typeof(MonoScript), false) as MonoScript;

        //Output
        GUILayout.Label("Output: ", EditorStyles.boldLabel);
        EditorGUILayout.TextField("", _STS_ScriptOutput);
        GUILayout.Space(20);
    }
    void STS_StringExample()
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
    string STS_ConvertScriptToString()
    {
        string newstring = "\"";
        string[] readText = File.ReadAllLines(STS_GetPath());

        for (int i = 0; i < readText.Length; i++)
        {
            string newline = "";
            for (int j = 0; j < readText[i].Length; j++)
            {
                if (System.Convert.ToString(readText[i][j]) == "\"")
                    newline += "\\";
                newline += System.Convert.ToString(readText[i][j]);
            }
            readText[i] = newline + "\\n";
            newstring += readText[i];
        }
        newstring += "\"";
        return newstring;
    }
    string STS_GetPath()
    {
        string[] filepaths = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
        for (int i = 0; i < filepaths.Length; i++)
        {
            if (filepaths[i].Contains(_STS_InputScript.name + ".cs"))
            {
                string[] filepathsplit = filepaths[i].Split(char.Parse("\\"));
                if (filepathsplit[filepathsplit.Length - 1] == _STS_InputScript.name + ".cs")
                    return filepaths[i];
            }
        }
        return "";
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

        _SceneStructure = new bool[_SceneStructureOptions.Length];
        for (int i = 0; i < _SceneStructure.Length; i++)
        {
            _SceneStructure[i] = true;
        }
        _UpdateLogFoldout[1] = true;
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
        if (_WindowID == 4)
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
        if (_WindowID == 4)
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


    //UpdateLog
    void UpdateLog()
    {
        _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Github"))
            Application.OpenURL("https://github.com/MarcelvanDuijnDev/Unity-Presets-Scripts-Tools");
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Update Log", EditorStyles.boldLabel);

        _UpdateLogFoldout[1] = EditorGUILayout.Foldout(_UpdateLogFoldout[1], "2022");
        if (_UpdateLogFoldout[1])
        {
            GUILayout.Label(
                "\n" +
                "V1.2.0 (7-jan-2022)\n" +
                "Scene Window Updates\n" +
                "* Added Scene GameObject Profiles\n" +
                "* Added Scene GameObject Explorer\n" +
                "* Added Scene Structure Options\n" +
                "* Added Multiple Filter Options\n" +
                "* Scene Structure checks if objects exist before creating\n" +
                "\n" +
                "V1.1.4 (6-jan-2022)\n" +
                "* Update log separated into years\n" +
                "\n" +
                "V1.1.3 (3-jan-2022)\n" +
                "* Added Script Descriptions\n" +
                "");
        }

        _UpdateLogFoldout[0] = EditorGUILayout.Foldout(_UpdateLogFoldout[0], "2021");
        if (_UpdateLogFoldout[0])
        {
            GUILayout.Label(
                "\nV1.1.2 (29-dec-2021)\n" +
                "* Realtime feedback > Script select \n" +
                "* Added QuickStart>2D>VisualNovel(wip) \n" +
                "\n" +
                "V1.1.1 (28-dec-2021)\n" +
                "* Multi Select improvements \n" +
                "\n" +
                "V1.1.0 (22-dec-2021)\n" +
                "* Added Add Scene Structure option\n" +
                "\n" +
                "V1.0.17 (20-dec-2021)\n" +
                "* Update UIEffects.cs\n" +
                "\n" +
                "V1.0.16 (9-dec-2021)\n" +
                "* Update ScriptToString\n" +
                "* Update AudioHandler.cs\n" +
                "* Update AudioZone.cs\n" +
                "* Added AudioZoneEditor.cs to scripts\n" +
                "* Added Select all option \n" +
                "* Fix Script > Tool_ScriptToString.cs\n" +
                "* Fix Script > DialogSystemEditor.cs\n" +
                "* Cleanup Generate examples\n" +
                "* Increased script search precision\n" +
                "\n" +
                "V1.0.15 (23-nov-2021)\n" +
                "* Wrong bullet script fix\n" +
                "\n" +
                "V1.0.14 (19-nov-2021)\n" +
                "* Update AudioHandler.cs\n" +
                "* Update AudioZoneSphere.cs\n" +
                "* Removed AudioZoneBox.cs\n" +
                "* Disabled Empty scripts\n" +
                "\n" +
                "V1.0.13 (4-nov-2021)\n" +
                "* Update Interactable.cs\n" +
                "* Update InteractionHandler.cs\n" +
                "\n" +
                "V1.0.12 (24-oct-2021)\n" +
                "* Added FadeInOut.cs\n" +
                "\n" +
                "V1.0.11 (17-oct-2021)\n" +
                "* Added Multi Select option\n" +
                "* Fix MapEditor not working\n" +
                "\n" +
                "V1.0.10 (15-oct-2021)\n" +
                "* Updated AudioHandler code\n" +
                "\n" +
                "V1.0.9 (5-sep-2021)\n" +
                "* Fix QuickUI formating\n" +
                "* Added QuickUI profiles\n" +
                "* Updated QuickUI editor layout\n" +
                "\n" +
                "V1.0.8 (22-aug-2021)\n" +
                "* Fix update log not scrolling\n" +
                "\n" +
                "V1.0.7 (20-aug-2021)\n" +
                "* Updated AudioZoneBox.cs\n" +
                "* Updated AudioZoneSphere.cs\n" +
                "\n" +
                "V1.0.6 (18-aug-2021)\n" +
                "* Added AudioZoneBox.cs\n" +
                "* Added AudioZoneSphere.cs\n" +
                "\n" +
                "V1.0.5 (13-aug-2021)\n" +
                "* Added dates to updatelog\n" +
                "* Fixed Loading wrong script (SaveLoad_JSON) \n" +
                "\n" +
                "V1.0.4 (23-jul-2021)\n" +
                "* Added DialogSystem.cs + DialogSystemEditor.cs\n" +
                "\n" +
                "V1.0.3 (22-jul-2021)\n" +
                "* Fixed Typo > Scripts\n" +
                "\n" +
                "V1.0.2 (22-jul-2021)\n" +
                "* Added Update log\n" +
                "\n" +
                "V1.0.1 (22-jul-2021)\n" +
                "* Updated Cleanup Script To String (STS)\n" +
                "* File Finder (FF) Now updates when changing type\n" +
                "\n" +
                "V1.0.0 (22-jul-2021)\n" +
                "* Start QuickStart update log \n" +
                "* Added Scripts\n" +
                "* Fixed Scripts formating\n" +
                "* Refactor Script To String (STS)\n" +
                "\n \n" +
                "(22-jul-2021) \n" +
                "Start Update Log\n" +
                "               △\n               △\n" +
                "Multiple changes \n" +
                "               △\n               △\n" +
                "(26-oct-2020)\n" +
                "* Created Tool_QuickStart.cs");
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
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
    private string _Script_Description;
    private string _Script_Code;
    private string _Script_Path;

    public bool Exist;

    public string ScriptName { get { return _Script_Name; } }
    public string ScriptTag { get { return _Script_Tag; } }
    public string ScriptState { get { return _Script_State; } }
    public string ScriptDescription { get { return _Script_Description; } }
    public string ScriptCode { get { return _Script_Code; } }
    public string ScriptPath { get { return _Script_Path; } set { _Script_Path = value; } }

    public Tool_QuickStart_Script(string name, string tags, string state, string description, string code)
    {
        _Script_Name = name;
        _Script_Tag = tags;
        _Script_State = state;
        _Script_Description = description;
        _Script_Code = code;
    }
}
public class Tool_QuickStart_SceneOrganizer
{
    public GameObject ParentObject;
    public List<Tool_QuickStart_SceneOrganizer_GameObjectProfile> ChildObjects = new List<Tool_QuickStart_SceneOrganizer_GameObjectProfile>();
}
public class Tool_QuickStart_SceneOrganizer_GameObjectProfile
{
    public GameObject ChildObject;
    public bool Changed = false;
    public int ScriptAmount;
}
public class Tool_QuickStart_SceneOrganizer_GameObjectProfile_All
{
    public GameObject ChildObject;
    public List<string> Scripts = new List<string>();
}