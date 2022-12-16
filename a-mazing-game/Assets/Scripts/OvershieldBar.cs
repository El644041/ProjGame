using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OvershieldBar : MonoBehaviour
{
    public Slider slider;
    
    public void SetOvershield(int overshield)
    {
        slider.value = overshield;
        print(slider.value);
    }

    public void SetMaxOvershield(int overshield)
    {
        slider.maxValue = overshield;
        slider.value = overshield;
    }
}
