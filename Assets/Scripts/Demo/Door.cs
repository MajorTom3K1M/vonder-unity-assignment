using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Door : Interactable
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private Vector2 targetPosition;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    public override void Interact()
    {
        SceneManager.LoadScene(sceneToLoad);
        player.transform.position = targetPosition;
        player.GetComponent<PlayerRespawn>().SetStartingPoint(targetPosition);
    }
}
