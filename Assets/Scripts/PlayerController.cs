using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    [Header("Movement Settings")]
    public float speed = 5f;            // Horizontal Movement speed
    public float jumpForce = 2f;        // Impulse force when jumping
    public float rotationSpeed = 0.15f; // Slerp Factor


    [Header("Ground Check Settings")]
    public float groundCheckDistance = 1.1f; // Distance to check for ground

    private Rigidbody rb;
    private Vector2 moveInput;          // Player input
    private bool jumpRequested;         // Jump Flag

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); // Get movement input from the player
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
        // Use context.started to trigger on key press only once
        if (context.started)
        {
            // Check if the player is grounded at the moment of input
            if (CheckGroundStatus())
            {
                jumpRequested = true;
            }
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        // Calculate horizontal movement based on input
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 targetVelocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);
        rb.velocity = targetVelocity;

        // Smoothly rotate the player to face the direction of movement if input is significant
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
            rb.MoveRotation(newRotation);
        }

        // Process jump if requested
        if (jumpRequested && CheckGroundStatus())
        {
            // Calculate the jump velocity using v = sqrt(2 * g * jumpHeight)
            float jumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * jumpForce);
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }

        // Reset jump request whether or not the jump was performed
        jumpRequested = false;

    }
    // Method to check if the player is grounded using a raycast
    private bool CheckGroundStatus()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
        {
            return true;
        }
        else
        {
            return false;
        }
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);
    }

}

