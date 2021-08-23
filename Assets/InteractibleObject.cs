using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleObject : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord, storedPosition;

    Vector3 playerStoredPosition,newPlayerPosition;

    private Camera camera;

    private bool isGrabbing = false;

    public bool islockDemension = false;


    private void Awake()
    {
        this.camera = FindObjectOfType<Camera>();
    }

    void OnMouseDown()
    {
        mZCoord = camera.WorldToScreenPoint(
            gameObject.transform.position).z;

        playerStoredPosition = FindObjectOfType<PlayerController>().transform.position;

        storedPosition = transform.localPosition.y;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();

    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return camera.ScreenToWorldPoint(mousePoint);
    }



    void OnMouseDrag()
    {
        if (!islockDemension)
        {
 
            transform.position = GetMouseAsWorldPoint() + mOffset;
            return;
        }

        newPlayerPosition = FindObjectOfType<PlayerController>().transform.position;

        Vector3 projection = Vector3.Project(newPlayerPosition - playerStoredPosition, transform.up);
        float magnitude = projection.magnitude * Mathf.Sign(Vector3.Dot(projection,transform.up));

        transform.localPosition = new Vector3(0, magnitude + storedPosition, 0);
    }

    private void OnMouseEnter()
    {
        print("Entered" + name);
    }

    private void OnMouseUp()
    {
        if (!islockDemension) return;
    }

}
