using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    //array to store the prefabs
    public GameObject[] levelSections;
    
    //number of sections to generate
    public int numberOfSections = 10;


    //distance between each section
    public float distanceBetweenSections = 10f;

    // Start is called before the first frame update
    void Start()
    {
        //generate the level
        GenerateLevel();
    }

    //method to generate the level
    private void GenerateLevel()
    {
        Vector3 spawnPosition = Vector3.zero;

        for (int i = 0; i < numberOfSections; i++)
        {
            //get a random section from the array
            GameObject section = levelSections[Random.Range(0, levelSections.Length)];

            //instantiate the section at the spawn position
            Instantiate(section, spawnPosition, Quaternion.identity);

            //update the spawn position for the next section
            spawnPosition.z += distanceBetweenSections;
        }
    }
}
