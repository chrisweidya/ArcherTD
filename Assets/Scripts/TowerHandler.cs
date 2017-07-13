using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class TowerHandler : CreatureHandler {


    //tower range
    private float towerRange = 50;
    private float acquisitionRange = 4;

    private float acquisitionInterval = 1;
    private float scanInterval = 0.5f;
    //tower's current target
    [SerializeField]
    private GameObject currentTarget;
    private CreatureHandler currentTargetScript;
    //list of creeps to check for range within tower
    private List<GameObject> CreepList;
    [SerializeField]
    private GameObject enemyPlayer;

    //getting a list of creeps from creep manager depending on faction
    public bool isLegion;
    public string team;
    private IList<GameObject> enemyCreepList;

    //tower firing point
    public Transform firingPoint;
    public GameObject TowerBullet;

    //dmg
    [SerializeField]
    private float dmg = 1;

    //towerhandler script
    private TowerHandler towerHandlerScript;

    //enum states
    public enum TowerAnimationTrigger { IdleTrigger, AttackTrigger, DeathTrigger};
    private enum TowerAnimationState { Idle, Attack,Death};
    public enum TowerState { Idling, Attacking,Dying};

    void Start() {
     
        if (isServer) {
            
            if (team == "Legion") {
                enemyCreepList = CreepManager.Instance.GetCreepList(GameManager.Factions.Hellbourne);
                enemyPlayer = PlayerManager.Instance.GetHero(GameManager.Factions.Hellbourne);
            }
            else {
                enemyCreepList = CreepManager.Instance.GetCreepList(GameManager.Factions.Legion);
                enemyPlayer = PlayerManager.Instance.GetHero(GameManager.Factions.Legion);
            }
            StartCoroutine(ScanForTargets(towerRange, scanInterval));
        }
        _radius = GetComponent<CapsuleCollider>().radius;
        print(_radius + " fads");
        towerHandlerScript = this;
    }

    void Update() {

    }



    //check range of target and returns a bool
    private bool CheckRange(Vector3 targetPos, float range) {

        if (Vector3.Distance(transform.position, targetPos) < range) {
            return true;
        }
        return false;
    }

    //scan for targets 
    private IEnumerator ScanForTargets(float range, float seconds) {
        //find a suitable target in the list of creeps that is within tower range every second
        while (true) {
            if (currentTargetScript == null || currentTargetScript.GetIsDead()) {
                foreach (GameObject go in enemyCreepList) {
                    
                    if (!go.GetComponent<CreepHandler>().GetIsDead() && CheckRange(go.transform.position, range)) {
                        currentTarget = go;
                        currentTargetScript = currentTarget.GetComponent<CreatureHandler>();
                        StartCoroutine(AttackTarget());
                        Debug.Log("Current Target " + currentTarget);
                        yield break;
                    }
                }
                if (!enemyPlayer.GetComponent<CreatureHandler>().GetIsDead() && CheckRange(enemyPlayer.transform.position, range)) {
                    currentTarget = enemyPlayer;
                    currentTargetScript = currentTarget.GetComponent<CreatureHandler>();
                    StartCoroutine(AttackTarget());
                    Debug.Log("Targeting Enemy Player");
                    yield break;
                }
            }
            yield return new WaitForSeconds(seconds);
        }
    }

    private IEnumerator AttackTarget() {
        CmdSetAnimationTrigger(TowerAnimationTrigger.AttackTrigger.ToString());
        while (true) {
            Debug.Log(currentTargetScript);
            if (!currentTargetScript.GetIsDead() && Utility.InRange(transform.position, currentTarget.transform.position, towerRange)) {
                //attack function
                Debug.Log("Attacking current target");
                RpcTowerAttack(currentTarget.transform.position, currentTarget);
            }
            else {
                currentTarget = null;
                currentTargetScript = null;
                StartCoroutine(ScanForTargets(towerRange, scanInterval));
                CmdSetAnimationTrigger(TowerAnimationTrigger.IdleTrigger.ToString());
                Debug.Log("finding new target");
                break;
            }
            yield return new WaitForSeconds(1.0f);

        }
    }

    [ClientRpc]
    private void RpcTowerAttack(Vector3 targetPos, GameObject target) {
        Debug.Log("towerrpcshot");
        GameObject bullet = Instantiate(TowerBullet, firingPoint.transform.position, firingPoint.transform.rotation) as GameObject;
        TowerProjectile tp = bullet.GetComponent<TowerProjectile>();
        tp.towerParent = towerHandlerScript;
        tp.currentTarget = target;
    }

    public void DoDamage(GameObject target) {
        if (isServer) {
            Debug.Log("tower do damage to " + target);
            CmdDoDamage(target, dmg);
        }
    }
     
}
