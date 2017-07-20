using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent))]

public class CreepHandler : CreatureHandler {
    private NavMeshAgent _agent;
    private Vector3 _startPosition;

    public enum CreepAnimationTrigger { RunTrigger, IdleTrigger, DeathTrigger, AttackTrigger };
    private enum CreepAnimationState { Idle, Attack, Run, Death }
    public enum CreepState { Attacking, Searching, Running, Idle };

    [SerializeField] private float _defaultCreepSpeed;
    [SerializeField] private float _attackIntervalSecs;
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _creepAttackDamage;
    [SerializeField] private float _towerAttackDamage;
    [SerializeField] private float _despawnTimeSecs;
    [SerializeField] private GameManager.Factions _creepType;
    [SerializeField] private float _waypointDetectionRadius;
    [SerializeField] private float _acquisitionRadius;
    [SerializeField] private float _attackRadius;
    [SerializeField] private List<Transform> Waypoints;

    private float _updateBehaviourInterval = 0.25f;
    private float _hitboxRadius; //NavMeshAgent radius
    private static float _closesDistanceSquared;

    public Vector3 _targetWaypoint;
    private int _waypointsReached = 0;
    private IList<GameObject> _enemyCreeps;
    private GameObject _enemyTower;
    private GameObject _enemyHero;
    public GameObject _targetEnemy;
    public CreepState _currentState;
    private Coroutine _currentCoroutine;

    public bool agentStopped = false;
    public Vector3 currentTargetPos;

    private void Awake() {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
        _startPosition = transform.position;
        _radius = _agent.radius;
    }

    private void Start() {
        if (isServer) {
            _closesDistanceSquared = _acquisitionRadius * _acquisitionRadius;
            if (_creepType == GameManager.Factions.Hellbourne) {
                _enemyTower = TowerManager.Instance.GetTower(GameManager.Factions.Legion);
                _enemyCreeps = CreepManager.Instance.GetCreepList(GameManager.Factions.Legion);
                _enemyHero = PlayerManager.Instance.GetHero(GameManager.Factions.Legion);
            }
            else if (_creepType == GameManager.Factions.Legion) {
                _enemyTower = TowerManager.Instance.GetTower(GameManager.Factions.Hellbourne);
                _enemyCreeps = CreepManager.Instance.GetCreepList(GameManager.Factions.Hellbourne);
                _enemyHero = PlayerManager.Instance.GetHero(GameManager.Factions.Hellbourne);
            }
            StartCoroutine(IdleCoroutine());
            //print("Started Coroutine from start");
        }
    }

    private void OnEnable() {
        EventManager.GameEndAction += StopCoroutinesOnGameEnd;
        if (isServer) {
            StartCoroutine(IdleCoroutine());
        }
    }

    private void OnDisable() {
        EventManager.GameEndAction -= StopCoroutinesOnGameEnd;
        transform.position = _startPosition;
        _waypointsReached = 0;
        _currentCoroutine = null;
        _agent.radius = _radius;
        //ResumeAgent();
    }

    private void Update() {
        if (isServer && Input.GetKeyDown(KeyCode.C)) {
            SetIsDead(true);
        }
    }

    private void ChangeState(CreepState state) {
        if (_currentState == state) {
            Debug.LogWarning("Trying to change to same state, not ideal behaviour OMEGALUL");
        }
        else
            _currentState = state;
    }

    //Does not loop
    private IEnumerator IdleCoroutine() {
        print("Entered Idle");
        ChangeState(CreepState.Idle);
        CmdAnimationPlay(CreepAnimationState.Idle.ToString());
        _currentCoroutine = StartCoroutine(RunningCoroutine());
        yield return null;
    }

    private IEnumerator RunningCoroutine() {
        print("Entered Running");
        ChangeState(CreepState.Running);
        if (AcquireTarget()) {
            _currentCoroutine = StartCoroutine(SearchingCoroutine());
            yield break;
        }
        CmdSetAnimationTrigger(CreepAnimationTrigger.RunTrigger.ToString());
        MoveToCurrentWaypoint();
        while (true) {
            ReachAndChangeWaypoint();
            if (AcquireTarget()) {
                _currentCoroutine = StartCoroutine(SearchingCoroutine());
                yield break;
            }
            yield return new WaitForSeconds(_updateBehaviourInterval);
        }
    }

    private IEnumerator SearchingCoroutine() {
        print("Entered Searching");
        ChangeState(CreepState.Searching);
        if (Utility.InRange(transform.position, _targetEnemy.transform.position,
            _attackRadius, _targetEnemy.GetComponent<CreatureHandler>().GetRadius())) {
            _currentCoroutine = StartCoroutine(AttackingCoroutine());
            yield break;
        }
        CmdSetAnimationTrigger(CreepAnimationTrigger.RunTrigger.ToString());
        while (true) {
            if (IsTargetDead()) {
                _currentCoroutine = StartCoroutine(RunningCoroutine());
                yield break;
            }
            if (Utility.InRange(transform.position, _targetEnemy.transform.position,
                _attackRadius, _targetEnemy.GetComponent<CreatureHandler>().GetRadius())) {
                _currentCoroutine = StartCoroutine(AttackingCoroutine());
                yield break;
            }
            SetDestination(_targetEnemy.transform.position);
            yield return new WaitForSeconds(_updateBehaviourInterval);
        }
    }

    private IEnumerator AttackingCoroutine() {
        print("Entered Attacking");
        StopAgent();
        //transform.LookAt(_targetEnemy.transform);
        StartCoroutine(Utility.RotateLerp(gameObject, gameObject.transform.rotation, _targetEnemy.transform.position, 0.5f));
        ChangeState(CreepState.Attacking);
        CmdSetAnimationTrigger(CreepAnimationTrigger.IdleTrigger.ToString());
        while (true) {
            if (!Utility.IsAliveAndInRange(gameObject, _targetEnemy, _attackRadius)) {
                ResumeAgent();
                if (AcquireTarget()) {
                    _currentCoroutine = StartCoroutine(SearchingCoroutine());
                }
                else {
                    _targetEnemy = null;
                    _currentCoroutine = StartCoroutine(RunningCoroutine());
                }
                yield break;
            }
            CmdSetAnimationTrigger(CreepAnimationTrigger.AttackTrigger.ToString());
            yield return new WaitForSeconds(_attackIntervalSecs);
        }
    }

    private bool IsTargetDead() {
        if (_targetEnemy.GetComponent<CreatureHandler>().GetIsDead()) {
            _targetEnemy = null;
            return true;
        }
        return false;
    }

    private bool AcquireTarget() {
        //print("Acquiring target...");
        float closestDistanceSqr = _closesDistanceSquared;
        _targetEnemy = null;
        for (int i = 0; i < _enemyCreeps.Count; i++) {
            if (!_enemyCreeps[i].GetComponent<CreatureHandler>().GetIsDead()) {
                float currDistance = Vector3.SqrMagnitude(_enemyCreeps[i].transform.position - transform.position);
                if (currDistance < closestDistanceSqr) {
                    _targetEnemy = _enemyCreeps[i];
                    _attackDamage = _creepAttackDamage;
                    closestDistanceSqr = currDistance;
                }
            }
        }
        if (_targetEnemy != null)
            return true;
        if (Utility.IsAliveAndInRange(gameObject, _enemyTower, _acquisitionRadius)) {
            _targetEnemy = _enemyTower;
            _attackDamage = _towerAttackDamage;
        }
        if (Utility.IsAliveAndInRange(gameObject, _enemyHero, _acquisitionRadius)) {
            _targetEnemy = _enemyHero;
            _attackDamage = _towerAttackDamage;
        }
        if (_targetEnemy != null)
            return true;
        return false;
    }



    private bool ReachAndChangeWaypoint() {
        if(Utility.InRange(transform.position, _targetWaypoint, _waypointDetectionRadius)) {
            _waypointsReached++;
            if(_waypointsReached >= Waypoints.Count) {
                Debug.LogError("Waypoint array exceeded");
                return false;
            }
            MoveToCurrentWaypoint();
            return true;
        }
        return false;
    }

    private void MoveToCurrentWaypoint() {
        _targetWaypoint = Waypoints[_waypointsReached].position;
        SetDestination(_targetWaypoint);
    }

    private void SetDestination(Vector3 pos) {
        currentTargetPos = pos;
        _agent.SetDestination(pos);
        _agent.speed = _defaultCreepSpeed;
    }

    private void StopAgent() {
        _agent.isStopped = true;
        agentStopped = true;
    }

    private void ResumeAgent() {
        _agent.isStopped = false;
        agentStopped = false;
    }

    private void DoDamage() {
        if(isServer)
            CmdDoDamage(_targetEnemy, _attackDamage);
    }

    private IEnumerator Despawn(float secs) {
        yield return new WaitForSeconds(secs);
        CreepManager.Instance.AddInactiveCreepsToStackAfterDelay(gameObject, _creepType);
        CmdSetActive(false);
    }

    public GameObject Ressurect() {
        _healthNetwork.ResetHealth();
        CmdSetActive(true);
        return gameObject;
    }

    public override void SetIsDead(bool isDead) {
        if(!isServer) {
            Debug.LogError("Client attempting to Set Creep Dead");
            return;
        }
        base.SetIsDead(isDead);
        SetAgentSpeed(0);
        _agent.radius = 0.1f;
        StopAllCoroutines();
        CmdSetAnimationTrigger(CreepAnimationTrigger.DeathTrigger.ToString());
        StartCoroutine(Despawn(_despawnTimeSecs));
    }

    private void StopCoroutinesOnGameEnd(GameManager.Factions faction) {
        if (!isServer)
            return;
        StopAllCoroutines();
        StopAgent();
    }

    public void SetAgentSpeed(float val) {
        _agent.speed = val;
    }


    public GameManager.Factions GetCreepType() {
        return _creepType;
    }

    private void OnDrawGizmos() {
    }
}
