using System.Collections.Generic;
using UnityEngine;

public class FlagEnemy : MonoBehaviour 
{
    [SerializeField] public float radius = 20f;
    [SerializeField] public CircleCollider2D leaderAura;
    [SerializeField] public List<EnemyCtrl> effectedEnemies;

    void Start()
    {
        leaderAura.radius = radius;
    }

    void Update()
    {
        Debug.DrawCircle(transform.position, radius/3, 8, Color.yellow);
    }

    void OnTriggerEnter2D(Collider2D collider) 
    {
        // Ignore other flag enemy
        var flagEnemyCheck = collider.gameObject.GetComponentInChildren<FlagEnemy>();
        if (flagEnemyCheck != null) return;

        if (collider.gameObject.TryGetComponent<EnemyCtrl>(out EnemyCtrl enemy))
        {
            if (enemy.followLeaderEnemy) return; // If target is already following someone ignore it.
            enemy.followLeaderEnemy = true;
            
            enemy.target = transform; // Set enemy's new target to follow this.
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent<EnemyCtrl>(out EnemyCtrl enemy))
        {
            enemy.followLeaderEnemy = false;
        }
    }

    void OnDestroy()
    { 
        leaderAura.radius = 0;
    }
}