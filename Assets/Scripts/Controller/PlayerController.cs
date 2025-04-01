using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    [Header("Movement Particles")] 
    public ParticleSystem[] runningDust;
    public ParticleSystem landingDust;

    [Header("Particle Settings")]
    public float minSpeedForDust = 2f;
    public LayerMask groundLayer;


    [Header("Movement Settings")]
    public float speed = 5f;            // Horizontal Movement speed
    public float jumpForce = 2f;        // Impulse force when jumping
    public float rotationSpeed = 0.15f; // Slerp Factor


    [Header("Ground Check Settings")]
    public float groundCheckDistance = 0.1f; // Distance to check for ground

    [Header("Animator Settings")]
    public Animator animator;

    private Rigidbody rb;
    private Vector2 moveInput;          // Player input
    private bool jumpRequested;         // Jump Flag
    private bool isGrounded;
    private bool wasGrounded;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); // Get movement input from the player
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Use context.started to trigger on key press only once
        if (context.started)
        {
            // Check if the player is grounded at the moment of input
            if (CheckGroundStatus())
            {
                jumpRequested = true;
                isGrounded = true;
                animator.SetTrigger("jump");
                animator.SetBool("grounded", false);
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
        // State changes
        wasGrounded = isGrounded;
        isGrounded = CheckGroundStatus();

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
        if (jumpRequested && isGrounded)
        {
            // Calculate the jump velocity using v = sqrt(2 * g * jumpHeight)
            float jumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * jumpForce);
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }

        // Reset jump request whether or not the jump was performed
        jumpRequested = false;


        // Running Particle Effect
        float horizontalSpeed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;

        // Enable or disable particle emissions based on horizontal speed
        foreach (var dust in runningDust)
        {
            var emission = dust.emission;
            emission.enabled = isGrounded && horizontalSpeed >= minSpeedForDust;
        }

        if (horizontalSpeed > 0)
        {
            animator.SetBool("running", true);
        }
        else
        {
            animator.SetBool("running", false);
        }

        // Landing Particle Effect
        if (!wasGrounded && isGrounded)
        {
            landingDust.Play();
            animator.SetBool("grounded", true);
        }

    }

    // Method to check if the player is grounded using a raycast
    private bool CheckGroundStatus()
    {
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.down * groundCheckDistance, Color.red);
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out _, groundCheckDistance))
        {
            Debug.Log("Grounded");
            return true;
        }
        Debug.Log("Not Grounded");
        return false;
        
    }

}

