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
            isDestroyed = true;
            Destroy(gameObject);
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
