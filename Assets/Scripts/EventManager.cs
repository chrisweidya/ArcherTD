using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EventManager : Singleton <EventManager> { 

    protected EventManager() {
    }

    public delegate void PlayerStateChange(PlayerHandler.PlayerState state, NetworkInstanceId netId);
    public static event PlayerStateChange ChangePlayerState;
    public static void FirePlayerStateChange(PlayerHandler.PlayerState state, NetworkInstanceId netId) {
        if (ChangePlayerState != null) {
            //print("Player state change event fired.");
            ChangePlayerState(state, netId);
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

    public delegate void TakeDamage(float dmg, NetworkInstanceId netId);
    public static event TakeDamage TakeDamageAction;
    public static void FireTakeDamage(float dmg, NetworkInstanceId netId)
    {
        if (TakeDamageAction != null)
        {
            TakeDamageAction(dmg, netId);
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
