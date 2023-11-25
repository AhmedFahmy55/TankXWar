using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] NetworkObject playerTankPrefap;


    [SerializeField] float respawnCallDown = 1;


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
        Destroy(playerTank.gameObject);
        StartCoroutine(RespawnPlayerTanl(playerTank.OwnerClientId));
    }

    private IEnumerator RespawnPlayerTanl(ulong ownerClientId)
    {
        yield return new WaitForSeconds(respawnCallDown);

        NetworkObject networkObject = Instantiate(playerTankPrefap, SpwanPoint.GetRandomSpwanPint(), Quaternion.identity);
        networkObject.SpawnAsPlayerObject(ownerClientId);
    }
}
