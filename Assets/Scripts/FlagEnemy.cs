using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
            if (enemy.followLeaderEnemy || enemy.enraged) return; // If target is already following or enraged someone ignore it.
            enemy.followLeaderEnemy = true;

            //If enemies target was a tracking point, we destroy the tracking point since child will not reach it.
            if (enemy.target.CompareTag("TrackingPoint"))
            {
                enemy.DestroyTrackingPoint();
            }


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