using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room10JumpDashGlitch : MonoBehaviour
{
    public static bool hasJumpDashed;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement.JumpDashActive)
            {
                hasJumpDashed = true;
            }
        }
    }
}
