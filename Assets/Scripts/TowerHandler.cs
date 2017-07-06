using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHandler : CreatureHandler {


    //tower range
    private float towerRange = 3;

    //tower's current target
    private GameObject currentTarget;

    //list of creeps to check for range within tower
    private List<GameObject> CreepList;

    //getting a list of creeps from creep manager depending on faction
    [SerializeField]
    private CreepManager creepManager;
    public bool isLegion;
    private List<GameObject> creepList;


    void Start() {
        if (isLegion) {
            creepList = creepManager.ReturnLegionList();
        }
        else {
            creepList = creepManager.ReturnHellbourneList();
        }
	}
	
	void Update () {
		
	}



    //check range of target and returns a bool
    private bool CheckRange(Vector3 targetPos) {

        if (Vector3.Distance(transform.position, targetPos) < towerRange) {
            return true;

        }
        return false;
    }

   private IEnumerable OuterScan() {

        //find a suitable target in the list of creeps that is within tower range
        foreach (GameObject go in creepList) {
            if (CheckRange(go.transform.position)) {
                currentTarget = go;
                yield return null;
            }
        }
        yield return null;
    }
}
