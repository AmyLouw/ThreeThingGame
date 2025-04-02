using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public float followHeight = 5f;
    public float followDistance = 15f;
    public float smoothSpeed = 0.5f;
    public float topDownHeight = 25f;
    public float topDownSmooth = 2f;
    public float followPitchAngle = 25.5f;
    public float topDownPitchAngle = 85f;

    private Vector3 currentVelocity = Vector3.zero;

    [Header("Follow Transition Settings")]
    public Transform followCamPoint;

    [Header("Transition Duration")]
    public float transitionDuration = 1f;

    private bool isTopDown = false;
    private bool isTransitioning = false;
    private float transitionTimer = 0f;

    private Vector3 transitionStartPos;
    private Quaternion transitionStartRot;
    private Vector3 transitionTargetPos;
    private Quaternion transitionTargetRot;

    [Header("Corner Rotation Settings")]
    public float cornerRotationTime = 1.0f;
    private bool isRotatingCorner = false;
    private float cornerTimer = 0f;
    private float cornerStartAngle = 0f;
    private float cornerTargetAngle = 0f;

    // The camera's current yaw around Y:
    private float currentAngleY = 0f;

    private void Start()
    {
        // Read the camera's initial rotation
        currentAngleY = transform.eulerAngles.y;
    }

    private void LateUpdate()
    {
        if (isTransitioning)
        {
            DoTransitionInterpolation();
        }
        else if (isRotatingCorner)
        {
            DoCornerRotation();
        }
        else if (isTopDown)
        {
            // Only runs AFTER the top-down transition is finished
            DoTopDown();
        }
        else
        {
            DoFollow();
        }
    }

    // --------------------------------------------------------------------
    // FOLLOW MODE
    // --------------------------------------------------------------------
    private void DoFollow()
    {
        Quaternion followRot = Quaternion.Euler(followPitchAngle, currentAngleY, 0f);

        // Negative Z => "behind" the player. If you prefer the camera "in front"
        // looking back, swap to +followDistance and maybe do +180f on the yaw.
        Vector3 localOffset = new Vector3(0f, followHeight, -followDistance);
        Vector3 rotatedOffset = followRot * localOffset;
        Vector3 desiredPos = player.position + rotatedOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref currentVelocity,
            smoothSpeed
        );

        transform.rotation = followRot;
    }

    // --------------------------------------------------------------------
    // TOP-DOWN MODE (runs each frame if isTopDown == true and not transitioning)
    // --------------------------------------------------------------------
    private void DoTopDown()
    {
        // If you want top-down "in front" of the player looking back, you can do:
        // Quaternion topDownRot = Quaternion.Euler(topDownPitchAngle, currentAngleY + 180f, 0f);
        // But typically top-down is overhead at 85° or 90°, ignoring corridor.

        Quaternion topDownRot = Quaternion.Euler(topDownPitchAngle, currentAngleY, 0f);

        Vector3 overhead = new Vector3(0f, topDownHeight, 0f);
        Vector3 desiredPos = Vector3.Lerp(
            transform.position,
            player.position + overhead,
            Time.deltaTime * topDownSmooth
        );

        transform.position = desiredPos;
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            topDownRot,
            Time.deltaTime * topDownSmooth
        );
    }

    // --------------------------------------------------------------------
    // CORNER ROTATION
    // --------------------------------------------------------------------
    private void DoCornerRotation()
    {
        cornerTimer += Time.deltaTime;
        float t = Mathf.Clamp01(cornerTimer / cornerRotationTime);

        // Lerp from cornerStartAngle to cornerTargetAngle
        float newYAngle = Mathf.LerpAngle(cornerStartAngle, cornerTargetAngle, t);
        currentAngleY = newYAngle;

        Quaternion cornerRot = Quaternion.Euler(followPitchAngle, currentAngleY, 0f);

        Vector3 localOffset = new Vector3(0f, followHeight, -followDistance);
        Vector3 rotatedOffset = cornerRot * localOffset;
        Vector3 desiredPos = Vector3.Lerp(transform.position, player.position + rotatedOffset, t);

        transform.position = desiredPos;
        transform.rotation = cornerRot;

        if (t >= 1f)
        {
            isRotatingCorner = false;
        }
    }

    // --------------------------------------------------------------------
    // TRANSITIONS (TOP-DOWN <-> FOLLOW)
    // --------------------------------------------------------------------
    private void DoTransitionInterpolation()
    {
        transitionTimer += Time.deltaTime;
        float t = Mathf.Clamp01(transitionTimer / transitionDuration);

        transform.position = Vector3.Lerp(transitionStartPos, transitionTargetPos, t);
        transform.rotation = Quaternion.Lerp(transitionStartRot, transitionTargetRot, t);

        if (t >= 1f)
        {
            isTransitioning = false;
            // Sync camera angle so no snap
            currentAngleY = transform.eulerAngles.y;
        }
    }

    /// <summary>
    /// Transition to TOP-DOWN:
    /// Here is where we define valid start & target transforms, so no invalid Quaternions occur.
    /// </summary>
    public void TransitionToTopDown()
    {
        if (isTransitioning || isTopDown)
            return;

        // 1) Capture current camera as the start
        transitionStartPos = transform.position;
        transitionStartRot = transform.rotation;

        // 2) Define your final top-down rotation & position
        // If you want the camera "in front" looking back, do +180f on Y,
        // or skip that if you want a normal overhead.
        Quaternion finalTopDownRot = Quaternion.Euler(topDownPitchAngle, currentAngleY, 0f);

        // We'll place it "topDownHeight" above the player
        // If you want a horizontal offset too, define it & rotate it
        Vector3 localTDOffset = new Vector3(0f, topDownHeight, 0f);
        Vector3 finalTDPos = player.position + localTDOffset;
        // If you want it "in front" from above, do something like:
        // Vector3 localTDOffset = new Vector3(0f, topDownHeight, 10f);
        // finalTDPos = player.position + (finalTopDownRot * localTDOffset);

        // 3) Store them
        transitionTargetPos = finalTDPos;
        transitionTargetRot = finalTopDownRot;

        // 4) Start the transition
        transitionTimer = 0f;
        isTransitioning = true;
        isTopDown = true;
    }

    /// <summary>
    /// Transition back to FOLLOW
    /// </summary>
    public void TransitionToFollow()
    {
        if (isTransitioning || !isTopDown)
            return;

        // Start is the camera's current transform
        transitionStartPos = transform.position;
        transitionStartRot = transform.rotation;

        // Build a final follow rotation from the corridor's angle
        Quaternion finalFollowRot = Quaternion.Euler(followPitchAngle, currentAngleY, 0f);

        // Offset behind the player
        Vector3 localOffset = new Vector3(0f, followHeight, -followDistance);
        Vector3 finalFollowPos = player.position + (finalFollowRot * localOffset);

        transitionTargetPos = finalFollowPos;
        transitionTargetRot = finalFollowRot;

        transitionTimer = 0f;
        isTransitioning = true;
        isTopDown = false;
    }

    /// <summary>
    /// Rotate the camera around the player by 'angle' degrees on Y
    /// </summary>
    public void RotateCorner(float angle)
    {
        if (isTransitioning || isRotatingCorner)
            return;

        cornerStartAngle = currentAngleY;
        cornerTargetAngle = currentAngleY + angle;
        cornerTimer = 0f;
        isRotatingCorner = true;
    }
}