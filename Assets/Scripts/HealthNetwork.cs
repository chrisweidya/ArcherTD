using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class HealthNetwork : NetworkBehaviour {

    [SerializeField]
    private float currentHealth;
    [SerializeField]
    private float maxHealth;

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
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void ReduceHealth(float dmg)
    {
        if (isLocalPlayer)
        {
            CmdReduceHealth(dmg);
        }
    }
    [Command]
    private void CmdReduceHealth(float dmg)
    {
        currentHealth -= dmg;
        Debug.Log("CMD:" + dmg +" health: " + currentHealth);
        RpcReduceHealth(dmg);
    }

    [ClientRpc]
    private void RpcReduceHealth(float dmg)
    {
        
        Debug.Log("Rpc " + dmg + " health: " + currentHealth);
    }
}
