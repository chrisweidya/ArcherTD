using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties : MonoBehaviour {
    [SerializeField]
    private string team = "";
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
