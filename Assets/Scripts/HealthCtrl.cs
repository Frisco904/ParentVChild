using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCtrl : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] private int currentHealth = 3;
    [SerializeField] private int maxHealth = 3;

    private bool isDestroyed = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage()
    {
        currentHealth--;
        if (currentHealth == 0 && !isDestroyed)
        {
            //Just in case if enemy spawn reached -1 to below
            isDestroyed = true;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enemy collided.");
        TakeDamage();
    }
}
