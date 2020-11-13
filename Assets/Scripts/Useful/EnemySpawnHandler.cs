using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnHandler : MonoBehaviour
{
    private enum Options {Endless, Waves, Endless_Waves }

    [Header("Settings")]
    [SerializeField] private Options _Option = Options.Endless;

    [Header("Object Pool")]
    [SerializeField] private ObjectPool _ObjectPool;

    [Header("Enemies")]
    [SerializeField] private EnemySpawnHandler_Enemy[] _Enemies;

    [Header("SpawnLocations")]
    [SerializeField] private Transform[] _SpawnLocations;

    [Header("Settings - Endless")]
    [SerializeField] private float _SpawnRate; // Seconds between spawning
    [SerializeField] private float _SpawnRateEncrease; // Decrease time between spawning per sec
    [SerializeField] private bool _RandomEnemy;
    [SerializeField] private bool _RandomSpawn;

    private float _Timer = 0;

    void Update()
    {

        _Timer += 1 * Time.deltaTime;

        switch(_Option)
        {
            case Options.Endless:
                Endless();
                break;
            case Options.Waves:

                break;
            case Options.Endless_Waves:

                break;
        }
    }

    private void Endless()
    {
        if (_Timer >= _SpawnRate)
        {
            int randomenemyid = 0;
            int randomspawnid = 0;
            if (_RandomEnemy)
                 randomenemyid = Random.Range(0, _Enemies.Length);
            if (_RandomSpawn)
                randomspawnid = Random.Range(0, _SpawnLocations.Length);
            Spawn(randomenemyid, randomspawnid);
            _Timer = 0;
        }
    }


    public void Spawn(int enemyid, int spawnid)
    {
        GameObject obj = _ObjectPool.GetObjectPrefabName(_Enemies[enemyid].EnemyPrefab.name);
        obj.transform.position = _SpawnLocations[spawnid].position;
    }
}

[System.Serializable]
public class EnemySpawnHandler_Enemy
{
    public string EnemyName;
    public GameObject EnemyPrefab;

    [Header("Settings")]
    public int StartWave;
}