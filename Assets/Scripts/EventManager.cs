﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EventManager : Singleton <EventManager> { 
    
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

    public delegate void DoDamage(float dmg, NetworkInstanceId netId);
    public static event DoDamage DoDamageAction;
    public static void FireDoDamage(float dmg, NetworkInstanceId netId)
    {
        if (DoDamageAction != null)
        {
            DoDamageAction(dmg, netId);
        }
    }

    public delegate void PlayerDeath(NetworkInstanceId deadNetId, bool isDead);
    public static event PlayerDeath PlayerDeathAction;
    public static void FirePlayerDeath(NetworkInstanceId deadNetId, bool isDead) {
        if (PlayerDeathAction != null && !GameManager.GameWon) {
            PlayerDeathAction(deadNetId, isDead);
            //GameManager.GameWon = true;   
        }
    }

    public delegate void GameEnd(NetworkInstanceId winnerId);
    public static event GameEnd GameEndAction;
    public static void FireGameEnd(NetworkInstanceId winnerId) {
        if (GameEndAction != null && !GameManager.GameWon) {
            GameEndAction(winnerId);
            GameManager.GameWon = true;
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
