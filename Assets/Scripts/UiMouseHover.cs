using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiMouseHover : MonoBehaviour
{
    [SerializeField] private Vector2 hoverCursorHotspot;
    [SerializeField] private Texture2D onHoverCursor;


    private TurretSpawner turretSpawner;


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
        //When cursor enters the collider area we check to see if we can place a turret, if so we will change the cursor.
        if(turretSpawner.GetCanPlaceTurret())
            Cursor.SetCursor(onHoverCursor, hoverCursorHotspot, CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        //Anytime the cursor exits the collider area we will change the cursor back to the original image.
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

    }
    public void MouseDown()
    {
        //When we place the turret we change the cursor back to the default image, indicating that we can no longer place turrets until another turret is purchased.
        if (turretSpawner.GetCanPlaceTurret()) { Cursor.SetCursor(onHoverCursor, hoverCursorHotspot, CursorMode.Auto); } else { Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); }
    }

    
}
