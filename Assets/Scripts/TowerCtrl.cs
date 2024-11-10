using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    //[SerializeField] private Transform turretRotationPoint;

    [Header("Attributes")]
    [SerializeField] private float targetingRange = 50f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private LayerMask enemyMask; //Mask where the enemies will be on, to ignore all other sprites on diff layers.
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private Transform projectileSpawnLocation;
    [SerializeField] private float fireRate = 1f;

    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellBtn;
    [SerializeField] private int baseUpgradeCost = 100;
    [SerializeField] private int baseSellCost = 100;
    
    [Header("Wwise")]
    [SerializeField] public AK.Wwise.Event TurretShot;

    private float bpsBase;
    private float targetingRangeBase;

    private int level = 1;

    private float timeUntilFire;

    private Transform target;


    private void Start()
    {
        bpsBase = fireRate;
        targetingRangeBase = targetingRange;
        upgradeButton.onClick.AddListener(UpgradeTurret);
        sellBtn.onClick.AddListener(SellTorrent);
    }

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
        TurretShot.Post(gameObject); // Wwise Event
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
   /*
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
   */
    //This is for upgrade function
    #region Upgrade Methods
    public void openUpgradeUI()
    {
        upgradeUI.SetActive(true);
    }

    public void closeUpgradeUI()
    {
        upgradeUI.SetActive(false);
        UIManager.main.setHoveringState(false);
    }

    public void UpgradeTurret()
    {

        //Calculates the cost and will automatically update the new price
        if (calculateCost() > LevelManager.main.GetCurrency()) return;

        LevelManager.main.SpendMoney(calculateCost());

        level++;

        //Calculates the new FireRate
        fireRate = CalculateFireRate();

        //Calculates the new Range
        targetingRange = calculateRange();

        closeUpgradeUI();
        //Debug.Log("New Fire Rate and Turret range: " + fireRate + targetingRange);
        //Debug.Log("New Cost: " + calculateCost());
    }

    private int calculateCost()
    {
        return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(level, 0.8f));
    }

    private float CalculateFireRate()
    {
        return bpsBase * Mathf.Pow(level, 0.5f);
    }

    private float calculateRange()
    {
        return targetingRangeBase * Mathf.Pow(level, 0.4f);
    }
    #endregion

    //For selling turrents
    public void SellTorrent()
    {
        LevelManager.main.GainMoney(baseSellCost);

        closeUpgradeUI();

        Destroy(this.gameObject);
    }
}
