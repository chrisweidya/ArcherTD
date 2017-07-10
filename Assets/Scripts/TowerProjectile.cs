using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : MonoBehaviour {

    public TowerHandler towerParent;
    public GameObject currentTarget;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        CheckForCollision();
	}

    private void CheckForCollision() {
        if (Vector3.Distance(transform.position, currentTarget.transform.position) < 1) {
            towerParent.DoDamage(currentTarget);
            Destroy(gameObject);
        }
    }
}
