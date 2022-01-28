using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

public class AudioHandler : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Only used for testing disable it for the final build to improve performance")]
    [SerializeField] private bool _RefreshSettingsOnUpdate = false;

    [Header("AudioMixer/Audio")]
    [SerializeField] private AudioMixerGroup _AudioMixer = null;
    [SerializeField] private List<AudioHandler_Sound> _Sound = new List<AudioHandler_Sound>();

    private string _CurrentScene;

    //You can call AudioHandler.AUDIO from every script as long as you have the script in the scene.
    public static AudioHandler AUDIO;

    void Awake()
    {
        AUDIO = this;

        //PlayOnStart
        for (int i = 0; i < _Sound.Count; i++)
        {
            //AudioSource
            if (_Sound[i].Settings.CreateAudioSource)
            {
                //3D Space
                if (_Sound[i].Audio3D.Enable3DAudio)
                {
                    //Create new object
                    GameObject audiopos = new GameObject("Audio_" + _Sound[i].AudioTrackName);

                    //Set audiopos position
                    if (_Sound[i].Audio3D.SpatialTransform != null)
                        audiopos.transform.position = _Sound[i].Audio3D.SpatialTransform.position;
                    else
                        audiopos.transform.position = _Sound[i].Audio3D.SpatialPosition;
                    audiopos.transform.parent = this.gameObject.transform;

                    //Add AudioSource to audioposition
                    _Sound[i].Settings.AudioSource = audiopos.AddComponent<AudioSource>();
                }
                else
                    _Sound[i].Settings.AudioSource = this.gameObject.AddComponent<AudioSource>();

                //SetVolume
                _Sound[i].Settings.AudioSource.volume = _Sound[i].AudioSettings.Volume;

                //AudioMixer
                _Sound[i].Settings.AudioSource.outputAudioMixerGroup = _AudioMixer;

                //AudioGroup
                if (_Sound[i].Settings.AudioGroup != null)
                    _Sound[i].Settings.AudioSource.outputAudioMixerGroup = _Sound[i].Settings.AudioGroup;
            }

            //3D Space Settings
            if (_Sound[i].Audio3D.Enable3DAudio)
            {
                _Sound[i].Settings.AudioSource.spatialBlend = 1;
            }

            //AudioClip
            _Sound[i].Settings.AudioSource.clip = _Sound[i].Settings.AudioClip;

            //Settings
            if (!_Sound[i].AudioSettings.PlayOnStart_DiplicateOnly)
            {
                if (_Sound[i].AudioSettings.PlayOnStart)
                {
                    _Sound[i].Settings.AudioSource.playOnAwake = _Sound[i].AudioSettings.PlayOnStart;
                    _Sound[i].Settings.AudioSource.Play();
                }
                if (_Sound[i].AudioEffects.FadeIn)
                {
                    _Sound[i].Settings.AudioSource.volume = 1;
                    _Sound[i].AudioEffects.FadeInSpeed = _Sound[i].AudioSettings.Volume / _Sound[i].AudioEffects.FadeInDuration;
                }
                if (_Sound[i].AudioEffects.FadeOut)
                {
                    _Sound[i].AudioEffects.FadeOutSpeed = _Sound[i].AudioSettings.Volume / _Sound[i].AudioEffects.FadeOutDuration;
                }
            }
        }

        RefreshSettings();
    }

    void Update()
    {
        CheckNewScene();

        if (_RefreshSettingsOnUpdate)
            RefreshSettings();

        for (int i = 0; i < _Sound.Count; i++)
        {
            //FadeIn
            if (_Sound[i].AudioEffects.FadingIn)
            {
                if (_Sound[i].AudioEffects.FadeIn && !_Sound[i].AudioEffects.FadeInDone)
                {
                    if (_Sound[i].Settings.AudioSource.volume < _Sound[i].AudioSettings.Volume)
                    {
                        _Sound[i].Settings.AudioSource.volume += _Sound[i].AudioEffects.FadeInSpeed * Time.deltaTime;
                    }
                    else
                    {
                        _Sound[i].AudioEffects.FadeInDone = true;
                        _Sound[i].Settings.AudioSource.volume = _Sound[i].AudioSettings.Volume;
                    }
                }
            }
            //FadeOut
            if (_Sound[i].AudioEffects.FadingOut)
            {
                if (_Sound[i].AudioEffects.FadeOutAfterTime > -0.1f)
                {
                    _Sound[i].AudioEffects.FadeOutAfterTime -= 1 * Time.deltaTime;
                }
                else
                {
                    if (_Sound[i].AudioEffects.FadeOut && !_Sound[i].AudioEffects.FadeOutDone)
                    {
                        if (_Sound[i].Settings.AudioSource.volume > 0)
                        {
                            _Sound[i].Settings.AudioSource.volume -= _Sound[i].AudioEffects.FadeOutSpeed * Time.deltaTime;
                        }
                        else
                        {
                            _Sound[i].AudioEffects.FadeOutDone = true;
                            _Sound[i].Settings.AudioSource.volume = 0;
                            _Sound[i].Settings.AudioSource.Stop();
                        }
                    }
                }
            }
        }
    }

    private void CheckNewScene()
    {
        if (_CurrentScene != SceneManager.GetActiveScene().name)
        {
            _CurrentScene = SceneManager.GetActiveScene().name;
            for (int i = 0; i < _Sound.Count; i++)
            {
                //Stop NextScene
                if (_Sound[i].AudioControl.StopOnNextScene)
                {
                    //FadeOut
                    if (_Sound[i].AudioEffects.FadeOut && !_Sound[i].AudioEffects.FadingOut)
                    {
                        _Sound[i].AudioEffects.FadingIn = false;
                        _Sound[i].AudioEffects.FadeOutDone = false;
                        _Sound[i].AudioEffects.FadingOut = true;
                    }
                    else
                        _Sound[i].Settings.AudioSource.Stop();
                }

                //Start AudioOnScene
                for (int o = 0; o < _Sound[i].AudioControl.StartAudioOnScene.Count; o++)
                {
                    if (_Sound[i].AudioControl.StartAudioOnScene[o] == _CurrentScene)
                    {
                        //FadeIn
                        if (_Sound[i].AudioEffects.FadeIn)
                        {
                            _Sound[i].AudioEffects.FadingOut = false;
                            _Sound[i].AudioEffects.FadeInDone = false;
                            _Sound[i].AudioEffects.FadingIn = true;
                        }
                        _Sound[i].Settings.AudioSource.Play();
                    }
                }

                //Stop AudioOnScene
                for (int o = 0; o < _Sound[i].AudioControl.StopAudioOnScene.Count; o++)
                {
                    if (_Sound[i].AudioControl.StopAudioOnScene[o] == _CurrentScene)
                    {
                        //FadeOut
                        if (_Sound[i].AudioEffects.FadeOut && !_Sound[i].AudioEffects.FadingOut)
                        {
                            _Sound[i].AudioEffects.FadingIn = false;
                            _Sound[i].AudioEffects.FadeOutDone = false;
                            _Sound[i].AudioEffects.FadingOut = true;
                        }
                        else
                            _Sound[i].Settings.AudioSource.Stop();
                    }
                }
            }
        }
    }
    private void AudioHandler_PlayTrack(int trackid)
    {
        _Sound[trackid].Settings.AudioSource.Play();
    }

    /// <summary>Plays the audiotrack.</summary>
    public void PlayTrack(string trackname)
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            if (_Sound[i].AudioTrackName == trackname)
                AudioHandler_PlayTrack(i);
        }
    }
    public void PlayTrack(int trackid)
    {
        AudioHandler_PlayTrack(trackid);
    }

    /// <summary>Plays the audiotrack if it's not playing yet.</summary>
    public void StartTrack(string trackname)
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            if (_Sound[i].AudioTrackName == trackname)
                if (!_Sound[i].Settings.AudioSource.isPlaying)
                    AudioHandler_PlayTrack(i);
        }
    }
    public void StartTrack(int trackid)
    {
        if (!_Sound[trackid].Settings.AudioSource.isPlaying)
            AudioHandler_PlayTrack(trackid);
    }

    /// <summary>Stops the audiotrack.</summary>
    public void StopTrack(string trackname)
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            if (_Sound[i].AudioTrackName == trackname)
                _Sound[i].Settings.AudioSource.Stop();
        }
    }
    public void StopTrack(int trackid)
    {
        _Sound[trackid].Settings.AudioSource.Stop();
    }

    /// <summary>Returns audio file name.</summary>
    public string Get_Track_AudioFileName(string trackname)
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            if (_Sound[i].AudioTrackName == trackname)
                return _Sound[i].Settings.AudioClip.name;
        }
        return "No AudioClip detected";
    }
    public string Get_Track_AudioFileName(int trackid)
    {
        return _Sound[trackid].Settings.AudioClip.name;
    }

    /// <summary>Set audiosource.</summary>
    public void SetAudioSource(string trackname, AudioSource audiosource)
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            if (_Sound[i].AudioTrackName == trackname)
                _Sound[i].Settings.AudioSource = audiosource;
        }
    }
    public void SetAudioSource(int trackid, AudioSource audiosource)
    {
        _Sound[trackid].Settings.AudioSource = audiosource;
    }

    /// <summary>Set track volume.</summary>
    public void SetTrackVolume(string trackname, float volume, bool checkmaxvolume)
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            if (_Sound[i].AudioTrackName == trackname)
            {
                if (!checkmaxvolume)
                    _Sound[i].AudioSettings.Volume = volume;
                else
                    if (volume >= _Sound[i].AudioSettings.MaxVolume)
                    _Sound[i].AudioSettings.Volume = _Sound[i].AudioSettings.MaxVolume;
                else
                    _Sound[i].AudioSettings.Volume = volume;
                break;
            }
        }
    }
    public void SetTrackVolume(int trackid, float volume, bool checkmaxvolume)
    {
        if (!checkmaxvolume)
            _Sound[trackid].AudioSettings.Volume = volume;
        else if (volume >= _Sound[trackid].AudioSettings.MaxVolume)
            _Sound[trackid].AudioSettings.Volume = _Sound[trackid].AudioSettings.MaxVolume;
        else
            _Sound[trackid].AudioSettings.Volume = volume;
    }

    /// <summary>Returns track id.</summary>
    public int Get_Track_ID(string trackname)
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            if (_Sound[i].AudioTrackName == trackname)
                return i;
        }
        return -1;
    }

    /// <summary>Refresh settings.</summary>
    public void RefreshSettings()
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            //SetClip
            if (_Sound[i].Settings.AudioSource.clip != _Sound[i].Settings.AudioClip)
                _Sound[i].Settings.AudioSource.clip = _Sound[i].Settings.AudioClip;
            //SetEffects
            if (!_Sound[i].AudioEffects.FadeIn || _Sound[i].AudioEffects.FadeIn && _Sound[i].AudioEffects.FadeInDone)
                _Sound[i].Settings.AudioSource.volume = _Sound[i].AudioSettings.Volume;
            _Sound[i].Settings.AudioSource.loop = _Sound[i].AudioSettings.Loop;
        }
    }

    /// <summary>Duplicate AudioTrack.</summary>
    public string DuplicateAudioTrack(string trackname)
    {
        int audioid = Get_Track_ID(trackname);
        if (audioid == -1)
            return null;

        AudioHandler_Sound newsound = new AudioHandler_Sound();
        GameObject newaudiopos = new GameObject();

        newsound.AudioTrackName = "Audio_" + _Sound[audioid].AudioTrackName;

        //Settings
        newsound.Settings = new AudioHandler_Settings();
        newsound.Settings.AudioClip = _Sound[audioid].Settings.AudioClip;
        newsound.Settings.AudioGroup = _Sound[audioid].Settings.AudioGroup;
        newsound.Settings.AudioSource = newaudiopos.AddComponent<AudioSource>();
        newsound.Settings.CreateAudioSource = _Sound[audioid].Settings.CreateAudioSource;

        //Control
        newsound.AudioControl = new AudioHandler_Control();
        newsound.AudioControl.SceneEnabled = _Sound[audioid].AudioControl.SceneEnabled;
        newsound.AudioControl.StartAudioOnScene = _Sound[audioid].AudioControl.StartAudioOnScene;
        newsound.AudioControl.StopAudioOnScene = _Sound[audioid].AudioControl.StopAudioOnScene;
        newsound.AudioControl.StopOnNextScene = _Sound[audioid].AudioControl.StopOnNextScene;

        //Audio3D
        newsound.Audio3D = new AudioHandler_3DAudio();
        newsound.Audio3D.Enable3DAudio = _Sound[audioid].Audio3D.Enable3DAudio;
        newsound.Audio3D.SpatialPosition = _Sound[audioid].Audio3D.SpatialPosition;
        newsound.Audio3D.SpatialTransform = _Sound[audioid].Audio3D.SpatialTransform;

        //AudioSettings
        newsound.AudioSettings = new AudioHandler_AudioSettings();
        newsound.AudioSettings.Loop = _Sound[audioid].AudioSettings.Loop;
        newsound.AudioSettings.MaxVolume = _Sound[audioid].AudioSettings.MaxVolume;
        newsound.AudioSettings.PlayOnStart = _Sound[audioid].AudioSettings.PlayOnStart;
        newsound.AudioSettings.Volume = _Sound[audioid].AudioSettings.Volume;

        //AudioEffect
        newsound.AudioEffects = new AudioHandler_Effects();
        newsound.AudioEffects.FadeIn = _Sound[audioid].AudioEffects.FadeIn;
        newsound.AudioEffects.FadeInDone = _Sound[audioid].AudioEffects.FadeInDone;
        newsound.AudioEffects.FadeInDuration = _Sound[audioid].AudioEffects.FadeInDuration;
        newsound.AudioEffects.FadeInSpeed = _Sound[audioid].AudioEffects.FadeInSpeed;
        newsound.AudioEffects.FadeOut = _Sound[audioid].AudioEffects.FadeOut;
        newsound.AudioEffects.FadeOutAfterTime = _Sound[audioid].AudioEffects.FadeOutAfterTime;
        newsound.AudioEffects.FadeOutDone = _Sound[audioid].AudioEffects.FadeOutDone;
        newsound.AudioEffects.FadeOutDuration = _Sound[audioid].AudioEffects.FadeOutDuration;
        newsound.AudioEffects.FadeOutSpeed = _Sound[audioid].AudioEffects.FadeOutSpeed;
        newsound.AudioEffects.FadingIn = _Sound[audioid].AudioEffects.FadingIn;
        newsound.AudioEffects.FadingOut = _Sound[audioid].AudioEffects.FadingOut;

        newsound.AudioTrackName += "_" + _Sound.Count.ToString();

        //Activate Settings
        newsound.Settings.AudioSource.loop = newsound.AudioSettings.Loop;
        newsound.Settings.AudioSource.volume = newsound.AudioSettings.Volume;
        if (newsound.Audio3D.Enable3DAudio)
            newsound.Settings.AudioSource.spatialBlend = 1;
        if (newsound.AudioSettings.PlayOnStart)
            newsound.Settings.AudioSource.Play();

        //Create new object
        newaudiopos.transform.name = newsound.AudioTrackName;

        //Audio Source Settings
        newsound.Settings.AudioSource.clip = newsound.Settings.AudioClip;
        newsound.Settings.AudioSource.outputAudioMixerGroup = newsound.Settings.AudioGroup;

        //Position
        if (newsound.Audio3D.SpatialTransform != null)
            ChangeAudioPosition(newsound.AudioTrackName, newsound.Audio3D.SpatialTransform.position);
        else
            ChangeAudioPosition(newsound.AudioTrackName, newsound.Audio3D.SpatialPosition);

        //PlayOnStart
        if (newsound.AudioSettings.PlayOnStart)
            newsound.Settings.AudioSource.Play();

        //Apply
        newaudiopos.transform.parent = this.transform;
        _Sound.Add(newsound);
        return newsound.AudioTrackName;
    }

    /// <summary>Change AudioSource Position.</summary>
    public void ChangeAudioPosition(string trackname, Vector3 newpos)
    {
        int audioid = Get_Track_ID(trackname);
        if (audioid != -1)
            _Sound[audioid].Settings.AudioSource.transform.position = newpos;
    }
}

[System.Serializable]
public class AudioHandler_Sound
{
    public string AudioTrackName;
    public AudioHandler_Settings Settings;
    public AudioHandler_AudioSettings AudioSettings;
    public AudioHandler_3DAudio Audio3D;
    public AudioHandler_Control AudioControl;
    public AudioHandler_Effects AudioEffects;
}

[System.Serializable]
public class AudioHandler_Settings
{
    [Header("AudioClip/AudioMixerGroup")]
    public AudioClip AudioClip;
    public AudioMixerGroup AudioGroup;

    [Header("AudioSource")]
    public AudioSource AudioSource;
    public bool CreateAudioSource;
}

[System.Serializable]
public class AudioHandler_AudioSettings
{
    [Header("AudioSettings")]
    [Range(0, 1)] public float Volume = 1;
    [Range(0, 1)] public float MaxVolume = 1;
    public bool Loop;
    public bool PlayOnStart;
    public bool PlayOnStart_DiplicateOnly;
}

[System.Serializable]
public class AudioHandler_Control
{
    [Header("Enable/Disable Song")]
    public List<string> StartAudioOnScene = new List<string>();
    public List<string> StopAudioOnScene = new List<string>();
    public bool StopOnNextScene;
    [HideInInspector] public int SceneEnabled;
}

[System.Serializable]
public class AudioHandler_Effects
{
    [Header("FadeIn")]
    public bool FadeIn;
    public float FadeInDuration;
    [HideInInspector] public float FadeInSpeed;
    [HideInInspector] public bool FadeInDone;
    [HideInInspector] public bool FadingIn;
    [Header("FadeOut")]
    public bool FadeOut;
    public float FadeOutAfterTime;
    public float FadeOutDuration;
    [HideInInspector] public float FadeOutSpeed;
    [HideInInspector] public bool FadeOutDone;
    [HideInInspector] public bool FadingOut;
}

[System.Serializable]
public class AudioHandler_3DAudio
{
    [Header("3D Space / (0,0,0)+null = this object position")]
    public bool Enable3DAudio;
    public Vector3 SpatialPosition;
    public Transform SpatialTransform;
}