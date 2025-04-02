using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;          // Speed at which the cat moves between waypoints
    public float rotationSpeed = 5f;      // Speed at which the cat rotates towards the next waypoint

    [Header("Waypoint Settings")]
    public List<GameObject> sections = new List<GameObject>(); // List of sections in the scene that contain waypoints
    public List<Transform> waypoints = new List<Transform>(); // List to store the waypoints

    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    void Start()
    {
        // Start a coroutine to wait for sections to be instantiated before looking for waypoints
        StartCoroutine(FindWaypointsAfterLevelGeneration());
    }

    void Update()
    {
        if (isMoving && waypoints.Count > 0)
        {
            MoveToWaypoint();
        }
    }

    void MoveToWaypoint()
    {
        // Get the current target waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Move the cat towards the target waypoint
        Vector3 targetPosition = targetWaypoint.position;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Smoothly rotate the cat towards the next waypoint
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Check if the cat has reached the waypoint
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Move to the next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count; // Loop through the waypoints
        }
    }

    // Coroutine to wait for the sections to be instantiated and then find the waypoints
    IEnumerator FindWaypointsAfterLevelGeneration()
    {
        // Wait until sections are instantiated in the scene (could be a small delay)
        yield return new WaitForSeconds(1f);

        // Find all sections that have been instantiated in the scene
        FindSectionsInScene();

        // Find all waypoints in the found sections
        FindWaypoints();

        // Start moving to the first waypoint if waypoints are available
        if (waypoints.Count > 0)
        {
            isMoving = true;
        }
        else
        {
            Debug.LogWarning("No waypoints found in the scene.");
        }
    }

    // Find all sections that are already instantiated in the scene
    void FindSectionsInScene()
    {
        // Find all GameObjects with the tag "Section" (assuming that waypoints are inside these sections)
        GameObject[] sectionObjects = GameObject.FindGameObjectsWithTag("Section");

        // Add these section objects to the list
        foreach (GameObject section in sectionObjects)
        {
            sections.Add(section);
        }
    }

    // Find all waypoints in the scene (empty GameObjects or waypoints inside sections)
    void FindWaypoints()
    {
        // Loop through each section and find its waypoints
        foreach (GameObject section in sections)
        {
            foreach (Transform child in section.transform)
            {
                if (child.CompareTag("waypoint")) // Assuming waypoints have a tag called "CatNavigation"
                {
                    waypoints.Add(child); // Add the waypoint to the list
                }
            }
        }

        // If no waypoints were found, stop the movement
        if (waypoints.Count == 0)
        {
            isMoving = false;
            Debug.LogWarning("No waypoints found in the scene.");
        }
    }
}
