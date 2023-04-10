using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;

public class HealthSystem : NetworkBehaviour
{

    public event EventHandler OnDead;
    public event EventHandler OnDamaged;

    
    [SerializeField] [Networked(OnChanged = nameof(UpdateHealthUI))] public int health { get; set; } = 100;
    //[SerializeField] [Networked] public int health { get; set; } = 100;

    private int healthMax;


    public override void Spawned()
    {
        healthMax = health;
        
    }
    [Rpc]
    public void RPC_Damage (int damageAmount)
    {
        health -= damageAmount;

        if ( health < 0)
        {
            health = 0;
        }

        OnDamaged?.Invoke(this, EventArgs.Empty);

        if ( health == 0)
        {
            Die();
        }

        Debug.Log(health);
    }

    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return (float)health / healthMax;
    }

    //network health ui update
    public static void UpdateHealthUI(Changed<HealthSystem> changed)
    {
        changed.Behaviour.OnNetworkDamage();
    }

    private void OnNetworkDamage()
    {
        Debug.Log("UpdateHealthUI Network Event: " + OnDamaged);
        OnDamaged?.Invoke(this, EventArgs.Empty);
    }

}
