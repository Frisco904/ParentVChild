using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HUDLevelProgress : MonoBehaviour
{

    [SerializeField] private Slider slider;
    [SerializeField] private int sliderValue;
    [SerializeField] private int curretKidsSaved;


    // Update is called once per frame
    void Update()
    { 
        slider.value = (float)(LevelManager.main.GetMaxEnemiesLeft() - LevelManager.main.GetEnemiesLeft()) / (LevelManager.main.GetMaxEnemiesLeft());
    }

    //public void IncrementSavedKids

}
