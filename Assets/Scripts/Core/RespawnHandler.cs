using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] PlayerTank playerTankPrefap;


    [SerializeField] float respawnCallDown = 1;
    [SerializeField,Range(0,1)] float coinsLossRation;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        PlayerTank[] playerTanks = FindObjectsByType<PlayerTank>(FindObjectsSortMode.None);
        foreach (var tank in playerTanks)
        {
            PlayerTank_OnPlayerSpawn(tank);
            PlayerTank_OnPlayerDespawn(tank);
        }

        PlayerTank.OnPlayerSpawn += PlayerTank_OnPlayerSpawn;
        PlayerTank.OnPlayerDespawn += PlayerTank_OnPlayerDespawn;

    }


    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        PlayerTank.OnPlayerSpawn -= PlayerTank_OnPlayerSpawn;
        PlayerTank.OnPlayerDespawn -= PlayerTank_OnPlayerDespawn;
    }

    private void PlayerTank_OnPlayerSpawn(PlayerTank playerTank)
    {
        playerTank.PlayerHealth.OnPlayerDie += () => PlayerTank_OnPlayerDie(playerTank);
    }

    private void PlayerTank_OnPlayerDespawn(PlayerTank playerTank)
    {
        playerTank.PlayerHealth.OnPlayerDie -= () => PlayerTank_OnPlayerDie(playerTank);
    }

    private void PlayerTank_OnPlayerDie(PlayerTank playerTank)
    {
        int remainingCoins = (int)(playerTank.CoinCollector.GetCoins() * coinsLossRation);
        Destroy(playerTank.gameObject);
        StartCoroutine(RespawnPlayerTanl(playerTank.OwnerClientId, remainingCoins));
    }

    private IEnumerator RespawnPlayerTanl(ulong ownerClientId,int remainingCoins)
    {
        yield return new WaitForSeconds(respawnCallDown);

        PlayerTank playerTank = Instantiate(playerTankPrefap, SpwanPoint.GetRandomSpwanPint(), Quaternion.identity);
        playerTank.NetworkObject.SpawnAsPlayerObject(ownerClientId);
        playerTank.CoinCollector.SetCoins(remainingCoins);
    }
}
