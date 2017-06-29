using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public class CreepHandler : NetworkBehaviour {
    [SerializeField] private float _defaultCreepSpeed = 3.5f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private NetworkAnimator _networkAnimator;
    private CreepManager.CreepType _creepType;
    private CreepManager _creepManager;

    public enum CreepAnimationTrigger {RunTrigger, IdleTrigger, DeathTrigger};

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _networkAnimator = GetComponent<NetworkAnimator>();
    }

    private void Start() {
        //_creepManager = CreepManager.Instance;
    }

    private void OnEnable() {
        print("initialized");
        _creepManager = CreepManager.Instance;
    }

    private void Update() {
        //print(_creepManager);
        if (isServer && Input.GetKeyDown(KeyCode.C)) {
            TriggerDeath();
        }
    }

    public void SetDestination(Vector3 pos) {
        _agent.SetDestination(pos);
        _agent.speed = _defaultCreepSpeed;
    }

    [ClientRpc]
    public void RpcSetAnimationTrigger(CreepAnimationTrigger trigger) {
        _animator.SetTrigger(trigger.ToString());
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
        _creepManager.ServerSetAnimationTrigger(creepGO, CreepAnimationTrigger.IdleTrigger);
        yield return null;
    }

    private void TriggerDeath() {
        _creepManager.SetDeath(gameObject);
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
