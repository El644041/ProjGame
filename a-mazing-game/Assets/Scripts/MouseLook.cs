using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private bool cursorLocked = true;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.lockState = CursorLockMode.Locked;
    }
    

    // Update is called once per frame
    // void Update()
    // {
    //     // Press C to toggle locking of the Cursor
    //     if (Input.GetKeyDown(KeyCode.C))
    //     {
    //         if (cursorLocked)
    //         {
    //             Cursor.lockState = CursorLockMode.None;
    //             cursorLocked = false;
    //         }
    //         else
    //         {
    //             Cursor.lockState = CursorLockMode.Locked;
    //             cursorLocked = true;
    //         }
    //     }
    // }
}
