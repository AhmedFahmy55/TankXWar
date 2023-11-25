using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{


    private Dictionary<ulong,string> clientIDToAuthID = new Dictionary<ulong,string>();
    private Dictionary<string, PlayerData> authIDToPlayerData = new Dictionary<string, PlayerData>();


    NetworkManager networkManager;


    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
        networkManager.ConnectionApprovalCallback = ConnetionApprovalCallBack;
        networkManager.OnServerStarted += OnServerReady;
    }

    public PlayerData GetPlayerDataByClientID(ulong clientID)
    {
        if(clientIDToAuthID.TryGetValue(clientID,out string authID))
        {
            if(authIDToPlayerData.TryGetValue(authID,out PlayerData playerData))
            {
                return playerData;
            }
            return null;
        }
        return null;
    }

    private void ConnetionApprovalCallBack(
         NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)

    {
        string paylaodString = Encoding.UTF8.GetString(request.Payload);
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(paylaodString);

        clientIDToAuthID[request.ClientNetworkId] = playerData.playerAuthID;
        authIDToPlayerData[playerData.playerAuthID] = playerData;

        response.Position = SpwanPoint.GetRandomSpwanPint();
        response.Rotation = Quaternion.identity;
        response.Approved = true;
        response.CreatePlayerObject = true;

    }

    private void OnServerReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientID)
    {
        if(clientIDToAuthID.TryGetValue(clientID,out string authID))
        {
            clientIDToAuthID.Remove(clientID);
            authIDToPlayerData.Remove(authID);
        }
    }

    public void Dispose()
    {
        if(networkManager) networkManager.ConnectionApprovalCallback = null;
        if(networkManager) networkManager.OnServerStarted -= OnServerReady;
        if(networkManager) networkManager.OnClientDisconnectCallback -= OnClientDisconnect;

    }
}
