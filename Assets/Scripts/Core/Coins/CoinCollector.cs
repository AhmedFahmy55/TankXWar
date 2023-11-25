using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinCollector : NetworkBehaviour
{


    public NetworkVariable<int> totalCoins = new NetworkVariable<int>(0);


    public int GetCoins()
    {
        return totalCoins.Value;
    }

    public void SpendCoins(int shootCost)
    {
        totalCoins.Value -= shootCost;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.TryGetComponent<Coin>(out Coin coin)) return;

        int value = coin.Collect();

        if (!IsServer) return;

        totalCoins.Value += value;
    }
}
