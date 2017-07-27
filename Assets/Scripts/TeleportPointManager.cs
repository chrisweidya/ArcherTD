using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPointManager : MonoBehaviour {
    private string factionName;
    public GameObject legionTelePoints;
    public GameObject hellborneTelePoints;

    public void TeleporterSetUp() {
        if (PlayerHandler.LocalFaction == GameManager.Factions.Hellbourne) {
            hellborneTelePoints.SetActive(true);
        }
        if (PlayerHandler.LocalFaction == GameManager.Factions.Legion) {
            legionTelePoints.SetActive(true);
        }
    }
}
