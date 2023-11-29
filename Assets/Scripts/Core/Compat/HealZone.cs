using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealZone : NetworkBehaviour
{

    public event Action<int> OnHealZoneValueChange;


    [field:SerializeField] public int MaxHealPowerCount { get; private set; } = 30;
    [SerializeField] private float rechargeCalldown = 10;
    [SerializeField] private float healTecCalldown = 1;
    [SerializeField] private int HealPerTec = 10;
    [SerializeField] private int costPerTec = 10;


    private NetworkVariable<int> _healPowerCount = new NetworkVariable<int>(0);

    private List<PlayerTank> playersTanks = new List<PlayerTank>();

    private float _timeSincelastHealTec;
    private float _timeSincelastRecharge;


    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            _healPowerCount.OnValueChanged += (oldValue, newValue) => OnHealZoneValueChange?.Invoke(newValue);
        }
        if(IsServer)
        {
            _healPowerCount.Value = MaxHealPowerCount;
        }
    }

    public override void OnNetworkDespawn()
    {
        _healPowerCount.OnValueChanged -= (oldValue, newValue) => OnHealZoneValueChange?.Invoke(newValue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsServer) return;
        if(!collision.attachedRigidbody.TryGetComponent<PlayerTank>(out PlayerTank playerTank)) return;
        if (playersTanks.Contains(playerTank)) return;

        playersTanks.Add(playerTank);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) return;
        if (!collision.attachedRigidbody.TryGetComponent<PlayerTank>(out PlayerTank playerTank)) return;
        if (!playersTanks.Contains(playerTank)) return;

        playersTanks.Remove(playerTank);
    }

    private void Update()
    {
        if (!IsServer) return;

        if(_timeSincelastRecharge > 0 )
        {
            _timeSincelastRecharge -= Time.deltaTime;
            if( _timeSincelastRecharge <= 0 )
            {
                _timeSincelastRecharge = 0;
                _healPowerCount.Value = MaxHealPowerCount;
            }
            else
            {
                return;
            }
        }
     
        if(_timeSincelastHealTec > 0)
        {
            _timeSincelastHealTec -= Time.deltaTime;
            return;
        }

        if (playersTanks.Count == 0) return;

        foreach (var tank in playersTanks)
        {
            if (tank.PlayerHealth.IsFullHealth() || tank.CoinCollector.GetCoins() < costPerTec) continue;

            tank.PlayerHealth.Heal(HealPerTec);
            tank.CoinCollector.SpendCoins(costPerTec);
            _healPowerCount.Value--;

            if(_healPowerCount.Value <= 0)
            {
                _timeSincelastRecharge = rechargeCalldown;
                return;
            }
        }
        _timeSincelastHealTec = healTecCalldown;
    }

    public int GetHealPowerCount()
    {
        return _healPowerCount.Value;
    }
}
