using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameTrigger : MonoBehaviour
{
    private GameObject endGameUI;

    private void Start()
    {
        endGameUI = Canvas.FindObjectOfType<Canvas>().transform.Find("EndGameUI").gameObject;
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
