﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerManager : NetworkBehaviour {

    public static PlayerManager Instance;

    [SerializeField] private GameObject _legionHeroGO;
    [SerializeField] private NetworkInstanceId _legionHeroNetId;
    [SerializeField] private GameObject _hellbourneHeroGO;
    [SerializeField] private NetworkInstanceId _hellbourneHeroNetId;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("Attempting to instantiate another PlayerManager instance.");
            return;
        }
        Instance = this;
    }

    void Start() {
    }

    public void SetHeroOnce(GameObject obj, GameManager.Factions faction, NetworkInstanceId id) {
        if (faction == GameManager.Factions.Legion) {
            if (_legionHeroGO != null) {
                Debug.LogError("Attempting to set hero more than once.");
                return;
            }
            _legionHeroNetId = id;
            _legionHeroGO = obj;
        }
        else if (faction == GameManager.Factions.Hellbourne) {
            if (_hellbourneHeroGO != null) {
                Debug.LogError("Attempting to set hero more than once.");
                return;
            }
            _hellbourneHeroNetId = id;
            _hellbourneHeroGO = obj;
        }
    }

    public GameObject GetHero(GameManager.Factions type) {
        if (type == GameManager.Factions.Legion)
            return _legionHeroGO;
        else if (type == GameManager.Factions.Hellbourne) {
            return _hellbourneHeroGO;
        }
        return null;
    }
}
