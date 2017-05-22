using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class CreepMovementNetworkTest : MonoBehaviour {

    public Transform trans;
    public Transform trans2;
    private bool point1;
    public NavMeshAgent nav;
    public Animator anim;
    private float timer;
	// Use this for initialization
	void Start () {
        nav.SetDestination(trans.position);
        anim.SetBool("WalkBool", true);
        point1 = true;
        timer = 9f;
	}
	
	// Update is called once per frame
	void Update () {

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            if (point1)
            {
                nav.SetDestination(trans2.position);
            }
            else
            {
                nav.SetDestination(trans.position);

            }
            point1 = !point1;
            timer = 9f;
        }

       

    }


}
