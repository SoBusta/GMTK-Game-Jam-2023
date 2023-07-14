using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room4CoyoteGlitch : MonoBehaviour
{
    public bool hasCoyoteAndJumped;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(collision.GetComponent<PlayerMovement>().pressedJumpButton || collision.GetComponent<PlayerMovement>().holdingJumpButton)
            {

                hasCoyoteAndJumped = true;
            }
        }
    }

}
