using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public class CreepHandler : CreatureHandler {
    private NavMeshAgent _agent;
    private Vector3 _startPosition;
    private HealthNetwork _healthNetwork;

    public enum CreepAnimationTrigger {RunTrigger, IdleTrigger, DeathTrigger};

    [SerializeField] private float _defaultCreepSpeed = 3.5f;
    [SerializeField] private float _despawnTimeSecs = 3f;
    [SerializeField] private CreepManager.CreepType _creepType;
    [SerializeField] private float _acquisitionRadius;
    [SerializeField] private float _attackRadius;
    [SerializeField] private List<Transform> Waypoints;

    private float _updateBehaviourInterval = 0.5f;
    //Taken from navmesh radius
    private float _hitboxRadius;
    private Vector3 _targetWaypoint;
    private int _waypointsReached = -1;
    private List<GameObject> _enemyCreeps;

    private void Awake() {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
        _healthNetwork = GetComponent<HealthNetwork>();
        _startPosition = transform.position;
        _hitboxRadius = _agent.radius;
    }

    private void Start() {
        if (isServer) {
            _enemyCreeps = CreepManager.Instance.GetCreepList(CreepManager.CreepType.Legion);
            StartCoroutine(CreepMainUpdateLoop(_updateBehaviourInterval));
            //print("Started Coroutine from start");
        }
    }
    private void OnEnable() {
        if (isServer) {
            StartCoroutine(CreepMainUpdateLoop(_updateBehaviourInterval));
            //print("Started Coroutine from enable");
        }
    }

    private void OnDisable() {
        transform.position = _startPosition;
        _waypointsReached = -1;
    }

    private void Update() {
        if (isServer && Input.GetKeyDown(KeyCode.C)) {
            SetIsDead(true);
        }
    }

    private IEnumerator CreepMainUpdateLoop(float interval) {
        while (true) {
            print("updateloop");
            yield return new WaitForSeconds(interval);
            print(_enemyCreeps.Count);
            MoveToWaypoint();
        }
    }

    private void AcquireTarget() {

    }

    private void MoveToWaypoint() {
        if(_waypointsReached == -1) {
            _targetWaypoint = Waypoints[++_waypointsReached].position;
            SetDestination(_targetWaypoint);
        }
        else if (Utility.InRange(transform.position, _targetWaypoint, _acquisitionRadius)) {
            _waypointsReached++;
            if (_waypointsReached >= Waypoints.Count) {
                Debug.LogError("Waypoint array exceeded");
                return;
            }
            _targetWaypoint = Waypoints[_waypointsReached].position;
            SetDestination(_targetWaypoint);
        }
    }

    private void SetDestination(Vector3 pos) {
        _agent.SetDestination(pos);
        _agent.speed = _defaultCreepSpeed;
        CmdSetAnimationTrigger(CreepAnimationTrigger.RunTrigger.ToString());
    }

    public GameObject Ressurect() {
        _healthNetwork.ResetHealth();
        CmdSetActive(true);
        return gameObject;
    }

    public override void SetIsDead(bool isDead) {
        if(!isServer) {
            Debug.LogError("Non-server attempting to kill creeps");
            return;
        }
        base.SetIsDead(isDead);
        SetAgentSpeed(0);
        StopAllCoroutines();
        CmdSetAnimationTrigger(CreepAnimationTrigger.DeathTrigger.ToString());
        StartCoroutine(Despawn(_despawnTimeSecs));
    }   

    public void SetAgentSpeed(float val) {
        _agent.speed = val;
    }

    private IEnumerator Despawn(float secs) {
        yield return new WaitForSeconds(secs);
        CreepManager.Instance.AddInactiveCreepsToStackAfterDelay(gameObject, _creepType);
        CmdSetActive(false);
    }

    public CreepManager.CreepType GetCreepType() {
        return _creepType;
    }

    private void OnDrawGizmos() {
    }
}
