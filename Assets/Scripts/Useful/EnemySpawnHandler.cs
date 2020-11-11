using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnHandler : MonoBehaviour
{
    private enum Options {Endless, Level, Waves, Endless_Waves }

    [Header("Settings")]
    [SerializeField] private Options _Option = Options.Endless;



    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

[System.Serializable]
public class EnemySpawnHandler_Enemy
{

}