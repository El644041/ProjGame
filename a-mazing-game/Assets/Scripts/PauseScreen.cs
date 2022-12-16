using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    public bool isPaused = false;
    private PlayerStats playerStats;


    public void resumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);


    }

    public void pauseGame()
    {
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
        gameObject.SetActive(true);

    }

    public void returnToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");

    }

}
