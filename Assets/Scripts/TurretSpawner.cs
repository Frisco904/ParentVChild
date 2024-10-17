using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretSpawner : MonoBehaviour
{

    private GameObject towerObj;
    private Tower Turret;
    private LevelManager levelManager;
    private bool bPlaceTurret = false;
    //public Camera camera;

    [Header("References")]
    [SerializeField] private Button turretButton;

    private void Start()
    {
        turretButton.onClick.AddListener(PlaceTurretToggle);
        levelManager = LevelManager.main;
    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && bPlaceTurret)
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
        else if (Input.GetMouseButtonDown(0))
        {
            if (DetectObject() != null)
            {
                Turret = DetectObject().GetComponent<Tower>();
                Turret.openUpgradeUI();
            }
        }

    }

    //Will raycast on mouse position to check if there exists a tower at that transform.
    private GameObject DetectObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hits2d = Physics2D.GetRayIntersection(ray);
        if (hits2d.collider != null)
        {
            if (hits2d.collider.tag == "Player")
            {
                Debug.Log("Hit 2D Collider " + hits2d.collider.tag);
                return hits2d.collider.gameObject;
            }
        }
        return null;
    }
    void PlaceTurretToggle()
    {
        bPlaceTurret = true;
    }
}
