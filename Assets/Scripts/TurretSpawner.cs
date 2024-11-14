using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurretSpawner : MonoBehaviour
{

    private GameObject towerObj;
    private Tower Turret;
    private LevelManager levelManager;
    private bool bPlaceTurret = false;
    private BoxCollider2D boundsCollider;
    private UiMouseHover uiMouseScript;

    [Header("References")]
    [SerializeField] private Button turretButton;
    [SerializeField] private GameObject BoundsGameObj;
    [SerializeField] private float timeScaleSlowdownSpeed = .5f;

    private void Start()
    { 
        turretButton.onClick.AddListener(ClickTurretButton);
        levelManager = LevelManager.main;
        boundsCollider = BoundsGameObj.GetComponent<BoxCollider2D>();
        uiMouseScript = BoundsGameObj.GetComponent<UiMouseHover>();
    }
    void Update()
    {
        //Checking if player is clicking in area within bounds and is able to place a turret.
        if (Input.GetMouseButtonDown(0) && bPlaceTurret && WithinBounds())
            { 
            //Spawning turret where the mouse is hovering over.
            if (CanBuyTurret() && levelManager.SpendMoney(BuildManager.main.GetSelectedTower().cost))
            {
                Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                UiTower towerBuild = BuildManager.main.GetSelectedTower();
                towerObj = Instantiate(towerBuild.prefab, new Vector3(cursorPos.x, cursorPos.y, 0), Quaternion.identity);

                UnclickTurretButton();
                uiMouseScript.MouseDown();
            }
            else
            {
                //Logic to be executed when there is not enough money.
            }
        }
        //Checking if player is clicking in an area within bounds and there is a detectable object within the raycast (Player Turret).
        else if (Input.GetMouseButtonDown(0) && WithinBounds() && DetectObject())
        {
            if (DetectObject().tag == "Player")
            {
                Turret = DetectObject().GetComponent<Tower>();
                Turret.openUpgradeUI();
            }
            else 
            { 

            }
            
        }

    }

    //Will raycast on mouse position to check if there exists a tower at that transform.
    private GameObject DetectObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits2d = Physics2D.GetRayIntersectionAll(ray);
        if (hits2d.Length > 0)
        {
            foreach (RaycastHit2D hit in hits2d)
            {
                if (hit.collider.gameObject.tag == "Player")
                { 
                    return hit.collider.gameObject;
                }
                else { }
            }
            
        }
        return null;
    }

    public bool WithinBounds()
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

    //Rework toggle to have it only be able to place turret only after the button has been clicked (ie. Grayed out).
    private void ClickTurretButton()
    {
        if (CanBuyTurret() && !bPlaceTurret)
        {
            turretButton.image.color = Color.gray;
            bPlaceTurret = true;
            //Slowing down time when placing a turret down.
            Time.timeScale = timeScaleSlowdownSpeed;
        }
        else
        {
            turretButton.image.color = Color.white;
            bPlaceTurret = false;
        }
        uiMouseScript.MouseDown();
    }
    private void UnclickTurretButton()
    {
        //Setting the time scale back to normal.
        Time.timeScale = 1f;
        turretButton.image.color = Color.white;
        bPlaceTurret = false;
    }
    public bool GetCanPlaceTurret(){ return bPlaceTurret; }
    public bool CanBuyTurret() { return levelManager.GetCurrency() >= BuildManager.main.GetSelectedTower().cost; }

}
