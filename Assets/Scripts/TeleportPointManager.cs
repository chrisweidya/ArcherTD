using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPointManager : MonoBehaviour {
    private string factionName;
    public GameObject legionTelePoints;
    public GameObject hellborneTelePoints;

    public void TeleporterSetUp() {
        if (GameManager.GetLocalPlayerFaction() == GameManager.Factions.Hellbourne) {
            hellborneTelePoints.SetActive(true);
        }
        if (GameManager.GetLocalPlayerFaction() == GameManager.Factions.Legion) {
            legionTelePoints.SetActive(true);
        }
    }
}
