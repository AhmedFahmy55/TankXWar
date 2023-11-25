using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Image healthBarImage;

    

    private void Start()
    {
        health.OnPlayerHealthChange += UpdateHealth;
        UpdateHealth(health.MaxHealth);
    }
    private void OnDestroy()
    {
        health.OnPlayerHealthChange -= UpdateHealth;
    }

    private void UpdateHealth(int newValue)
    {
        float healtBarValue = (float) newValue / health.MaxHealth;
        healthBarImage.fillAmount = healtBarValue;
    }

}
