using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Glitch1_OutOfBounds : MonoBehaviour
{
    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Transform nextRoomSpawnPoint;

    private GameMaster gm;

    private float defaultOrthoSize;
    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        defaultOrthoSize = cameraAnimator.gameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gm.lastCheckPointPos = nextRoomSpawnPoint.position;
            StartCoroutine(NextLevelTransition());
        }
    }

    IEnumerator NextLevelTransition()
    {
        cameraAnimator.SetTrigger("StartZoomOut");
        //cameraAnimator.gameObject.GetComponent<CinemachineConfiner>().enabled = false;
        yield return new WaitForSeconds(2f);
        StartCoroutine(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().Death(2.0f));
        cameraAnimator.gameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = defaultOrthoSize;
        loadingScreen.SetActive(true);
    }
}
