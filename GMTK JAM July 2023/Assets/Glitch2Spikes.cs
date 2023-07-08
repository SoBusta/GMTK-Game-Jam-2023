using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Glitch2Spikes : MonoBehaviour
{
    [SerializeField] private GameObject SpikesTilemapObject;
    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Transform nextRoomSpawnPoint;

    private float defaultOrthoSize;

    private GameMaster gm;
    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        defaultOrthoSize = cameraAnimator.gameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            playerMovement.canFlip = false;
            playerMovement.canMove = false;
            Rigidbody2D Player = collision.gameObject.GetComponent<Rigidbody2D>();
            Player.velocity = new Vector2(0, Player.velocity.y);

            gm.lastCheckPointPos = nextRoomSpawnPoint.position;
            StartCoroutine(NextLevelTransition());
        }

    }
    IEnumerator NextLevelTransition()
    {
        cameraAnimator.SetTrigger("StartZoom");
        yield return new WaitForSeconds(2f);
        StartCoroutine(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().Death(2.0f));
        cameraAnimator.gameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = defaultOrthoSize;
        loadingScreen.SetActive(true);
        yield return new WaitForSeconds(3f);
        SpikesTilemapObject.layer = LayerMask.NameToLayer("Death");
    }

}