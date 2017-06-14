using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCollisionDetection : MonoBehaviour {

    private PlayerProperties playerProps;
    private HealthNetwork health;
    private bool collided;
    public string team;
	// Use this for initialization
	void Start () {
        //playerProps = GetComponent<PlayerProperties>();
        collided = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        health = other.gameObject.GetComponent<HealthNetwork>();
        if ( health != null && team != other.gameObject.GetComponent<PlayerProperties>().GetTeam() && !collided)
        {
            Debug.Log("Collsion Event");
            EventManager.FireTakeDamage(13, health);
            collided = true;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        //CmdReduceHealth(10);  
        //if (collision.gameObject.tag == "Avatar")
        //{
        //    Debug.Log(playerProps.GetTeam()+gameObject.name + " got collided by " + collision.gameObject.name);
        //    EventManager.FireTakeDamage(13,playerProps.GetTeam());
        //}
        Debug.Log("Collision with " + collision.gameObject.name);
        health = collision.gameObject.GetComponent<HealthNetwork>();
        if (health != null && !collided)
        {
            Debug.Log("Collsion Event");
            EventManager.FireTakeDamage(13, health);
            collided = true;
        }
    }

}
