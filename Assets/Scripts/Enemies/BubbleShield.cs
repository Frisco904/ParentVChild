using Unity.VisualScripting;
using UnityEngine;

public class BubbleShield : MonoBehaviour
{
    [SerializeField] public int shieldHealth = 3;
    [Header("Wwise")]
    [SerializeField] public AK.Wwise.Event Bubble_Hit;
    [SerializeField] public AK.Wwise.Event Bubble_Broke;
    public void DamageShield()
    {
        shieldHealth -= 1;
        // Handle shield destroyed logic
        if (shieldHealth == 0)
        {
            // Find closest turret
            Turret[] turrets = FindObjectsOfType<Turret>();
            // Index Vars
            Transform closestTurret = transform;
            float oldDistance = 100000f; // Really big number to avoid fense-posting :)
            
            foreach (Turret turret in turrets)
            {
                float newDistance = Vector2.Distance(transform.position, turret.gameObject.transform.position);
                if (turret.turretDamaged) break;
                if (newDistance < oldDistance)
                {
                    oldDistance = newDistance;
                    closestTurret = turret.gameObject.transform;
                }
            }
            // Set enrage state and target
            EnemyCtrl enemy = GetComponentInParent<EnemyCtrl>();
            enemy.enraged = true;
            enemy.target = closestTurret;
            enemy.movSpeed *= 2f;
            Bubble_Broke.Post(gameObject);

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Projectile")
        {
            DamageShield();
            Bubble_Hit.Post(gameObject); // Send Wwise event;
            Destroy(collider.gameObject);
        }
    }
}