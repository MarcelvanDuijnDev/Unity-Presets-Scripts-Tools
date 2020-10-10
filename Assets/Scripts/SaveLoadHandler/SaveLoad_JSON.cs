using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class SaveLoad_JSON : MonoBehaviour
{
    private SaveData _SaveData = new SaveData();

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
            _SaveData = JsonUtility.FromJson<SaveData>(dataAsJson);
        }
        catch
        {
            SaveData();
        }
    }
    public SaveData GetSaveData()
    {
        return _SaveData;
    }
    public void CreateNewSave()
    {
        ExampleData newsave = new ExampleData();
        newsave.randomfloat = Random.Range(0,100);
        _SaveData.saveData.Add(newsave);
    }
}

[System.Serializable]
public class SaveData
{
    public List <ExampleData> saveData = new List<ExampleData>();
}
[System.Serializable]
public class ExampleData
{
    public float randomfloat = 0;
}