using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{

    [Header("Attributes")]
    //[SerializeField] private float lockPost = 0;
    [SerializeField] public float movSpeed = 2f;
    [SerializeField] private float enemyStunTimer = 3;
    [SerializeField] private float knockbackAmount = 100000000;
    [SerializeField] private float currentFull;
    [SerializeField] private float maxFull;
    [SerializeField] private float fillAmount;
    [SerializeField] private int currencyWorth = 50;
    [SerializeField] private float candyproximity = .1f;
    [SerializeField] private float redTickDmgLength = .03f;
    [SerializeField] public bool followLeaderEnemy = false;
    [SerializeField] public bool enraged = false;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] public Sprite[] kidSprite;
    [SerializeField] public SpriteRenderer kidRenderer;
    [SerializeField] public Sprite enragedSprite;
    [SerializeField] public Animator anim;

    [Header("AI Pathing Attributes")]
    [SerializeField] private float horizontalVariation = 8f;
    [SerializeField] private float targetingRange = 15f;
    [SerializeField] private LayerMask candyMask;
    [SerializeField] private LayerMask pathingMask;


    private WaveSpawnEnemies spawnEnemies;
    private SpawnPoints spawnPoint;
    public Transform target;
    private int pathIndex = 0;
    private float ogSpeed;
    private bool frozen = false;
    private float timer = -1;
    private bool isDestroyed = false;
    private Transform[] path;
    int randomKidImg;

    

    private void Awake()
    {
        
        //Target is set to candy by default for any enemy that is spawned outside of using the AiPathing.
        target = LevelManager.main.candyPile.transform;

    }

    private void Start()
    {
        ogSpeed = movSpeed;
        spawnPoint = gameObject.GetComponentInParent<WaveSpawnEnemies>().GetSpawnPoint();

        //Switch that sets the relevant path/target for the enemy based on which spawner they spawned at.
        switch (spawnPoint)
        {
            case SpawnPoints.SpawnPoint1:
                target = LevelManager.main.path1[0];
                path = LevelManager.main.path1.ToArray();
                break;
            case SpawnPoints.SpawnPoint2:
                target = LevelManager.main.path2[0];
                path = LevelManager.main.path2.ToArray();
                break;
            case SpawnPoints.SpawnPoint3:
                target = LevelManager.main.path3[0];
                path = LevelManager.main.path3.ToArray();
                break;
        }

        GenerateRandomKidImage();
    }

    private void Update()
    {
        if (followLeaderEnemy) return;
        if (target == null) target = path[pathIndex]; 
        // Set enraged animation state.
        if (enraged)
        {
            if (anim != null) anim.SetBool("Enraged", true);
            kidRenderer.sprite = enragedSprite;
            return;
        } else {
            if (anim != null) anim.SetBool("Enraged", false);
            kidRenderer.sprite = kidSprite[0];
        }

        // When the candy pile is destroyed we will get the RigidBody2D component of the enemy and restrain it to its current position (stop it from moving).
        if (LevelManager.main.candyPile.IsDestroyed()) { gameObject.GetComponent<Rigidbody2D>().MovePosition(gameObject.transform.position); }
        //Checking that Candy Pile is valid.
        if (LevelManager.main.candyPile)
        {
            CandyInRange();
            DetectObject();

            //Makes the radius smaller if the target is the candypile, to make the enemy collide with the candy pile to trigger the appropriate logic.
            if (target == LevelManager.main.candyPile.transform) targetingRange = candyproximity;
            
            //Check if target is within range, if so move on to next target in list.
            if (Vector2.Distance(target.position, transform.position) <= targetingRange)
            {
                DestroyTrackingPoint();
                pathIndex++;
                if (pathIndex == path.Length)
                {
                    return;
                }
                else
                {
                    target = path[pathIndex];

                    //We only want to adjust the values if its not the candy pile
                    if (target != LevelManager.main.candyPile.transform)
                    {

                        //Creating game object to hold the transform information for where the AI will be moving towards. Adding components to the game object
                        GameObject trackingPoint = new GameObject("TrackingPoint");
                        trackingPoint.AddComponent<BoxCollider2D>().size = new Vector2(3, 3); //TODO Should be public var so its adjustable
                        trackingPoint.GetComponent<BoxCollider2D>().isTrigger = true;
                        trackingPoint.AddComponent<TrackingPoints>();

                        //Setting tag and parent to created game object
                        trackingPoint.tag = "TrackingPoint";
                        trackingPoint.transform.SetParent(target);

                        float xPos = AdjustPathing(target.transform.position.x);
                        float yPos = target.transform.position.y;
                        float zPos = target.transform.position.z;

                        trackingPoint.transform.position = new Vector3(xPos, yPos, zPos);
                        target = trackingPoint.transform;
                    }
                }

            }

            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = -1;
                    frozen = false;
                }
            }
        }
    }

    public void DestroyTrackingPoint()
    {
        if (target.CompareTag("TrackingPoint"))
        {
            //Once the Enemy reaches the point we will destroy the game object.
            target.GetComponent<TrackingPoints>().DestroyGameObject();
        }
    }

    private void FixedUpdate()
    {
        if (LevelManager.main.candyPile)
        {
            if (frozen) return;

            //this.transform.position = Vector2.Lerp(target.position, transform.position, .5f).normalized;
            Vector2 direction = (target.position - transform.position).normalized;
            
            //Create a vector and add the y- points to shift where the move and add variability to the path.

            rb.velocity = direction * movSpeed;

        }
    }
    private void CandyInRange()
    {
        //Checks to see if candy is within range of target, if so the target is set to the candy pile.
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, candyMask);
        if (hits.Length > 0)
        {
            //We are setting the target to be the Candy Pile.
            target = hits[0].transform;
            //We are reducing the circumfrence of the area that the enemy has to be within to be smaller in order to get the Enemy to collide with the candy pile.
            targetingRange = candyproximity;

        }

    }

    public void TakeDamage(float dmg)
    {
        currentFull += dmg;
        gameObject.GetComponent<Rigidbody2D>().AddForce(transform.forward * knockbackAmount);
        //Freeze();
        StartCoroutine(FlashRed());
        if(currentFull >= maxFull && !isDestroyed)
        {
            isDestroyed = true;
            Eliminate();
        }
    }

    public void Freeze()
    {
        frozen = true;
        timer = enemyStunTimer;
    }

    public void Eliminate()
    {
        //This gains money when enemy are killed by the turrets
        LevelManager.main.GainMoney(currencyWorth);
        LevelManager.main.DecrementEnemiesLeft();
        LevelManager.main.AddScore(1);
        gameObject.GetComponentInParent<WaveSpawnEnemies>().DecrementEnemiesAlive();
        Destroy(gameObject);
    }
    public void CandyReached()
    {
        LevelManager.main.DecrementEnemiesLeft();
        gameObject.GetComponentInParent<WaveSpawnEnemies>().DecrementEnemiesAlive();
        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {

    }
    private void DetectObject()
    {
        if (target)
        {
            float distToTarget = Vector3.Distance(transform.position, target.transform.position);
            Vector3 dirToTarget = transform.position - target.transform.position;
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(transform.position, dirToTarget, distToTarget, pathingMask);

            if (hits.Length > 0)
            {
                //Beginning to create logic for pathing of enemies to avoid getting stuck on turret if in its path.
                //Debug.Log("The target is " + target.ToString() + " and the gameObject is " + hits[0].collider.gameObject);
                //Debug.Log(hits[0].collider.gameObject);
            }
        }
    }

    private float AdjustPathing(float xValueDirection)
    {
        xValueDirection = Random.Range(xValueDirection - horizontalVariation, xValueDirection + horizontalVariation);
        return xValueDirection;
    }

    void GenerateRandomKidImage()
    {
        // randomKidImg = Random.Range(1, kidImg.Length + 1);
        // kidRenderer.sprite = kidImg[randomKidImg - 1];
    }

    public IEnumerator FlashRed()
    {
        //Debug.Log("Flash red logic is called.");
        kidRenderer.color = Color.red;
        yield return new WaitForSeconds(redTickDmgLength);
        kidRenderer.color = Color.white;

    }

    #region Enemy effect
    public void ApplySlow(float slowMultiplier, float duration)
    {
        Debug.Log("Enemy slowed!");
        movSpeed *= slowMultiplier; // Reduce speed
        CancelInvoke(nameof(RemoveSlow)); // Prevent overlapping calls
        Invoke(nameof(RemoveSlow), duration); // Restore speed after duration
    }
    private void RemoveSlow()
    {
        movSpeed = ogSpeed;
        Debug.Log(ogSpeed);
        Debug.Log(movSpeed);
        Debug.Log("Enemy speed restored.");
    }
    public void ApplyParalysis(float duration)
    {
        Debug.Log("Enemy paralyzed!");
        movSpeed = 0f; // Stop the enemy
        CancelInvoke(nameof(RemoveParalysis)); // Prevent overlapping calls
        Invoke(nameof(RemoveParalysis), duration); // Restore speed after duration
    }
    private void RemoveParalysis()
    {
        movSpeed = ogSpeed;
        Debug.Log(ogSpeed);
        Debug.Log(movSpeed);
        Debug.Log("Enemy recovered from paralysis.");
    }
    #endregion
}
