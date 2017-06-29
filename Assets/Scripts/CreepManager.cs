using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class CreepManager : NetworkBehaviour {

    public static CreepManager Instance;

    //private NetworkConnection _serverConnection;
    [SerializeField] private Transform _legionCreepTargetPosTransform;
    [SerializeField] private Transform _legionCreepSpawnPoint;
    [SerializeField] private GameObject _legionCreepPrefab;
    [SerializeField] private Stack<GameObject> _legionCreepsDead;
    [SerializeField] private List<GameObject> _legionCreeps;
    [SerializeField] private Transform _legionCreepsContainer;
    [SerializeField] private float _despawnSecs = 3f;

    public enum CreepType { Legion, Hellbourne};

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
    }
    
    private void SpawnCreep(CreepType type) {
        GameObject creep = GetCreepGO(type);
        if(type == CreepType.Legion)
            SetCreepDestination(creep, _legionCreepTargetPosTransform.position);
    }
    
    private void SetCreepDestination(GameObject creep, Vector3 targetPos) {
        CreepHandler handler = creep.GetComponent<CreepHandler>();
        handler.SetDestination(targetPos);
        SetAnimationTrigger(creep, CreepHandler.CreepAnimationTrigger.RunTrigger);
        handler.StartOnReachRoutine();
    }
    
    private void SetAnimationTrigger(GameObject creep, CreepHandler.CreepAnimationTrigger trigger) {
        creep.GetComponent<CreepHandler>().RpcSetAnimationTrigger(trigger);
    }

    private GameObject GetCreepGO(CreepType type) {
        Stack<GameObject> creepDeadStack = _legionCreepsDead;
        List<GameObject> creepList = _legionCreeps;
        Vector3 spawnPosition = _legionCreepSpawnPoint.position;
        GameObject creepPrefab = _legionCreepPrefab;
        Transform parentTransform = _legionCreepsContainer;
        GameObject creep = null;

        if(type == CreepType.Legion) {
            creepDeadStack = _legionCreepsDead;
            spawnPosition = _legionCreepSpawnPoint.position;
            creepPrefab = _legionCreepPrefab;
            creepList = _legionCreeps;
            parentTransform = _legionCreepsContainer;
        }
        if(creepDeadStack.Count == 0) {
            creep = CreateCreep(creepList, creepPrefab, parentTransform, spawnPosition, type);
        }
        else {
            creep = creepDeadStack.Pop();
            creep = Resurrect(creep, spawnPosition);
        }
        return creep;
    }

    private GameObject CreateCreep(List<GameObject> creepList, GameObject creepPrefab, Transform parentTransform, Vector3 position, CreepType type) {
        GameObject creep = Instantiate(creepPrefab);
        creep.transform.parent = parentTransform;
        creep.GetComponent<CreepHandler>().SetCreepType(type);
        creepList.Add(creep);
        creep = Resurrect(creep, position);

        return creep;
    }

    private GameObject Resurrect(GameObject creep, Vector3 position) {
        creep.SetActive(true);
        creep.transform.position = position;
        NetworkServer.Spawn(creep);
        return creep;
    }

    private IEnumerator Unspawn(GameObject creep) {
        yield return new WaitForSeconds(_despawnSecs);
        NetworkServer.UnSpawn(creep);
        creep.SetActive(false);
        _legionCreepsDead.Push(creep);
    }

    public void SetDeath(GameObject creep) {
        if (!isServer)
            return;
        CreepHandler creepHandler = creep.GetComponent<CreepHandler>();
        creepHandler.SetAgentSpeed(0);
        creepHandler.RpcSetAnimationTrigger(CreepHandler.CreepAnimationTrigger.DeathTrigger);
        StartCoroutine(Unspawn(creep));
    }

    public void ServerSetAnimationTrigger(GameObject creep, CreepHandler.CreepAnimationTrigger trigger) {
        if (!isServer)
            return;
        SetAnimationTrigger(creep, trigger);
    }
}
