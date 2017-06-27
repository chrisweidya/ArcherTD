using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public class WolfHandler : NetworkBehaviour {
    private NavMeshAgent _agent;
    private Animator _animator;
    private NetworkAnimator _networkAnimator;
    private CreepManager _creepManager;

    public enum WolfAnimationTrigger {RunTrigger, IdleTrigger};

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _networkAnimator = GetComponent<NetworkAnimator>();
    }

    private void Start() {
        _creepManager = CreepManager.Instance;
    }

    public void SetDestination(Vector3 pos) {
        _agent.SetDestination(pos);
    }

    [ClientRpc]
    public void RpcSetAnimationTrigger(WolfAnimationTrigger trigger) {
        _animator.SetTrigger(trigger.ToString());
    }

    public void StartOnReachRoutine() {
        StartCoroutine(HasReached());
    }

    private IEnumerator HasReached() {
        while(_agent.pathPending) {
            yield return new WaitForSeconds(0.1f);
        }
        while (_agent.remainingDistance > float.Epsilon) {
            yield return new WaitForSeconds(0.1f);
        }
        _creepManager.ServerSetAnimationTrigger(gameObject, WolfAnimationTrigger.IdleTrigger);
        yield return null;
    }
}
