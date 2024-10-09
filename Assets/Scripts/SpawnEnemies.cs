using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum SpawnPoints
{
    SpawnPoint1 = 0,
    SpawnPoint2 = 1,
    SpawnPoint3= 2,
    SpawnPoint4 = 3,
}

public class SpawnEnemies : MonoBehaviour
{
    public static SpawnEnemies main;
    public GameObject enemyType;
    public float spawnFrequency;
    public SpawnPoints spawnPoint;
    [SerializeField] private LevelManager levelManager;
    private int index;



    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(spawnPoint);

        switch (spawnPoint)
        {
            //Debug.Log(spawnPoint);
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

        InvokeRepeating("SpawnEnemy", 0.0f, spawnFrequency);


    }

    void SpawnEnemy()
    {
        Instantiate(enemyType, levelManager.startPoint[index]);
    }

    public SpawnPoints GetSpawnPoint()
    {
        return spawnPoint;
    }
        
}
