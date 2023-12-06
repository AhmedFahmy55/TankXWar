using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("Refs")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpawnPivot;
    [SerializeField] private Transform clientProjectilePrefap;
    [SerializeField] private Transform serverProjectilePrefap;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private CoinCollector coinCollector;

    [Space, Header("Settings")]
    [SerializeField] private float bulletSpeed;

    [Tooltip("bullets/second")]
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleDuration;
    [SerializeField] private int shootCost;


    private Collider2D playerCollider;

    private bool _shouldFire;
    private float _TimeSinceLastShoot;
    private float _timeSinceLastFlashMuzzle;



    private void Awake()
    {
        playerCollider = GetComponentInChildren<Collider2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputReader.OnPlayreFire += (x) => _shouldFire = x;
    }

    private void Update()
    {
        if (_timeSinceLastFlashMuzzle > 0)
        {
            _timeSinceLastFlashMuzzle -= Time.deltaTime;
            if (_timeSinceLastFlashMuzzle <= 0) muzzleFlash.SetActive(false);
        }


        if (!IsOwner) return;
        
        HandleShooting();

    }

    private void HandleShooting()
    {
        if (_TimeSinceLastShoot > 0)
        {
            //on call down 
            _TimeSinceLastShoot -= Time.deltaTime;
            
        }
        else
        {
            if(!_shouldFire) return;
            if (coinCollector.GetCoins() < shootCost) return;
            SpwanProjectileServerRpc();
            SpawnProjectile(clientProjectilePrefap,OwnerClientId);
        }
    }

    private void SpawnProjectile(Transform projectilePrefap,ulong clientID)
    {
        _timeSinceLastFlashMuzzle = muzzleDuration;
        _TimeSinceLastShoot = 1 / fireRate;

        muzzleFlash.SetActive(true);

        Transform projectile = Instantiate(projectilePrefap, projectileSpawnPivot.position, Quaternion.identity);
        projectile.up = projectileSpawnPivot.up;

        if(projectile.TryGetComponent<DealDamageOnCollision>(out DealDamageOnCollision damager))
        {
            damager.Init(clientID);
        }

        Physics2D.IgnoreCollision(playerCollider, projectile.GetComponentInChildren<Collider2D>());

        if (projectile.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * bulletSpeed;
        }
        
    }


    [ServerRpc(RequireOwnership = false)]
    private void SpwanProjectileServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (coinCollector.GetCoins() < shootCost) return;

        coinCollector.SpendCoins(shootCost);
        SpawnProjectile(serverProjectilePrefap,serverRpcParams.Receive.SenderClientId);
        SpwanProjectileClientRpc(serverRpcParams.Receive.SenderClientId);
    }


    [ClientRpc]
    private void SpwanProjectileClientRpc(ulong clientID)
    {
        if (IsOwner) return;
        SpawnProjectile(clientProjectilePrefap, clientID);
    }
}
