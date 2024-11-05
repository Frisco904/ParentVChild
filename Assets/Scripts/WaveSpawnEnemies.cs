using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TMPro;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public enum SpawnPoints
{
    SpawnPoint1 = 0,
    SpawnPoint2 = 1,
    SpawnPoint3= 2,
    SpawnPoint4 = 3,
}

public class WaveSpawnEnemies : MonoBehaviour
{


    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemyPerSecond = 0.5f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float diffScalingFactor = 0.75f;
    [SerializeField] private float enemiesPerSecCap = 15f;
    [SerializeField] private SpawnPoints spawnPoint;

    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefab;
    [SerializeField] public static WaveSpawnEnemies main;
    [SerializeField] private TextMeshProUGUI waveUI;

    [Header("Events")]
    public static UnityEvent onEnemyDeath = new UnityEvent();

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private float enemiesPerSecond; 
    private int enemiesLeftToSpawn;
    private bool isSpawning = false;
    private int index;



    // Start is called before the first frame update

    private void Awake()
    {
        //onEnemyDeath.AddListener(EnemyDeath);
        
    }

    void Start()
    {
        switch (spawnPoint)
        {
            case SpawnPoints.SpawnPoint1:
                index = 0;
                break;
            case SpawnPoints.SpawnPoint2:
                index = 1;
                break;
            case SpawnPoints.SpawnPoint3:
                index = 2;
                break;
            case SpawnPoints.SpawnPoint4:
                index = 3;
                break;

        }

        StartCoroutine(SpawnWave());
    }

    private void Update()
    {
        //Debug.Log("Spawn point : "+ spawnPoint+ " enemies left to spawn: " + enemiesLeftToSpawn);

        int wave = currentWave;
        
        if(isSpawning)
        {
            GetCurrentWaveTxt(wave);
        }

        if (LevelManager.main.CandyPile)
        {

            if (LevelManager.main.GetEnemiesLeft() == 0)
            {
                //EndWave();
               
            }

            if (!isSpawning) return;

            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= (1f / enemiesPerSecond) && enemiesLeftToSpawn > 0)
            {
                SpawnEnemy();
                enemiesLeftToSpawn--;
                timeSinceLastSpawn = 0f;
            }

        }
    }

    void SpawnEnemy()
    {
        //enemiesAlive++;

        int prefabIndex = Random.Range(0, enemyPrefab.Length);
        GameObject prefabToSPawn = enemyPrefab[prefabIndex];
        Instantiate(prefabToSPawn, LevelManager.main.startPoints[index]);

    }

    public int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, diffScalingFactor));
    }

    private float EnemiesPerSeconds()
    {
        return Mathf.Clamp((enemyPerSecond * Mathf.Pow(currentWave, diffScalingFactor)), 0f, enemiesPerSecCap);
    }

    private IEnumerator SpawnWave()
    {
        LevelManager.main.SetEnemiesLeft(EnemiesPerWave());
        //GameObject main = GameObject.FindGameObjectWithTag("Prime");
        if (currentWave == 1) yield return new WaitForSeconds(LevelManager.main.GetInitialWaveDelay()); else yield return new WaitForSeconds(timeBetweenWaves);
        //yield return new WaitForSeconds(timeBetweenWaves);


        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave();
        enemiesPerSecond = EnemiesPerSeconds();
    }

    public void EndWave()
    {

        isSpawning = false;
        timeSinceLastSpawn = 0f;
        currentWave++;
        //Debug.Log("End Wave has started, values for enemies left are :" + LevelManager.main.GetEnemiesLeft() +" and get max waves: " +LevelManager.main.GetMaxWaves());
        if (currentWave <= LevelManager.main.GetMaxWaves())
        {
            //Debug.Log("Starting next wave");
            StartCoroutine(SpawnWave());
        }
        else if(currentWave > LevelManager.main.GetMaxWaves())
        {
            LevelManager.main.SetWinCondition(true);
        }
        else
        {

        }
    }

    private string GetCurrentWaveTxt(int wave)
    {
        return waveUI.text = "Wave: " + wave.ToString() + " out of " + LevelManager.main.GetMaxWaves();
    }

    public SpawnPoints GetSpawnPoint()
    {
        return spawnPoint;
    }
}
