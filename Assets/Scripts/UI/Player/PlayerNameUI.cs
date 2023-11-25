using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] PlayerTank PlayerTank;




    void Start()
    {
        PlayerTank_OnPlayernameChane("", PlayerTank.PlayerName.Value);
        PlayerTank.PlayerName.OnValueChanged += PlayerTank_OnPlayernameChane;
    }

    private void PlayerTank_OnPlayernameChane(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        playerNameText.text = newValue.ToString();
    }
}
