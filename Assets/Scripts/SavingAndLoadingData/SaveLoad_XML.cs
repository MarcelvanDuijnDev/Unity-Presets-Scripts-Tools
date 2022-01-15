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

        using (FileStream stream = new FileStream(Application.persistentDataPath + "/SaveData.xml", FileMode.Create))
        {
            serializer.Serialize(stream, _SaveData);
        }
    }

    public void LoadData()
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XML_SaveData));

            using (FileStream stream = new FileStream(Application.persistentDataPath + "/SaveData.xml", FileMode.Open))
            {
                _SaveData = serializer.Deserialize(stream) as XML_SaveData;
            }
        }
        catch
        {
            SaveData();
        }
    }

    public XML_SaveData GetSaveData()
    {
        return _SaveData;
    }
    public void CreateNewSave()
    {
        XML_ExampleData newsave = new XML_ExampleData();
        newsave.exampleValue = 10;
        _SaveData.saveData.Add(newsave);
    }
}

[System.Serializable]
public class XML_SaveData
{
    public List<XML_ExampleData> saveData = new List<XML_ExampleData>();
}
[System.Serializable]
public class XML_ExampleData
{
    public float exampleValue = 0;
}