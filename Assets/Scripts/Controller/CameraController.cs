using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    // Backwards Camera 
    [Header("Player Settings")]
    public Transform player;            //player  transform
    public float height      = 5f;      //height from player
    public float distance    = 15f;     //distance from player
    public float smoothSpeed = 0.5f;  //smoothness of camera movement
    public float pitchAngle = 25.5f;

    private Vector3 currentVelocity = Vector3.zero; //current velocity of camera

    [Header("Top Down Transition Settings")]
    public Transform topDownCamPoint;
    
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

    [Header("Corner Rotation Settings")] 
    public float cornerRotationTime  = 1.0f;
    private bool isRotatingCorner    = false;
    private float cornerTimer        = 0f;
    private float cornerStartAngle   = 0f;
    private float cornerTargetAngle  = 0f;

    private float currentAngleY      = 0f;
 


    private void Start()
    {
        currentAngleY = transform.eulerAngles.y;
    }

    private void LateUpdate() {
        if (isTransitioning)
        {
            // Interpolate between start and target values
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionDuration);

            transform.position = Vector3.Lerp(transitonStartPos, transitionTargetPos, t);
            transform.rotation = Quaternion.Lerp(transitionStartRot, transitionTargetRot, t);

            if (t >= 1f)
            {
                isTransitioning = false;
                currentAngleY = transform.eulerAngles.y;
            }
        }
        else if (isRotatingCorner)
        {
                // SMOOTH CORNER ROTATION
            cornerTimer += Time.deltaTime;
            float t = Mathf.Clamp01(cornerTimer / cornerRotationTime);

                // INTERPOLATE ANGLE
            currentAngleY = Mathf.LerpAngle(cornerStartAngle, cornerTargetAngle, t);

                // Build a rotation around Y only
            Quaternion cornerRot = Quaternion.Euler(pitchAngle, currentAngleY, 0f);

            // Now position the camera behind the player using this rotation
            // We'll do a simple approach: the camera is offsetZ behind the player
            // along the rotated negative Z direction, plus some height

            Vector3 offset        = new Vector3(0f, height, -distance);
            Vector3 rotatedOffset = cornerRot * offset;   
            Vector3 desiredPos    = player.position + rotatedOffset;

                // INTERPOLATE POSITION
            transform.position = Vector3.Lerp(transform.position, desiredPos, t);
            transform.rotation = cornerRot;

            // If we've reached 100%, corner rotation is complete
            if (t >= 1f)
                isRotatingCorner = false;
               
            

        }
        else if (isTopDown)
        {
            Vector3 desiredPosition = new Vector3(
                topDownCamPoint.position.x,
                topDownCamPoint.position.y,
                player.position.z + topDownCamPoint.position.z
            );
                // INTERPOLATE POSITION AND ROTATION
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, topDownCamPoint.rotation,Time.deltaTime);
        }
        else
        {
            float normalizedAngle = Mathf.Repeat(currentAngleY, 360f);

            // We'll keep a threshold for picking which quadrant
            const float threshold = 45f;

            // Build the camera's rotation around Y
            Quaternion followRot = Quaternion.Euler(pitchAngle, currentAngleY, 0f);
            transform.rotation = followRot;

            // We'll start with the current camera position as a baseline
            Vector3 desiredPos = transform.position;

            float playerX = player.position.x;
            float playerZ = player.position.z;

            // Are we near 0° or 180°? => lock Z
            // or near 90° or 270°? => lock X
            normalizedAngle = (normalizedAngle + 360f) % 360f;

            // We'll define small helpers:
            bool near0 = (normalizedAngle >= 0f && normalizedAngle < threshold);
            bool near180 = (normalizedAngle > 180f - threshold && normalizedAngle < 180f + threshold);
            bool near360 = (normalizedAngle > 360f - threshold && normalizedAngle <= 360f);
            bool near90 = (normalizedAngle > 90f - threshold && normalizedAngle < 90f + threshold);
            bool near270 = (normalizedAngle > 270f - threshold && normalizedAngle < 270f + threshold);

            // We'll define an offset behind the player in local -Z
            // then rotate it to the camera's angle, so we remain that "distance" behind
            Vector3 localOffset = new Vector3(0f, height, -distance);
            Vector3 rotatedOffset = followRot * localOffset;

            // Default to just applying the rotated offset
            desiredPos = player.position + rotatedOffset;

            // But if you want to literally "ignore" sideways movement, we can clamp
            // one axis to the camera's current position. For instance:
            if (near0 || near180 || near360)
            {
                // That means we only want to track the player's Z,
                // so we preserve the camera's existing X
                desiredPos.x = transform.position.x;
            }
            else if (near90 || near270)
            {
                // That means we only want to track the player's X,
                // so we preserve the camera's existing Z
                desiredPos.z = transform.position.z;
            }
            // If the angle is in-between, the camera is mid-rotation or something,
            // but if you only ever rotate exactly ±90°, you'll likely never see it

            // Now we SmoothDamp to the desired position
            Vector3 smoothed = Vector3.SmoothDamp(
                transform.position,
                desiredPos,
                ref currentVelocity,
                smoothSpeed
            );
            transform.position = smoothed;
        }
    }

    /// <summary>
    /// Call this method to transition the camera to top-down mode
    /// </summary>
    public void TransitionToTopDown()
    {
        if (isTransitioning || isTopDown)
            return;
        
        transitonStartPos    = transform.position;
        transitionStartRot   = transform.rotation;

        transitionTargetPos  = new Vector3(
            topDownCamPoint.position.x,
            topDownCamPoint.position.y,
            player.position.z + topDownCamPoint.position.z
        );
        transitionTargetRot  = topDownCamPoint.rotation;
      
        transitionTimer      = 0f;
        isTransitioning      = true;
        isTopDown            = true;
    }

    /// <summary>
    /// Call this method to transition the camera back to player follow mode
    /// </summary>
    public void TransitionToFollow()
    {
        if (isTransitioning || !isTopDown)
            return;
       
        transitonStartPos   = transform.position;
        transitionStartRot  = transform.rotation;

        transitionTargetPos = new Vector3(
            followCamPoint.position.x,
            followCamPoint.position.y,
            player.position.z + followCamPoint.position.z
        );
        transitionTargetRot = followCamPoint.rotation;
      
    
        transitionTimer     = 0f;
        isTransitioning     = true;
        isTopDown           = false;
    }
    /// <summary>
    /// Call this method to rotate the camera around an angle centred on the player
    /// </summary>
    /// <param name="angle"></param>
    public void RotateCorner(float angle)
    {
        if (isTransitioning || isRotatingCorner)
            return;

        cornerStartAngle  = currentAngleY;
        cornerTargetAngle = currentAngleY + angle;
        cornerTimer       = 0f;
        isRotatingCorner  = true;
    }
}
