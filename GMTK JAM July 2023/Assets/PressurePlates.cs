using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlates : MonoBehaviour
{
    public GameObject door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GrabbableObject")) {
            door.GetComponent<OpenDoor>().opened = true;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("GrabbableObject"))
        {
            door.GetComponent<OpenDoor>().opened = false;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
        }
    }
}
