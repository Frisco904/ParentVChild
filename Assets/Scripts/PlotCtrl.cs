using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotCtrl : MonoBehaviour
{
    //This script is for the mouse or check if available to build
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    private GameObject tower;
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
        //This function is for torrent drop or torrent build
        Debug.Log("Build Here: " + name);
        if (tower != null) return;

        UiTower towerBuild = BuildManager.main.GetSelectedTower();
        tower = Instantiate(towerBuild.prefab, transform.position, Quaternion.identity);
    }
}
