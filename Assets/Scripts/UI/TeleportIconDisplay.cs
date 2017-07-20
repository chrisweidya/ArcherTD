using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportIconDisplay : MonoBehaviour {
    [SerializeField] private GameObject _teleportIcon;

    private void OnEnable() {
        EventManager.DisplayTeleportIcon += enableTeleportIconGO;
    }

    private void OnDisable() {
        EventManager.DisplayTeleportIcon -= enableTeleportIconGO;
    }

    private void enableTeleportIconGO(bool val) {
        _teleportIcon.SetActive(val);
    }
}
