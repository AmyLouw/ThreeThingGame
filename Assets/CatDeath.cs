using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatDeath : StateMachineBehaviour
{
    private GameObject endGameUI;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        endGameUI = Canvas.FindObjectOfType<Canvas>().transform.Find("Death").gameObject;
        Time.timeScale = 0;
        endGameUI.SetActive(true);      
    }
}
