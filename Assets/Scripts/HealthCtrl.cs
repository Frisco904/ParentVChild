using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HealthCtrl : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] private LayerMask enemyMask; //Mask where the enemies will be on, to ignore all other sprites on diff layers.
    [SerializeField] private int currentHealth = 3;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] HealthMeter healthBar;
    [SerializeField] public float dangerRange;

    [Header("Wwise")]
    [SerializeField] public AK.Wwise.RTPC DanagerLevel;
    [SerializeField] public AK.Wwise.Event DamageTaken;

    private bool isDestroyed = false;
    private int enemiesNearby = 0;
    
    // Start is called before the first frame update
    void Awake()
    {
        healthBar = GetComponentInChildren<HealthMeter>();
        healthBar.GetComponent<Slider>().value = maxHealth;

    }

    private void Start()
    {
        currentHealth = maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
       UpdateDangerLevel();
    }

    public void TakeDamage()
    {
        currentHealth --;
        healthBar.UpdateMeter(currentHealth, maxHealth);
        DamageTaken.Post(gameObject); // Wwise Event
        if (currentHealth <= 0 && !isDestroyed)
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
    }

    // Check for nearby enemies and send the current "danger level" to Wwise
    private void UpdateDangerLevel()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, dangerRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length != enemiesNearby) 
        {
            enemiesNearby = hits.Length;
            DanagerLevel.SetGlobalValue(enemiesNearby);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //If the collider is an enemy then we destroy the enemy game object, decrement the enemy counter, and call TakeDamage function. Enemy counter is used as the objective to move to the next level.
        if (collision.tag == "Enemy")
        {

            collision.gameObject.GetComponent<EnemyCtrl>().CandyReached();
            TakeDamage();
        }
    }
    //private void OnDrawGizmosSelected()
    //{
    //    Handles.color = Color.cyan;
    //    Handles.DrawWireDisc(transform.position, transform.forward, 3.5f);
    //}
}
