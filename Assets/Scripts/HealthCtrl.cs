using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HealthCtrl : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] private int currentHealth = 3;
    [SerializeField] private int maxHealth = 3;

    [Header("FX")]
    [SerializeField] private AudioClip damageSFX;

    [Header("References")]
    [SerializeField] HealthMeter healthBar;

    private bool isDestroyed = false;


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
        
    }

    public void TakeDamage()
    {
        currentHealth --;
        SoundFXManager.instance.PlaySoundFXClip(damageSFX, MixerGroup.World, transform, 1f, 1f, false, Random.Range(.9f, 1.1f));
        healthBar.UpdateMeter(currentHealth, maxHealth);
        if (currentHealth <= 0 && !isDestroyed)
        {
            //Just in case if enemy spawn reached -1 to below
            WaveSpawnEnemies.onEnemyDeath.Invoke();
            isDestroyed = true;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
        LevelManager.main.DecrementEnemiesLeft();
        TakeDamage();
    }
    //private void OnDrawGizmosSelected()
    //{
    //    Handles.color = Color.cyan;
    //    Handles.DrawWireDisc(transform.position, transform.forward, 3.5f);
    //}
}
