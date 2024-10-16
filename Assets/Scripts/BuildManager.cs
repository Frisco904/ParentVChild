using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    //This script is for building torrents
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private UiTower[] towers;

    private int SelectedTower = 0;

    private void Awake()
    {
        main = this;
    }

    //This is for selecting a turret.
    public UiTower GetSelectedTower()
    {
        return towers[SelectedTower];
    }

    //This is for the selected turrent and will automatically get the cost.
    public void setSelectedTurrent(int _selectedTurrent)
    {
        SelectedTower = _selectedTurrent;
    }
}
