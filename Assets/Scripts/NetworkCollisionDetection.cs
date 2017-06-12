using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCollisionDetection : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        //CmdReduceHealth(10);
        if (collision.gameObject.tag == "projectile")
        {
            Debug.Log(gameObject.name + " got collided by " + collision.gameObject.name);
            EventManager.FireTakeDamage(13);
        }
        
    }
    //[Command]
    //void CmdReduceHealth(float dmg)
    //{
    //    Debug.Log("Took " + dmg + " damage");
    //}
}
