using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameOverScreen : MonoBehaviour
{
    private GameObject player;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Show() { 
        gameObject.SetActive(true);
    }

    public void RestartGame(InputAction.CallbackContext context) {
        if (context.performed) { 
            gameObject.SetActive(false);
            player.GetComponent<PlayerMovement>().Respawn();
        }
    }
}
