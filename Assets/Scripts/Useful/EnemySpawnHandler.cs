using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnHandler : MonoBehaviour
{
    private enum Options {Endless, Waves}

    [Header("Settings")]
    [SerializeField] private Options _Option = Options.Endless;
    [SerializeField] private int _Seed = 0;
    [SerializeField] private bool _SetRandomSeed = true;

    [Header("Object Pool")]
    [SerializeField] private ObjectPool _ObjectPool = null;

    [Header("Enemies")]
    [SerializeField] private EnemySpawnHandler_Enemy[] _Enemies = null;

    [Header("SpawnLocations")]
    [SerializeField] private Transform[] _SpawnLocations = null;

    [Header("Settings - Endless")]
    [SerializeField] private float _SpawnRate = 5; // Seconds between spawning
    [SerializeField] private float _SpawnRateEncrease = 0.05f; // Decrease time between spawning per sec
    [SerializeField] private bool _RandomEnemy = true;
    [SerializeField] private bool _RandomSpawn = true;

    [Header("Settings - Waves")]
    [SerializeField] private EnemySpawnHandler_WaveSettings _Waves = null;

    private float _Timer = 0;
    private int _CurrentWave = 0;
    private int _CheckWave = 999;
    private float _TimerBetweenWaves = 0;
    private float _SpawnSpeed = 0;

    private void Start()
    {
        if (_SetRandomSeed)
            Random.InitState(Random.Range(0, 10000));
        else
            Random.InitState(_Seed);

        if (_Waves.WaveOption == EnemySpawnHandler_WaveSettings.WaveOptions.Generate)
            GenerateWaves();
        if (_Waves.WaveOption == EnemySpawnHandler_WaveSettings.WaveOptions.Endless)
        {
            _Waves.WaveAmount = 1;
            GenerateWaves();
            GenerateWaves();
        }
    }

    void Update()
    {
        _Timer += 1 * Time.deltaTime;

        switch (_Option)
        {
            case Options.Endless:
                Update_Endless();
                break;
            case Options.Waves:
                Update_Waves();
                break;
        }
    }

    //Update
    private void Update_Endless()
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
        _SpawnRate -= _SpawnRateEncrease * Time.deltaTime;
    }
    private void Update_Waves()
    {
        if (_CurrentWave < _Waves.WaveAmount)
        {
            if (_CheckWave != _CurrentWave)
            {
                //Get info / time between
                _TimerBetweenWaves += 1 * Time.deltaTime;
                if (_TimerBetweenWaves >= _Waves.TimeBetweenWaves)
                {
                    _TimerBetweenWaves = 0;
                    _CheckWave = _CurrentWave;
                    _SpawnSpeed = _Waves.Waves[_CurrentWave].SpawnDuration / _Waves.Waves[_CurrentWave].TotalEnemies;
                    if (_Waves.WaveOption == EnemySpawnHandler_WaveSettings.WaveOptions.Endless)
                        GenerateWaves();
                }
            }
            else
            {
                //Spawn
                if (_Waves.Waves[_CurrentWave].TotalEnemies > 0)
                {
                    if (_Timer > _SpawnSpeed)
                    {
                        bool spawncheck = false;
                        while (!spawncheck)
                        {
                            int spawnid = Random.Range(0, _Enemies.Length);
                            if (_Waves.Waves[_CurrentWave].EnemyID[spawnid] > 0)
                            {
                                Spawn(spawnid, Random.Range(0, _SpawnLocations.Length));
                                _Waves.Waves[_CheckWave].EnemyID[spawnid]--;
                                _Waves.Waves[_CurrentWave].TotalEnemies--;
                                spawncheck = true;
                            }
                        }
                        _Timer = 0;
                    }
                }
                else
                {
                    _CurrentWave++;
                }
            }
        }
    }

    //Generate Waves
    private void GenerateWaves()
    {
        int enemytypes = _Enemies.Length;
        for (int i = 0; i < _Waves.WaveAmount; i++)
        {
            EnemySpawnHandler_Wave newwave = new EnemySpawnHandler_Wave();
            int enemyamount = Mathf.RoundToInt(_Waves.EnemyAmount * ((_Waves.EnemyIncreaseAmount * i) + 1));

            //Set enemy amount
            newwave.EnemyID = new int[enemytypes];
            int checkenemyamount = 0;
            newwave.TotalEnemies = enemyamount;

            while (checkenemyamount < enemyamount)
            {
                for (int j = 0; j < enemytypes; j++)
                {
                    if (_Enemies[j].StartWave <= i)
                    {
                        int addamount = 0;
                        if (enemyamount < 2)
                            addamount = Random.Range(0, enemyamount);
                        else
                            addamount = Random.Range(0, Mathf.RoundToInt(enemyamount*0.5f));

                        if (enemyamount > checkenemyamount + addamount)
                        {
                            newwave.EnemyID[j] += addamount;
                            checkenemyamount += addamount;
                        }
                        else
                        {
                            newwave.EnemyID[j] += enemyamount - checkenemyamount;
                            checkenemyamount = enemyamount;
                            continue;
                        }
                    }
                }
            }
            _Waves.Waves.Add(newwave);
        }
    }

    public void Spawn(int enemyid, int spawnid)
    {
        GameObject obj = _ObjectPool.GetObjectPrefabName(_Enemies[enemyid].EnemyPrefab.name, false);
        obj.transform.position = _SpawnLocations[spawnid].position;
        obj.SetActive(true);
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

[System.Serializable]
public class EnemySpawnHandler_WaveSettings
{
    public enum WaveOptions {Endless, Manually, Generate}
    public WaveOptions WaveOption;

    [Header("Endless")]
    public float EnemyIncreaseAmount;

    [Header("Manual")]
    public List<EnemySpawnHandler_Wave> Waves;

    [Header("Generate")]
    public int WaveAmount;
    public int EnemyAmount;

    [Header("Other")]
    public float TimeBetweenWaves;
}

[System.Serializable]
public class EnemySpawnHandler_Wave
{
    public int[] EnemyID;
    public float SpawnDuration = 5;

    [HideInInspector] public int TotalEnemies;
}