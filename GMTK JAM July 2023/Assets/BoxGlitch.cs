using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxGlitch : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Transform nextRoomSpawnPoint;
    private GameMaster gm;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        gm.lastCheckPointPos = nextRoomSpawnPoint.position;
        PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
        playerMovement.canFlip = false;
        playerMovement.canMove = false;
        playerMovement.ableToDash = true;
        playerMovement.dashSpeedGlitch = true;
        StartCoroutine(NextLevelTransition());
        
    }

    IEnumerator NextLevelTransition()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().Death(2.0f,true));
        loadingScreen.SetActive(true);
    }
}
