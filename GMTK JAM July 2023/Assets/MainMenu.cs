using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {
        StartCoroutine(EnterGame());
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    IEnumerator EnterGame()
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}
