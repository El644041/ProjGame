using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public Button continueGame;
    private int flag;
    AsyncOperation asyncLoadLevel;


    private void Start()
    {
        flag = PlayerPrefs.GetInt("continue", 0);
        if (flag == 1)
        {
            continueGame.gameObject.SetActive(true);
        }
        else
        {
            continueGame.gameObject.SetActive(false);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    

}
