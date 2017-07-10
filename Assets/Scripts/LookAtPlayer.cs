using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour {

    public string  str;
    public GameObject target;
	// Use this for initialization
	void Start () {
        if(str != null)
        target = GameObject.Find(str);
	}
	
	// Update is called once per frame
	void Update () {
        if (target != null) {
            gameObject.transform.LookAt(target.transform);
        }
    }
}
