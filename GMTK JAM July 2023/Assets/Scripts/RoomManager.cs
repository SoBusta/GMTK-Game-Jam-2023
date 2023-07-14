using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomManager : MonoBehaviour
{

    [SerializeField] private GameObject virtualCam;

    [SerializeField] private List<GameObject> objectsToActivate;

    [SerializeField] private Dialog dialog;

    [SerializeField] private bool TextRoom;

    [SerializeField] private bool nextTextRoom;

    [SerializeField] private GameObject loadingScreen;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCam.SetActive(true);
            virtualCam.GetComponent<CinemachineConfiner>().enabled = true;
         
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }


        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCam.SetActive(false);

        }
    }



}
