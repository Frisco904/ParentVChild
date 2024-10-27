using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiMouseHover : MonoBehaviour
{
    [SerializeField] private Vector2 hoverCursorHotspot;
    [SerializeField] private Texture2D onHoverCursor;

    [Header("Reference")]
    [SerializeField] private TurretSpawner turretSpawner;


    // Start is called before the first frame update
    void Start()
    {
        turretSpawner = FindAnyObjectByType<TurretSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseEnter()
    {
        //Debug.Log("Mouse is hovering over the Bounds.");
        if(turretSpawner.GetCanPlaceTurret())
            Cursor.SetCursor(onHoverCursor, hoverCursorHotspot, CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        //Debug.Log("Mouse is Not hovering over the Bounds.");
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

    }
    public void MouseDown()
    {
        if (turretSpawner.GetCanPlaceTurret())
        {
            Cursor.SetCursor(onHoverCursor, hoverCursorHotspot, CursorMode.Auto);
            turretSpawner.SetCanPlaceTurret(false);

        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            //turretSpawner.SetCanPlaceTurret(true);
        }
            
    }

    
}
