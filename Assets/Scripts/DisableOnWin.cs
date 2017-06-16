using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DisableOnWin : MonoBehaviour {

    [SerializeField]
    private List<GameObject> _objects = new List<GameObject>();

    private void OnEnable() {
        EventManager.GameEndAction += DisableObjects;
    }

    private void OnDisable() {
        EventManager.GameEndAction -= DisableObjects;
    }

    private void DisableObjects(NetworkInstanceId id) {
        if(PlayerHandler.localWardenNetId == id) {
            print("reached");
            for(int i = 0; i < _objects.Count; i++) {
                print("disabled");
                _objects[i].SetActive(false);
            }
        }
    }
}
