using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialScreen : MonoBehaviour
{
    public bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
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
    }
    
}
