using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Coin : NetworkBehaviour
{

    [SerializeField] private SpriteRenderer coinSprite;


    protected int coinValue;
    protected bool isCollected;

    public abstract int Collect();

    public void Show(bool flag)
    {
        if (coinSprite != null)
        {
            coinSprite.enabled = flag;
        }
    }

    public void SetCoinValue(int value)
    {
        coinValue = value;
    }

}
