using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnArrow : NetworkBehaviour {

    [SerializeField]
    private GameObject ArrowPrefab = null;

    private void OnEnable() {
        print("Added create arrow event1");
        print(isLocalPlayer);
        //if (isLocalPlayer) {
        //    print("Added create arrow event");
        //    print(NetworkServer.active);
            EventManager.CreateArrowAction += StartFireArrow;
        //}
    }

    private void OnDisable() {
        //if (isLocalPlayer) {
            EventManager.CreateArrowAction -= StartFireArrow;
        //}
    }

    private void StartFireArrow(Vector3 position, Quaternion rotation, Vector3 forward, float force) {
        if (isLocalPlayer)
            CmdFireArrow(position, rotation, forward, force);
    }

    [Command]
    private void CmdFireArrow(Vector3 position, Quaternion rotation, Vector3 forward, float force) {
        GameObject ArrowGO = Instantiate(ArrowPrefab, position, rotation);
        Valve.VR.InteractionSystem.Arrow ArrowScript = ArrowGO.GetComponent<Valve.VR.InteractionSystem.Arrow>();

        ArrowScript.shaftRB.isKinematic = false;
        ArrowScript.shaftRB.useGravity = true;
        ArrowScript.shaftRB.transform.GetComponent<BoxCollider>().enabled = true;

        ArrowScript.arrowHeadRB.isKinematic = false;
        ArrowScript.arrowHeadRB.useGravity = true;
        ArrowScript.arrowHeadRB.transform.GetComponent<BoxCollider>().enabled = true;

        ArrowScript.arrowHeadRB.AddForce(forward * force, ForceMode.VelocityChange);
        ArrowScript.arrowHeadRB.AddTorque(forward * 10);

        ArrowScript.ArrowReleased(force);
        print("Server arrow spawn");
        NetworkServer.Spawn(ArrowGO);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
