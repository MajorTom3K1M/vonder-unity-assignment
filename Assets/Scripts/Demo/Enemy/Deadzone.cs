using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Deadzone : MonoBehaviour
{
    [SerializeField] private int damage;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(damage);
            collision.gameObject.GetComponent<PlayerRespawn>().RespawnAtCheckpoint();
        }
    }
}
