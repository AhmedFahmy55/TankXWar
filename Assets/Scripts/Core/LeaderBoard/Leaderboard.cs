using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{

    public event Action<NetworkListEvent<LeaderboardItemData>> OnLeaderboardValueChange;

    private NetworkList<LeaderboardItemData> leaderboardItemDatas;



    private void Awake()
    {
        leaderboardItemDatas = new NetworkList<LeaderboardItemData>();
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            leaderboardItemDatas.OnListChanged += OnLeaderboardChange;
            foreach (var item in leaderboardItemDatas)
            {
                OnLeaderboardChange(new NetworkListEvent<LeaderboardItemData>()
                {
                    Type = NetworkListEvent<LeaderboardItemData>.EventType.Add,
                    Value = item
                });
            }
        } 

        if (!IsServer) return;

        PlayerTank.OnPlayerSpawn += PlayerTank_OnPlayerSpawn;
        PlayerTank.OnPlayerDespawn += PlayerTank_OnPlayerDespawn;
        HostSingelton.Instance.HostManager.NetworkServer.OnClientDisconnect += NetworkServer_OnClientDisconnect;

        PlayerTank[] playerTanks = FindObjectsByType<PlayerTank>(FindObjectsSortMode.None);
        foreach (var tank in playerTanks)
        {
            PlayerTank_OnPlayerSpawn(tank);
        }
    }



    public override void OnNetworkDespawn()
    {
        leaderboardItemDatas.OnListChanged -= OnLeaderboardChange;

        if (!IsServer) return;

        PlayerTank.OnPlayerSpawn -= PlayerTank_OnPlayerSpawn;
        PlayerTank.OnPlayerDespawn -= PlayerTank_OnPlayerDespawn;
        HostSingelton.Instance.HostManager.NetworkServer.OnClientDisconnect -= NetworkServer_OnClientDisconnect;
    }

    private void OnLeaderboardChange(NetworkListEvent<LeaderboardItemData> changeEvent)
    {
        OnLeaderboardValueChange?.Invoke(changeEvent);
        if(!IsServer) return;

    }

    private void PlayerTank_OnPlayerSpawn(PlayerTank tank)
    {
        AddPlayerToLeaderboard(tank);

        tank.PlayerHealth.OnPlayerDie += Health_OnPlayerDie;
            
    }


    private void PlayerTank_OnPlayerDespawn(PlayerTank tank)
    {
        tank.PlayerHealth.OnPlayerDie -= Health_OnPlayerDie;

    }


    private void Health_OnPlayerDie(ulong killerID)
    {
        for(int i = 0; i < leaderboardItemDatas.Count; i++)
        {
            if (leaderboardItemDatas[i].ClientID == killerID)
            {
                PlayerData playerData = HostSingelton.Instance.HostManager.NetworkServer.GetPlayerDataByClientID(killerID);
                playerData.PlayerScore = leaderboardItemDatas[i].Score + 1;
                HostSingelton.Instance.HostManager.NetworkServer.UpdatePlayerData(killerID, playerData);

                leaderboardItemDatas[i] = new LeaderboardItemData()
                {
                    ClientID = killerID,
                    PlayerName = leaderboardItemDatas[i].PlayerName,
                    Score = playerData.PlayerScore,
                };

                return;
            }
        }
    }
    private void NetworkServer_OnClientDisconnect(ulong ClientID, string AuthID)
    {
        Debug.Log("LeaderBoard client disconnected" + ClientID);

        RemovePlayerFromLeaderboard(ClientID);
    }
    private void AddPlayerToLeaderboard(PlayerTank tank)
    {
        if (leaderboardItemDatas == null) return;

        foreach (var data in leaderboardItemDatas)
        {
            if (data.ClientID == tank.OwnerClientId) return;
        }

        PlayerData playerData = HostSingelton.Instance.HostManager.NetworkServer.GetPlayerDataByClientID(tank.OwnerClientId);
        LeaderboardItemData leaderboardItemData = new LeaderboardItemData
        {
            ClientID = tank.OwnerClientId,
            PlayerName = tank.PlayerName.Value,
            Score = playerData == null ? 0 :playerData.PlayerScore
        };
        leaderboardItemDatas.Add(leaderboardItemData);
    }

    private void RemovePlayerFromLeaderboard(ulong ClientID)
    {
        if (leaderboardItemDatas == null ) return;
        foreach (var itemData in leaderboardItemDatas)
        {
            if (itemData.ClientID != ClientID) continue;
            leaderboardItemDatas.Remove(itemData);
            break;
        }
    }

}
