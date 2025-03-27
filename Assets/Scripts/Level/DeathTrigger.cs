using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private GameObject endGameUI;
    // Start is called before the first frame update
    void Start()
    {
        endGameUI = Canvas.FindObjectOfType<Canvas>().transform.Find("Death").gameObject;
        endGameUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Time.timeScale = 0;
        if (other.gameObject.CompareTag("Player"))
        {
            endGameUI.SetActive(true);
        }
    }

}
