using UnityEngine;

public class CameraTransitionTrigger : MonoBehaviour
{
    public enum TransitionType { TopDown, Follow }
    [Tooltip("Select TopDown to switch to bird’s-eye view, or Follow to return to normal camera mode.")]
    public TransitionType transitionType = TransitionType.TopDown;

    [Tooltip("If true, the trigger only works once.")]
    public bool oneShot = true;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered && oneShot) return;

        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            // Find the main camera's CameraController component.
            CameraController camController = Camera.main.GetComponent<CameraController>();
            if (camController != null)
            {
                if (transitionType == TransitionType.TopDown)
                {
                    camController.TransitionToTopDown();
                }
                else if (transitionType == TransitionType.Follow)
                {
                    camController.TransitionToFollow();
                }
            }
            triggered = true;
        }
    }
}
