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
    private float dmg = 15;

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
            if (currentTargetScript == null || currentTargetScript.GetIsDead()) {
                foreach (GameObject go in enemyCreepList) {
                    if (Utility.IsAliveAndInRange(gameObject, go, range)) {
                        currentTarget = go;
                        currentTargetScript = currentTarget.GetComponent<CreatureHandler>();
                        StartCoroutine(AttackTarget());
                        yield break;
                    }
                }
                if (Utility.IsAliveAndInRange(gameObject, enemyPlayer, range)) {
                    currentTarget = enemyPlayer;
                    currentTargetScript = currentTarget.GetComponent<CreatureHandler>();
                    StartCoroutine(AttackTarget());
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
                RpcTowerAttack(currentTarget.transform.position, currentTarget);
            }
            else {
                currentTarget = null;
                currentTargetScript = null;
                StartCoroutine(ScanForTargets(towerRange, scanInterval));
                CmdSetAnimationTrigger(TowerAnimationTrigger.IdleTrigger.ToString());
                break;
            }
            yield return new WaitForSeconds(1.0f);

        }
    }

    [ClientRpc]
    private void RpcTowerAttack(Vector3 targetPos, GameObject target) {
        GameObject bullet = Instantiate(TowerBullet, firingPoint.transform.position, firingPoint.transform.rotation) as GameObject;
        TowerProjectile tp = bullet.GetComponent<TowerProjectile>();
        tp.towerParent = towerHandlerScript;
        tp.currentTarget = target;
    }

    public void DoDamage(GameObject target) {
        if (isServer) {
            CmdDoDamage(target, dmg);
        }
    }
     
}
