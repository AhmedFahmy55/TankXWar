using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CoinsUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI coinsText;

    private void Start()
    {

        PlayerTank.OnPlayerSpawn += PlayerTank_OnPlayerSpwan;
        PlayerTank.OnPlayerDespawn += PlayerTank_OnPlayerDeSpwan;

        PlayerTank[] playerTanks = FindObjectsByType<PlayerTank>(FindObjectsSortMode.None);

        foreach (var player in playerTanks)
        {
            if (player.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {

                PlayerTank_OnPlayerSpwan(player);
                return;
            }
        }
    }

    private void PlayerTank_OnPlayerDeSpwan(PlayerTank tank)
    {
        if (tank.OwnerClientId != NetworkManager.Singleton.LocalClientId) return;
        Debug.Log("UnSubbing");
        tank.CoinCollector.OnCoinsValueChage -= CoinCollector_OnCoinValueChange;
    }

    private void PlayerTank_OnPlayerSpwan(PlayerTank tank)
    {
        if (tank.OwnerClientId != NetworkManager.Singleton.LocalClientId) return;
        Debug.Log("Subbing");
        tank.CoinCollector.OnCoinsValueChage += CoinCollector_OnCoinValueChange;
        CoinCollector_OnCoinValueChange(tank.CoinCollector.GetCoins());
    }

    private void CoinCollector_OnCoinValueChange(int value)
    {        
        coinsText.text = $"{value}";
    }
}
