using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [SerializeField] public Transform[] startPoints;
    [SerializeField] public List<Transform> path1;
    [SerializeField] public List<Transform> path2;
    [SerializeField] public List<Transform> path3;

    private int enemiesLeft = 0;
    private bool WinConditionMet = false;
    private int score = 0;
    private WaveSpawnEnemies[] WaveSpawners;
    private bool bMoveToNextWaveFlag = false;
    private float startTime;
    public float waveDelayTime;

    [Header("Enemy Wave Attributes")]
    [SerializeField] private int MaxWaves = 3;
    [SerializeField] public float initialWaveDelay = 5f;
    [SerializeField] public float timeBetweenWaves = 5f;

    [Header("References")]
    [SerializeField] public GameObject CandyPile;
    public PauseMenu MenuObj;

    [Header("Currency System")]
    [SerializeField] private int currency = 100;

    [Header("Wwise")]
    [SerializeField] public AK.Wwise.Event LevelMusicStart;
    [SerializeField] public AK.Wwise.State Playing;


    private void Awake()
    {
        main = this;
        //Adding the Candy Pile transform to the end of the list for the AI pathing.
        path1.Add(CandyPile.transform);
        path2.Add(CandyPile.transform);
        path3.Add(CandyPile.transform);

    }

    private void Start()
    {
        waveDelayTime = FindObjectOfType<WaveSpawnEnemies>().GetWaveDelayTime();
        // Let Wwise know what level we are on.
        AkSoundEngine.SetSwitch("Level", "Level" + SceneManager.GetActiveScene().buildIndex, gameObject);
        Playing.SetValue();
        // Wwise call to start music.
        LevelMusicStart.Post(gameObject);
    }
    private void Update()
    {
        //Debug.Log("Bool for reset flag" + bMoveToNextWaveFlag);

        WaveSpawners = FindObjectsOfType<WaveSpawnEnemies>();
        List<bool> allSpawnsFinished = new List<bool>();


        foreach (WaveSpawnEnemies spawner in WaveSpawners)
        {
            //Logic will be bypased if spawner is actively spawning
            //Debug.Log(spawner.GetIsSpawning());
            //if(spawner.GetIsSpawning()) { return; }

            if (spawner.GetIsSpawning())
            {
                if (Time.time >= waveDelayTime + startTime + 3f)
                {
                    //Debug.Log("Checking for the end wave condition now");
                    if (spawner.GetEnemiesAlive() == 0 && spawner.GetEnemiesLeftToSpawn() == 0)
                    {
                        Debug.Log("Spawner is finished");
                        allSpawnsFinished.Add(true);
                    }
                }
            }
        }
        //Debug.Log(allSpawnsFinished.Count);

        //At this logic check all spawners would be done witht heir waves.
        if (allSpawnsFinished.Count == WaveSpawners.Length)
        {

            foreach (WaveSpawnEnemies spawner in WaveSpawners)
            {
                //bMoveToNextWaveFlag = true;
                //Debug.Log("For spawner: " + spawner.name + "Called end wave funciton");
                spawner.EndWave();
                if (spawner == WaveSpawners[WaveSpawners.Length - 1])
                {
                    Debug.Log("This should occure once.");
                    allSpawnsFinished.Clear();
                }
            }
        }


        if (WinConditionMet && enemiesLeft == 0)
        {
            Debug.Log("Victory should be dispalyed");
            MenuObj.Invoke("Victory", 5);
        }
        if (CandyPile.IsDestroyed())
        {
            MenuObj.Defeat();
        }

    }


    public void GainMoney(int cash)
    {
        //Called in the EnemyCtrl to gain Money
        currency += cash;
    }

    //This is where the UI comes in
    public bool SpendMoney(int cash)
    {
        if(cash <= currency)
        {
            //Buy Torrent
            currency -= cash;
            return true;
        } else
        {
            Debug.Log("Not enough cash");
            return false;
        }
    }

    //Getters and Setters
    public int GetCurrency() {return currency; }
    public int GetEnemiesLeft()
    {
        return enemiesLeft;
    }
    public void SetEnemiesLeft(int value) { enemiesLeft += value; }
    public void DecrementEnemiesLeft() { enemiesLeft--; }
    public int GetMaxWaves() { return MaxWaves; }
    public void SetWinCondition(bool bSetWinCondition) { WinConditionMet = bSetWinCondition; }
    public void AddScore(int value) { score += value; }
    public int GetScore() { return score; }
    public float GetInitialWaveDelay() { return initialWaveDelay; }
    public bool GetMoveToNextWave() { return bMoveToNextWaveFlag; }
    public void SetMoveToNextWave(bool value) { bMoveToNextWaveFlag = value; }
}
