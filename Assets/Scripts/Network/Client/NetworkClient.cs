using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        if (clientID != 0) return;

        if(SceneManager.GetActiveScene().name != Main_Menu_Scene_Name)
        {
            SceneManager.LoadScene(Main_Menu_Scene_Name);
        }
        if(networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        if(networkManager) networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
    }
}
