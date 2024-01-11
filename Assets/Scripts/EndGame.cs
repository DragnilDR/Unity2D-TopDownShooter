using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] private Manager manager;
    [SerializeField] private GameObject endGameScreen;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "Player" && Input.GetKeyDown(KeyCode.F) && manager.countItem >= 3)
        {
            endGameScreen.SetActive(true);
        }
    }
}
