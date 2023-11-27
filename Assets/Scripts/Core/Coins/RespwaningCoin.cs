using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespwaningCoin : Coin
{
    public event Action<RespwaningCoin> OnCoinCollected;

    public override int Collect()
    {
        if(!IsServer)
        {
            Show(false); 
            return 0;
        }

        if(isCollected) return 0;

        isCollected = true;

        OnCoinCollected?.Invoke(this);
        return coinValue;

    }


    public void ResetCoin()
    {
        isCollected = false;
        Show(true);
    }

}
