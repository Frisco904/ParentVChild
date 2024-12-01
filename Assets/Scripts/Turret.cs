using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float rotationSpeed = 200f; // Time it takes to rotate then fire on new target.
    [SerializeField] private LayerMask enemyMask; //Mask where the enemies will be on, to ignore all other sprites on diff layers.
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private Transform projectileSpawnLocation;
    [SerializeField] private ParticleSystem turretDamagedSmoke;
    [SerializeField] private float fireRate = 1f; // Time in seconds between shots.
    [SerializeField] private float turretDmg = 1f; // Damage done by projectile.
    [SerializeField] private float targetingRange; // Range the tower can find a target.
    [SerializeField] private float sprtRate = 0.5f; // Range the tower can find a target.
    [SerializeField] private float ctrlRate = 1f;
    [SerializeField] public bool turretDamaged = false;
    [SerializeField] private float circumferenceLineThickness = 1f;

    public TurretType turretType;

    private LineRenderer lineRenderer;
    private int segments = 50;
    private bool drawCircleOutlineToScreen = false;
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
    [SerializeField] private int upgradeCostMultiplyer = 150;

    [SerializeField] private int baseSellCost = 100;
    [SerializeField] public int maxLvl = 6;


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


    public enum TurretType
    {
        None,
        Dmg, //Damage
        Spd, //Speed
        Ctrl, //Control
        Sprt  //Support
    }



    void Start()
    {
        bpsBase = fireRate;
        ctrlBase = ctrlRate;
        sprtBase = sprtRate;
        targetingRangeBase = targetingRange;
        turretDmgBase = turretDmg;
        Turret_Built.Post(gameObject);

        //Line Renderer Logic
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        // Blend alpha from opaque at 0% to transparent at 100%
        lineRenderer.positionCount = segments;
        lineRenderer.useWorldSpace = true;
        //Set to the same thickness to have a consistent size around the object.
        lineRenderer.startWidth = circumferenceLineThickness;
        lineRenderer.endWidth = circumferenceLineThickness;
        lineRenderer.loop = true;
        //CreatePoints();

    }

    void Update()
    {
        if (turretDamaged) return;
        //Draw Circle outline is controled in select/deselect turret via enable property.
        DrawCircleOutline();

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
                //Debug.Log(targetingRange);
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
        projectileScript.setBulletSprite(this.turretType);
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
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }



    //This is for upgrade function
    public void SelectTurret()
    {
        LevelManager.main.selectedTurret = this;
        GetComponent<Renderer>().material.color = Color.red;
        //drawCircleOutlineToScreen = true;
        gameObject.GetComponent<LineRenderer>().enabled = true;
        //Debug.Log(gameObject.name + " - Turret now selected");
    }

    public void DeselectTurret()
    {
        //Debug.Log(gameObject.name + " - Turret now deselected");
        LevelManager.main.selectedTurret = null;
        GetComponent<Renderer>().material.color = Color.white;
        //drawCircleOutlineToScreen = false;
        gameObject.GetComponent<LineRenderer>().enabled = false;
    }

    public void ChosenType()
    {
        switch (turretType)
        {
            case TurretType.None:
                break;
            case TurretType.Dmg:
                calculateDamage();
                break;
            case TurretType.Spd:
                calculateFireRate();
                break;
            case TurretType.Ctrl:
                calculateCtrl();
                break;
            case TurretType.Sprt:
                calculateSprt();
                break;
        }
    }

    #region Upgrade Method
    public void UpgradeSpd()
    {
        //Calculates the cost and will automatically update the new price
        if (spdLevel != maxLvl)
        {
            if (calculateCostFireRate() > LevelManager.main.GetCurrency()) return;

            LevelManager.main.SpendMoney(calculateCostFireRate());
            spdLevel++;

            //Calculates the new FireRate
            if (turretType != TurretType.Spd)
            {
                AdjustDmg();
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
        if (dmgLevel != maxLvl)
        {
            if (calculateCostDamage() > LevelManager.main.GetCurrency()) return;

            LevelManager.main.SpendMoney(calculateCostDamage());
            dmgLevel++;

            if (turretType != TurretType.Dmg)
            {
                AdjustRange();
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
        if (ctrlLevel != maxLvl)
        {
            ctrlChosen = true;
            //Calculates the cost and will automatically update the new price
            if (calculateCostCtrl() > LevelManager.main.GetCurrency()) return;

            LevelManager.main.SpendMoney(calculateCostCtrl());
            ctrlLevel++;

            //Calculates the new Ctrl
            if (turretType != TurretType.Ctrl)
            {
                AdjustCtrl();
                ctrlRate = calculateCtrl();
            }
            else
            {
                increaseEffectChance();
            }
            //Debug.Log(gameObject.name + " - Ctrl Upgraded");

            Turret_Upgraded.Post(gameObject); // Send Wwise event.
        }

    }

    public void UpgradeSprt()
    {
        if (sprtLevel != maxLvl)
        {
            //Calculates the cost and will automatically update the new price
            if (calculateCostSprt() > LevelManager.main.GetCurrency()) return;

            LevelManager.main.SpendMoney(calculateCostSprt());
            sprtLevel++;

            //Calculates the new Range
            if (turretType != TurretType.Sprt)
            {
                AdjustSprt();
                targetingRange = calculateSprt();
            }
            else
            {
                targetingRange = calculateNewSprt();
            }
            //Debug.Log(gameObject.name + " - Support Upgraded");

            Turret_Upgraded.Post(gameObject); // Send Wwise event.
        }

    }
    #endregion

    #region Adjust method
    private void AdjustRange()
    {
        if (turretType != TurretType.Dmg)
        {
            targetingRange -= .5f;
        }
    }

    private void AdjustDmg()
    {
        if (turretType != TurretType.Spd)
        {
            turretDmg = .25f;
        }
    }

    private void AdjustSprt()
    {
        if (turretType != TurretType.Sprt)
        {
            turretDmg = .5f;
        }
    }

    #region For Ctrl
    private void AdjustCtrl()
    {
        if (turretType != TurretType.Ctrl)
        {
            fireRate = .7f;
            turretDmg = .5f;
            slowChance += .075f;
            paralyzeChance += .05f;
        }
    }

    private void increaseEffectChance()
    {
        if (turretType == TurretType.Ctrl)
        {
            slowChance += .075f;
            paralyzeChance += .05f;
        }
    }
    #endregion
    #endregion

    #region Calculate Cost
    public int calculateCostCtrl()
    {
        return ctrlLevel * upgradeCostMultiplyer;
    }

    public int calculateCostDamage()
    {
        return dmgLevel * upgradeCostMultiplyer;
    }

    public int calculateCostFireRate()
    {
        return spdLevel * upgradeCostMultiplyer;
    }

    public int calculateCostSprt()
    {
        return sprtLevel * upgradeCostMultiplyer;
    }
    #endregion


    #region Calculate the first upgrades
    private float calculateDamage()
    {
        return turretDmgBase + dmgLevel;
    }

    private float calculateFireRate()
    {
        return bpsBase + Mathf.Pow(spdLevel, 0.2f);
    }

    private float calculateCtrl()
    {
        return ctrlBase + Mathf.Pow(ctrlLevel, 0.4f);
    }

    private float calculateSprt()
    {
        return targetingRange + Mathf.Pow(sprtLevel, 0.2f);
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

    private float calculateNewSprt()
    {
        return targetingRangeBase + Mathf.Pow(sprtLevel, 0.5f);
    }

    public bool IsCtrlChosen()
    {
        return ctrlChosen;
    }
    #endregion

    public void DrawCircleOutline()
    {
        var centerPos = this.transform.position;

        for (int currentStep = 0; currentStep < segments; currentStep++)
        {
            float circumferenceProgress = (float)currentStep / segments;

            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled * targetingRange;
            float y = yScaled * targetingRange;

            Vector3 currentPosition = new Vector3(x, y, 0) + centerPos;

            lineRenderer.SetPosition(currentStep, currentPosition);
        }
    }


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

    public void RepairTurret()
    {
        if (!turretDamaged) { return; }
        if (CalculateRepairCost() <= LevelManager.main.GetCurrency())
        {
            LevelManager.main.SpendMoney((int)CalculateRepairCost());
            turretDamaged = false;
            turretDamagedSmoke.Stop();

        }
    }

    private float CalculateRepairCost()
    { 

        //Current logic for Repair cost is 1/4 of their calculated cost.
        switch (turretType)
        {
            case TurretType.Dmg:
                return (calculateCostDamage() / 4);
                //break;
            case TurretType.Spd:
                return (calculateCostFireRate() / 4);
                //break;
            case TurretType.Ctrl:
                return (calculateCostCtrl() / 4);
                //break;
            case TurretType.Sprt:
                return (calculateCostSprt() / 4);
            //break;
            case TurretType.None:
                return 25;
            default:
                return 0;
        }
    }
    #region Not in use
    private int calculateCostRange()
    {
        return Mathf.RoundToInt(baseUpgradeRange * Mathf.Pow(ctrlLevel, 0.8f));
    }

    private float calculateRange()
    {
        return targetingRangeBase + Mathf.Pow(sprtLevel, 0.4f);
    }
    #endregion

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent<EnemyCtrl>(out EnemyCtrl enemy))
        {
            if (enemy.enraged)
            {
                turretDamaged = true;
                turretDamagedSmoke.Play();
                enemy.enraged = false;
                enemy.movSpeed /= 2f;
                enemy.target = null;
            }
        }
    }

    public void SetDrawCircleOutline(bool value) { drawCircleOutlineToScreen = value; }
}
