using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpawner : MonoBehaviour
{

    public GameObject towerObj;
    private GameObject tower;
    public Tower turrent;

    void Update()
    {

        if (Input.GetMouseButtonDown(0)) {

            //Checks if pointer is hovering on a turrent
            if ((UIManager.main.IsHoveringUI())) return;
            //This function is for torrent drop or torrent build
            //Debug.Log("Build Here: " + name);
            if (towerObj != null)
            {
                turrent.openUpgradeUI();
                return;
            }
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            UiTower towerBuild = BuildManager.main.GetSelectedTower();
            if (towerBuild.cost > LevelManager.main.currency)
            {
                Debug.Log("Insufficient Currency");
                return;
            }

            //The Currency spending system
            LevelManager.main.SpendMoney(towerBuild.cost);


            //Checks if the turrent is ready to drop with the right price
            //turrent = towerObj.GetComponent<Tower>();
            tower = Instantiate(towerBuild.prefab, new Vector3(cursorPos.x,cursorPos.y, 0), Quaternion.identity);

        }
            
    }
}
