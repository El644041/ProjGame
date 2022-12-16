
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameSettings : MonoBehaviour
{

    AsyncOperation asyncLoadLevel;
    float sliderSensitivity;


    // Start is called before the first frame update
    public void smallGame()
    {
        sliderSensitivity = PlayerPrefs.GetFloat("sensitivity", 2f);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("rows", 13);
        PlayerPrefs.SetInt("cols", 13);
        PlayerPrefs.SetInt("size", 0);
        PlayerPrefs.SetInt("continue", 1);
        PlayerPrefs.SetFloat("sensitivity", sliderSensitivity);
        AudioListener.pause = false;
        StartCoroutine(LoadLevel());


    }

    public void mediumGame()
    {
        sliderSensitivity = PlayerPrefs.GetFloat("sensitivity", 2f);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("rows", 25);
        PlayerPrefs.SetInt("cols", 25);
        PlayerPrefs.SetInt("size", 1);
        PlayerPrefs.SetInt("continue", 1);
        PlayerPrefs.SetFloat("sensitivity", sliderSensitivity);
        StartCoroutine(LoadLevel());

    }

    public void largeGame()
    {
        sliderSensitivity = PlayerPrefs.GetFloat("sensitivity", 2f);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("rows", 37);
        PlayerPrefs.SetInt("cols", 37);
        PlayerPrefs.SetInt("size", 2);
        PlayerPrefs.SetInt("continue", 1);
        PlayerPrefs.SetFloat("sensitivity", sliderSensitivity);
        StartCoroutine(LoadLevel());

    }

    public void tutorialLevel()
    {
        sliderSensitivity = PlayerPrefs.GetFloat("sensitivity", 2f);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("rows", 13);
        PlayerPrefs.SetInt("cols", 13);
        PlayerPrefs.SetInt("size", 4);
        PlayerPrefs.SetInt("continue", 1);
        PlayerPrefs.SetFloat("sensitivity", sliderSensitivity);
        PlayerPrefs.SetInt("tutorial", 1);
        PlayerPrefs.SetFloat("sensitivity", sliderSensitivity);
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        asyncLoadLevel = SceneManager.LoadSceneAsync("Scene");
        while (!asyncLoadLevel.isDone)
        {
            // print("Loading the Scene");
            yield return null;
        }
        Thread.Sleep(2000);
    }
}

