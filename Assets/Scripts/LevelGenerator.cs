using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Array to store the prefab sections that will be used to generate the level
    public GameObject[] levelSections;

    // The start and end platforms to be placed at the beginning and end of the level
    public GameObject startPlatform;
    public GameObject endPlatform;

    // The number of sections to generate between the start and end
    public int numberOfSections = 10;

    // The distance between each section (for fallback purposes)
    public float sectionLength = 10f;

    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel();
    }

    // Method to generate the level
    void GenerateLevel()
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
        if (endPlatform != null)
        {
            Instantiate(endPlatform, spawnPosition, Quaternion.identity);
        }
    }

}
