using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTrail : MonoBehaviour
{
    private Camera mainCameras;

    void Start()
    {
        // Get the main camera
        mainCameras = Camera.main;
    }

    void Update()
    {
        // Get the mouse position in screen space
        Vector3 mousePosition = Input.mousePosition;

        // Convert the screen position to world position
        mousePosition.z = 10f; // Set the distance from the camera to the cursor in the z-axis
        Vector3 worldPosition = mainCameras.ScreenToWorldPoint(mousePosition);

        // Move the GameObject to the cursor's position
        transform.position = worldPosition;
    }
}
