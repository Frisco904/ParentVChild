using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager main;

    
    private bool isHoveringUI;

    // Update is called once per frame
    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        
    }

    //For Hovering on the turrent or Menu
    public void setHoveringState(bool state)
    {
        isHoveringUI = state;
    }

    public bool IsHoveringUI()
    {
        return isHoveringUI;
    }
}
    