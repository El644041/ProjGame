using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour
{

    public Slider sensitivitySlider;

    void Awake()
    {
        sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity", 2f);
    }

    public void ApplySensitivity()
    {
        PlayerPrefs.SetFloat("sensitivity", sensitivitySlider.value);
    }



}
