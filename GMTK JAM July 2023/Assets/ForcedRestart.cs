using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedRestart : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(collision.GetComponent<PlayerMovement>().Death(0.6f,false));
        }
    }
}
