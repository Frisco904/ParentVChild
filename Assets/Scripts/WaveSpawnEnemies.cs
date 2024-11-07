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
    [SerializeField] private float diffScalingFactor = 0.75f;
    [SerializeField] private float enemiesPerSecCap = 15f;
    [SerializeField] private SpawnPoints spawnPoint;

    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefab;
    [SerializeField] private TextMeshProUGUI waveUI;
    [SerializeField] private TextMeshProUGUI IndicateWaveUI;
    [SerializeField] private GameObject finalWaveTxt;

    //public static UnityEvent onEnemyDeath = new UnityEvent();

    private float timeBetweenWaves;
    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private float enemiesPerSecond; 
    private int enemiesLeftToSpawn;
    private bool isSpawning = false;
    private int enemiesAlive;
    private bool isLvlFinished = false;
    private bool isWaveFinished = false;
    private int index;
    private bool enemyHasSpawned = false;
    private WaveSpawnEnemies[] WaveSpawners;
    //private bool updatedNextWave = false;



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

        StartCoroutine(StartWave());
    }

    private void Update()
    {
        //Debug.Log("Spawn point : "+ spawnPoint+ " enemies left to spawn: " + enemiesLeftToSpawn);

        int wave = currentWave;

        if (currentWave == 1) { timeBetweenWaves = LevelManager.main.initialWaveDelay; } else { timeBetweenWaves = LevelManager.main.timeBetweenWaves; }


        if (isSpawning)
        {
            GetCurrentWaveTxt(wave);
            if(LevelManager.main.GetMaxWaves() != wave && !isWaveFinished)
            {
                ShowWave(wave);
            }
        }

        if (LevelManager.main.CandyPile)
        {

            if (!isSpawning) return;

            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= (1f / enemiesPerSecond) && enemiesLeftToSpawn > 0)
            {
                SpawnEnemy();
                enemiesLeftToSpawn--;
                timeSinceLastSpawn = 0f;
            }
            if (enemiesAlive == 0 && enemiesLeftToSpawn == 0 && LevelManager.main.GetMoveToNextWave())
            {
                //EndWave();
                //LevelManager.main.SetMoveToNextWave(false);
            }

        }
        //This is for the final wave text to spawn after it reaches the max wave
        if (LevelManager.main.GetMaxWaves() == wave && !isLvlFinished)
        {
            finalWaveTxt.SetActive(true);

            Invoke("DelayAction", 4f);

            isLvlFinished = true;
        }
    }

    void SpawnEnemy()
    {
        enemyHasSpawned = true;
        enemiesAlive++;
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

    private IEnumerator StartWave()
    {
        LevelManager.main.SetEnemiesLeft(EnemiesPerWave());
        LevelManager.main.SetMoveToNextWave(false);

        if (currentWave == 1) yield return new WaitForSeconds(timeBetweenWaves); else yield return new WaitForSeconds(timeBetweenWaves);

        isSpawning = true;
        isWaveFinished = false;
        enemiesLeftToSpawn += EnemiesPerWave();
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
            StartCoroutine(StartWave());
        }
        else if(currentWave > LevelManager.main.GetMaxWaves())
        {
            LevelManager.main.SetWinCondition(true);
        }
        else
        {

        }
    }
    private void ShowWave(int waveNum)
    {
        IndicateWaveUI.text = GetIndicateWave(waveNum);
        IndicateWaveUI.gameObject.SetActive(true);
        Invoke("WaveDisappearTxt", 3f);
        isWaveFinished = true;
    }

    private string GetCurrentWaveTxt(int wave)
    {
        return waveUI.text = "Wave: " + wave.ToString();
    }

    private string GetIndicateWave(int wavNum)
    {
        switch (wavNum)
        {
            case 1: return "First Wave";
            case 2: return "Second Wave";
            case 3: return "Third Wave";
            default: return wavNum + "th Wave";
        }

    }

    private void EnemyDeath()
    {
        enemiesAlive--;

    }
    public SpawnPoints GetSpawnPoint()
    {
        return spawnPoint;
    }

    //Do once
    void DelayAction()
    {
        finalWaveTxt.SetActive(false);
    }

    void WaveDisappearTxt()
    {
        IndicateWaveUI.gameObject.SetActive(false);
    }
    public int GetEnemiesAlive() { return enemiesAlive; }
    public bool GetIsSpawning() { return isSpawning; }
    public bool GetEnemyHasSpawned() { return enemyHasSpawned; }
    public float GetWaveDelayTime() { return timeBetweenWaves; }
    public int GetEnemiesLeftToSpawn() { return enemiesLeftToSpawn; }
    public void DecrementEnemiesAlive() { enemiesAlive--; }
}
