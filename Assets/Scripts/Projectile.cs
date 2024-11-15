using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float life = 3;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float spinSpeed = 80f;
    

    [Header("Wwise")]
    [SerializeField] public AK.Wwise.Event Projectile_Hit;

    private Transform target;
    private float spinDirection = 1f;
    public int PDmg = 1;
    public static Projectile main;

    // Update is called once per frame
    void Awake()
    {
        main = this;
        Destroy(gameObject, life);
        if (Random.Range(0,1) == 0) spinDirection = -1f;
    }

    public void SetTarget(Transform _target)
    {
        target = _target; 
    }

    void Update()
    {
        transform.Rotate(Vector3.forward, spinDirection * projectileSpeed * spinSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (!target) return;
        Vector2 direction = (target.position - transform.position).normalized;
        gameObject.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Enemy") return;
        //Adding knockback to the collided object and calling the TakeDamage function from the EnemyCtrl Script.
        EnemyCtrl enemy = collision.gameObject.GetComponent<EnemyCtrl>();
        enemy.TakeDamage(PDmg);
        Projectile_Hit.Post(gameObject);
        Destroy(gameObject);
    }
}
