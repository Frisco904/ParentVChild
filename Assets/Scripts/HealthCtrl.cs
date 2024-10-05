using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthCtrl : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    [Header("References")]
    [SerializeField] HealthMeter healthBar;

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthMeter>();
        healthBar.GetComponent<Slider>().value = maxHealth;
    }
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthBar.UpdateMeter(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
