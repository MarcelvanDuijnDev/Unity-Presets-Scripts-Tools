using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;

public class SettingsHandler : MonoBehaviour
{
    [SerializeField] private AudioMixer _AudioMixer = null;
    [SerializeField] private float _Current_Volume = 1;


    [SerializeField] private TMP_Dropdown _Dropdown_Resolution = null;
    [SerializeField] private TMP_Dropdown _Dropdown_Quality = null;
    [SerializeField] private TMP_Dropdown _Dropdown_Texture = null;
    [SerializeField] private TMP_Dropdown _Dropdown_AA = null;
    [SerializeField] private Slider _Slider_Volume = null;

    [SerializeField] private Resolution[] _Resolutions = null;

    private void Start()
    {
        _Resolutions = Screen.resolutions;
        _Dropdown_Resolution.ClearOptions();
        List<string> options = new List<string>();

        int currentresid = 0;
        for (int i = 0; i < _Resolutions.Length; i++)
        {
            string option = _Resolutions[i].width + " x " + _Resolutions[i].height;
            options.Add(option);

            if (_Resolutions[i].width == Screen.currentResolution.width && _Resolutions[i].height == Screen.currentResolution.height)
                currentresid = i;
        }

        _Dropdown_Resolution.AddOptions(options);
        _Dropdown_Resolution.value = currentresid;
        _Dropdown_Resolution.RefreshShownValue();
    }

    // [Display]
    //Resolution
    public void Set_Resolution(int resid)
    {
        Resolution resolution = _Resolutions[resid];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    //FullScreen
    public void Set_FullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    //Quiality
    public void Set_Quality(int qualityid)
    {
        if (qualityid != 6) // if the user is not using 
                               //any of the presets
            QualitySettings.SetQualityLevel(qualityid);
        switch (qualityid)
        {
            case 0: // quality level - very low
                _Dropdown_Texture.value = 3;
                _Dropdown_AA.value = 0;
                break;
            case 1: // quality level - low
                _Dropdown_Texture.value = 2;
                _Dropdown_AA.value = 0;
                break;
            case 2: // quality level - medium
                _Dropdown_Texture.value = 1;
                _Dropdown_AA.value = 0;
                break;
            case 3: // quality level - high
                _Dropdown_Texture.value = 0;
                _Dropdown_AA.value = 0;
                break;
            case 4: // quality level - very high
                _Dropdown_Texture.value = 0;
                _Dropdown_AA.value = 1;
                break;
            case 5: // quality level - ultra
                _Dropdown_Texture.value = 0;
                _Dropdown_AA.value = 2;
                break;
        }

        _Dropdown_Quality.value = qualityid;
    }

    //Vsync
    //MaxFOS
    //Gama

    // [Grapics]
    //Antialiasing
    public void SetAntiAliasing(int aaid)
    {
        QualitySettings.antiAliasing = aaid;
        _Dropdown_Quality.value = 6;
    }

    //Shadows
    //ViewDistance
    //TextureQuality
    public void Set_TextureQuality(int textureid)
    {
        QualitySettings.masterTextureLimit = textureid;
        _Dropdown_Quality.value = 6;
    }

    //ViolageDistance
    //ViolageDensity

    // [Gameplay]
    //SoundAll
    public void Set_Volume(float volume)
    {
        _AudioMixer.SetFloat("Volume", volume);
        _Current_Volume = volume;
    }

    //SoundEffects
    //Music

    // Quit / Save / Load
    public void ExitGame()
    {
        Application.Quit();
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySettingPreference",
                   _Dropdown_Quality.value);
        PlayerPrefs.SetInt("ResolutionPreference",
                   _Dropdown_Resolution.value);
        PlayerPrefs.SetInt("TextureQualityPreference",
                   _Dropdown_Texture.value);
        PlayerPrefs.SetInt("AntiAliasingPreference",
                   _Dropdown_AA.value);
        PlayerPrefs.SetInt("FullscreenPreference",
                   Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("VolumePreference",
                   _Current_Volume);
    }
    public void LoadSettings(int currentResolutionIndex)
    {
        if (PlayerPrefs.HasKey("QualitySettingPreference"))
            _Dropdown_Quality.value =
                         PlayerPrefs.GetInt("QualitySettingPreference");
        else
            _Dropdown_Quality.value = 3;
        if (PlayerPrefs.HasKey("ResolutionPreference"))
            _Dropdown_Resolution.value =
                         PlayerPrefs.GetInt("ResolutionPreference");
        else
            _Dropdown_Resolution.value = currentResolutionIndex;
        if (PlayerPrefs.HasKey("TextureQualityPreference"))
            _Dropdown_Texture.value =
                         PlayerPrefs.GetInt("TextureQualityPreference");
        else
            _Dropdown_Texture.value = 0;
        if (PlayerPrefs.HasKey("AntiAliasingPreference"))
            _Dropdown_AA.value =
                         PlayerPrefs.GetInt("AntiAliasingPreference");
        else
            _Dropdown_AA.value = 1;
        if (PlayerPrefs.HasKey("FullscreenPreference"))
            Screen.fullScreen =
            Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        else
            Screen.fullScreen = true;
        if (PlayerPrefs.HasKey("VolumePreference"))
            _Slider_Volume.value =
                        PlayerPrefs.GetFloat("VolumePreference");
        else
            _Slider_Volume.value =
                        PlayerPrefs.GetFloat("VolumePreference");
    }


    //Set
    public void SetDropDown_Resolution(TMP_Dropdown resolutions)
    {
        _Dropdown_Resolution = resolutions;
    }
    public void SetDropDown_Quality(TMP_Dropdown quality)
    {
        _Dropdown_Quality = quality;
    }
    public void SetDropDown_TextureQuality(TMP_Dropdown texturequality)
    {
        _Dropdown_Texture = texturequality;
    }
    public void SetDropDown_AA(TMP_Dropdown aa)
    {
        _Dropdown_AA = aa;
    }
    public void SetSlider_VolumeSlider(Slider volumeslider)
    {
        _Slider_Volume = volumeslider;
    }
}
