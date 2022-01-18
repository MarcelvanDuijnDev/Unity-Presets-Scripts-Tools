using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Load_External_Image : MonoBehaviour
{
    private string _Path = "";

    [Header("JPG")]
    [SerializeField] private List<Texture2D> _Images = new List<Texture2D>();
    private string[] _Files_JPG;
    private string pathPreFix = @"file://";

    void Start()
    {
        _Files_JPG = System.IO.Directory.GetFiles(_Path, "*.jpg");
        StartCoroutine(LoadImages());
    }

    private IEnumerator LoadImages()
    {
        //Load JPG
        foreach (string tstring in _Files_JPG)
        {
            string pathTemp = pathPreFix + tstring;
            WWW www = new WWW(pathTemp);
            yield return www;
            Texture2D texTmp = new Texture2D(1024, 1024, TextureFormat.DXT1, false);
            www.LoadImageIntoTexture(texTmp);

            _Images.Add(texTmp);
        }
    }
}
