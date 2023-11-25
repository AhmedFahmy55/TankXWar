using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingelton : MonoBehaviour
{

    public static ClientSingelton Instance { get; private set; }

    public ClientManager ClientManager { get; private set; }


    private void Awake()
    {
        if(Instance != null && Instance != this)
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
        ClientManager?.Dispose();
    }


    public async Task CreatCLient()
    {
        ClientManager = new ClientManager();
        await ClientManager.Init();
    }
}
