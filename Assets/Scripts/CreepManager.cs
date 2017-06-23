using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class CreepManager : NetworkBehaviour {

    //private NetworkConnection _serverConnection;
    [SerializeField] private WolfHandler wolfHandler;
    [SerializeField] private Transform pos;
    [SerializeField] private Transform _creepSpawnPoint;
    [SerializeField] private GameObject _creepPrefab;

    private void Awake() {
    }
    // Use this for initialization
    void Start () {
        //_serverConnection = GameManager.GetServerConnection();
        //print(_serverConnection);
        GameObject creep = Instantiate(_creepPrefab);
        creep.transform.position = _creepSpawnPoint.position;
        NetworkServer.Spawn(creep);
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
