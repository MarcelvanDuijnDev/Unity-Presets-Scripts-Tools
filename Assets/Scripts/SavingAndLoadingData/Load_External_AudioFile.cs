using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Load_External_AudioFile : MonoBehaviour
{
    private string _Path = "";

    [Header("Audio")]
    [SerializeField] private List<AudioClip> _AudioFiles = new List<AudioClip>();
    private string[] _Files_Audio_MP3;
    private string pathPreFix = @"file://";

    void Start()
    {
        _Files_Audio_MP3 = System.IO.Directory.GetFiles(_Path, "*.mp3");
        StartCoroutine(LoadAudio());
    }

    private IEnumerator LoadAudio()
    {
        //Load mp3
        foreach (string tstring in _Files_Audio_MP3)
        {
            string temppath = pathPreFix + tstring;

            WWW www = new WWW(temppath);
            yield return www;

            AudioClip audioclip = www.GetAudioClip(false, false);
            audioclip.LoadAudioData();
            audioclip.name = Path.GetFileNameWithoutExtension(temppath);

            _AudioFiles.Add(audioclip);
        }
    }
}
