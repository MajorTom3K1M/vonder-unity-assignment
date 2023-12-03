using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float followSpeed = 2f;
    public float yOffset = 1f;
    public float maxOffset = 3.5f;
    public float minOffset = 1f;
    public Transform target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        if (target != null) { 
            Vector3 newPos = new Vector3(target.position.x, Mathf.Clamp(target.position.y + yOffset, minOffset, maxOffset), -10f);
            transform.position = Vector3.Slerp(transform.position, newPos, followSpeed * Time.deltaTime);
        }
    }
}
