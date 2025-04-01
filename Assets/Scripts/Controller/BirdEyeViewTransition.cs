using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdEyeViewTransition : MonoBehaviour
{

    [Header("Camera Settings")]
    public Transform normalCamPoint;
    public Transform topDownCamPoint;
    public float transitionSpeed = 2f;

    private Camera mainCam;
    private bool isInTopDownView = false;


    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        if (!mainCam)
        {
            Debug.LogError("No main camera found. Make sure your camera is tagged 'MainCamera'.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isInTopDownView)
            {
                StopAllCoroutines();
                StartCoroutine(SwitchCameraView(mainCam.transform, topDownCamPoint));
                isInTopDownView = true;
            }
            else
            {
                // Optional for re-entering a collision box to set it back to normal view

                //StopAllCoroutines();
                //StartCoroutine(SwitchCameraView(mainCam.transform, normalCamPoint));
                //isInTopDownView = false;
            }
        }
    }

    private System.Collections.IEnumerator SwitchCameraView(Transform camTransform, Transform target)
    {
        float t = 0f;
        // Capture the start position/rotation
        Vector3 startPos = camTransform.position;
        Quaternion startRot = camTransform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            // Interpolate position & rotation
            camTransform.position = Vector3.Lerp(startPos, target.position, t);
            camTransform.rotation = Quaternion.Slerp(startRot, target.rotation, t);

            yield return null;
        }
    }
}
