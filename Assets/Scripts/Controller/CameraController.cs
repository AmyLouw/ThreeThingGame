using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; //player  transform
    public float offSetZ = 10f; //distance from player
    public float height = 5f; //height from player
    public float smoothSpeed = 0.125f; //smoothness of camera movement

    private Vector3 currentVelocity = Vector3.zero; //current velocity of camera

    private void LateUpdate()
    {
        //get  desired position of camera
        Vector3 desiredPosition = new Vector3(transform.position.x, height, player.position.z + offSetZ);

        //smoothly move camera to desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);
    }
}
