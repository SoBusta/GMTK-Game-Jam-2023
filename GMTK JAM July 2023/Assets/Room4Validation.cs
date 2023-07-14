using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Room4Validation : MonoBehaviour
{

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Transform nextRoomSpawnPoint;
    [SerializeField] private Dialog dialog;

    private GameMaster gm;

    private float defaultOrthoSize;
    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetComponentInParent<Room4CoyoteGlitch>().hasCoyoteAndJumped)
        {
            gm.lastCheckPointPos = nextRoomSpawnPoint.position;
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            playerMovement.canFlip = false;
            playerMovement.canMove = false;
            playerMovement.coyoteTimeThreshold = 0.1f;
            playerMovement.DoubleJumpGlitch = true;
            StartCoroutine(NextLevelTransition());
        }
    }

    IEnumerator NextLevelTransition()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().Death(2.0f, true));
        loadingScreen.SetActive(true);
        dialog.StartCoroutine(dialog.Type());
    }
}
