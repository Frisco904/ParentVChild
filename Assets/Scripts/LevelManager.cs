using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [SerializeField] public Transform[] startPoints;
    [SerializeField] public List<Transform> path1;
    [SerializeField] public List<Transform> path2;
    [SerializeField] public List<Transform> path3;

    public Turret selectedTurret = null;
    private int enemiesLeft = 0;
    private bool winConditionMet = false;
    private int score = 0;
    private WaveSpawnEnemies[] waveSpawners;
    List<int> allSpawnsFinishedList = new List<int>();
    private float startTime;
    public float waveDelayTime;
    private bool isCountdownActive = false;
    private bool autoLevelStarted = false;
    private bool levelStart;
    private int maxEnemyPerWave = -1;

    [SerializeField] private bool autoLevelStart;
    [Header("Enemy Wave Attributes")]
    [SerializeField] private int maxWaves = 3;
    [SerializeField] private float initialWaveDelay;
    [SerializeField] private float timeBetweenWaves;

    [Header("References")]
    [SerializeField] public GameObject candyPile;
    [SerializeField] private GameObject countDownTxt;
    [SerializeField] private TextMeshProUGUI countDown;
    public Canvas VideoPlayer;
    public bool SkipTutorialVideo;
    public PauseMenu menuObj;
    public int preparationTime = 5;

    [Header("Currency System")]
    [SerializeField] private int currency = 100;

    [Header("Wwise")]
    [SerializeField] public AK.Wwise.State Playing;

    [SerializeField] public AK.Wwise.Event LevelLoaded;

    [SerializeField] public AK.Wwise.Event MusicStart;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
            path1.Add(candyPile.transform);
            path2.Add(candyPile.transform);
            path3.Add(candyPile.transform);
            levelStart = false;
        }
    }

    private void Start()
    {
        LevelLoaded.Post(gameObject);
        // Let Wwise know what level we are on.
        AkSoundEngine.SetSwitch("Level", "Level" + SceneManager.GetActiveScene().buildIndex, gameObject);
        Debug.Log("Starting Music for Level " + SceneManager.GetActiveScene().buildIndex);
        MusicStart.Post(gameObject);
        //Enabling Canvas for Intro Video.
        if(SceneManager.GetActiveScene().name != "Tutorial")
        {
            return;
        }

        if (!SkipTutorialVideo) 
        {
           VideoPlayer.gameObject.SetActive(true);
        }
        
    }

    private void Update()
    {
        
        //Using autoLevelStarted as flas as a way to tell if we have already started the level, and only calling the function once.
        if (autoLevelStart && !autoLevelStarted)
        {
            StartLevel();
            autoLevelStarted = true;
        }
        //Finding all instances of WaveSpawnEnemies and checking if they are spawning.
        waveSpawners = FindObjectsOfType<WaveSpawnEnemies>();
        foreach (WaveSpawnEnemies spawner in waveSpawners)
        {
            if (spawner.GetIsSpawning())
            {
                if (Time.time >= initialWaveDelay + startTime)
                {
                    // Debug.Log("Checking for the end wave condition now");
                    if (spawner.GetEnemiesAlive() == 0 && spawner.GetEnemiesLeftToSpawn() == 0)
                    {
                        //Debug.Log("Spawner is finished");
                        if (!allSpawnsFinishedList.Contains(spawner.GetInstanceID()))
                        {
                            allSpawnsFinishedList.Add(spawner.GetInstanceID());
                        }
                        
                    }
                }
            }
        }
        //Debug.Log(allSpawnsFinishedList.Count);

        //At this logic check all spawners would be done witht their waves.
        if (allSpawnsFinishedList.Count == waveSpawners.Length)
        {

            foreach (WaveSpawnEnemies spawner in waveSpawners)
            {
                //Debug.Log("For spawner: " + spawner.name + "Called end wave funciton");
                spawner.EndWave();
                if (spawner == waveSpawners[waveSpawners.Length - 1])
                {
                    //Debug.Log("This should occur once.");
                    allSpawnsFinishedList.Clear();
                }
            }
        }

        if (winConditionMet && enemiesLeft == 0)
        {
            menuObj.Invoke("VictoryScreen", 5);
        }
        if (candyPile.IsDestroyed())
        {
            menuObj.Defeat();
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
            //Buy Turret
            currency -= cash;
            return true;
        } else
        {
            Debug.Log("Not enough cash");
            return false;
        }
    }

    public void StartCountDown()
    {
        if (!isCountdownActive)
        {
            isCountdownActive = true;
            StartCoroutine(CountDownTimer());
        }
    }

    IEnumerator CountDownTimer()
    {
        int timeRemaining = preparationTime;

        while (timeRemaining > 0)
        {
            countDown.text = timeRemaining.ToString();
            yield return new WaitForSeconds(1);
            timeRemaining--;
        }

        countDownTxt.gameObject.SetActive(false);
    }

    //Getters and Setters
    public int GetCurrency() { return currency; }
    public int GetEnemiesLeft() { return enemiesLeft; }
    public void SetEnemiesLeft(int value) 
    { 
        enemiesLeft += value;
        if (enemiesLeft >= maxEnemyPerWave)
        {
            maxEnemyPerWave = enemiesLeft;
        }
    }
    public void DecrementEnemiesLeft() 
    { 
            enemiesLeft--;
            
    }
    public int GetMaxWaves() { return maxWaves; }

    public int GetMaxEnemiesLeft() { return maxEnemyPerWave; }
    public void SetWinCondition(bool bSetWinCondition) { winConditionMet = bSetWinCondition; }
    public void AddScore(int value) { score += value; }
    public int GetScore() { return score; }
    public float GetInitialWaveDelay() { return initialWaveDelay; }
    public float GetTimeBetweenWavesDelay() { return timeBetweenWaves; }
    public bool GetStartLevel() {  return levelStart; }
    public void StartLevel() { 
        levelStart = true;
        StartCountDown();
        countDownTxt.gameObject.SetActive(true);
    }

}
