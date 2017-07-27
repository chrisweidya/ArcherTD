using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class CreepManager : NetworkBehaviour {

    public static CreepManager Instance;

    [SerializeField] private GameObject _legionCreepPrefab;
    [SerializeField] private Transform _legionCreepsContainer;
    [SerializeField] private List<GameObject> _legionCreeps;
    private Stack<GameObject> _legionCreepsDead;
    
    [SerializeField] private GameObject _hellbourneCreepPrefab;
    [SerializeField] private Transform _hellbourneCreepsContainer;
    [SerializeField] private List<GameObject> _hellbourneCreeps;
    private Stack<GameObject> _hellbourneCreepsDead;

    [SerializeField] private int _creepsInBatch;
    [SerializeField] private float _creepSpawnSecs;
    [SerializeField] private float _creepIntervalSecs = 0.5f;

    private void Awake() {
        if(Instance != null) {
            Debug.LogWarning("Attempting to instantiate another CreepManager instance.");
            return;
        }
        Instance = this;
        _legionCreepsDead = new Stack<GameObject>();
        _hellbourneCreepsDead = new Stack<GameObject>();
    }

    private void OnEnable() {
        EventManager.GameEndAction += StopCoroutinesOnGameEnd;
    }

    private void OnDisable() {
        EventManager.GameEndAction -= StopCoroutinesOnGameEnd;
    }

    private void Start() {
        if (isServer) {
            StartCoroutine(CreepSpawner(_creepSpawnSecs, _creepsInBatch, _creepIntervalSecs));
        }
    }

	void Update () {
        if (isServer && Input.GetKeyDown(KeyCode.K)) {
            SpawnCreep(GameManager.Factions.Legion);
        }
    }

    private IEnumerator CreepSpawner(float betweenBatchSecs, int numCreeps, float intervalSecs) {
        while (true) {
            yield return new WaitForSeconds(betweenBatchSecs);
            for (int i = 0; i < numCreeps; i++) {
                SpawnCreep(GameManager.Factions.Legion);
                SpawnCreep(GameManager.Factions.Hellbourne);
                //Time interval between each creep in a batch
                yield return new WaitForSeconds(intervalSecs);
            }
        }
    }
    
    private void SpawnCreep(GameManager.Factions type) {
        if(!isServer) {
            Debug.LogError("Only Server can spawn creeps.");
            return;
        }
        GameObject creep = GetCreepGO(type);
    }

    private GameObject GetCreepGO(GameManager.Factions type) {
        Stack<GameObject> creepDeadStack = _legionCreepsDead;
        List<GameObject> creepList = _legionCreeps;
        GameObject creepPrefab = _legionCreepPrefab;
        Transform parentTransform = _legionCreepsContainer;
        GameObject creep = null;

        if(type == GameManager.Factions.Legion) {
            creepDeadStack = _legionCreepsDead;
            creepPrefab = _legionCreepPrefab;
            creepList = _legionCreeps;
            parentTransform = _legionCreepsContainer;
        }
        else if(type == GameManager.Factions.Hellbourne) {
            creepDeadStack = _hellbourneCreepsDead;
            creepPrefab = _hellbourneCreepPrefab;
            creepList = _hellbourneCreeps;
            parentTransform = _hellbourneCreepsContainer;
        }
        if(creepDeadStack.Count == 0) {
            creep = CreateCreep(creepList, creepPrefab, parentTransform, type);
        }
        else {
            creep = creepDeadStack.Pop();
            creep = creep.GetComponent<CreepHandler>().Ressurect();
        }
        return creep;
    }

    private GameObject CreateCreep(List<GameObject> creepList, GameObject creepPrefab, Transform parentTransform, GameManager.Factions type) {
        GameObject creep = Instantiate(creepPrefab, parentTransform);
        creepList.Add(creep);
        NetworkServer.Spawn(creep);
        creep = creep.GetComponent<CreepHandler>().Ressurect();
        return creep;
    }

    private IEnumerator AddInactiveCreepAfterDelay(GameObject creep, Stack<GameObject> creepStack, float delaySecs) {
        yield return new WaitForSeconds(delaySecs);
        creepStack.Push(creep);
    }

    //Server
    private void StopCoroutinesOnGameEnd(GameManager.Factions faction) {
        if (!isServer)
            return;
        StopAllCoroutines();
    }

    public void AddInactiveCreepsToStackAfterDelay(GameObject creep, GameManager.Factions creepType) {
        if(creepType == GameManager.Factions.Legion)
            StartCoroutine(AddInactiveCreepAfterDelay(creep, _legionCreepsDead, 2f));
        else if(creepType == GameManager.Factions.Hellbourne)
            StartCoroutine(AddInactiveCreepAfterDelay(creep, _hellbourneCreepsDead, 2f));
    }
    
    public IList<GameObject> GetCreepList(GameManager.Factions type) {
        if (type == GameManager.Factions.Legion)
            return _legionCreeps.AsReadOnly();
        else if (type == GameManager.Factions.Hellbourne)
            return _hellbourneCreeps.AsReadOnly();
        return null;
    }
}
