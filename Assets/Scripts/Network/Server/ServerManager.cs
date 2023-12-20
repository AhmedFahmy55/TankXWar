using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerManager : IDisposable
{
    private string _IP;
    private int _port;
    private int _Qport;

    private MultiplayAllocationService _multiplayAllocationService;
    private NetworkServer _networkServer;

    public ServerManager(string IP,int port,int Qport,NetworkManager networkManager)
    {
        _IP = IP;
        _port = port;
        _Qport = Qport;
        _networkServer = new NetworkServer(networkManager);
    }


    public async Task StartGameServer()
    {
        await _multiplayAllocationService.BeginServerCheck();
        if(!_networkServer.OpenConnection(_IP, _port))
        {
            Debug.LogWarning("NetworkServer didnt start as expected");
            return;
        }
        NetworkManager.Singleton.SceneManager.LoadScene("MainMenu",LoadSceneMode.Single);
    }
    public void Dispose()
    {
        _multiplayAllocationService?.Dispose();
        _networkServer?.Dispose();
    }
}
