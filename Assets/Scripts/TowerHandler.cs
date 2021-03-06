﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class TowerHandler : CreatureHandler {
    private static bool oneTowerDestroyed = false;

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
    private IList<GameObject> enemyCreepList;

    //tower firing point
    public Transform firingPoint;
    public GameObject TowerBullet;

    //dmg
    [SerializeField]
    private float dmg;
    [SerializeField]
    private float playerDmg; 
    //towerhandler script
    private TowerHandler towerHandlerScript;

    //enum states
    public enum TowerAnimationTrigger { IdleTrigger, AttackTrigger, DeathTrigger};
    private enum TowerAnimationState { Idle, Attack,Death};
    public enum TowerState { Idling, Attacking,Dying};

    private void Start() {     
        if (isServer) { 
            StartCoroutine(Initialize());
        }
        _radius = GetComponent<CapsuleCollider>().radius;
        towerHandlerScript = this;
    }

    private void OnEnable() {
        EventManager.GameEndAction += StopCoroutinesOnGameEnd;
    }

    private void OnDisable() {
        EventManager.GameEndAction -= StopCoroutinesOnGameEnd;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.T) && GetComponentInChildren<PlayerProperties>().GetFaction() == GameManager.Factions.Legion) {
            CmdDoDamage(gameObject, 9999999);
        }
        if (Input.GetKeyDown(KeyCode.Y) && GetComponentInChildren<PlayerProperties>().GetFaction() == GameManager.Factions.Hellbourne) {
            CmdDoDamage(gameObject, 9999999);
        }
    }

    private IEnumerator Initialize() {
        while (true) {
            if (GetComponent<PlayerProperties>().GetFaction() == GameManager.Factions.Legion) {
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
        
        while (true) {
            if (Utility.IsAliveAndInRange(gameObject, currentTarget, towerRange)) {
                //attack function
                CmdSetAnimationTrigger(TowerAnimationTrigger.AttackTrigger.ToString());
                RpcTowerAttack(currentTarget.transform.position, currentTarget);
            }
            else {
                currentTarget = null;
                currentTargetScript = null;
                StartCoroutine(ScanForTargets(towerRange, scanInterval));
                CmdSetAnimationTrigger(TowerAnimationTrigger.IdleTrigger.ToString());
                break;
            }
            yield return new WaitForSeconds(2.0f);

        }
    }

    private void StopCoroutinesOnGameEnd(GameManager.Factions faction) {
        if (!isServer)
            return;
        StopAllCoroutines();
    }

    //Server
    public override void SetIsDead(bool isDead) {
        if (!oneTowerDestroyed) {
            base.SetIsDead(isDead);
            TowerManager.Instance.DeadTower(GetComponent<PlayerProperties>().GetFaction());
            oneTowerDestroyed = true;
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
            if (target.GetComponent<CreepHandler>() == null) {
                CmdDoDamage(target, playerDmg);
            }
            else {
                CmdDoDamage(target, dmg);
            }
        }
    }

    public override void OnIsDead(bool isDead) {
        base.OnIsDead(isDead);
        EventManager.FireGameEnd(GetComponent<PlayerProperties>().GetFaction());
    }
}
