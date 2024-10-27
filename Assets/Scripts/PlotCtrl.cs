using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotCtrl : MonoBehaviour
{
    //This script is for the mouse or check if available to build
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    public GameObject towerObj;
    public Tower turrent;
    private Color startColor;


    private void Start()
    {
        startColor = sr.color;
    }
    private void OnMouseEnter()
    {
        sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    private void OnMouseDown()
    {
        //Checks if pointer is hovering on a turret
        if ((UIManager.main.IsHoveringUI())) return;
    
        //This function is for torrent drop or torrent build
        Debug.Log("Build Here: " + name);
        if (towerObj != null) 
        {
            turrent.openUpgradeUI();
            return; 
        }

        UiTower towerBuild = BuildManager.main.GetSelectedTower();
        if (towerBuild.cost > LevelManager.main.GetCurrency())
        {
            Debug.Log("Insufficient Currency");
            return;
        }
        //The Currency spending system
        LevelManager.main.SpendMoney(towerBuild.cost);

        towerObj = Instantiate(towerBuild.prefab, transform.position, Quaternion.identity);
        //Checks if the turrent is ready to drop with the right price
        turrent = towerObj.GetComponent<Tower>();
    }
}
