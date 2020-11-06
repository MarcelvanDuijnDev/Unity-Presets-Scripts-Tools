using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class SaveLoad_XML : MonoBehaviour
{
    private XML_SaveData _SaveData = new XML_SaveData();

    void Start()
    {
        LoadData();
    }

    public void SaveData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(XML_SaveData));

        using (FileStream stream = new FileStream(_SaveData, FileMode.Create))
        {
            serializer.Serialize(stream, _SaveData);
        }
    }

    public void LoadData()
    {

        XmlSerializer serializer = new XmlSerializer(typeof(XML_SaveData));

        using (FileStream stream = new FileStream(fileName, FileMode.Open))
        {
            var test = serializer.Deserialize(stream);

            Debug.Log(test);
        }
    }
}

[System.Serializable]
public class XML_SaveData
{
    public List<Json_ExampleData> saveData = new List<Json_ExampleData>();
}
[System.Serializable]
public class XML_ExampleData
{
    public float exampleValue = 0;
}