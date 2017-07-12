using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPointManager : MonoBehaviour {
    private string factionName;
    public GameObject legionTelePoints;
    public GameObject hellborneTelePoints;
	// Use this for initialization
	void Start () {
       
    }
    public void TeleporterSetUp() {

        Debug.Log("teleport" + GameManager.GetLocalPlayerFaction());

        if (GameManager.GetLocalPlayerFaction() == GameManager.Factions.Hellbourne) {
            hellborneTelePoints.SetActive(true);
        }
        if (GameManager.GetLocalPlayerFaction() == GameManager.Factions.Legion) {
            legionTelePoints.SetActive(true);
        }
    }
	// Update is called once per frame
	void Update () {
        
    }
}
