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

    public enum CreepType { Legion, Hellbourne};

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
            StartCoroutine(CreepSpawner(_creepSpawnSecs, _creepsInBatch, _creepIntervalSecs));
        }
    }

	void Update () {
        if (isServer && Input.GetKeyDown(KeyCode.K)) {
            SpawnCreep(CreepType.Legion);
        }
    }

    private IEnumerator CreepSpawner(float betweenBatchSecs, int numCreeps, float intervalSecs) {
        while (true) {
            yield return new WaitForSeconds(betweenBatchSecs);
            for (int i = 0; i < numCreeps; i++) {
                SpawnCreep(CreepType.Legion);
                SpawnCreep(CreepType.Hellbourne);
                yield return new WaitForSeconds(intervalSecs);
            }
        }
    }
    
    private void SpawnCreep(CreepType type) {
        if(!isServer) {
            Debug.LogError("Only Server can spawn creeps.");
            return;
        }
        GameObject creep = GetCreepGO(type);
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
        }
        else {
            creep = creepDeadStack.Pop();
            creep = creep.GetComponent<CreepHandler>().Ressurect();
        }
        return creep;
    }

    private GameObject CreateCreep(List<GameObject> creepList, GameObject creepPrefab, Transform parentTransform, CreepType type) {
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

    public void AddInactiveCreepsToStackAfterDelay(GameObject creep, CreepType creepType) {
        if(creepType == CreepType.Legion)
            StartCoroutine(AddInactiveCreepAfterDelay(creep, _legionCreepsDead, 2f));
        else if(creepType == CreepType.Hellbourne)
            StartCoroutine(AddInactiveCreepAfterDelay(creep, _hellbourneCreepsDead, 2f));
    }
}
