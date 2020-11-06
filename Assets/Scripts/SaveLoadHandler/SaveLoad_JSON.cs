using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoad_JSON : MonoBehaviour
{
    private Json_SaveData _SaveData = new Json_SaveData();

    void Start()
    {
        LoadData();
    }

    public void SaveData()
    {
        string jsonData = JsonUtility.ToJson(_SaveData, true);
        File.WriteAllText(Application.persistentDataPath + "/SaveData.json", jsonData);
    }
    public void LoadData()
    {
        try
        {
            string dataAsJson = File.ReadAllText(Application.persistentDataPath + "/SaveData.json");
            _SaveData = JsonUtility.FromJson<Json_SaveData>(dataAsJson);
        }
        catch
        {
            SaveData();
        }
    }
    public Json_SaveData GetSaveData()
    {
        return _SaveData;
    }
    public void CreateNewSave()
    {
        Json_ExampleData newsave = new Json_ExampleData();
        newsave.exampleValue = 10;
        _SaveData.saveData.Add(newsave);
    }
}

[System.Serializable]
public class Json_SaveData
{
    public List <Json_ExampleData> saveData = new List<Json_ExampleData>();
}
[System.Serializable]
public class Json_ExampleData
{
    public float exampleValue = 0;
}