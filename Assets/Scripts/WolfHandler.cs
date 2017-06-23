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

    private const string ANIM_RUN_TRIGGER = "RunTrigger";

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    public void SetDestination(Vector3 pos) {
        _agent.SetDestination(pos);
        _animator.SetTrigger(ANIM_RUN_TRIGGER);
        StartCoroutine(HasReached());
    }

    private IEnumerator HasReached() {

        print(_agent.remainingDistance);
        while(_agent.pathPending) {
            print("path pending");
            yield return new WaitForSeconds(0.1f);
        }
        while (_agent.remainingDistance > float.Epsilon) {
            print(_agent.remainingDistance);
            yield return new WaitForSeconds(0.1f);
        }
        print("reached");
        yield return null;
    }
}
