using UnityEngine;

public class Turret : MonoBehaviour
{
    //[SerializeField] private Transform turretRotationPoint;

    [Header("Attributes")]
    [SerializeField] private float targetingRange = 50f; // Range the tower can find a target.
    [SerializeField] private float rotationSpeed = 200f; // Time it takes to rotate then fire on new target.
    [SerializeField] private LayerMask enemyMask; //Mask where the enemies will be on, to ignore all other sprites on diff layers.
    [SerializeField] private Transform turretRotationPoint; 
    [SerializeField] private Transform projectileSpawnLocation;
    [SerializeField] private float fireRate = 1f; // Time in seconds between shots.
    [SerializeField] private float turretDmg = 1f; // Damage done by projectile.

    private float bpsBase;
    private float turretDmgBase;
    private float targetingRangeBase;
    public int dmgLevel = 1;
    public int spdLevel = 1;
    public int ctrlLevel = 1;
    public int sprtLevel = 1;

    [Header("Upgrades")]
    [SerializeField] private int baseUpgradeRange = 10; 
    [SerializeField] private int baseUpgradeDamage = 20;
    [SerializeField] private int baseUpgradeFireRate = 15;
    [SerializeField] private int baseSellCost = 100;
    
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab; // Prefab turret will shoot.

    [Header("Wwise")]
    [SerializeField] public AK.Wwise.Event TurretShot;

    private float timeUntilFire;
    private Transform target;

    void Start()
    {
        bpsBase = fireRate;
        targetingRangeBase = targetingRange;
        turretDmgBase = turretDmg;
    }

    void Update()
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

    //This is for upgrade function
    public void SelectTurret()
    {
        LevelManager.main.selectedTurret = this;
        GetComponent<Renderer>().material.color = Color.red;
        Debug.Log(gameObject.name + " - Turret now selected");
    }

    public void DeselectTurret()
    {
        Debug.Log(gameObject.name + " - Turret now deselected");
        LevelManager.main.selectedTurret = null;
        GetComponent<Renderer>().material.color = Color.white;
    }

    public void UpgradeSpd()
    {
        //Calculates the cost and will automatically update the new price
        if (calculateCostRange() > LevelManager.main.GetCurrency()) return;

        LevelManager.main.SpendMoney(calculateCostRange());
        spdLevel++;

        //Calculates the new Range
        targetingRange = calculateRange();
        Debug.Log(gameObject.name + " - Speed Upgraded");
    }

    public void UpgradeDmg()
    {
        //Calculates the cost and will automatically update the new price
        if (calculateCostDamage() > LevelManager.main.GetCurrency()) return;

        LevelManager.main.SpendMoney(calculateCostDamage());
        dmgLevel++;

        turretDmg = calculateDamage();
        Debug.Log(gameObject.name + " - Damage Upgraded");

    }

    public void UpgradeCtrl()
    {
        //Calculates the cost and will automatically update the new price
        if (calculateCostFireRate() > LevelManager.main.GetCurrency()) return;

        LevelManager.main.SpendMoney(calculateCostFireRate());
        ctrlLevel++;

        //Calculates the new FireRate
        fireRate = calculateFireRate();
        Debug.Log(gameObject.name + " - Control Upgraded");

    }

    public void UpgradeSprt()
    {
        //Calculates the cost and will automatically update the new price
        if (calculateCostFireRate() > LevelManager.main.GetCurrency()) return;

        LevelManager.main.SpendMoney(calculateCostFireRate());
        sprtLevel++;

        //Calculates the new FireRate
        fireRate = calculateFireRate();
        Debug.Log(gameObject.name + " - Support Upgraded");
    }

    private int calculateCostRange()
    {
        return Mathf.RoundToInt(baseUpgradeRange * Mathf.Pow(sprtLevel, 0.8f));
    }

    private int calculateCostDamage()
    {
        return Mathf.RoundToInt(baseUpgradeDamage * Mathf.Pow(dmgLevel, 0.8f));
    }

    private int calculateCostFireRate()
    {
        return Mathf.RoundToInt(baseUpgradeFireRate * Mathf.Pow(spdLevel, 0.8f));
    }

    private float calculateFireRate()
    {
        return bpsBase * Mathf.Pow(spdLevel, 0.5f);
    }

    private float calculateRange()
    {
        return targetingRangeBase * Mathf.Pow(sprtLevel, 0.4f);
    }

    private float calculateDamage()
    {
        return turretDmgBase * Mathf.Pow(dmgLevel, 1f);
    }

    public void SellTurret()
    {
        LevelManager.main.GainMoney(baseSellCost);
        Destroy(this.gameObject);
    }
}
