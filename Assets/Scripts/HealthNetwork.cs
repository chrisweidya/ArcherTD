﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class HealthNetwork : NetworkBehaviour {

    [SerializeField]
    [SyncVar(hook = "OnHealthUpdate")]
    private float currentHealth;
    [SerializeField]
    private float maxHealth;

    public HealthBarUI hpBar;
    private PlayerProperties playerProps;

    private void OnEnable()
    {
        EventManager.TakeDamageAction += ReduceHealth;

    }

    private void OnDisable()
    {
        EventManager.TakeDamageAction -= ReduceHealth;
    }

    void Start () {
        playerProps = GetComponent<PlayerProperties>();
	}
	
    public void ReduceHealth(float dmg, NetworkInstanceId victimNetId)
    {
        if (isLocalPlayer) {
            print("minus" + victimNetId);
            CmdReduceHealth(dmg, victimNetId, this.netId);
        }
    }

    [Command]
    private void CmdReduceHealth(float dmg, NetworkInstanceId victimNetId, NetworkInstanceId killerNetId)
    {
        GameObject playerGO = NetworkServer.FindLocalObject(victimNetId);
        CreatureHandler killerPlayerHandler = NetworkServer.FindLocalObject(killerNetId).GetComponent<CreatureHandler>();
        CreatureHandler victimPlayerHandler = playerGO.GetComponent<CreatureHandler>();

        if (!killerPlayerHandler.GetIsDead() && !victimPlayerHandler.GetIsDead()) {
            HealthNetwork affectedHealth = playerGO.GetComponent<HealthNetwork>();
            affectedHealth.currentHealth -= dmg;
            Debug.Log("Cmd health " + currentHealth);
            if (affectedHealth.currentHealth <= 0) {
                victimPlayerHandler.SetIsDead(true);
            }
        }
    }
    
    void OnHealthUpdate(float health)
    {
        Debug.Log("UI health " + health);
        hpBar.SetHealthBar(health,maxHealth);
        currentHealth = health;
        Debug.Log("Rpc: healthLeft: " + currentHealth);
    }

    public void ResetHealth() {
        currentHealth = maxHealth;
        GetComponent<CreatureHandler>().ResetDeath();
    }
    
}
