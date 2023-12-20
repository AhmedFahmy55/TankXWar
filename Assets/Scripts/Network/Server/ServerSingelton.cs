using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

public class ServerSingelton : MonoBehaviour
{
    public static ServerSingelton Instance { get; private set; }

    public ServerManager ServerManager { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        ServerManager?.Dispose();
    }

    public async Task CreaServer()
    {
        await UnityServices.InitializeAsync();

        ServerManager = new ServerManager(
            ApplicationData.IP(),
            ApplicationData.Port(),
            ApplicationData.QPort(),
            NetworkManager.Singleton
        );
    }
}
