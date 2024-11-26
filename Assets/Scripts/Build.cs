using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Build : MonoBehaviour
{
    private TurretBuilder builder;
    private Button button;
    void Start()
    {
        builder = FindAnyObjectByType<TurretBuilder>();
        button = gameObject.GetComponent<Button>();
    }
    
    void Update()
    {
        if (builder != null && builder.CanBuildTurret()) 
        {
            button.interactable = true;
        } else
        { 
            button.interactable = false;
        }
    }

    public void ToggleBuildMode()
    {
        TurretBuilder.main.BuildMode();
    }
}
