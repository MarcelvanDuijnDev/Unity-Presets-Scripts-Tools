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

    [Header("Audio Categorys")]
    public List<AudioHandler_Category> Category = new List<AudioHandler_Category>();

    private string _CurrentScene;

    //You can call AudioHandler.AUDIO from every script as long as you have the script in the scene.
    public static AudioHandler AUDIO;

    void Awake()
    {
        AUDIO = this;

        //PlayOnStart

        for (int cat = 0; cat < Category.Count; cat++)
        {
            for (int i = 0; i < Category[cat].Sounds.Count; i++)
            {
                //AudioSource
                if (Category[cat].Sounds[i].Settings.CreateAudioSource)
                {
                    //3D Space
                    if (Category[cat].Sounds[i].Audio3D.Enable3DAudio)
                    {
                        //Create new object
                        GameObject audiopos = new GameObject("Audio_" + Category[cat].Sounds[i].AudioTrackName);

                        //Set audiopos position
                        if (Category[cat].Sounds[i].Audio3D.SpatialTransform != null)
                            audiopos.transform.position = Category[cat].Sounds[i].Audio3D.SpatialTransform.position;
                        else
                            audiopos.transform.position = Category[cat].Sounds[i].Audio3D.SpatialPosition;
                        audiopos.transform.parent = this.gameObject.transform;

                        //Add AudioSource to audioposition
                        Category[cat].Sounds[i].Settings.AudioSource = audiopos.AddComponent<AudioSource>();
                    }
                    else
                        Category[cat].Sounds[i].Settings.AudioSource = this.gameObject.AddComponent<AudioSource>();

                    //SetVolume
                    Category[cat].Sounds[i].Settings.AudioSource.volume = Category[cat].Sounds[i].AudioSettings.Volume;

                    //AudioMixer
                    Category[cat].Sounds[i].Settings.AudioSource.outputAudioMixerGroup = _AudioMixer;

                    //AudioGroup
                    if (Category[cat].Sounds[i].Settings.AudioGroup != null)
                        Category[cat].Sounds[i].Settings.AudioSource.outputAudioMixerGroup = Category[cat].Sounds[i].Settings.AudioGroup;
                }

                //3D Space Settings
                if (Category[cat].Sounds[i].Audio3D.Enable3DAudio)
                {
                    Category[cat].Sounds[i].Settings.AudioSource.spatialBlend = 1;
                }

                //AudioClip
                if (Category[cat].Sounds[i].Settings.CreateAudioSource)
                    Category[cat].Sounds[i].Settings.AudioSource.clip = Category[cat].Sounds[i].Settings.AudioClip;

                //Settings
                if (!Category[cat].Sounds[i].AudioSettings.PlayOnStart_DiplicateOnly)
                {
                    if (Category[cat].Sounds[i].AudioSettings.PlayOnStart)
                    {
                        Category[cat].Sounds[i].Settings.AudioSource.playOnAwake = Category[cat].Sounds[i].AudioSettings.PlayOnStart;
                        Category[cat].Sounds[i].Settings.AudioSource.Play();
                    }
                    if (Category[cat].Sounds[i].AudioEffects.FadeIn)
                    {
                        Category[cat].Sounds[i].Settings.AudioSource.volume = 1;
                        Category[cat].Sounds[i].AudioEffects.FadeInSpeed = Category[cat].Sounds[i].AudioSettings.Volume / Category[cat].Sounds[i].AudioEffects.FadeInDuration;
                    }
                    if (Category[cat].Sounds[i].AudioEffects.FadeOut)
                    {
                        Category[cat].Sounds[i].AudioEffects.FadeOutSpeed = Category[cat].Sounds[i].AudioSettings.Volume / Category[cat].Sounds[i].AudioEffects.FadeOutDuration;
                    }
                }
            }
        }


        RefreshSettings_AllCategories();
    }

    void Update()
    {
        CheckNewScene();

        if (_RefreshSettingsOnUpdate)
            RefreshSettings_AllCategories();

        for (int cat = 0; cat < Category.Count; cat++)
        {
            if (!Category[cat]._CallOnly)
            {
                for (int i = 0; i < Category[cat].Sounds.Count; i++)
                {
                    //FadeIn
                    if (Category[cat].Sounds[i].AudioEffects.FadingIn)
                    {
                        if (Category[cat].Sounds[i].AudioEffects.FadeIn && !Category[cat].Sounds[i].AudioEffects.FadeInDone)
                        {
                            if (Category[cat].Sounds[i].Settings.AudioSource.volume < Category[cat].Sounds[i].AudioSettings.Volume)
                            {
                                Category[cat].Sounds[i].Settings.AudioSource.volume += Category[cat].Sounds[i].AudioEffects.FadeInSpeed * Time.deltaTime;
                            }
                            else
                            {
                                Category[cat].Sounds[i].AudioEffects.FadeInDone = true;
                                Category[cat].Sounds[i].Settings.AudioSource.volume = Category[cat].Sounds[i].AudioSettings.Volume;
                            }
                        }
                    }
                    //FadeOut
                    if (Category[cat].Sounds[i].AudioEffects.FadingOut)
                    {
                        if (Category[cat].Sounds[i].AudioEffects.FadeOutAfterTime > -0.1f)
                        {
                            Category[cat].Sounds[i].AudioEffects.FadeOutAfterTime -= 1 * Time.deltaTime;
                        }
                        else
                        {
                            if (Category[cat].Sounds[i].AudioEffects.FadeOut && !Category[cat].Sounds[i].AudioEffects.FadeOutDone)
                            {
                                if (Category[cat].Sounds[i].Settings.AudioSource.volume > 0)
                                {
                                    Category[cat].Sounds[i].Settings.AudioSource.volume -= Category[cat].Sounds[i].AudioEffects.FadeOutSpeed * Time.deltaTime;
                                }
                                else
                                {
                                    Category[cat].Sounds[i].AudioEffects.FadeOutDone = true;
                                    Category[cat].Sounds[i].Settings.AudioSource.volume = 0;
                                    Category[cat].Sounds[i].Settings.AudioSource.Stop();
                                }
                            }
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
            for (int cat = 0; cat < Category.Count; cat++)
            {
                if (!Category[cat]._CallOnly)
                {
                    for (int i = 0; i < Category[cat].Sounds.Count; i++)
                    {
                        //Stop NextScene
                        if (Category[cat].Sounds[i].AudioControl.StopOnNextScene)
                        {
                            //FadeOut
                            if (Category[cat].Sounds[i].AudioEffects.FadeOut && !Category[cat].Sounds[i].AudioEffects.FadingOut)
                            {
                                Category[cat].Sounds[i].AudioEffects.FadingIn = false;
                                Category[cat].Sounds[i].AudioEffects.FadeOutDone = false;
                                Category[cat].Sounds[i].AudioEffects.FadingOut = true;
                            }
                            else
                                Category[cat].Sounds[i].Settings.AudioSource.Stop();
                        }

                        //Start AudioOnScene
                        for (int o = 0; o < Category[cat].Sounds[i].AudioControl.StartAudioOnScene.Count; o++)
                        {
                            if (Category[cat].Sounds[i].AudioControl.StartAudioOnScene[o] == _CurrentScene)
                            {
                                //FadeIn
                                if (Category[cat].Sounds[i].AudioEffects.FadeIn)
                                {
                                    Category[cat].Sounds[i].AudioEffects.FadingOut = false;
                                    Category[cat].Sounds[i].AudioEffects.FadeInDone = false;
                                    Category[cat].Sounds[i].AudioEffects.FadingIn = true;
                                }
                                Category[cat].Sounds[i].Settings.AudioSource.Play();
                            }
                        }

                        //Stop AudioOnScene
                        for (int o = 0; o < Category[cat].Sounds[i].AudioControl.StopAudioOnScene.Count; o++)
                        {
                            if (Category[cat].Sounds[i].AudioControl.StopAudioOnScene[o] == _CurrentScene)
                            {
                                //FadeOut
                                if (Category[cat].Sounds[i].AudioEffects.FadeOut && !Category[cat].Sounds[i].AudioEffects.FadingOut)
                                {
                                    Category[cat].Sounds[i].AudioEffects.FadingIn = false;
                                    Category[cat].Sounds[i].AudioEffects.FadeOutDone = false;
                                    Category[cat].Sounds[i].AudioEffects.FadingOut = true;
                                }
                                else
                                    Category[cat].Sounds[i].Settings.AudioSource.Stop();
                            }
                        }
                    }
                }
            }
        }
    }
    //Always returns positive number to prevent errors
    private int AudioHandler_GetTrackID_Safe(string trackname, int categoryid)
    {
        for (int i = 0; i < Category[categoryid].Sounds.Count; i++)
            if (Category[categoryid].Sounds[i].AudioTrackName == trackname)
                return i;
        Debug.Log($"<color=#215EFF>AudioTrack:</color> {trackname} is not found in category: {Category[categoryid].CategoryName}, returned 0");
        return 0;
    }
    //Return negative when audiotrack is not found (used for functions that check if trackname exists)
    private int AudioHandler_GetTrackID_Unsafe(string trackname, int categoryid)
    {
        for (int i = 0; i < Category[categoryid].Sounds.Count; i++)
            if (Category[categoryid].Sounds[i].AudioTrackName == trackname)
                return i;
        Debug.Log($"<color=#215EFF>AudioTrack:</color> {trackname} is not found in category: {Category[categoryid].CategoryName},{categoryid}, returned 0");
        return 0;
    }
    private void AudioHandler_PlayTrack(int trackid, int categoryid = 0)
    {
        Category[categoryid].Sounds[trackid].Settings.AudioSource.Play();
    }

    /// <summary>Plays the audiotrack.</summary>
    public void PlayTrack(int trackid, int categoryid = 0)
    {
        AudioHandler_PlayTrack(trackid, categoryid);
    }
    public void PlayTrack(string trackname, int categoryid = 0)
    {
        AudioHandler_PlayTrack(AudioHandler_GetTrackID_Safe(trackname, categoryid), categoryid);
    }

    /// <summary>Plays the audiotrack if it's not playing yet.</summary>
    public void StartTrack(string trackname, int categoryid = 0)
    {
        int trackid = AudioHandler_GetTrackID_Safe(trackname, categoryid);
        if (!Category[categoryid].Sounds[trackid].Settings.AudioSource.isPlaying)
            AudioHandler_PlayTrack(trackid, categoryid);
    }
    public void StartTrack(int trackid, int categoryid = 0)
    {
        if (!Category[categoryid].Sounds[trackid].Settings.AudioSource.isPlaying)
            AudioHandler_PlayTrack(trackid, categoryid);
    }

    /// <summary>Stops the audiotrack.</summary>
    public void StopTrack(string trackname, int categoryid = 0)
    {
        Category[categoryid].Sounds[AudioHandler_GetTrackID_Safe(trackname, categoryid)].Settings.AudioSource.Stop();
    }
    public void StopTrack(int trackid, int categoryid = 0)
    {
        Category[categoryid].Sounds[trackid].Settings.AudioSource.Stop();
    }

    /// <summary>Returns audio file name.</summary>
    public string Get_Track_AudioFileName(string trackname, int categoryid = 0)
    {
        int trackid = AudioHandler_GetTrackID_Unsafe(trackname, categoryid);
        if (trackid >= 0)
            return Category[categoryid].Sounds[trackid].Settings.AudioClip.name;
        else
            return "No AudioClip detected";
    }
    public string Get_Track_AudioFileName(int trackid, int categoryid = 0)
    {
        return Category[categoryid].Sounds[trackid].Settings.AudioClip.name;
    }

    /// <summary>Returns audio source.</summary>
    public AudioSource Get_AudioSource(string trackname, int categoryid = 0)
    {
        int trackid = AudioHandler_GetTrackID_Unsafe(trackname, categoryid);
        if (trackid >= 0)
            return Category[categoryid].Sounds[trackid].Settings.AudioSource;
        else
            return null;
    }
    public AudioSource Get_AudioSource(int trackid, int categoryid = 0)
    {
        return Category[categoryid].Sounds[trackid].Settings.AudioSource;
    }

    /// <summary>Set audiosource.</summary>
    public void SetAudioSource(string trackname, AudioSource audiosource, int categoryid = 0)
    {
        Category[categoryid].Sounds[AudioHandler_GetTrackID_Safe(trackname, categoryid)].Settings.AudioSource = audiosource;
    }
    public void SetAudioSource(int trackid, AudioSource audiosource, int categoryid = 0)
    {
        Category[categoryid].Sounds[trackid].Settings.AudioSource = audiosource;
    }

    /// <summary>Set track volume.</summary>
    public void SetTrackVolume(string trackname, float volume, bool checkmaxvolume, int categoryid = 0)
    {
        int trackid = AudioHandler_GetTrackID_Safe(trackname, categoryid);
        if (!checkmaxvolume)
            Category[categoryid].Sounds[trackid].AudioSettings.Volume = volume;
        else
                if (volume >= Category[categoryid].Sounds[trackid].AudioSettings.MaxVolume)
            Category[categoryid].Sounds[trackid].AudioSettings.Volume = Category[categoryid].Sounds[trackid].AudioSettings.MaxVolume;
        else
            Category[categoryid].Sounds[trackid].AudioSettings.Volume = volume;
    }
    public void SetTrackVolume(int trackid, float volume, bool checkmaxvolume, int categoryid = 0)
    {
        if (!checkmaxvolume)
            Category[categoryid].Sounds[trackid].AudioSettings.Volume = volume;
        else if (volume >= Category[categoryid].Sounds[trackid].AudioSettings.MaxVolume)
            Category[categoryid].Sounds[trackid].AudioSettings.Volume = Category[categoryid].Sounds[trackid].AudioSettings.MaxVolume;
        else
            Category[categoryid].Sounds[trackid].AudioSettings.Volume = volume;
    }

    /// <summary>Returns track id.</summary>
    public int Get_Track_ID(string trackname, int categoryid = 0)
    {
        return AudioHandler_GetTrackID_Unsafe(trackname, categoryid);
    }

    /// <summary>Refresh settings.</summary>
    public void RefreshSettings_AllCategories()
    {
        for (int i = 0; i < Category.Count; i++)
        {
            for (int j = 0; j < Category[i].Sounds.Count; j++)
            {
                if (Category[i].Sounds[j].Settings.CreateAudioSource)
                {
                    if (Category[i].Sounds[j].Settings.AudioSource.clip != Category[i].Sounds[j].Settings.AudioClip)
                        Category[i].Sounds[j].Settings.AudioSource.clip = Category[i].Sounds[j].Settings.AudioClip;
                    //SetEffects
                    if (!Category[i].Sounds[j].AudioEffects.FadeIn || Category[i].Sounds[j].AudioEffects.FadeIn && Category[i].Sounds[j].AudioEffects.FadeInDone)
                        Category[i].Sounds[j].Settings.AudioSource.volume = Category[i].Sounds[j].AudioSettings.Volume;
                    Category[i].Sounds[j].Settings.AudioSource.loop = Category[i].Sounds[j].AudioSettings.Loop;
                }
            }
        }
    }

    /// <summary>Duplicate AudioTrack.</summary>
    public string DuplicateAudioTrack(string trackname, int categoryid = 0)
    {
        int audioid = Get_Track_ID(trackname, categoryid);
        if (audioid == -1)
            return null;

        AudioHandler_Sound newsound = new AudioHandler_Sound();
        GameObject newaudiopos = new GameObject();

        newsound.AudioTrackName = "Audio_" + Category[categoryid].Sounds[audioid].AudioTrackName;

        //Settings
        newsound.Settings = new AudioHandler_Settings();
        newsound.Settings.AudioClip = Category[categoryid].Sounds[audioid].Settings.AudioClip;
        newsound.Settings.AudioGroup = Category[categoryid].Sounds[audioid].Settings.AudioGroup;
        newsound.Settings.AudioSource = newaudiopos.AddComponent<AudioSource>();
        newsound.Settings.CreateAudioSource = Category[categoryid].Sounds[audioid].Settings.CreateAudioSource;

        //Control
        newsound.AudioControl = new AudioHandler_Control();
        newsound.AudioControl.SceneEnabled = Category[categoryid].Sounds[audioid].AudioControl.SceneEnabled;
        newsound.AudioControl.StartAudioOnScene = Category[categoryid].Sounds[audioid].AudioControl.StartAudioOnScene;
        newsound.AudioControl.StopAudioOnScene = Category[categoryid].Sounds[audioid].AudioControl.StopAudioOnScene;
        newsound.AudioControl.StopOnNextScene = Category[categoryid].Sounds[audioid].AudioControl.StopOnNextScene;

        //Audio3D
        newsound.Audio3D = new AudioHandler_3DAudio();
        newsound.Audio3D.Enable3DAudio = Category[categoryid].Sounds[audioid].Audio3D.Enable3DAudio;
        newsound.Audio3D.SpatialPosition = Category[categoryid].Sounds[audioid].Audio3D.SpatialPosition;
        newsound.Audio3D.SpatialTransform = Category[categoryid].Sounds[audioid].Audio3D.SpatialTransform;

        //AudioSettings
        newsound.AudioSettings = new AudioHandler_AudioSettings();
        newsound.AudioSettings.Loop = Category[categoryid].Sounds[audioid].AudioSettings.Loop;
        newsound.AudioSettings.MaxVolume = Category[categoryid].Sounds[audioid].AudioSettings.MaxVolume;
        newsound.AudioSettings.PlayOnStart = Category[categoryid].Sounds[audioid].AudioSettings.PlayOnStart;
        newsound.AudioSettings.Volume = Category[categoryid].Sounds[audioid].AudioSettings.Volume;

        //AudioEffect
        newsound.AudioEffects = new AudioHandler_Effects();
        newsound.AudioEffects.FadeIn = Category[categoryid].Sounds[audioid].AudioEffects.FadeIn;
        newsound.AudioEffects.FadeInDone = Category[categoryid].Sounds[audioid].AudioEffects.FadeInDone;
        newsound.AudioEffects.FadeInDuration = Category[categoryid].Sounds[audioid].AudioEffects.FadeInDuration;
        newsound.AudioEffects.FadeInSpeed = Category[categoryid].Sounds[audioid].AudioEffects.FadeInSpeed;
        newsound.AudioEffects.FadeOut = Category[categoryid].Sounds[audioid].AudioEffects.FadeOut;
        newsound.AudioEffects.FadeOutAfterTime = Category[categoryid].Sounds[audioid].AudioEffects.FadeOutAfterTime;
        newsound.AudioEffects.FadeOutDone = Category[categoryid].Sounds[audioid].AudioEffects.FadeOutDone;
        newsound.AudioEffects.FadeOutDuration = Category[categoryid].Sounds[audioid].AudioEffects.FadeOutDuration;
        newsound.AudioEffects.FadeOutSpeed = Category[categoryid].Sounds[audioid].AudioEffects.FadeOutSpeed;
        newsound.AudioEffects.FadingIn = Category[categoryid].Sounds[audioid].AudioEffects.FadingIn;
        newsound.AudioEffects.FadingOut = Category[categoryid].Sounds[audioid].AudioEffects.FadingOut;

        newsound.AudioTrackName += "_" + Category[categoryid].Sounds.Count.ToString();

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

        //Apply
        newaudiopos.transform.parent = this.transform;
        Category[categoryid].Sounds.Add(newsound);

        //Position
        if (newsound.Audio3D.SpatialTransform != null)
            ChangeAudioPosition(newsound.AudioTrackName, newsound.Audio3D.SpatialTransform.position, categoryid);
        else
            ChangeAudioPosition(newsound.AudioTrackName, newsound.Audio3D.SpatialPosition, categoryid);

        //PlayOnStart
        if (newsound.AudioSettings.PlayOnStart)
            newsound.Settings.AudioSource.Play();
        return newsound.AudioTrackName;
    }

    /// <summary>Change AudioSource Position.</summary>
    public void ChangeAudioPosition(string trackname, Vector3 newpos, int categoryid = 0)
    {
        int audioid = Get_Track_ID(trackname, categoryid);
        if (audioid != -1)
            Category[categoryid].Sounds[audioid].Settings.AudioSource.transform.position = newpos;
    }
    public void ChangeAudioPosition(int trackid, Vector3 newpos, int categoryid = 0)
    {
        Category[categoryid].Sounds[trackid].Settings.AudioSource.transform.position = newpos;
    }

    /// <summary>Set AudioSource Parent.</summary>
    public void ChangeAudioParent(string trackname, Transform newparent, int categoryid = 0)
    {
        int audioid = Get_Track_ID(trackname, categoryid);
        if (audioid != -1)
            Category[categoryid].Sounds[audioid].Settings.AudioSource.transform.parent = newparent;
    }
    public void ChangeAudioParent(int trackid, Transform newparent, int categoryid = 0)
    {
        Category[categoryid].Sounds[trackid].Settings.AudioSource.transform.parent = newparent;
    }

    /// <summary>Returns clip names
    public string GetTracksActive()
    {
        string searchtracksactive = "";
        for (int cat = 0; cat < Category.Count; cat++)
        {
            for (int i = 0; i < Category[cat].Sounds.Count; i++)
            {
                if (Category[cat].Sounds[i].Settings.CreateAudioSource)
                    if (Category[cat].Sounds[i].Settings.AudioSource.isPlaying)
                        searchtracksactive += Category[cat].Sounds[i].Settings.AudioClip.name + "\n";
            }
        }
        return searchtracksactive;
    }
    public int GetTracksActiveAmount()
    {
        int searchtracksactive = 0;
        for (int cat = 0; cat < Category.Count; cat++)
        {
            for (int i = 0; i < Category[cat].Sounds.Count; i++)
            {
                if (Category[cat].Sounds[i].Settings.CreateAudioSource)
                    if (Category[cat].Sounds[i].Settings.AudioSource.isPlaying)
                        searchtracksactive++;
            }
        }
        return searchtracksactive;
    }
}

[System.Serializable]
public class AudioHandler_Category
{
    public string CategoryName;
    public List<AudioHandler_Sound> Sounds;

    [Header("Performance")]
    public bool _CallOnly;
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
