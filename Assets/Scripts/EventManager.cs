using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton <EventManager> { 

    protected EventManager() {
    }

    public delegate void PlayerStateChange(Enums.PlayerState state);
    public static event PlayerStateChange ChangePlayerState;
    public static void FirePlayerStateChange(Enums.PlayerState state) {
        if (ChangePlayerState != null) {
            //print("Player state change event fired.");
            ChangePlayerState(state);
        }
    }

    public delegate void CreateArrow(Vector3 position, Quaternion rotation, Vector3 forward, float force);
    public static event CreateArrow CreateArrowAction;
    public static void FireArrow(Vector3 position, Quaternion rotation, Vector3 forward, float force) {
        if (CreateArrowAction != null) {
            //print("Arrow fired.");
            CreateArrowAction(position, rotation, forward, force);
        }
    }

    public delegate void TakeDamage(float dmg, string teamName);
    public static event TakeDamage TakeDamageAction;
    public static void FireTakeDamage(float dmg, string teamName)
    {
        if (TakeDamageAction != null)
        {
            TakeDamageAction(dmg, teamName);
        }
    }

    protected override void Awake() {
        base.Awake();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
