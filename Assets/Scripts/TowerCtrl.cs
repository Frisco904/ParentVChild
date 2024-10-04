using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

public class Tower : MonoBehaviour
{
    //[SerializeField] private Transform turretRotationPoint;

    [Header("Attributes")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private LayerMask enemyMask; //Mask where the enemies will be on, to ignore all other sprites on diff layers.
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private Transform projectileSpawnLocation;
    [SerializeField] private float fireRate = 1f;

    [Header("References")]
    [SerializeField]  private GameObject projectilePrefab;



    private float timeUntilFire;

    private Transform target;

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        RotateTowardsTarget();

        if (!CheckTargetInRange())
        {
            target = null;
        }
        else
        {
            timeUntilFire += Time.deltaTime;
            if (timeUntilFire >= 1f / fireRate)
            {

                Shoot();
                timeUntilFire = 0f;
            }

        }
    }
        
    private void Shoot()
    {
        //Instantiating the projectile and calling the SetTarget function for the projectile.
        GameObject projectileObj = Instantiate(projectilePrefab, projectileSpawnLocation.position, projectileSpawnLocation.rotation);
        Projectile projectileScript = projectileObj.GetComponent<Projectile>();
        projectileScript.SetTarget(target);
    }
    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length > 0) 
        {
            target = hits[0].transform;
        }

    }

    private void RotateTowardsTarget()
    {
        //Rotate the 'Barrel' towards the enemy. Subtracting 90 degrees to make the positioning line up with the starting rotation of the barrel.
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;
        
        //Rotating the Z-value of the Barrel.
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        //Rotation speed of the barrel.
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private bool CheckTargetInRange()
    {
        return Vector2.Distance(target.position,transform.position) <= targetingRange;
    }
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
}
