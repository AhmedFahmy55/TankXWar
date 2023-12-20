using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{


    private const string Main_Menu_Scene_Name = "MainMenu";

    NetworkManager networkManager;



    public NetworkClient(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }



    private void OnClientDisconnect(ulong clientID)
    {
        //to guard against the host cus this will be invoked at host too >> he is client too
        if (clientID != 0) return;
        LeaveGame();
    }

    public void LeaveGame()
    {
        if (SceneManager.GetActiveScene().name != Main_Menu_Scene_Name)
        {
            SceneManager.LoadScene(Main_Menu_Scene_Name);
        }
        if (networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        if(networkManager) networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
    }
}
