using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class TowerHandler : CreatureHandler {


    //tower range
    [SerializeField] private float towerRange;
    private float acquisitionRange = 4;

    private float acquisitionInterval = 1;
    private float scanInterval = 0.5f;
    //tower's current target
    [SerializeField] private GameObject currentTarget;
    private CreatureHandler currentTargetScript;
    //list of creeps to check for range within tower
    private List<GameObject> CreepList;
    [SerializeField] private GameObject enemyPlayer;

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
            StartCoroutine(Initialize());
        }
        _radius = GetComponent<CapsuleCollider>().radius;
        print(_radius + " fads");
        towerHandlerScript = this;
    }

    private IEnumerator Initialize() {
        while (true) {
            if (team == "Legion") {
                enemyCreepList = CreepManager.Instance.GetCreepList(GameManager.Factions.Hellbourne);
                enemyPlayer = PlayerManager.Instance.GetHero(GameManager.Factions.Hellbourne);
            }
            else {
                enemyCreepList = CreepManager.Instance.GetCreepList(GameManager.Factions.Legion);
                enemyPlayer = PlayerManager.Instance.GetHero(GameManager.Factions.Legion);
            }
            if (enemyCreepList != null && enemyPlayer != null)
                break;
            yield return new WaitForSeconds(0.2f);
        }
        StartCoroutine(ScanForTargets(towerRange, scanInterval));
        yield return null;
    }

    //scan for targets 
    private IEnumerator ScanForTargets(float range, float seconds) {
        //find a suitable target in the list of creeps that is within tower range every second
        while (true) {
            print("hi1");
            if (currentTargetScript == null || currentTargetScript.GetIsDead()) {
                print("hiq");
                foreach (GameObject go in enemyCreepList) {
                    print("hi3");
                    if (Utility.IsAliveAndInRange(gameObject, go, range)) {
                        currentTarget = go;
                        currentTargetScript = currentTarget.GetComponent<CreatureHandler>();
                        StartCoroutine(AttackTarget());
                        Debug.Log("Current Target " + currentTarget);
                        yield break;
                    }
                }
                print("hi4");
                if (Utility.IsAliveAndInRange(gameObject, enemyPlayer, range)) {
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
            if (Utility.IsAliveAndInRange(gameObject, currentTarget, towerRange)) {
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
