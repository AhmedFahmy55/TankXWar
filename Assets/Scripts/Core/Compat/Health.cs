using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{

    public event Action<ulong> OnPlayerDie;
    public event Action<int> OnPlayerHealthChange;

    [field: SerializeField] public int MaxHealth { get; private set; }


    private NetworkVariable<int> _currentHealth = new NetworkVariable<int>(0);

    private bool _isDead;



    public override void OnNetworkSpawn()
    {
        _currentHealth.OnValueChanged += OnHealthValueChange;

        if (!IsServer) return;
        _currentHealth.Value = MaxHealth;
    }
    public override void OnNetworkDespawn()
    {
        _currentHealth.OnValueChanged -= OnHealthValueChange;

    }
    private void OnHealthValueChange(int previousValue, int newValue)
    {
        OnPlayerHealthChange?.Invoke(newValue);
    }

    public void DealDamage(int damageAmount,ulong damagerID)
    {
        ModifyHealth(-damageAmount,damagerID);
    }

    public void Heal(int healAmount)
    {
        ModifyHealth(healAmount,0);
    }

    private void ModifyHealth(int amount,ulong damagerID)
    {
        if (_isDead) return;

        int health = _currentHealth.Value + amount;
        health = Mathf.Clamp(health, 0, MaxHealth);
        _currentHealth.Value = health;

        if (health == 0)
        {
            OnPlayerDie?.Invoke(damagerID);
        }

    }
    public void ResetHealth()
    {
        _currentHealth.Value = MaxHealth;
        _isDead = false;
    }
    public bool IsFullHealth()
    {
        return _currentHealth.Value == MaxHealth;
    }
}
