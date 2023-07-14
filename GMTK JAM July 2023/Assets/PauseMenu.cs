using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject SettingsMenuUI;
    public GameObject DarkeningEffect;
    private GameObject player;

    private void Start()
    {
        GameIsPaused = false;

        player = GameObject.FindGameObjectWithTag("Player");

    }
    private void Update()
    {
        //Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        //UI
        pauseMenuUI.SetActive(false);
        SettingsMenuUI.SetActive(false);
        GameIsPaused = false;
        DarkeningEffect.SetActive(false);

        //Le temps est remis normal
        Time.timeScale = 1f;

        player.GetComponent<PlayerMovement>().canFlip = true;


    }
    void Pause()
    {
        //Inverse de Resume

        pauseMenuUI.SetActive(true);
        DarkeningEffect.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;

        player.GetComponent<PlayerMovement>().canFlip = false;

    }
    public void RestartLevel()
    {
        Resume();
        StartCoroutine(player.GetComponent<PlayerMovement>().Death(0.6f, false));
    }

}
