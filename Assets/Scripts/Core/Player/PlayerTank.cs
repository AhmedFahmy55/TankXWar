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
    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private SpriteRenderer MiniMapIcon;
    [field: SerializeField] public Health PlayerHealth { get; private set; }
    [field: SerializeField] public CoinCollector CoinCollector { get; private set; }



    [Header("Settings")]
    [SerializeField] private int playerCamPrio = 20;
    [SerializeField] private Color PlayerminiMapIconColor = Color.cyan;

    
    [HideInInspector] public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();




    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerCam.Priority = playerCamPrio;
            MiniMapIcon.color = PlayerminiMapIconColor;
        }

        if (IsServer)
        {
            PlayerName.Value = HostSingelton.Instance.HostManager.NetworkServer.
                GetPlayerDataByClientID(OwnerClientId).playerName;
        }
        if(IsClient)
        {
            OnPlayerSpawn?.Invoke(this);

        }
    }

    public override void OnNetworkDespawn()
    {
        OnPlayerDespawn?.Invoke(this);
    }
}
