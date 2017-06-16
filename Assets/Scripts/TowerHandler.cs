using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHandler : MonoBehaviour {

    [SerializeField]
    private float health = 100;
    [SerializeField]
    private string team;

	void Start () {
		
	}
	
	void Update () {
		
	}

    public void MinusHealth(float amt) {
        if (health <= 0) {
            Debug.Log("Tower already destroyed, HP: " + health);
        }
        else {
            Debug.Log("Tower health: " + health);
            health -= amt;
        }
    }
}
