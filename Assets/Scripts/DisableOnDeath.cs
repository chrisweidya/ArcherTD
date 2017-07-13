using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DisableOnDeath : MonoBehaviour {

    [SerializeField]
    private List<GameObject> _objects = new List<GameObject>();

    private void OnEnable() {
        EventManager.PlayerDeathAction += DisableObjects;
    }

    private void OnDisable() {
        EventManager.PlayerDeathAction -= DisableObjects;
    }

    private void DisableObjects(NetworkInstanceId id, bool death) {
        if(PlayerHandler.localWardenNetId == id) {
            print("reached");
            for(int i = 0; i < _objects.Count; i++) {
                _objects[i].SetActive(!death);
            }
        }
    }
}
