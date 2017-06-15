using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class HealthNetwork : NetworkBehaviour {

    [SerializeField]
    [SyncVar(hook = "OnHealthUpdate")]
    private float currentHealth;
    [SerializeField]
    private float maxHealth;

    [SyncVar]
    bool isDead = false;

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
    // Use this for initialization
    void Start () {
        playerProps = GetComponent<PlayerProperties>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ReduceHealth(float dmg, NetworkInstanceId victimNetId)
    {
        if (isLocalPlayer) {
            print("minus" + victimNetId);
            if (victimNetId == this.netId) {

            }

            CmdReduceHealth(dmg, victimNetId, this.netId);
        }
    }

    [Command]
    private void CmdReduceHealth(float dmg, NetworkInstanceId victimNetId, NetworkInstanceId killerNetId)
    {
        GameObject playerGO = NetworkServer.FindLocalObject(victimNetId);
        
        PlayerHandler playerHandler = playerGO.GetComponent<PlayerHandler>();
        if (!playerHandler.GetIsDead()) {
            HealthNetwork affectedHealth = playerGO.GetComponent<HealthNetwork>();
            affectedHealth.currentHealth -= dmg;
            Debug.Log("Cmd health " + currentHealth);
            if (affectedHealth.currentHealth <= 0) {
                playerHandler.SetIsDead(true);
            }
        }
    }
    

    //[ClientRpc]
    //private void RpcReduceHealth(float dmg)
    //{
    //    hpBar.SetHealthBar(currentHealth);
      
    //}
    
    void OnHealthUpdate(float health)
    {
        Debug.Log("UI health " + health);
        hpBar.SetHealthBar(health);
        currentHealth = health;
        Debug.Log("Rpc: healthLeft: " + currentHealth);
    }
    
}
