using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class HealthNetwork : NetworkBehaviour {

    [SerializeField]
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
    // Use this for initialization
    void Start () {
        playerProps = GetComponent<PlayerProperties>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void ReduceHealth(float dmg, string name)
    {
        if (name == playerProps.GetTeam())
        {
            CmdReduceHealth(dmg);
        }
    }
    [Command]
    private void CmdReduceHealth(float dmg)
    {
        //currentHealth -= dmg;
        Debug.Log("CMD:" + dmg +" health: " + currentHealth);
        RpcReduceHealth(dmg);
    }

    [ClientRpc]
    private void RpcReduceHealth(float dmg)
    {
        currentHealth -= dmg;
        hpBar.SetHealthBar(currentHealth);
        Debug.Log("Rpc: healthLeft: " + currentHealth);
    }
}
