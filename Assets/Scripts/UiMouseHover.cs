using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiMouseHover : MonoBehaviour
{
    [SerializeField] private Texture2D onHoverCursorInBounds;
    [SerializeField] private Vector2 hoverInBoundsCursorHotspot;

    [SerializeField] private Texture2D onHoverCursorOutOfBounds;
    [SerializeField] private Vector2 hoverOutBoundsCursorHotspot;

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
        if (turretSpawner.GetCanPlaceTurret())
            Cursor.SetCursor(onHoverCursorInBounds, hoverInBoundsCursorHotspot, CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        //Anytime the cursor exits the collider area we will change the cursor back to the original image.
        if (turretSpawner.GetCanPlaceTurret())
        {

            Cursor.SetCursor(onHoverCursorOutOfBounds, hoverOutBoundsCursorHotspot, CursorMode.Auto);

        }
        else
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

    }


    public void MouseDown()

        //Add logic to change the cursor based on if we can place a turret down or now and update the cursor accordingly.
    {
        
        if (turretSpawner.GetCanPlaceTurret() && turretSpawner.WithinBounds())
        {
            Cursor.SetCursor(onHoverCursorInBounds, hoverInBoundsCursorHotspot, CursorMode.Auto);
        }
        else if (turretSpawner.GetCanPlaceTurret() && !turretSpawner.WithinBounds())
        {
            Cursor.SetCursor(onHoverCursorOutOfBounds, hoverOutBoundsCursorHotspot, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        }
    }
}