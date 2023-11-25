using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinsSpwaner : NetworkBehaviour
{
    [SerializeField] RespwaningCoin respwaningCoinPrefap;
    [SerializeField] int coinValue;
    [SerializeField] int maxCoinsNumb;
    [SerializeField] Vector2 xRange;
    [SerializeField] Vector2 yRange;
    [SerializeField] LayerMask blocksLayer;


    private const int Iterations_Numb = 40;

    private Collider2D[] raycastHit2Ds = new Collider2D[1];



    private float _coinRad;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        _coinRad = respwaningCoinPrefap.GetComponent<CircleCollider2D>().radius;
        SpwanCoins();
    }


    private void SpwanCoins()
    {
        for(int i = 0; i < maxCoinsNumb; i++)
        {
            if(!TryGetPosition(out Vector2 pos)) continue;

            RespwaningCoin coin =  Instantiate(respwaningCoinPrefap,pos,Quaternion.identity);
            coin.SetCoinValue(coinValue);
            coin.GetComponent<NetworkObject>().Spawn();
            coin.OnCoinCollected += RespwaningCoin_OnCoinCollected;
        }
    }

    private void RespwaningCoin_OnCoinCollected(RespwaningCoin coin)
    {
        Vector2 newPos;

        if (!TryGetPosition(out newPos))
        {
            coin.transform.position = Vector2.zero;
            ResetCoinClientRpc(coin.GetComponent<NetworkObject>());
        }
        else
        {
            coin.transform.position = newPos;
            ResetCoinClientRpc(coin.GetComponent<NetworkObject>());
        }
    }


    [ClientRpc]
    private void ResetCoinClientRpc(NetworkObjectReference coin)
    {
        if (!coin.TryGet(out NetworkObject coinRef)) return;
        coinRef.GetComponent<RespwaningCoin>().Reset();
    }
    private bool TryGetPosition(out Vector2 pos)
    {
        float x ;
        float y ;

        for (int i = 0; i < Iterations_Numb; i++)
        {
            x = UnityEngine.Random.Range(xRange.x,xRange.y);
            y = UnityEngine.Random.Range(yRange.x, yRange.y);

            Vector2 validPos = new Vector2(x, y);

            int hitNumb = Physics2D.OverlapCircleNonAlloc(validPos, _coinRad * 4, raycastHit2Ds,blocksLayer);
            if (hitNumb == 0)
            {
                pos = validPos;
                return true;
            }

        }
        pos = Vector2.zero;
        return false;
    }

}
