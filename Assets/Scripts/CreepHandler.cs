using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public class CreepHandler : CreatureHandler {
    [SerializeField] private float _defaultCreepSpeed = 3.5f;
    [SerializeField] private CreepManager.CreepType _creepType;

    private NavMeshAgent _agent;
    private Vector3 _startPosition;
    private CreepManager _creepManager;

    public enum CreepAnimationTrigger {RunTrigger, IdleTrigger, DeathTrigger};

    private void Awake() {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
        _startPosition = transform.position;
    }

    private void OnDisable() {
        transform.position = _startPosition;
    }

    private void Update() {
        if (isServer && Input.GetKeyDown(KeyCode.C)) {
            SetIsDead(true);
        }
    }

    public void SetDestination(Vector3 pos) {
        _agent.SetDestination(pos);
        StartCoroutine(HasReached(gameObject));
        _agent.speed = _defaultCreepSpeed;
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

    public void SetAgentSpeed(float val) {
        if (val == 0)
            StopAllCoroutines();
        _agent.speed = val;
    }

    public CreepManager.CreepType GetCreepType() {
        return _creepType;
    }
}
