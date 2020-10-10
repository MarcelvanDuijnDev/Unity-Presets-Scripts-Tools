using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleTestScript : MonoBehaviour
{
    private enum ExampleOptions {ObjectPool };
    [SerializeField] private ExampleOptions _ChosenOption;

    [Header("ObjectPool")]
    [SerializeField] private ObjectPool _ObjectPool;
     
    void Start()
    {
        
    }

    void Update()
    {
        switch(_ChosenOption)
        {
            case ExampleOptions.ObjectPool:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    GameObject spawnedObject = _ObjectPool.GetObject("Object1");
                    spawnedObject.transform.position = new Vector3(0, 0, 0);
                }
                break;

        }
    }
}
