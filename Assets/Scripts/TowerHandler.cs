using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class TowerHandler : CreatureHandler {


    //tower range
    private float towerRange = 10;
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
    private IList<GameObject> creepList;

    //tower firing point
    public Transform firingPoint;
    public GameObject TowerBullet;

    void Start() {
        if (team == "Legion") {
            creepList = creepManager.GetCreepList(CreepManager.CreepType.Legion);
        }
        else {
            creepList = creepManager.GetCreepList(CreepManager.CreepType.Hellbourne);
        }
        if (isServer) {
            StartCoroutine(ScanForTargets(towerRange, scanInterval));
        }
    }
	
	void Update () {
		
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
                foreach (GameObject go in creepList) {
                    if (CheckRange(go.transform.position,range)) {
                        currentTarget = go;
                        currentTargetScript = currentTarget.GetComponent<CreatureHandler>();
                        StartCoroutine(AttackTarget());
                        Debug.Log("Current Target " + currentTarget);
                        yield return null;
                    }
                }
            }
            yield return new WaitForSeconds(seconds);
        }        
    }

    private IEnumerator AttackTarget() {

        while (true) {
            if (!currentTargetScript.GetIsDead()) {
                //attack function
                RpcTowerAttack();
                Debug.Log("Attacking current target");
            }
            else {
                StartCoroutine(ScanForTargets(towerRange, scanInterval));
                Debug.Log("finding new target");
                yield return null;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    [ClientRpc]
    private void RpcTowerAttack() {

        GameObject bullet = Instantiate(TowerBullet, firingPoint) as GameObject;
        bullet.transform.LookAt(currentTarget.transform);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 10);
        bullet.GetComponent<NetworkCollisionDetection>().team = team;
    }

}
