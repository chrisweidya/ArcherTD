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
    [SerializeField] private List<GameObject> _legionCreeps;
    [SerializeField] private Transform _legionCreepsContainer;
    private Stack<GameObject> _legionCreepsDead;
    [SerializeField] private Transform _hellbourneCreepTargetPosTransform;
    //[SerializeField] private Transform _legionCreepSpawnPoint;
    [SerializeField] private GameObject _hellbourneCreepPrefab;
    [SerializeField] private List<GameObject> _hellbourneCreeps;
    [SerializeField] private Transform _hellbourneCreepsContainer;
    private Stack<GameObject> _hellbourneCreepsDead;
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
        }
    }
    
    private void SpawnCreep(CreepType type) {
        if (!isServer) {
            Debug.LogError("Only Server can spawn creeps.");
            return;
        }
        GameObject creep = GetCreepGO(type);
        if(type == CreepType.Legion)
            SetCreepDestination(creep, _legionCreepTargetPosTransform.position);
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
        if(creep.GetComponent<CreepHandler>().GetCreepType() == CreepType.Legion)
            _legionCreepsDead.Push(creep);
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
