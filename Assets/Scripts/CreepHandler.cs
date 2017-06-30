using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public class CreepHandler : CreatureHandler {
    [SerializeField] private float _defaultCreepSpeed = 3.5f;

    private NavMeshAgent _agent;
    private CreepManager.CreepType _creepType;
    private Vector3 _startPosition;
    private CreepManager _creepManager;

    public enum CreepAnimationTrigger {RunTrigger, IdleTrigger, DeathTrigger};

    private void Awake() {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
        _startPosition = transform.position;
    }

    private void Start() {
        //_creepManager = CreepManager.Instance;
        //print(_startPosition);
    }

    private void OnEnable() {
        print("creep onenable");
    }

    private void OnDisable() {
        transform.position = _startPosition;
    }

    private void Update() {
        //print(_creepManager);
        if (isServer && Input.GetKeyDown(KeyCode.C)) {
            SetIsDead(true);
        }
    }

    public void SetDestination(Vector3 pos) {
        _agent.SetDestination(pos);
        _agent.speed = _defaultCreepSpeed;
    }

    [ClientRpc]
    public void RpcSetActive(bool val) {
        gameObject.SetActive(val);
    }

    public void StartOnReachRoutine() {
        StartCoroutine(HasReached(gameObject));
    }

    private IEnumerator HasReached(GameObject creepGO) {
        while(_agent.pathPending) {
            yield return new WaitForSeconds(0.1f);
        }
        while (_agent.remainingDistance > float.Epsilon) {
            yield return new WaitForSeconds(0.1f);
        }
        CreepManager.Instance.ServerSetAnimationTrigger(creepGO, CreepAnimationTrigger.IdleTrigger);
        yield return null;
    }

    public override void SetIsDead(bool isDead) {
        base.SetIsDead(isDead);
        CreepManager.Instance.SetDeath(gameObject);
    }

    //base class sync var hook
    public override void OnIsDead(bool isDead) {
    }

    public void SetAgentEnabled(bool val) {
        if (!val)
            StopAllCoroutines();
        _agent.enabled = val;
    }

    public void SetAgentSpeed(float val) {
        if (val == 0)
            StopAllCoroutines();
        _agent.speed = val;
    }

    public CreepManager.CreepType GetCreepType() {
        return _creepType;
    }

    public void SetCreepType(CreepManager.CreepType type) {
        _creepType = type;
    }

}
