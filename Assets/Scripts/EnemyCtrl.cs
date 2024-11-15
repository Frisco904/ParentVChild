using Unity.VisualScripting;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{

    [Header("Attributes")]
    //[SerializeField] private float lockPost = 0;
    [SerializeField] private float movSpeed = 2f;
    [SerializeField] private float enemyStunTimer = 3;
    [SerializeField] private float knockbackAmount = 100000000;
    [SerializeField] private float currentFull;
    [SerializeField] private float maxFull;
    [SerializeField] private float fillAmount;
    [SerializeField] private int currencyWorth = 50;
    [SerializeField] private float candyproximity = .1f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] Sprite[] kidImg;
    [SerializeField] SpriteRenderer kidRenderer;

    [Header("AI Pathing Attributes")]
    [SerializeField] private float horizontalVariation = 8f;
    [SerializeField] private float targetingRange = 15f;
    [SerializeField] private LayerMask candyMask;
    [SerializeField] private LayerMask pathingMask;

    private WaveSpawnEnemies spawnEnemies;
    private SpawnPoints spawnPoint;
    private Transform target;
    private int pathIndex = 0;
    private bool frozen = false;
    private float timer = -1;
    private bool isDestroyed = false;
    private Transform[] path;
    int randomKidImg;

    

    private void Awake()
    {
        
        //Target is set to candy by default for any enemy that is spawned outside of using the AiPathing.
        target = LevelManager.main.CandyPile.transform;

    }

    private void Start()
    {
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

        // When the candy pile is destroyed we will get the RigidBody2D component of the enemy and restrain it to its current position (stop it from moving).
        if (LevelManager.main.CandyPile.IsDestroyed()) { gameObject.GetComponent<Rigidbody2D>().MovePosition(gameObject.transform.position); }
        //Checking that Candy Pile is valid.
        if (LevelManager.main.CandyPile)
        {
            CandyInRange();
            DetectObject();

            //Makes the radius smaller if the target is the candypile, to make the enemy collide with the candy pile to trigger the appropriate logic.
            if (target == LevelManager.main.CandyPile.transform) targetingRange = candyproximity;
            
            //Check if target is within range, if so move on to next target in list.
            if (Vector2.Distance(target.position, transform.position) <= targetingRange)
            {
                if(target.CompareTag("TrackingPoint"))
                {
                    //Once the Enemy reaches the point we will destroy the game object.
                    target.GetComponent<TrackingPoints>().DestroyGameObject();
                }
                pathIndex++;
                if (pathIndex == path.Length)
                { 
                    return;
                }
                else
                { 
                    target = path[pathIndex];

                    //We only want to adjust the values if its not the candy pile
                    if (target != LevelManager.main.CandyPile.transform)
                    {

                        //Creating game object to hold the transform information for where the AI will be moving towards. Adding components to the game object
                        GameObject trackingPoint = new GameObject("TrackingPoint");
                        trackingPoint.AddComponent<BoxCollider2D>().size = new Vector2(5,5);
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
                    else
                    {

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

    private void FixedUpdate()
    {
        if (LevelManager.main.CandyPile)
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

    public void TakeDamage(int dmg)
    {
        currentFull += dmg;
        gameObject.GetComponent<Rigidbody2D>().AddForce(transform.forward * knockbackAmount);
        //Freeze();
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
    /*
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }*/
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
        xValueDirection = UnityEngine.Random.Range(xValueDirection - horizontalVariation, xValueDirection + horizontalVariation);
        return xValueDirection;
    }

    void GenerateRandomKidImage()
    {
        randomKidImg = UnityEngine.Random.Range(1, kidImg.Length + 1);
        kidRenderer.sprite = kidImg[randomKidImg - 1];
    }

}
