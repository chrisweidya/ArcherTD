using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerProperties : NetworkBehaviour {
    [SyncVar]
    [SerializeField] private GameManager.Factions faction;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetFaction(GameManager.Factions faction) {
        this.faction = faction;
    }

    public GameManager.Factions GetFaction() {
        return faction;
    }
}
