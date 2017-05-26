using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton <EventManager> { 

    protected EventManager() {
    }

    public delegate void PlayerStateChange(Enums.PlayerState state);
    public static event PlayerStateChange changePlayerState;

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
