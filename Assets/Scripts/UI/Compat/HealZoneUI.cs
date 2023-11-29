using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealZoneUI : MonoBehaviour
{
    [SerializeField] HealZone healZone;
    [SerializeField] Image healBarImage;




    private void Start()
    {
        healZone.OnHealZoneValueChange += HealZone_OnHealBarValueChange;
        HealZone_OnHealBarValueChange(healZone.GetHealPowerCount());
    }

    private void OnDestroy()
    {
        healZone.OnHealZoneValueChange -= HealZone_OnHealBarValueChange;
    }

    private void HealZone_OnHealBarValueChange(int newValue)
    {
        healBarImage.fillAmount =  newValue / (float)healZone.MaxHealPowerCount;
    }
}
