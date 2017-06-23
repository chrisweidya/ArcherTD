using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class CreepManager : NetworkBehaviour {

    private NetworkConnection _serverConnection;
    [SerializeField]
    private WolfHandler wolfHandler;
    [SerializeField]
    private Transform pos;
    

    private void Awake() {
    }
    // Use this for initialization
    void Start () {
        //_serverConnection = GameManager.GetServerConnection();
        //print(_serverConnection);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.K)) {
            CmdSetWolfDestination(pos.position);
        }
    }
    [Command]
    private void CmdSetWolfDestination(Vector3 targetPos) {
        wolfHandler.SetDestination(targetPos);
    }
}
