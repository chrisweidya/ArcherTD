using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent))]
public class WolfHandler : NetworkBehaviour {
    private NavMeshAgent _agent;
    [SerializeField]
    private Transform _targetDestination;

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
    }
    
	void Start () {
		
	}
	
	void Update () {
	}

    public void SetDestination(Vector3 pos) {
        _agent.SetDestination(pos);
    }
}
