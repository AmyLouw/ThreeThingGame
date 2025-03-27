using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    private GameObject endGameUI;

    private void Start()
    {
        endGameUI = GameObject.Find("Canvas");
        endGameUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            endGameUI.SetActive(true);
        }
    }

}
