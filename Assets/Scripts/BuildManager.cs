using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    //This script is for building torrents
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private GameObject[] buildPrefabs;

    private int SelectedTower = 0;

    private void Awake()
    {
        main = this;
    }

    public GameObject GetSelectedTower()
    {
        return buildPrefabs[SelectedTower];
    }
}
