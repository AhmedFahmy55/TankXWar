using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinCollector : NetworkBehaviour
{
    public event Action<int> OnCollectCoin;
    public event Action<int> OnCoinsValueChage;

    private NetworkVariable<int> totalCoins = new NetworkVariable<int>(0);



    public override void OnNetworkSpawn()
    {
        totalCoins.OnValueChanged += (old, newV) => OnCoinsValueChage?.Invoke(newV);
    }
    public override void OnNetworkDespawn()
    {
        totalCoins.OnValueChanged -= (old, newV) => OnCoinsValueChage?.Invoke(newV);

    }

    public int GetCoins()
    {
        return totalCoins.Value;
    }

    public void SetCoins(int value)
    {
        totalCoins.Value = value;
    }

    public void SpendCoins(int shootCost)
    {
        totalCoins.Value -= shootCost;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.TryGetComponent<Coin>(out Coin coin)) return;

        int value = coin.Collect();

        OnCollectCoin?.Invoke(value);
        if (!IsServer) return;

        totalCoins.Value += value;

    }
}
