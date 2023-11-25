using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerTank : NetworkBehaviour
{
    
    public static event Action<PlayerTank> OnPlayerSpawn;
    public static event Action<PlayerTank> OnPlayerDespawn;


    [Header("Refs")]
    [SerializeField] CinemachineVirtualCamera playerCam;
    [field: SerializeField] public Health PlayerHealth { get; private set; }
    [field: SerializeField] public CoinCollector CoinCollector { get; private set; }



    [Header("Settings")]
    [SerializeField] int playerCamPrio = 20;

    
    [HideInInspector] public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();




    public override void OnNetworkSpawn()
    {

        if (IsServer)
        {
            PlayerName.Value = HostSingelton.Instance.HostManager.NetworkServer.
                GetPlayerDataByClientID(OwnerClientId).playerName;
        }
        if(IsOwner)
        {
            playerCam.Priority = playerCamPrio;
        }
        OnPlayerSpawn?.Invoke(this);

    }

    public override void OnNetworkDespawn()
    {
        OnPlayerDespawn?.Invoke(this);
    }
}
