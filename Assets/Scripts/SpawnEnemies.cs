using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public static SpawnEnemies main;
    public GameObject enemyType;
    public float spawnFrequency;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnEnemy", 0.0f, spawnFrequency);
    }

    void SpawnEnemy()
    {
        Instantiate(enemyType, transform);
    }

}
