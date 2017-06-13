using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerProperties : NetworkBehaviour {
    [SyncVar]
    [SerializeField]
    private string team;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetTeam(string team) {
        this.team = team;
    }

    public string GetTeam() {
        return team;
    }
}
