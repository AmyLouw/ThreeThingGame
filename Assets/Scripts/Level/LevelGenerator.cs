using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Array to store the prefab sections that will be used to generate the level
    public GameObject[] levelSections;

    // The start and end platforms to be placed at the beginning and end of the level
    public GameObject startPlatform;
    public GameObject zCornerPlatform;
    public GameObject xCornerPlatform;
    public GameObject endPlatform;

    // The number of sections to generate between the start and end
    public int numberOfSections = 10;

    // The distance between each section (for fallback purposes)
    public float sectionLength = 10f;

    //has the playe reached the corner?
    private bool playerAtCorner = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        GenerateFirstLevel();
    }

    // Method to generate the level
    void GenerateFirstLevel()
    {
        Vector3 spawnPosition = Vector3.zero;

        // Adjust spawn position before instantiating the start platform
        if (startPlatform != null)
        {
            // Instantiate the start platform at the beginning of the level
            Instantiate(startPlatform, spawnPosition, Quaternion.identity);

            // Try to get the Renderer or Collider of the start platform to update spawn position
            Renderer startRenderer = startPlatform.GetComponent<Renderer>();
            if (startRenderer != null)
            {
                // If the start platform has a Renderer, adjust spawn position based on its size
                spawnPosition.z += startRenderer.bounds.size.z;
            }
            else
            {
                // If no Renderer is found, check for a Collider
                Collider startCollider = startPlatform.GetComponent<Collider>();
                if (startCollider != null)
                {
                    // Use the Collider's bounds to adjust the spawn position
                    spawnPosition.z += startCollider.bounds.size.z;
                }
                else
                {
                    // Fallback: If no Renderer or Collider, use a default length (same as sectionLength)
                    spawnPosition.z += sectionLength;
                }
            }
        }

        // Generate random level sections in between the start and end platforms
        for (int i = 0; i < numberOfSections; i++)
        {
            // Pick a random prefab section from the array
            int randomIndex = Random.Range(0, levelSections.Length);

            // Instantiate the selected prefab at the current position
            GameObject section = Instantiate(levelSections[randomIndex], spawnPosition, Quaternion.identity);

            // Get the bounds of the prefab section to determine its length
            Renderer sectionRenderer = section.GetComponent<Renderer>();
            if (sectionRenderer != null)
            {
                // Get the size of the section's renderer to calculate the correct positioning
                float sectionWidth = sectionRenderer.bounds.size.z;

                // Adjust the spawn position by the width of the current section
                spawnPosition.z += sectionWidth; // Move by the section's length along Z axis
            }
            else
            {
                // Fallback in case the prefab doesn't have a Renderer (it might be empty or a non-rendered object)
                spawnPosition.z += sectionLength; // Use the default sectionLength if no renderer
            }
        }

        // Spawn the end platform at the end of the level
        if (zCornerPlatform != null)
        {
            Instantiate(zCornerPlatform, spawnPosition, Quaternion.identity);
        }

        GenerateSecondLevel(spawnPosition);
    }

    void GenerateSecondLevel(Vector3 spawnPosition)
    {
        // Rotate sections along the X-axis to face the negative X direction
        Quaternion sectionRotation = Quaternion.Euler(0, -90, 0);

        // Spawn x corner at the start of the second level
        if (xCornerPlatform != null)
        {
            // Adjust spawn position before corner platform spawn
            spawnPosition.x -= 34;  // Move the spawn position along negative X (adjust based on your scene)
            spawnPosition.z += 15;  // Adjust along Z-axis (adjust as needed for corner offset)

            // Instantiate the corner platform
            Instantiate(xCornerPlatform, spawnPosition, Quaternion.identity);

            // Adjust spawn position after corner platform
            // Make sure to account for the full width of the corner platform along the X-axis
            Collider cornerCollider = xCornerPlatform.GetComponent<Collider>();
            if (cornerCollider != null)
            {
                // Subtract the corner platform's width from spawnPosition based on collider bounds
                spawnPosition.x -= sectionLength;
            }
        }

        // Generate new sections along the negative X-axis
        for (int i = 0; i < numberOfSections; i++)
        {
            int randomIndex = Random.Range(0, levelSections.Length);
            GameObject section = Instantiate(levelSections[randomIndex], spawnPosition, sectionRotation); // Apply rotation to face negative X direction

            // Get the bounds of the prefab section to determine its length along the X-axis using its Collider
            Collider sectionCollider = section.GetComponent<Collider>();
            if (sectionCollider != null)
            {
                // Get the size of the section's collider to calculate the correct positioning
                float sectionWidth = sectionCollider.bounds.size.x; // Use the X dimension, as we are moving along the X-axis

                // Adjust the spawn position along the negative X-axis (subtract section width)
                spawnPosition.x -= sectionWidth;  // Move by the section's width along the negative X-axis
            }
            else
            {
                // Fallback in case the prefab doesn't have a Collider (it might be empty or a non-colliding object)
                spawnPosition.x -= sectionLength; // Use the default sectionLength if no collider
            }
        }

        //spawn end platform
        if(endPlatform != null)
        {
            Instantiate(endPlatform, spawnPosition, sectionRotation);
        }
    }
}
