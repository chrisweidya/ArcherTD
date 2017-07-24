using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillReward : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Reward(GameObject enemyHero) {
        enemyHero.GetComponent<PlayerHandler>().PowerGain();
    }
}
