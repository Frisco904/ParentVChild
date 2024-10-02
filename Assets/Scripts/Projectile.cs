using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float life = 3;
    [SerializeField] private float projectileSpeed = 5f;
    //[SerializeField] private Transform projectileSpawnLocation;

    private Transform target;

    // Update is called once per frame
    void Awake()
    {
        Destroy(gameObject, life);
    }

    public void SetTarget(Transform _target)
    {
        target = _target; 
    }

    private void FixedUpdate()
    {
        if (!target) return;

        Vector2 direction = (target.position - transform.position).normalized;

        gameObject.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);

        //Moving the hit projectile up
        collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.forward * 100000000);
        EnemyCtrl enemy = collision.gameObject.GetComponent<EnemyCtrl>();
        enemy.TakeDamage();

    }
}
