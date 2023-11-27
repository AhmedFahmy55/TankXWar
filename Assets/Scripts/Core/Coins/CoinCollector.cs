using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinCollector : NetworkBehaviour
{
    public event Action<int> OnCollectCoin;
    public event Action<int> OnCoinsValueChage;

    [Header("Refs")]
    [SerializeField] private BountyCoin bountyCoinPrefap;
    [SerializeField] private Health playerHealth;

    [Header("Settings")]
    [SerializeField, Range(0, 1)] private float bountyValueRation = .3f;
    [SerializeField] private int bountyCoinsCount = 10;
    [SerializeField] private int minBountyVlaue = 30;
    [SerializeField] private float dropeRange = 3f;
    [SerializeField] private LayerMask blockLayerMask;

     // 1 normal coin = 5   
    //  2 

    private NetworkVariable<int> totalCoins = new NetworkVariable<int>(0);

    private Collider2D[] raycastHit2Ds = new Collider2D[1];

    private const int Iterations_Numb = 40;
    private float _coinRad;


    public override void OnNetworkSpawn()
    {
        totalCoins.OnValueChanged += (old, newV) => OnCoinsValueChage?.Invoke(newV);

        if (!IsServer) return;
        
        playerHealth.OnPlayerDie += Health_OnPlayerDie;
        _coinRad = bountyCoinPrefap.GetComponent<CircleCollider2D>().radius;
       
    }



    public override void OnNetworkDespawn()
    {
        totalCoins.OnValueChanged -= (old, newV) => OnCoinsValueChage?.Invoke(newV);

        if (IsServer) return;
        
        playerHealth.OnPlayerDie -= Health_OnPlayerDie;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Coin>(out Coin coin)) return;

        int value = coin.Collect();

        OnCollectCoin?.Invoke(value);
        if (!IsServer) return;

        totalCoins.Value += value;

    }

    private void Health_OnPlayerDie()
    {
        SpwanBountyCoin();
    }

    private void SpwanBountyCoin()
    {
        int bountyValue = (int)(totalCoins.Value * bountyValueRation);
        if (bountyValue < minBountyVlaue) return;

        int bountyCoinValue = bountyValue / bountyCoinsCount;
        for (int i = 0; i < bountyCoinsCount; i++)
        {
            Vector2 pos;

            if (TryGetPosition(out pos))
            {
                BountyCoin bountyCoin = Instantiate(bountyCoinPrefap, pos, Quaternion.identity);
                bountyCoin.SetCoinValue(bountyCoinValue);
                bountyCoin.NetworkObject.Spawn(true);
            }
            else
            {
                BountyCoin bountyCoin = Instantiate(bountyCoinPrefap, transform.position, Quaternion.identity);
                bountyCoin.SetCoinValue(bountyCoinValue);
                bountyCoin.NetworkObject.Spawn(true);
            }

        }
    }

    private bool TryGetPosition(out Vector2 pos)
    {

        for (int i = 0; i < Iterations_Numb; i++)
        {

            Vector2 validPos = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * dropeRange;

            int hitNumb = Physics2D.OverlapCircleNonAlloc(validPos, _coinRad * 4, raycastHit2Ds, blockLayerMask);
            if (hitNumb == 0)
            {
                pos = validPos;
                return true;
            }

        }
        pos = Vector2.zero;
        return false;
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


}
