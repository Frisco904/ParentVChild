using UnityEngine;

public class UIMouseHover : MonoBehaviour
{
    [SerializeField] private Texture2D onHoverCursorInBounds;
    [SerializeField] private Vector2 hoverInBoundsCursorHotspot;

    [SerializeField] private Texture2D onHoverCursorOutOfBounds;
    [SerializeField] private Vector2 hoverOutBoundsCursorHotspot;

    private TurretBuilder turretBuilder;


    // Start is called before the first frame update
    void Start()
    {
        turretBuilder = FindAnyObjectByType<TurretBuilder>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseEnter()
    {
        //When cursor enters the collider area we check to see if we can place a turret, if so we will change the cursor.
        if (turretBuilder.canPlaceTurret)
            Cursor.SetCursor(onHoverCursorInBounds, hoverInBoundsCursorHotspot, CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        //Anytime the cursor exits the collider area we will change the cursor back to the original image.
        if (turretBuilder.canPlaceTurret)
        {

            Cursor.SetCursor(onHoverCursorOutOfBounds, hoverOutBoundsCursorHotspot, CursorMode.Auto);

        }
        else
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

    }


    public void MouseDown()

        //Add logic to change the cursor based on if we can place a turret down or now and update the cursor accordingly.
    {
        
        //If we can place a turret and we are within bounds
        if (turretBuilder.canPlaceTurret && turretBuilder.WithinBounds())
        {
            Cursor.SetCursor(onHoverCursorInBounds, hoverInBoundsCursorHotspot, CursorMode.Auto);
        }
        //If we can place a turret but clicked out of bounds then the cursor stays red
        else if (turretBuilder.canPlaceTurret && !turretBuilder.WithinBounds())
        {
            Cursor.SetCursor(onHoverCursorOutOfBounds, hoverOutBoundsCursorHotspot, CursorMode.Auto);
        }
        else
        {
            //When we have placed a turret.
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        }
    }
}