using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    //Static means it can be called from anywhere.
    public static LevelManager main;
    [SerializeField] public Transform[] startPoints;
    [SerializeField] public List<Transform> path1;
    [SerializeField] public List<Transform> path2;
    [SerializeField] public List<Transform> path3;
    [SerializeField] public GameObject CandyPile;
    [SerializeField] private int currency = 100;
    [SerializeField] private int MaxWaves = 3;
    private int enemiesLeft = 0;
    private bool WinConditionMet = false;
    private int score = 0;
    private WaveSpawnEnemies[] WaveSpawners;
    public PauseMenu MenuObj;
    private bool bMoveToNextWaveFlag = false;
    public float waveDelayTime = 5f;
    public float initialWaveDelayTime = 5f;
    private float startTime;

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
        startTime = Time.time;
    }
    private void Update()
    {
        //Debug.Log(enemiesLeft);

        WaveSpawners = FindObjectsOfType<WaveSpawnEnemies>();
        List<bool> allSpawnsFinished = new List<bool>();


        foreach (WaveSpawnEnemies spawner in WaveSpawners)
        {
            //if(spawner.GetIsSpawning()) { return; }

            if (Time.time >= waveDelayTime + startTime + 3f)
            {
                Debug.Log("Checking for the end wave condition now");
                if (spawner.GetEnemiesAlive() == 0 && spawner.GetEnemiesLeftToSpawn() == 0)
                {
                    allSpawnsFinished.Add(true);
                }
            }
        }

        if (allSpawnsFinished.Count == WaveSpawners.Length)
        {

            foreach (WaveSpawnEnemies spawner in WaveSpawners) 
            {
                Debug.Log("The wave is finished.");
                bMoveToNextWaveFlag = true;

            }
        }


        if (WinConditionMet && enemiesLeft == 0)
        {
            MenuObj.Invoke("Victory", 5);
        }
        if (CandyPile.IsDestroyed()) {
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

    public bool GetMoveToNextWave() { return bMoveToNextWaveFlag; }
    public void SetMoveToNextWave(bool value) {  bMoveToNextWaveFlag = value; }
}
