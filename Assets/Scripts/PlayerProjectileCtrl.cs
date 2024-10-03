using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileCtrl : MonoBehaviour
{
    public float projectileSpeed = 10f;
    [SerializeField]
    private Transform projectileSpawnLocation;

    [SerializeField]
    private GameObject projectile;

    private Vector2 lookDirection;
    private float lookAngle;

    private void Update()
    {
        lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, lookAngle - 90f);

        if (Input.GetMouseButtonDown(0))
        {
            FireProjectile();
        }
    }

    private void FireProjectile()
    {
        GameObject firedProjectile = Instantiate(projectile, projectileSpawnLocation.position, projectileSpawnLocation.rotation);
        firedProjectile.GetComponent<Rigidbody2D>().velocity = projectileSpawnLocation.up * projectileSpeed;
    }
}
