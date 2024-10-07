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
    [SerializeField] private LayerMask candyMask;
    [SerializeField] private float targetingRange = 5f;

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
        CandyInRange();
        if (Vector2.Distance(target.position, transform.position) <= .1f)
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
    private void CandyInRange()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, candyMask);
        Debug.Log(hits.Length);
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
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
}
