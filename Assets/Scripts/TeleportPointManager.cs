using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPointManager : MonoBehaviour {
    public GameObject legionTelePoints;
    public GameObject hellborneTelePoints;

	// Use this for initialization
	void Start () {

        Debug.Log("teleport" + GameManager.GetLocalPlayerTeam());
        if (GameManager.GetLocalPlayerTeam() == "Hellborne") {
            hellborneTelePoints.SetActive(true);
        }
        if (GameManager.GetLocalPlayerTeam() == "Legion") {
            legionTelePoints.SetActive(true);
        }
    }
	// Update is called once per frame
	void Update () {
        Debug.Log("teleport" + GameManager.GetLocalPlayerTeam());

        if (GameManager.GetLocalPlayerTeam() == "Hellborne") {
            hellborneTelePoints.SetActive(true);
        }
        if (GameManager.GetLocalPlayerTeam() == "Legion") {
            legionTelePoints.SetActive(true);
        }
    }
}
