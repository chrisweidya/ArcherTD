using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnArrow : NetworkBehaviour {

    [SerializeField]
    private GameObject ArrowPrefab = null;

    private List<GameObject> arrowList;
    private int maxArrowCount;

    private void OnEnable() {
        //print("Added create arrow event1");
        //print(isLocalPlayer);
        print("enabled arrow firing event");
        EventManager.CreateArrowAction += StartFireArrow;
    }

    private void OnDisable() {

        print("disabled arrow firing event");
        EventManager.CreateArrowAction -= StartFireArrow;
    }

    private void StartFireArrow(Vector3 position, Quaternion rotation, Vector3 forward, float force) {
        if (isLocalPlayer) {
            CmdFireArrow(position, rotation, forward, force);
        }
    }

    [Command]
    private void CmdFireArrow(Vector3 position, Quaternion rotation, Vector3 forward, float force) {
        RpcFireArrow(position, rotation, forward, force);
    }

    [ClientRpc]
    private void RpcFireArrow(Vector3 position, Quaternion rotation, Vector3 forward, float force)
    {
        GameObject ArrowGO = Instantiate(ArrowPrefab, position, rotation);
        Valve.VR.InteractionSystem.Arrow ArrowScript = ArrowGO.GetComponent<Valve.VR.InteractionSystem.Arrow>();
        ArrowGO.GetComponent<NetworkCollisionDetection>().team = GetComponent<PlayerProperties>().GetTeam();
        ArrowScript.shaftRB.isKinematic = false;
        ArrowScript.shaftRB.useGravity = true;
        ArrowScript.shaftRB.transform.GetComponent<BoxCollider>().enabled = true;

        ArrowScript.arrowHeadRB.isKinematic = false;
        ArrowScript.arrowHeadRB.useGravity = true;
        ArrowScript.arrowHeadRB.transform.GetComponent<BoxCollider>().enabled = true;

        ArrowScript.arrowHeadRB.AddForce(forward * force, ForceMode.VelocityChange);
        ArrowScript.arrowHeadRB.AddTorque(forward * 10);

        EventManager.FirePlayerStateChange(PlayerHandler.PlayerState.BowReleased);

        arrowList.Add(ArrowGO);

        while (arrowList.Count > maxArrowCount) {
            GameObject oldArrow = arrowList[0];
            arrowList.RemoveAt(0);
            if (oldArrow) {
                Destroy(oldArrow);
            }
        }


        // ArrowScript.ArrowReleased(force);
        //print("Rpc arrow spawn");
    }
    // Use this for initialization
    void Start () {
        maxArrowCount = 10;
        arrowList = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
