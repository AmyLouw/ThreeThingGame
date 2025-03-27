using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    public float jumpHeight = 2f; // Jump height
    public float gravity = -9.8f; // Gravity force
    public float groundCheckDistance = 1f; // Distance to check for ground
    private Vector2 move; // Player's movement input

    private bool isGrounded; // Is the player grounded?
    private Vector3 velocity; // Player's velocity (including jump and gravity)

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>(); // Get movement input from the player
    }

    // Update is called once per frame
    void Update()
    {
        CheckGroundStatus(); // Check if player is grounded
        MovePlayer(); // Handle player movement

        // Jumping logic using legacy input
        if (Input.GetKeyDown(KeyCode.E) && isGrounded) // Only jump if grounded
        {
            Debug.Log("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Calculate the upward velocity for the jump
        }

        // Apply gravity if the player is not grounded
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime; // Apply gravity when in the air
        }
        else if (velocity.y < 0) // If grounded, set a small downward force to keep player grounded
        {
            velocity.y = -2f; // Small negative value to avoid floating or bouncing
        }
    }

    // Method to check if the player is grounded using a raycast
    public void CheckGroundStatus()
    {
        RaycastHit hit;
        // Cast a ray downward to check if the player is grounded
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
        {
            isGrounded = true;
            Debug.Log("Grounded");
        }
        else
        {
            isGrounded = false;
            Debug.Log("Not Grounded");
        }

        // Debugging ray to visualize the ground check
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);
    }

    // Method to move the player based on input and apply velocity
    public void MovePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0.0f, move.y); // Convert input to 3D movement (only x and z axis)

        // Rotate the player to face the movement direction
        if (movement.magnitude > 0.1f) // Only rotate if there is movement
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
        }

        // Move the player using the movement input (horizontal movement)
        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        // Apply the vertical velocity (jump or fall)
        transform.Translate(velocity * Time.deltaTime, Space.World);
    }
}
