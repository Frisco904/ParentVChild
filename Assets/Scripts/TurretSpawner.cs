using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpawner : MonoBehaviour
{
    private GameObject tower;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            UiTower towerBuild = BuildManager.main.GetSelectedTower();
            tower = Instantiate(towerBuild.prefab, new Vector3(cursorPos.x,cursorPos.y, 0), Quaternion.identity);

        }
            
    }
}
