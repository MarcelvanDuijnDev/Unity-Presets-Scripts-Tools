using UnityEngine;
using System.IO;

public class ReadWrite_TextFile : MonoBehaviour
{
    [SerializeField] private string _Path = "";
    [SerializeField] private string _FileName = "ExampleTextFile";

    [Header("Example")]
    [SerializeField] private string _Message = "Test Message";

    void Start()
    {
        if (_Path == "")
        {
            _Path = "Assets/" + _FileName;
        }

        WriteTextFile();
        ReadTextFile();
    }

    public void ReadTextFile()
    {
        StreamReader reader = new StreamReader(_Path + ".txt");
        Debug.Log("Read Result: " + reader.ReadToEnd());
        reader.Close();
    }

    public void WriteTextFile()
    {
        StreamWriter writer = new StreamWriter(_Path + ".txt", true);
        writer.WriteLine(_Message);
        writer.Close();
        Debug.Log("Write Complete");
    }
}
