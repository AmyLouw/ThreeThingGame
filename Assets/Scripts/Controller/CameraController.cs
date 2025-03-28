using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;               // Reference to the player's Transform
    public float offSetZ = 10f;            // Distance from the player on the Z-axis
    public float height = 5f;              // Height from the player
    public float smoothSpeed = 0.125f;     // Smoothness of camera movement

    // New settings for camera rotation and movement along X-axis after the corner
    public Transform cornerTriggerZone;    // Reference to the trigger zone where the camera rotates
    public float rotationSpeed = 1f;       // Speed at which the camera rotates
    public float moveSpeed = 5f;           // Speed at which the camera moves along the X-axis after the corner
    //public bool playerDetected = false;    // Flag to indicate if the player is detected

    private Vector3 currentVelocity = Vector3.zero;  // Current velocity of the camera (for SmoothDamp)
    public bool isAtCorner = false;        // Flag to determine when the player reaches the corner
    private Quaternion targetRotation;      // Target rotation when transitioning around the corner

    void Update()
    {
        if (!isAtCorner)
        {
            // Follow the player along the Z-axis before reaching the corner
            FollowPlayerAlongZ();
        }
        else
        {
            FollowPlayerAlongX();
        }
    }

    // Smoothly follows the player along the Z-axis (existing behavior)
    private void FollowPlayerAlongZ()
    {
        // Desired position: Follow the player along the Z-axis and maintain the height
        Vector3 desiredPosition = new Vector3(transform.position.x, height, player.position.z + offSetZ);

        // Smoothly move the camera to the desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);
    }

    private void FollowPlayerAlongX()
    {
        // Desired position: Follow the player along the X-axis and maintain the height
        Vector3 desiredPosition = new Vector3(player.position.x, height, transform.position.z);
        // Smoothly move the camera to the desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);
    }

    // Smoothly rotates the camera and moves along the X-axis after the corner
    public IEnumerator SmoothCameraTransition()
    {
        // Rotate the camera to face along the X-axis
        targetRotation = Quaternion.Euler(0, 90, 0); // 90-degree rotation to face the X-axis
        float timeElapsed = 0f;

        // Smoothly rotate the camera towards the target rotation
        while (timeElapsed < rotationSpeed)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, timeElapsed / rotationSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the camera finishes at the target rotation
        transform.rotation = targetRotation;
    }

    // Move the camera along the X-axis after the corner
    private void MoveCameraAlongX()
    {
        // Move the camera along the X-axis at the specified speed
        float moveAmount = moveSpeed * Time.deltaTime;

        // Update the camera position along the X-axis after the corner
        transform.position = new Vector3(transform.position.x + moveAmount, transform.position.y, transform.position.z);
    }

}
