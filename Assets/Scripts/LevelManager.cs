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

    public Turret selectedTurret = null;
    private int enemiesLeft = 0;
    private bool WinConditionMet = false;
    private int score = 0;
    private WaveSpawnEnemies[] WaveSpawners;
    List<int> allSpawnsFinishedList = new List<int>();
    private float startTime;
    public float waveDelayTime;

    [Header("Enemy Wave Attributes")]
    [SerializeField] private int MaxWaves = 3;
    [SerializeField] private float initialWaveDelay;
    [SerializeField] private float timeBetweenWaves;

    [Header("References")]
    [SerializeField] public GameObject CandyPile;
    public PauseMenu MenuObj;

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
            path1.Add(CandyPile.transform);
            path2.Add(CandyPile.transform);
            path3.Add(CandyPile.transform);
        }
    }

    private void Start()
    {
        LevelLoaded.Post(gameObject);
        // Let Wwise know what level we are on.
        AkSoundEngine.SetSwitch("Level", "Level" + SceneManager.GetActiveScene().buildIndex, gameObject);
        MusicStart.Post(gameObject);
    }

    private void Update()
    {
        //Finding all instances of WaveSpawnEnemies and i
        WaveSpawners = FindObjectsOfType<WaveSpawnEnemies>();
        foreach (WaveSpawnEnemies spawner in WaveSpawners)
        {
            if (spawner.GetIsSpawning())
            {
                if (Time.time >= initialWaveDelay + startTime)
                {
                    // Debug.Log("Checking for the end wave condition now");
                    if (spawner.GetEnemiesAlive() == 0 && spawner.GetEnemiesLeftToSpawn() == 0)
                    {
                        Debug.Log("Spawner is finished");
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
        if (allSpawnsFinishedList.Count == WaveSpawners.Length)
        {

            foreach (WaveSpawnEnemies spawner in WaveSpawners)
            {
                //Debug.Log("For spawner: " + spawner.name + "Called end wave funciton");
                spawner.EndWave();
                if (spawner == WaveSpawners[WaveSpawners.Length - 1])
                {
                    //Debug.Log("This should occur once.");
                    allSpawnsFinishedList.Clear();
                }
            }
        }

        if (WinConditionMet && enemiesLeft == 0)
        {
            MenuObj.Invoke("VictoryScreen", 5);
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
            //Buy Turret
            currency -= cash;
            return true;
        } else
        {
            Debug.Log("Not enough cash");
            return false;
        }
    }

    //Getters and Setters
    public int GetCurrency() { return currency; }
    public int GetEnemiesLeft() { return enemiesLeft; }
    public void SetEnemiesLeft(int value) { enemiesLeft += value; }
    public void DecrementEnemiesLeft() { enemiesLeft--; }
    public int GetMaxWaves() { return MaxWaves; }
    public void SetWinCondition(bool bSetWinCondition) { WinConditionMet = bSetWinCondition; }
    public void AddScore(int value) { score += value; }
    public int GetScore() { return score; }
    public float GetInitialWaveDelay() { return initialWaveDelay; }
    public float GetTimeBetweenWavesDelay() { return timeBetweenWaves; }
}
