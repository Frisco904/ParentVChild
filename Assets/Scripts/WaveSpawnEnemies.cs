using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField] public float spawnFrequency;

    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefab;


    [Header("Events")]
    public static UnityEvent onEnemyDeath = new UnityEvent();

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private float enemiesPerSecond; 
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawning = false;
    private int index;
    private bool enemyHasSpawned = false;



    // Start is called before the first frame update

    private void Awake()
    {
        onEnemyDeath.AddListener(EnemyDeath);
    }

    private void Update()
    {
        if(currentWave == 1) { timeBetweenWaves = LevelManager.main.initialWaveDelayTime; } else { timeBetweenWaves = LevelManager.main.waveDelayTime; }

        //Debug.Log("Enemies alive: " +enemiesAlive);
        //Debug.Log("Enemies left to spawn: " + enemiesLeftToSpawn);
        if (LevelManager.main.CandyPile)
        {
            if (!isSpawning) return;

            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= (1f / enemiesPerSecond) && enemiesLeftToSpawn > 0)
            {
                SpawnEnemy();
                enemiesLeftToSpawn--;
                timeSinceLastSpawn = 0f;
                //Debug.Log("Spawn Enemies");
            }
            //Use this code something related to this will help to move on to the next stage.
            //enemyAlive == 0 && enemies left to spawn
            if (enemiesAlive == 0 && enemiesLeftToSpawn == 0 && LevelManager.main.GetMoveToNextWave())
            {
                EndWave();
                LevelManager.main.SetMoveToNextWave(false);
            }
        }
    }
    private void EnemyDeath()
    {
        enemiesAlive--;

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
        //InvokeRepeating("SpawnEnemy", 0.0f, spawnFrequency);

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
        if (currentWave == 1) yield return new WaitForSeconds(timeBetweenWaves); else yield return new WaitForSeconds(timeBetweenWaves);
       //yield return new WaitForSeconds(timeBetweenWaves);

        isSpawning = true;
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
            Debug.Log("Starting next wave");
            //StopAllCoroutines();
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

    public SpawnPoints GetSpawnPoint()
    {
        return spawnPoint;
    }
    public int GetCurrentWave() { return currentWave; }
    public int GetEnemiesLeftToSpawn() { return enemiesLeftToSpawn; }

    public int GetEnemiesAlive() { return enemiesAlive; }
    public bool GetIsSpawning() { return isSpawning; }

    public void DecrementEnemiesAlvie() {  enemiesAlive--; }

    public bool GetEnemyHasSpawned() {  return enemyHasSpawned; }

    public float GetWaveDelayTime() { return timeBetweenWaves; }

}
