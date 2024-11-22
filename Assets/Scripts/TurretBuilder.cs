using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurretBuilder : MonoBehaviour
{
    public static TurretBuilder main;

    private Turret Turret;
    private LevelManager levelManager;
    public bool canPlaceTurret = false;
    private BoxCollider2D boundsCollider;
    private UIMouseHover uiMouseScript;

    [Header("Parameters")]
    [SerializeField] private int buildCost; // The amoutn of $ required to build a turret.
    [SerializeField] private float timeScaleSlowdownSpeed = .5f;

    [Header("References")]
    [SerializeField] public GameObject towerPrefab;
    [SerializeField] public Button buildButton;
    [SerializeField] public GameObject BoundsGameObj;
    [SerializeField] public SlideMenu SideMenu;

    void Awake()
    {
        if (main == null) main = this;
    }

    void Start()
    {
        levelManager = LevelManager.main;
        boundsCollider = BoundsGameObj.GetComponent<BoxCollider2D>();
        uiMouseScript = BoundsGameObj.GetComponent<UIMouseHover>();
    }

    void Update()
    {
        if(IsMouseOverUI()) { return; }
    
        // Checking if player is clicking in area within bounds and is able to place a turret.
        if (Input.GetMouseButtonDown(0) && canPlaceTurret && WithinBounds())
        {
            // Spawning turret where the mouse is hovering over.
            if (CanBuildTurret() && levelManager.SpendMoney(buildCost))
            {
                // Build Turret 
                //Check if there is a prev selected turret then deselect it.
                DeselectTurretCheck();
                Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var newTurret = Instantiate(towerPrefab, new Vector3(cursorPos.x, cursorPos.y, 0), Quaternion.identity);
                newTurret.GetComponent<Turret>().SelectTurret();
                SideMenu.SetMenu(true);
                DeselectBuildButton();
                uiMouseScript.MouseDown();
            }
            else
            {
                //Logic to be executed when there is not enough money.
            }
        }
        //Checking if player is clicking in an area within bounds and there is a detectable object within the raycast (Player Turret).
        else if (Input.GetMouseButtonDown(0) && DetectObject())
        {
            if (DetectObject().CompareTag("Player"))
            {
                //Check if there is a prev selected turret then deselect it.
                DeselectTurretCheck();
                // This is where the selected current turret is.
                Turret = DetectObject().GetComponent<Turret>();
                Turret.SelectTurret();
                SideMenu.SetMenu(true);
            }
            else
            {
                //Debug.Log("Clicked on something other than turret.");
                DeselectAll();
                SideMenu.SetMenu(false);
            }
        }


        
    }

    //Will raycast on mouse position to check if there exists a tower at that transform.
    private GameObject DetectObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits2d = Physics2D.GetRayIntersectionAll(ray);


        //There are objects that were captured in the raycast.
        if (hits2d.Length > 0)
        {
            var detectedList = hits2d.ToList();

            //Will execute if there exists a game object with the tag "Player" in the hits2d array.
            if (detectedList.Where(x=> x.collider.CompareTag("Player")).ToList().Count > 0) 
            {
                return detectedList.Where(x => x.collider.CompareTag("Player")).ToList().First().collider.gameObject;
            }
            else
            {
                return hits2d[0].collider.gameObject;
            }

        }
        else
        {
           //Debug.Log("hits is empty.");
            DeselectTurretCheck();
            SideMenu.SetMenu(false);
        }
        return null;
    }

    public bool WithinBounds()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits2d = Physics2D.GetRayIntersectionAll(ray);
        foreach (RaycastHit2D hit in hits2d)
        {
            return hit.collider.gameObject.tag == "Bounds";
        }

        return false;
    }

    //Rework toggle to have it only be able to place turret only after the button has been clicked (ie. Grayed out).
    public void BuildMode()
    {
        if (CanBuildTurret() && !canPlaceTurret)
        {
            buildButton.image.color = Color.gray;
            canPlaceTurret = true;
            Time.timeScale = timeScaleSlowdownSpeed;
        }
        else
        {
            buildButton.image.color = Color.white;
            canPlaceTurret = false;
            Time.timeScale = 1f;
        }
        uiMouseScript.MouseDown();
    }

    // Resets all selected mouse elements.
    public void DeselectAll()
    {
        if (levelManager.selectedTurret != null) levelManager.selectedTurret.DeselectTurret();
        DeselectBuildButton();
    }

    private void DeselectBuildButton()
    {
        buildButton.image.color = Color.white;
        canPlaceTurret = false;
        Time.timeScale = 1f;
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void DeselectTurretCheck() { if (levelManager.selectedTurret != null) { levelManager.selectedTurret.DeselectTurret(); } }
    public bool CanBuildTurret() { return levelManager.GetCurrency() >= buildCost; }
}
