using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class CreatureHandler : NetworkBehaviour {


    [SyncVar(hook = "OnIsDeadHook")]
    private bool isDead = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void SetIsDead(bool isDead) {
        this.isDead = isDead;
    }

    public bool GetIsDead() {
        return isDead;
    }
    public void ResetDeath() {
        isDead = false;
    }
    private void OnIsDeadHook(bool dead) {
        isDead = dead;
        OnIsDead(dead);
    }
    public virtual void OnIsDead(bool isDead) {

    }


}
