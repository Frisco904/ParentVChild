using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] private float movSpeed = 2f;
    [SerializeField] public float enemyStunTimer = 3;
    [SerializeField] private float currentFull;
    [SerializeField] private float maxFull;
    [SerializeField] private float fillAmount;
    [SerializeField] private int currenyWorth = 50;
    [SerializeField] private LayerMask candyMask;
    [SerializeField] private float targetingRange = 5f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] EnemyFloatingFeedMeter feedMeter;
    //[SerializeField] private SpawnEnemies enemySpawner;


    private SpawnEnemies spawnEnemies;
    private SpawnPoints spawnPoint;
    private Transform target;
    private int pathIndex = 0;
    private bool frozen = false;
    private float timer = -1;
    private bool isDestroyed = false;
    private Transform[] path;


    private void Awake()
    {
        feedMeter = GetComponentInChildren<EnemyFloatingFeedMeter>();

    }

    private void Start()
    {
        spawnPoint = gameObject.GetComponentInParent<SpawnEnemies>().GetSpawnPoint();

        switch (spawnPoint)
        {
            case SpawnPoints.SpawnPoint1:
                target = LevelManager.main.path1[0];
                path = LevelManager.main.path1.ToArray();
                break;
            case SpawnPoints.SpawnPoint2:
                target = LevelManager.main.path2[0];
                path = LevelManager.main.path2.ToArray();
                break;
            case SpawnPoints.SpawnPoint3:
                target = LevelManager.main.path3[0];
                path = LevelManager.main.path3.ToArray();
                break;
        }
    }

    private void Update()
    {
        CandyInRange();
        if (Vector2.Distance(target.position, transform.position) <= .1f)
        {
            pathIndex++;
            if (pathIndex == path.Length)
            {
                SpawnEnemies.onEnemyDeath.Invoke();
                Destroy(gameObject);
                return;
            }
            else
            {
                target = path[pathIndex];
            }
             
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = -1;
                frozen = false;
            }
        }
    }   

    private void FixedUpdate()
    {
        if (frozen) return;
        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * movSpeed;

    }
    private void CandyInRange()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, candyMask);
        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }

    }

    public void TakeDamage()
    {
        Freeze();
        currentFull += fillAmount;
        feedMeter.UpdateFeedMeter(currentFull, maxFull);
        if(currentFull == maxFull && !isDestroyed)
        {
            //Calculates the enemy killed and won't go overboard to negative numbers
            SpawnEnemies.onEnemyDeath.Invoke();
            isDestroyed = true;
            Eliminate();
        }
    }

    public void Freeze()
    {
        frozen = true;
        timer = enemyStunTimer;
    }

    public void Eliminate()
    {
        //This gains money when enemy are killed by the torrents
        LevelManager.main.GainMoney(currenyWorth);
        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {

    }
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
}
