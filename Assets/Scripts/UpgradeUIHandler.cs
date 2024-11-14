using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeUIHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool mouse_over = false;
    [SerializeField] private SpriteRenderer sr;

    private void Update()
    {
        
    }

    //Point is entering or clicking the turrent
    public void OnPointerEnter(PointerEventData pData)
    {
        mouse_over = true; 
        //UIManager.main.setHoveringState(true);
    }

    //Exiting the Upgrade UI on hover
    public void OnPointerExit(PointerEventData pData) 
    {
        //mouse_over = false;
        //UIManager.main.setHoveringState(false);
        //gameObject.SetActive(false);  
    }
}
