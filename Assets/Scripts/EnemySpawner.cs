using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    //Independent enemy wave spawner not connected to wave spawners.


    [Header("Attributes")]
    private float spawnFrequency = 15f;


    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefab;
    


    private void Awake()
    {
    }
    private void Update()
    {

    }
    void Start()
    {
        InvokeRepeating("SpawnEnemy", 5, spawnFrequency);
    }

    void SpawnEnemy()
    {
        int prefabIndex = Random.Range(0, enemyPrefab.Length);
        GameObject prefabToSPawn = enemyPrefab[prefabIndex];
        Instantiate(prefabToSPawn, gameObject.transform);
    }

}
