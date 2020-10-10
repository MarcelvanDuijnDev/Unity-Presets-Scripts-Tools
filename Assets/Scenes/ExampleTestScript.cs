using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExampleTestScript : MonoBehaviour
{
    private enum ExampleOptions {ObjectPool, SaveLoad_JSON };
    [SerializeField] private ExampleOptions _ChosenOption;

    [Header("ObjectPool")] [SerializeField] private ObjectPool _ObjectPool;
    [Header("LoadSave_JSON")] [SerializeField] private SaveLoad_JSON _SaveLoadJSON;
    [SerializeField] private SaveData _SaveDataExample = new SaveData();
    [Header("Showinfo")]
    [SerializeField] private TextMeshProUGUI _InfoText;
    [SerializeField] private TextMeshProUGUI _InfoText2;
    void Start()
    {
        
    }

    void Update()
    {
        switch(_ChosenOption)
        {
            case ExampleOptions.ObjectPool:
                _InfoText.text = "Press spacebar to spawn a object from the objectpool";
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    GameObject spawnedObject = _ObjectPool.GetObject("Object1");
                    spawnedObject.transform.position = new Vector3(0, 0, 0);
                }
                break;
            case ExampleOptions.SaveLoad_JSON:
                _InfoText.text = "Press S to save / L to load / N to create a new save";
                if (Input.GetKeyDown(KeyCode.S))
                {
                    _SaveLoadJSON.SaveData();
                }
                if(Input.GetKeyDown(KeyCode.L))
                {
                    _SaveLoadJSON.LoadData();
                    _SaveDataExample = _SaveLoadJSON.GetSaveData();
                    _InfoText2.text = "Savedata Loaded " + System.DateTime.Now + "\n";
                    for (int i = 0; i < _SaveDataExample.saveData.Count; i++)
                    {
                        _InfoText2.text += _SaveDataExample.saveData[i].randomfloat.ToString() + " \n";
                    }
                }
                if(Input.GetKeyDown(KeyCode.N))
                {
                    _SaveLoadJSON.CreateNewSave();
                }
                break;
        }
    }
}
