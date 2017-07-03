using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class CreepManager : NetworkBehaviour {

    public static CreepManager Instance;
    
    [SerializeField] private Transform _legionCreepTargetPosTransform;
    //[SerializeField] private Transform _legionCreepSpawnPoint;
    [SerializeField] private GameObject _legionCreepPrefab;
    [SerializeField] private Transform _legionCreepsContainer;
    [SerializeField] private List<GameObject> _legionCreeps;
    private Stack<GameObject> _legionCreepsDead;
    [SerializeField] private Transform _hellbourneCreepTargetPosTransform;
    //[SerializeField] private Transform _legionCreepSpawnPoint;
    [SerializeField] private GameObject _hellbourneCreepPrefab;
    [SerializeField] private Transform _hellbourneCreepsContainer;
    [SerializeField] private List<GameObject> _hellbourneCreeps;
    private Stack<GameObject> _hellbourneCreepsDead;

    [SerializeField] private int _creepsInBatch = 3;
    [SerializeField] private float _creepSpawnSecs = 20f;
    [SerializeField] private float _despawnSecs = 3f;

    public enum CreepType { Legion, Hellbourne};

    //timer 
    private void Awake() {
        if(Instance != null) {
            Debug.LogWarning("Attempting to instantiate another CreepManager instance.");
            return;
        }
        Instance = this;
        _legionCreepsDead = new Stack<GameObject>();
        _hellbourneCreepsDead = new Stack<GameObject>();
    }
    private void Start() {
        if (isServer) {
            StartCoroutine(CreepSpawner(_creepSpawnSecs, _creepsInBatch));
        }
    }

	void Update () {
        if (isServer && Input.GetKeyDown(KeyCode.K)) {
            SpawnCreep(CreepType.Legion);
        }
    }

    private IEnumerator CreepSpawner(float secs, int numCreeps) {
        while (true) {
            yield return new WaitForSeconds(secs);
            for (int i = 0; i < numCreeps; i++) {
                SpawnCreep(CreepType.Legion);
                SpawnCreep(CreepType.Hellbourne);
            }
        }
    }
    
    private void SpawnCreep(CreepType type) {
        if(!isServer) {
            Debug.LogError("Only Server can spawn creeps.");
            return;
        }
        GameObject creep = GetCreepGO(type);
        if(type == CreepType.Legion)
            SetCreepDestination(creep, _legionCreepTargetPosTransform.position);
        else if(type == CreepType.Hellbourne)
            SetCreepDestination(creep, _hellbourneCreepTargetPosTransform.position);
    }
    
    private void SetCreepDestination(GameObject creep, Vector3 targetPos) {
        CreepHandler handler = creep.GetComponent<CreepHandler>();
        handler.SetDestination(targetPos);
        ServerSetAnimationTrigger(creep, CreepHandler.CreepAnimationTrigger.RunTrigger);
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
        else if(type == CreepType.Hellbourne) {
            creepDeadStack = _hellbourneCreepsDead;
            creepPrefab = _hellbourneCreepPrefab;
            creepList = _hellbourneCreeps;
            parentTransform = _hellbourneCreepsContainer;
        }
        if(creepDeadStack.Count == 0) {
            creep = CreateCreep(creepList, creepPrefab, parentTransform, type);
            Debug.Log("New Creep " + creepDeadStack.Count);
        }
        else {
            Debug.Log("Ressurect Creep " + creepDeadStack.Count);
            creep = creepDeadStack.Pop();
            Debug.Log("afterpop " + creepDeadStack.Count + " " + creep);
            creep = Resurrect(creep);
        }
        return creep;
    }

    private GameObject CreateCreep(List<GameObject> creepList, GameObject creepPrefab, Transform parentTransform, CreepType type) {
        GameObject creep = Instantiate(creepPrefab, parentTransform);
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
        if (creep.GetComponent<CreepHandler>().GetCreepType() == CreepType.Legion)
            _legionCreepsDead.Push(creep);
        else if (creep.GetComponent<CreepHandler>().GetCreepType() == CreepType.Hellbourne)
            _hellbourneCreepsDead.Push(creep);
    }

    public void SetDeath(GameObject creep) {
        if (!isServer) {
            Debug.LogError("Set Death not called by server");
            return;
        }
        CreepHandler creepHandler = creep.GetComponent<CreepHandler>();
        creepHandler.SetAgentSpeed(0);
        ServerSetAnimationTrigger(creep, CreepHandler.CreepAnimationTrigger.DeathTrigger);
        StartCoroutine(Unspawn(creep));
    }

    public void ServerSetAnimationTrigger(GameObject creep, CreepHandler.CreepAnimationTrigger trigger) {
        if (!isServer) {
            Debug.LogError("Server creep animation trigger not called by server");
            return;
        }
        creep.GetComponent<CreepHandler>().RpcSetAnimationTrigger(trigger.ToString());
    }
}
