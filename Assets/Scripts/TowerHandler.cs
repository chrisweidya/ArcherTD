using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class TowerHandler : CreatureHandler {


    //tower range
    private float towerRange = 7;
    private float acquisitionRange = 4;

    private float acquisitionInterval = 1;
    private float scanInterval = 0.5f;
    //tower's current target
    private GameObject currentTarget;
    private CreatureHandler currentTargetScript;
    //list of creeps to check for range within tower
    private List<GameObject> CreepList;

    //getting a list of creeps from creep manager depending on faction
    [SerializeField]
    private CreepManager creepManager;
    public bool isLegion;
    public string team;
    private IList<GameObject> enemyCreepList;

    //tower firing point
    public Transform firingPoint;
    public GameObject TowerBullet;

    //dmg
    private float dmg;

    //enum states
    public enum TowerAnimationTrigger { IdleTrigger, AttackTrigger, DeathTrigger };
    private enum TowerAnimationState { Idle, Attack,Deat};
    public enum TowerState { Idling, Attacking,Dying};

    void Start() {
        if (team == "Legion") {
            enemyCreepList = creepManager.GetCreepList(CreepManager.CreepType.Hellbourne);
        }
        else {
            enemyCreepList = creepManager.GetCreepList(CreepManager.CreepType.Legion);
        }
        if (isServer) {
            StartCoroutine(ScanForTargets(towerRange, scanInterval));
        }
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
        CmdSetAnimationTrigger(TowerAnimationTrigger.IdleTrigger.ToString());
        while (true) {
            if (currentTargetScript == null || currentTargetScript.GetIsDead()) {
                foreach (GameObject go in enemyCreepList) {
                    
                    if (!go.GetComponent<CreepHandler>().GetIsDead() && CheckRange(go.transform.position, range)) {
                        currentTarget = go;
                        currentTargetScript = currentTarget.GetComponent<CreatureHandler>();
                        StartCoroutine(AttackTarget());
                        Debug.Log("Current Target " + currentTarget);
                        break;
                    }
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
                RpcTowerAttack(currentTarget.transform.position);
                
            }
            else {
                currentTarget = null;
                currentTargetScript = null;
                StartCoroutine(ScanForTargets(towerRange, scanInterval));
                Debug.Log("finding new target");
                break;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    [ClientRpc]
    private void RpcTowerAttack(Vector3 targetPos) {
        Debug.Log("towerrpcshot");
        GameObject bullet = Instantiate(TowerBullet, firingPoint.transform.position, firingPoint.transform.rotation) as GameObject;
        bullet.transform.LookAt(targetPos);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 20);
        //bullet.GetComponent<NetworkCollisionDetection>().team = team;
        bullet.GetComponent<LookAtPlayer>().target = currentTarget;
        TowerProjectile tp = bullet.GetComponent<TowerProjectile>();
        tp.towerParent = GetComponent<TowerHandler>();
        tp.currentTarget = currentTarget;
    }

    public void DoDamage() {
        if (isServer) {
            CmdDoDamage(currentTarget, dmg);
        }
    }



 
}
