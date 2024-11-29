using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float life = 3;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float spinSpeed = 80f;
    [SerializeField] Sprite[] bulletImg;
    [SerializeField] SpriteRenderer bulletRenderer;


    [Header("Wwise")]
    [SerializeField] public AK.Wwise.Event Projectile_Hit;

    private Transform target;
    private float spinDirection = 1f;
    public float PDmg = 1;
    public static Projectile main;
    private Dictionary<Turret.TurretType, Sprite> turrents;
    int rBulletImg;
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
        Turret[] turrents = FindObjectsOfType<Turret>();

        if (collision.gameObject.tag != "Enemy") return;
        //Adding knockback to the collided object and calling the TakeDamage function from the EnemyCtrl Script.
        EnemyCtrl enemy = collision.gameObject.GetComponent<EnemyCtrl>();
        foreach (Turret turret in turrents)
        {
            if (turret != null)
            {
                if (turrents.Length > 0)
                {
                    foreach (Turret turrent in turrents)
                    {
                        if (turrent.IsCtrlChosen())
                        {
                            turrent.ApplyEffect(enemy);
                        }

                    }

                }
            }
        }
        enemy.TakeDamage(PDmg);
        Projectile_Hit.Post(gameObject);
        Destroy(gameObject);
    }

    public void setBulletSprite(Turret.TurretType turrent)
    {
        switch (turrent)
        {
            case Turret.TurretType.Dmg: 
                bulletRenderer.sprite = bulletImg[4]; 
                break;
            case Turret.TurretType.Spd:
                bulletRenderer.sprite = bulletImg[2];
                break;
            case Turret.TurretType.Ctrl:
                bulletRenderer.sprite = bulletImg[3];
                break;
            case Turret.TurretType.Sprt:
                bulletRenderer.sprite = bulletImg[0];
                break;
            case Turret.TurretType.None:
            default:
                rBulletImg = UnityEngine.Random.Range(1, bulletImg.Length + 1);
                bulletRenderer.sprite = bulletImg[rBulletImg - 1];
                break;
        }
    }

}
