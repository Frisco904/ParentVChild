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
    private int enemiesLeft = 0;
    private bool WindConditionMet = false;
    private int score = 0;
    private WaveSpawnEnemies[] WaveSpawners;

    [Header("Enemy Wave Attributes")]
    [SerializeField] private int MaxWaves = 3;
    [SerializeField] private float initialWaveDelay = 5f;

    [Header("References")]
    [SerializeField] public GameObject CandyPile;
    public PauseMenu MenuObj;

    [Header("Currency System")]
    [SerializeField] private int currency = 100;


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

    }
    private void Update()
    {

        if (WindConditionMet && enemiesLeft == 0)
        {
            MenuObj.Invoke("Victory", 5);
        }
        if (CandyPile.IsDestroyed()) {
            MenuObj.Defeat();
        }
        if (enemiesLeft == 0) {
            WaveSpawners = FindObjectsOfType<WaveSpawnEnemies>();

            foreach (WaveSpawnEnemies spawner in WaveSpawners)
            {
                spawner.EndWave();

            }
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
    public void SetWinCondition(bool bSetWinCondition) { WindConditionMet = bSetWinCondition; }
    public void AddScore(int value) { score += value; }
    public int GetScore() { return score; }
    public float GetInitialWaveDelay() { return initialWaveDelay; }
}
