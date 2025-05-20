using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Slider staminaSlider;

    public void UpdateStaminaBar(float current, float max)
    {
        staminaSlider.value = current / max;
    }
}