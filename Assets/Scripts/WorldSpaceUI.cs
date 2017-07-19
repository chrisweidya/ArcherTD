using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceUI : MonoBehaviour {
    [SerializeField] private List<GameObject> _victoryDisplayGO;
    [SerializeField] private List<GameObject> _defeatDisplayGO;

    private void OnEnable() {
        EventManager.GameEndAction += EnableGameEndUI;
    }

    private void OnDisable() {
        EventManager.GameEndAction -= EnableGameEndUI;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void EnableGameEndUI(GameManager.Factions faction) {
        if(PlayerHandler.LocalFaction == faction) {
            foreach(GameObject go in _victoryDisplayGO) {
                go.SetActive(true);
            }
        }
        else {
            foreach(GameObject go in _defeatDisplayGO) {
                go.SetActive(true);
            }
        }
    }

}
