using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCollisionDetection : MonoBehaviour {

    private PlayerProperties playerProps;

	// Use this for initialization
	void Start () {
        playerProps = GetComponent<PlayerProperties>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        //CmdReduceHealth(10);
        if (collision.gameObject.tag == "projectile")
        {
            Debug.Log(playerProps.GetTeam()+gameObject.name + " got collided by " + collision.gameObject.name);
            EventManager.FireTakeDamage(13,playerProps.GetTeam());
        }
        
    }

}
