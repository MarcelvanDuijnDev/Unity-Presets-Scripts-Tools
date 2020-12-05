using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsHandler : MonoBehaviour
{

    private TMP_Dropdown _ResolutionDropdown;
    private Resolution[] _Resolutions;

    // [Display]
    //Resolution

    private void Start()
    {
        _Resolutions = Screen.resolutions;
        _ResolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentresid = 0;
        for (int i = 0; i < _Resolutions.Length; i++)
        {
            string option = _Resolutions[i].width + " x " + _Resolutions[i].height;
            options.Add(option);

            if (_Resolutions[i].width == Screen.currentResolution.width && _Resolutions[i].height == Screen.currentResolution.height)
                currentresid = i;
        }

        _ResolutionDropdown.AddOptions(options);
        _ResolutionDropdown.value = currentresid;
        _ResolutionDropdown.RefreshShownValue();
    }

    void Set_Resolution(int resid)
    {
        Resolution resolution = _Resolutions[resid];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    //FullScreen
    //Quiality
    //Vsync
    //MaxFOS
    //Gama

    // [Grapics]
    //Antialiasing
    //Shadows
    //ViewDistance
    //TextureQuality
    //ViolageDistance
    //ViolageDensity

    // [Gameplay]
    //SoundEffects
    //Music
}
