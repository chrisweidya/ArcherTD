﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class CreepManager : NetworkBehaviour {

    public static CreepManager Instance;
    
    [SerializeField] private Transform _legionCreepTargetPosTransform;
    //[SerializeField] private Transform _legionCreepSpawnPoint;
    [SerializeField] private GameObject _legionCreepPrefab;
    [SerializeField] private Stack<GameObject> _legionCreepsDead;
    [SerializeField] private List<GameObject> _legionCreeps;
    [SerializeField] private Transform _legionCreepsContainer;
    [SerializeField] private float _despawnSecs = 3f;

    public enum CreepType { Legion, Hellbourne};

    //timer 
    float timer= 3;
    private void Awake() {
        if(Instance != null) {
            Debug.LogWarning("Attempting to instantiate another CreepManager instance.");
            return;
        }
        Instance = this;
        _legionCreepsDead = new Stack<GameObject>();
    }
    
    void Start () {
    }

	void Update () {
        if (isServer && Input.GetKeyDown(KeyCode.K)) {
            SpawnCreep(CreepType.Legion);
        }
        timer -= Time.deltaTime;
        if (timer < 0) {
            timer = 5;
            SpawnCreep(CreepType.Legion);
            Debug.Log("SpawnCreep");
        }
    }
    
    private void SpawnCreep(CreepType type) {
        GameObject creep = GetCreepGO(type);
        if (creep == null)
            Debug.LogError("No creep initialized");
        if(type == CreepType.Legion)
            SetCreepDestination(creep, _legionCreepTargetPosTransform.position);
    }
    
    private void SetCreepDestination(GameObject creep, Vector3 targetPos) {
        CreepHandler handler = creep.GetComponent<CreepHandler>();
        handler.SetDestination(targetPos);
        ServerSetAnimationTrigger(creep, CreepHandler.CreepAnimationTrigger.RunTrigger);
        handler.StartOnReachRoutine();
    }

    private GameObject GetCreepGO(CreepType type) {
        Stack<GameObject> creepDeadStack = _legionCreepsDead;
        List<GameObject> creepList = _legionCreeps;
        GameObject creepPrefab = _legionCreepPrefab;
        Transform parentTransform = _legionCreepsContainer;
        GameObject creep = null;

        if(type == CreepType.Legion) {
            creepDeadStack = _legionCreepsDead;
            creepPrefab = _legionCreepPrefab;
            creepList = _legionCreeps;
            parentTransform = _legionCreepsContainer;
        }
        if(creepDeadStack.Count == 0) {
            creep = CreateCreep(creepList, creepPrefab, parentTransform, type);
        }
        else {
            creep = creepDeadStack.Pop();
            creep = Resurrect(creep);
        }
        return creep;
    }

    private GameObject CreateCreep(List<GameObject> creepList, GameObject creepPrefab, Transform parentTransform, CreepType type) {
        GameObject creep = Instantiate(creepPrefab, parentTransform);
        creep.GetComponent<CreepHandler>().SetCreepType(type);
        creepList.Add(creep);
        NetworkServer.Spawn(creep);
        creep = Resurrect(creep);

        return creep;
    }

    private GameObject Resurrect(GameObject creep) {
        creep.GetComponent<HealthNetwork>().ResetHealth();
        creep.SetActive(true);
        creep.GetComponent<CreepHandler>().RpcSetActive(true);
        return creep;
    }

    private IEnumerator Unspawn(GameObject creep) {
        yield return new WaitForSeconds(_despawnSecs);
        //NetworkServer.UnSpawn(creep);
        creep.SetActive(false);
        creep.GetComponent<CreepHandler>().RpcSetActive(false);
        _legionCreepsDead.Push(creep);
    }

    public void SetDeath(GameObject creep) {
        if (!isServer)
            return;
        CreepHandler creepHandler = creep.GetComponent<CreepHandler>();
        creepHandler.SetAgentSpeed(0);
        ServerSetAnimationTrigger(creep, CreepHandler.CreepAnimationTrigger.DeathTrigger);
        StartCoroutine(Unspawn(creep));
    }

    public void ServerSetAnimationTrigger(GameObject creep, CreepHandler.CreepAnimationTrigger trigger) {
        if (!isServer)
            return;
        creep.GetComponent<CreepHandler>().RpcSetAnimationTrigger(trigger.ToString());
    }
}
