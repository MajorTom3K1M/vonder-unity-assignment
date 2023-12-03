using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextStay : MonoBehaviour
{
    private Transform characterTransform;
    private Vector3 originalScale;

    void Start()
    {
        characterTransform = this.transform.parent;

        originalScale = this.transform.localScale;
    }

    void Update()
    {
        if (characterTransform.localScale.x < 0)
        {
            this.transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }
        else
        {
            this.transform.localScale = originalScale;
        }
    }
}
