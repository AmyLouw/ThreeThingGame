using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Backwards Camera 
    public Transform player;            //player  transform
    public float offSetZ = 10f;         //distance from player
    public float height = 5f;           //height from player
    public float smoothSpeed = 0.125f;  //smoothness of camera movement

    private Vector3 currentVelocity = Vector3.zero; //current velocity of camera

    // Top Down Camera
    [Header("Top Down Transition Settings")]
    public Transform topDownCamPoint;
    
    // Follow Camera 
    [Header("Follow Transition Settings")]
    public Transform followCamPoint;

    [Header("Transition Duration")]
    public float transitionDuration = 1f;


    private bool isTopDown          = false;
    private bool isTransitioning    = false;
    private float transitionTimer   = 0f;

    // Cached Start and Target Values for Interpolation
    private Vector3 transitonStartPos;
    private Quaternion transitionStartRot;
    private Vector3 transitionTargetPos;
    private Quaternion transitionTargetRot;

    // Internal Flags

    private void LateUpdate() {
        if (isTransitioning) {
            // Interpolate between start and target values
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionDuration);
            transform.position = Vector3.Lerp(transitonStartPos, transitionTargetPos, t);
            transform.rotation = Quaternion.Lerp(transitionStartRot, transitionTargetRot, t);

            if (t >= 1f)
                isTransitioning = false;
        }
        else if (isTopDown)
        {
            Vector3 desiredPosition = new Vector3(
                topDownCamPoint.position.x,
                topDownCamPoint.position.y,
                player.position.z + topDownCamPoint.position.z
            );
            // Instead of direct assignment, interpolate the position.
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime);
            // Interpolate the rotation as well.
            transform.rotation = Quaternion.Lerp(transform.rotation, topDownCamPoint.rotation,Time.deltaTime);
        }
        else {
            // When not transitioning or in Top Down Mode, follow the player
            //get  desired position of camera
            Vector3 desiredPosition = new Vector3(transform.position.x, height, player.position.z + offSetZ);
            //smoothly move camera to desired position
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);
        }
    }

    /// <summary>
    /// Call this method to transition the camera to top down mode
    /// </summary>
    public void TransitionToTopDown()
    {
        if (isTransitioning || isTopDown)
            return;
        // Cache Start and Target Values
        transitonStartPos = transform.position;
        transitionStartRot = transform.rotation;
        transitionTargetPos = new Vector3(
            topDownCamPoint.position.x,
            topDownCamPoint.position.y,
            player.position.z + topDownCamPoint.position.z
        );
        transitionTargetRot = topDownCamPoint.rotation;
        // Reset Timer
        transitionTimer = 0f;
        // Set Flags
        isTransitioning = true;
        isTopDown = true;
    }

    /// <summary>
    /// Call this method to transition the camera back to player follow mode
    /// </summary>
    public void TransitionToFollow()
    {
        if (isTransitioning || !isTopDown)
            return;
        // Cache Start and Target Values
        transitonStartPos = transform.position;
        transitionStartRot = transform.rotation;
        transitionTargetPos = player.position + followCamPoint.position;
        transitionTargetRot = followCamPoint.rotation;
      
        // Reset Timer
        transitionTimer = 0f;
        // Set Flags
        isTransitioning = true;
        isTopDown = false;
    }
}
