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
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public enum SpawnPoints
{
    SpawnPoint1 = 0,
    SpawnPoint2 = 1,
    SpawnPoint3 = 2,
    SpawnPoint4 = 3,
}

public class WaveSpawnEnemies : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemyPerSecond = 0.5f;
    [SerializeField] private float spawnVariance = .2f;
    [SerializeField] private float diffScalingFactor = 0.75f;
    [SerializeField] private float enemiesPerSecCap = 15f;
    [SerializeField] private SpawnPoints spawnPoint;

    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefab;
    [SerializeField] private TextMeshProUGUI waveUI;
    [SerializeField] private TextMeshProUGUI IndicateWaveUI;
    [SerializeField] private GameObject finalWaveTxt;

    [Header("Wwise")]
    [SerializeField] private AK.Wwise.Event ZombieSFX;

    private float timeBetweenWaves = 5f;
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

    private void Awake()
    {


    }
    private void LateUpdate()
    {
        //Initializing time between wave values which are set in the Level Manager game object.
        timeBetweenWaves = LevelManager.main.GetInitialWaveDelay();
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

        int wave = currentWave;

        if (isSpawning)
        {
            GetCurrentWaveTxt(wave);
            if (LevelManager.main.GetMaxWaves() != wave && !isWaveFinished)
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
                ZombieSFX.Post(gameObject);
                enemiesLeftToSpawn--;
                timeSinceLastSpawn = 0f;
            }

        }

        //This is for the final wave text to spawn after it reaches the max wave
        if (LevelManager.main.GetMaxWaves() == wave && !isLvlFinished)
        {
            finalWaveTxt.SetActive(true);

            Invoke("ChangeTxt", 3f);
            Invoke("DelayAction", 6f);

            isLvlFinished = true;
        }
    }

    void SpawnEnemy()
    {
        enemyHasSpawned = true;
        enemiesAlive++;
        //This is to determine of what type of enemies to spawn
        int prefabIndex;

        if (currentWave == 1)
        {
            prefabIndex = 0;
        }
        else if (currentWave == 2)
        {
            prefabIndex = 1;
        }
        else if (currentWave == LevelManager.main.GetMaxWaves())
        {
            prefabIndex = Random.Range(0, enemyPrefab.Length);
        }
        else
        {
            prefabIndex = Random.Range(0, enemyPrefab.Length);
        }

        GameObject prefabToSpawn = enemyPrefab[prefabIndex];
        Instantiate(prefabToSpawn, LevelManager.main.startPoints[index]);
    }

    public int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, diffScalingFactor));
    }

    private float EnemiesPerSeconds()
    {
        return Mathf.Clamp((Random.Range(enemyPerSecond - spawnVariance, enemiesPerSecond + spawnVariance) * Mathf.Pow(currentWave, diffScalingFactor)), 0f, enemiesPerSecCap);
    }

    private IEnumerator StartWave()
    {
        //Setting value of total enimies alive in the Level manager, used for tracking the total number of enemies alive.
        LevelManager.main.SetEnemiesLeft(EnemiesPerWave());

        //Checking if its the first wave, to apply the initial wave delay, else we apply the time between waves.
        if (currentWave == 1) { timeBetweenWaves = LevelManager.main.GetInitialWaveDelay(); } else { timeBetweenWaves = LevelManager.main.GetTimeBetweenWavesDelay(); }
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


        if (currentWave <= LevelManager.main.GetMaxWaves())
        {
            StartCoroutine(StartWave());
        }
        else if (currentWave > LevelManager.main.GetMaxWaves())
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
        Invoke("ChangeTxt", 2f);
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

    void ChangeTxt()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            IndicateWaveUI.text = "Incoming small kids";
            Invoke("WaveDisappearTxt", 3f);
        }
        else if (currentWave == 1 && SceneManager.GetActiveScene().name != "Tutorial")
        {
            IndicateWaveUI.text = "Meh just small kids.";
            Invoke("WaveDisappearTxt", 3f);
        }
        else if (currentWave == 2)
        {
            IndicateWaveUI.text = "Big kids incoming!!!";
            Invoke("WaveDisappearTxt", 3f);
        }
        else if (currentWave != LevelManager.main.GetMaxWaves())
        {
            IndicateWaveUI.text = "Here they come!!!";
            Invoke("WaveDisappearTxt", 3f);
        }
        else if (currentWave == LevelManager.main.GetMaxWaves() && LevelManager.main.GetEnemiesLeft() > 20)
        {
            IndicateWaveUI.text = "HORDE OF KIDS ARE COMING!!!";
            Invoke("WaveDisappearTxt", 3f);
        }
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
