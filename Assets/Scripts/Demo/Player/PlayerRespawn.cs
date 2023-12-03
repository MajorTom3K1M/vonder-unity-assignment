using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector2 currentCheckpoint;
    private Vector2 startingPoint;

    public void SetStartingPoint(Vector2 _startingPoint)
    {
        startingPoint = _startingPoint;
    }

    public void Respawn() {
        transform.position = startingPoint;
    }

    public void RespawnAtCheckpoint()
    {
        transform.position = currentCheckpoint;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            currentCheckpoint = new Vector2(collision.transform.position.x, collision.transform.position.y);
        }
    }

}
