using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room5DoubleJumpGlitch : MonoBehaviour
{
    public bool hasDoubleJumped;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if ((playerMovement.pressedJumpButton || playerMovement.holdingJumpButton) && playerMovement.jumpCount == 1)
            {
                hasDoubleJumped = true;
            }
        }
    }
}
