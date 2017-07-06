using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHandler : CreatureHandler {


    //tower range
    private float towerRange = 3;
    private float acquisitionRange = 4;
    //tower's current target
    private GameObject currentTarget;

    //list of creeps to check for range within tower
    private List<GameObject> CreepList;

    //getting a list of creeps from creep manager depending on faction
    [SerializeField]
    private CreepManager creepManager;
    public bool isLegion;
    private IList<GameObject> creepList;


    void Start() {
        if (isLegion) {
            creepList = creepManager.GetCreepList(CreepManager.CreepType.Legion);
        }
        else {
            creepList = creepManager.GetCreepList(CreepManager.CreepType.Hellbourne);
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

   private IEnumerable OuterScan() {
        //find a suitable target in the list of creeps that is within tower range every second
        while (true) {
            if (currentTarget == null) {
                foreach (GameObject go in creepList) {
                    if (CheckRange(go.transform.position,acquisitionRange)) {
                        currentTarget = go;
                        yield return null;
                    }
                }
            }
            yield return new WaitForSeconds(1.0f);
        }        
    }
    private IEnumerable InnerScan() {
        while (true) {

            return null;
        }
        
    }
}
