using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    //find the camera object in the scene

    private GameObject camera;

    private void Start()
    {
        // Find the camera object in the scene
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        // Set the flag to indicate the player is at the corner
    //        camera.GetComponent<CameraController>().isAtCorner = true;
    //        // Start the coroutine to smoothly transition the camera
    //        StartCoroutine(camera.GetComponent<CameraController>().SmoothCameraTransition());
    //    }
    //}
}
