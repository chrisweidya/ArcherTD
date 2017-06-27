using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class CreepManager : NetworkBehaviour {

    public static CreepManager Instance;

    //private NetworkConnection _serverConnection;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Transform _creepSpawnPoint;
    [SerializeField] private GameObject _creepPrefab;

    private void Awake() {
        if(Instance != null) {
            Debug.LogWarning("Attempting to instantiate another CreepManager instance.");
            return;
        }
        Instance = this;
    }


    // Use this for initialization
    void Start () {
        //_serverConnection = GameManager.GetServerConnection();
        //print(_serverConnection);
        //GameObject creep = Instantiate(_creepPrefab);
        //creep.transform.position = _creepSpawnPoint.position;
        //NetworkServer.Spawn(creep);
        //wolfHandler = 
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.K)) {
            CmdSpawnWolf();
        }
    }

    [Command]
    private void CmdSpawnWolf() {
        GameObject wolf = Instantiate(_creepPrefab);
        wolf.transform.position = _creepSpawnPoint.position;
        NetworkServer.Spawn(wolf);
        CmdSetWolfDestination(wolf, _targetTransform.position);
    }

    [Command]
    private void CmdSetWolfDestination(GameObject wolf, Vector3 targetPos) {
        WolfHandler handler = wolf.GetComponent<WolfHandler>();
        handler.SetDestination(targetPos);
        CmdSetAnimationTrigger(wolf, WolfHandler.WolfAnimationTrigger.RunTrigger);
        handler.StartOnReachRoutine();
    }

    [Command]
    private void CmdSetAnimationTrigger(GameObject wolf, WolfHandler.WolfAnimationTrigger trigger) {
        wolf.GetComponent<WolfHandler>().RpcSetAnimationTrigger(trigger);
    }

    public void ServerSetAnimationTrigger(GameObject wolf, WolfHandler.WolfAnimationTrigger trigger) {
        CmdSetAnimationTrigger(wolf, trigger);
    }
}
