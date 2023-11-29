using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    }

    private void OnLeaderboardChange(NetworkListEvent<LeaderboardItemData> changeEvent)
    {
        OnLeaderboardValueChange?.Invoke(changeEvent);
    }

    private void PlayerTank_OnPlayerSpawn(PlayerTank tank)
    {
        AddPlayerToLeaderboard(tank);

        tank.CoinCollector.OnCollectCoin += (newValue)=> 
            CoinCollector_OnCollectCoins(tank.OwnerClientId,newValue);
    }


    private void PlayerTank_OnPlayerDespawn(PlayerTank tank)
    {
        RemovePlayerFromLeaderboard(tank);

        tank.CoinCollector.OnCollectCoin -= (newValue) =>
            CoinCollector_OnCollectCoins(tank.OwnerClientId, newValue);
    }


    private void CoinCollector_OnCollectCoins(ulong clientID,int newValue)
    {
        for(int i = 0; i < leaderboardItemDatas.Count; i++)
        {
            if (leaderboardItemDatas[i].ClientID == clientID)
            {
                PlayerData playerData = HostSingelton.Instance.HostManager.NetworkServer.GetPlayerDataByClientID(clientID);
                playerData.PlayerScore = leaderboardItemDatas[i].Score + newValue;
                HostSingelton.Instance.HostManager.NetworkServer.UpdatePlayerData(clientID, playerData);

                leaderboardItemDatas[i] = new LeaderboardItemData()
                {
                    ClientID = clientID,
                    PlayerName = leaderboardItemDatas[i].PlayerName,
                    Score = playerData.PlayerScore,
                };

                return;
            }
        }
    }

    private void AddPlayerToLeaderboard(PlayerTank tank)
    {
        if (leaderboardItemDatas == null) return;
        
        PlayerData playerData = HostSingelton.Instance.HostManager.NetworkServer.GetPlayerDataByClientID(tank.OwnerClientId);
        LeaderboardItemData leaderboardItemData = new LeaderboardItemData
        {
            ClientID = tank.OwnerClientId,
            PlayerName = tank.PlayerName.Value,
            Score = playerData == null ? 0 :playerData.PlayerScore
        };
        leaderboardItemDatas.Add(leaderboardItemData);
    }
    private void RemovePlayerFromLeaderboard(PlayerTank tank)
    {
        if (leaderboardItemDatas == null ) return;
        foreach (var itemData in leaderboardItemDatas)
        {
            if (itemData.ClientID != tank.OwnerClientId ) continue;
            leaderboardItemDatas.Remove(itemData);
            break;
        }
    }
}
