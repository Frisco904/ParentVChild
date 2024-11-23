using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float rotationSpeed = 200f; // Time it takes to rotate then fire on new target.
    [SerializeField] private LayerMask enemyMask; //Mask where the enemies will be on, to ignore all other sprites on diff layers.
    [SerializeField] private Transform turretRotationPoint; 
    [SerializeField] private Transform projectileSpawnLocation;
    [SerializeField] private ParticleSystem turretDamagedSmoke ;
    [SerializeField] private float fireRate = 1f; // Time in seconds between shots.
    [SerializeField] private float turretDmg = 1f; // Damage done by projectile.
    [SerializeField] private float targetingRange = 50f; // Range the tower can find a target.
    [SerializeField] private float sprtRate = 0.5f; // Range the tower can find a target.
    [SerializeField] private float ctrlRate = 1f;
    [SerializeField] private bool turretDamaged = false;

    private float bpsBase;
    private float sprtBase;
    private float turretDmgBase;
    private float targetingRangeBase;
    private float ctrlBase;
    public int dmgLevel = 1;
    public int spdLevel = 1;
    public int ctrlLevel = 1;
    public int sprtLevel = 1;

    [Header("Upgrades")]
    [SerializeField] private int baseUpgradeRange = 10;
    [SerializeField] private int baseUpgradeDamage = 20;
    [SerializeField] private int baseUpgradeFireRate = 15;
    [SerializeField] private int baseUpgradeSprt = 5;
    [SerializeField] private int baseSellCost = 100;

    [Header("References")]
    [SerializeField] private GameObject projectilePrefab; // Prefab turret will shoot.

    [Header("Wwise")]
    [SerializeField] public AK.Wwise.Event Turret_Built;
    [SerializeField] public AK.Wwise.Event Turret_Shot;
    [SerializeField] public AK.Wwise.Event Turret_Sold;
    [SerializeField] public AK.Wwise.Event Turret_Upgraded;

    private float slowChance = 0.2f;    // 20% chance to slow
    private float paralyzeChance = 0.05f; // 5% chance to paralyze
    private float slowMultiplier = 0.75f; // Reduce speed to 75%
    private float slowDuration = 3f;     // Slow effect lasts 3 seconds
    private float paralysisDuration = 2f; // Paralysis lasts 2 seconds
    private float timeUntilFire;
    private Transform target;
    private bool ctrlChosen = false;
    private bool upgradeDmgDone = false;
    private bool upgradeSpdDone = false;
    private bool upgradeCtrlDone = false;

    void Start()
    {
        bpsBase = fireRate;
        ctrlBase = ctrlRate;
        sprtBase = sprtRate;
        targetingRangeBase = targetingRange;
        turretDmgBase = turretDmg;
        Turret_Built.Post(gameObject);
    }

    void Update()
    {
        if (turretDamaged) return;

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
        projectileScript.PDmg = turretDmg;
        Turret_Shot.Post(gameObject); // Wwise Event
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
        //Debug.Log(gameObject.name + " - Turret now selected");
    }

    public void DeselectTurret()
    {
        //Debug.Log(gameObject.name + " - Turret now deselected");
        LevelManager.main.selectedTurret = null;
        GetComponent<Renderer>().material.color = Color.white;
    }

    #region Upgrade Method
    public void UpgradeSpd()
    {
        //Calculates the cost and will automatically update the new price
        if (spdLevel != 5)
        {
            if (calculateCostFireRate() > LevelManager.main.GetCurrency()) return;

            LevelManager.main.SpendMoney(calculateCostFireRate());
            spdLevel++;

            //Calculates the new FireRate
            if (!upgradeSpdDone)
            {
                fireRate = calculateFireRate();
            }
            else
            {
                fireRate = calculateNewFireRate();
            }
            //Debug.Log(gameObject.name + " - Speed Upgraded");

            Turret_Upgraded.Post(gameObject); // Send Wwise event.
        }
    }

    public void UpgradeDmg()
    {
        //Calculates the cost and will automatically update the new price
        if (dmgLevel != 5)
        {
            if (calculateCostDamage() > LevelManager.main.GetCurrency()) return;

            LevelManager.main.SpendMoney(calculateCostDamage());
            dmgLevel++;

            if (!upgradeDmgDone)
            {
                turretDmg = calculateDamage();
            }
            else
            {
                turretDmg = calculateNewDamage();
            }
            //Debug.Log(gameObject.name + " - Damage Upgraded");
            
            Turret_Upgraded.Post(gameObject); // Send Wwise event.
        }
    }
    public void UpgradeCtrl()
    {
        if (ctrlLevel != 5)
        {
            ctrlChosen = true;
            //Calculates the cost and will automatically update the new price
            if (calculateCostCtrl() > LevelManager.main.GetCurrency()) return;

            LevelManager.main.SpendMoney(calculateCostCtrl());
            ctrlLevel++;

            //Calculates the new Ctrl
            ctrlRate = calculateCtrl();
            //Debug.Log(gameObject.name + " - Ctrl Upgraded");
            
            Turret_Upgraded.Post(gameObject); // Send Wwise event.
        }

    }

    public void UpgradeSprt()
    {
        if (sprtLevel != 5)
        {
            //Calculates the cost and will automatically update the new price
            if (calculateCostSprt() > LevelManager.main.GetCurrency()) return;

            LevelManager.main.SpendMoney(calculateCostSprt());
            sprtLevel++;

            //Calculates the new Range
            sprtRate = calculateSprt();
            //Debug.Log(gameObject.name + " - Support Upgraded");

            Turret_Upgraded.Post(gameObject); // Send Wwise event.
        }

    }
    #endregion

    #region Adjust method
    private void AdjustRange()
    {
        if (!upgradeDmgDone)
        {
            targetingRangeBase = .5f;
            upgradeDmgDone = true;
        }
    }

    private void AdjustDmg()
    {
        if (!upgradeSpdDone)
        {
            turretDmg = +.25f;
            upgradeSpdDone = true;
        }
    }

    #region For Ctrl
    private void AdjustDmgAndRange()
    {
        if (!upgradeCtrlDone)
        {
            fireRate = .2f;
            turretDmg = .5f;
            upgradeCtrlDone = true;
        }
    }

    private void increaseEffectChance()
    {
        if (upgradeCtrlDone)
        {
            slowChance += .075f;
            paralyzeChance += .05f;
        }
    }
    #endregion
    #endregion

    #region Calculate Cost
    private int calculateCostCtrl()
    {
        return Mathf.RoundToInt(ctrlBase * Mathf.Pow(ctrlLevel, 0.8f));
    }

    private int calculateCostDamage()
    {
        return Mathf.RoundToInt(baseUpgradeDamage * Mathf.Pow(dmgLevel, 0.8f));
    }

    private int calculateCostFireRate()
    {
        return Mathf.RoundToInt(baseUpgradeFireRate * Mathf.Pow(spdLevel, 0.8f));
    }

    private int calculateCostSprt()
    {
        return Mathf.RoundToInt(baseUpgradeSprt * Mathf.Pow(sprtLevel, 0.5f));
    }
    #endregion


    #region Calculate the first upgrades
    private float calculateDamage()
    {
        AdjustRange();
        return turretDmgBase + Mathf.Pow(dmgLevel, 0.25f);
    }

    private float calculateFireRate()
    {
        AdjustDmg();
        return bpsBase + Mathf.Pow(spdLevel, 0.2f);
    }

    private float calculateCtrl()
    {
        AdjustDmgAndRange();
        increaseEffectChance();
        return targetingRangeBase + Mathf.Pow(ctrlLevel, 0.4f);
    }


    private float calculateSprt()
    {
        //AdjustRange();
        return sprtBase + Mathf.Pow(sprtLevel, 0.3f);
    }
    #endregion

    #region Calculate the new upgrades
    private float calculateNewDamage()
    {
        return turretDmgBase + Mathf.Pow(dmgLevel, 0.1f);
    }

    private float calculateNewFireRate()
    {
        return bpsBase + Mathf.Pow(spdLevel, 0.1f);
    }

    public bool IsCtrlChosen()
    {
        return ctrlChosen;
    }
    #endregion


    public void ApplyEffect(EnemyCtrl enemy)
    {
        float randomValue = Random.Range(0f, 1f);

        if (randomValue <= paralyzeChance)
        {
            enemy.ApplyParalysis(paralysisDuration); // Apply paralysis
        }
        else if (randomValue <= slowChance)
        {
            enemy.ApplySlow(slowMultiplier, slowDuration); // Apply slow
        }
    }

    public void SellTurret()
    {
        LevelManager.main.GainMoney(baseSellCost);
        Turret_Sold.Post(gameObject);
        Destroy(this.gameObject);
    }


    //Not yet in use
    private int calculateCostRange()
    {
        return Mathf.RoundToInt(baseUpgradeRange * Mathf.Pow(ctrlLevel, 0.8f));
    }

    private float calculateRange()
    {
        //AdjustDmgAndRange();
        return targetingRangeBase + Mathf.Pow(ctrlLevel, 0.4f);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent<EnemyCtrl>(out EnemyCtrl enemy))
        {
            if (enemy.enraged) turretDamaged = true;
            turretDamagedSmoke.Play();
            enemy.enraged = false;
            enemy.movSpeed /= 2f;
            enemy.target = null;
        }
    }
}
