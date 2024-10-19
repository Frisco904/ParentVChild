using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TurretSpawner : MonoBehaviour
{

    private GameObject towerObj;
    private Tower Turret;
    private LevelManager levelManager;
    private bool bPlaceTurret = false;
    private BoxCollider2D boundsCollider;
    //public Camera camera;

    [Header("References")]
    [SerializeField] private Button turretButton;
    [SerializeField] private GameObject BoundsGameObj;
    private void Start()
    {
        turretButton.onClick.AddListener(PlaceTurretToggle);
        levelManager = LevelManager.main;
        boundsCollider = BoundsGameObj.GetComponent<BoxCollider2D>();
    }
    void Update()
    {


        if (Input.GetMouseButtonDown(0) && bPlaceTurret && WithinBounds())
            { 
            //Spawning turret where the mouse is hovering over.

            if (levelManager.SpendMoney(BuildManager.main.GetSelectedTower().cost))
            {
                Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                UiTower towerBuild = BuildManager.main.GetSelectedTower();
                towerObj = Instantiate(towerBuild.prefab, new Vector3(cursorPos.x, cursorPos.y, 0), Quaternion.identity);
                //Turret = towerObj.GetComponent<Tower>();
                bPlaceTurret = false;

            }
            else
            {
                Debug.Log("Not enough money to buy tower.");
            }
        }
        else if (Input.GetMouseButtonDown(0) && WithinBounds())
        {
            if (DetectObject().tag == "Player")
            {
                Turret = DetectObject().GetComponent<Tower>();
                Turret.openUpgradeUI();
            }
            
        }

        else
        {

        }

    }

    //Will raycast on mouse position to check if there exists a tower at that transform.
    private GameObject DetectObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits2d = Physics2D.GetRayIntersectionAll(ray);
        if (hits2d.Length > 0)
        {
            Debug.Log("Hits Exist");
            foreach (RaycastHit2D hit in hits2d)
            {
                if (hit.collider.gameObject.tag == "Player")
                { 
                    return hit.collider.gameObject;
                }
            }
            
        }
        return null;
    }

    private bool WithinBounds()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits2d = Physics2D.GetRayIntersectionAll(ray);
        foreach (RaycastHit2D hit in hits2d)
        {
            if(hit.collider.gameObject.tag == "Bounds")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;

    }

    void PlaceTurretToggle()
    {
        bPlaceTurret = true;
    }
}
