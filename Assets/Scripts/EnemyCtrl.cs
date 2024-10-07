using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] private float movSpeed = 2f;
    [SerializeField] public float enemyStunTimer = 3;
    [SerializeField] private float currentFull;
    [SerializeField] private float maxFull;
    [SerializeField] private float fillAmount;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] EnemyFloatingFeedMeter feedMeter;


    private Transform target;
    private int pathIndex = 0;
    private bool frozen = false;
    private float timer = -1;


    private void Awake()
    {
        feedMeter = GetComponentInChildren<EnemyFloatingFeedMeter>();
    }

    private void Start()
    {
        target = LevelManager.main.targetPoint;
        target = LevelManager.main.path[0];
  
    }

    private void Update()
    {
        if(Vector2.Distance(target.position, transform.position) <= 0.1f)
        {
            pathIndex++;
            if (pathIndex == LevelManager.main.path.Length)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                target = LevelManager.main.path[pathIndex];
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

    public void TakeDamage()
    {
        Freeze();
        currentFull += fillAmount;
        feedMeter.UpdateFeedMeter(currentFull, maxFull);

        if(currentFull == maxFull)
        {
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
        Destroy(gameObject);
    }
}
