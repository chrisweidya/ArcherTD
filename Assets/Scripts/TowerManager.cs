using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class TowerManager : NetworkBehaviour {

    public static TowerManager Instance;

    [SerializeField] private GameObject _legionTowerPrefab;
    [SerializeField] private GameObject _hellbourneTowerPrefab;
    [SerializeField] private GameObject _legionTowerDeadPrefab;
    [SerializeField] private GameObject _hellbourneTowerDeadPrefab;
    [SerializeField] private Transform _legionTowerTransform;
    [SerializeField] private Transform _hellbourneTowerTransform;

    [SerializeField] private GameObject _legionTowerGO;
    [SerializeField] private GameObject _hellbourneTowerGO;
    [SerializeField] private GameObject _legionTowerDeadGO;
    [SerializeField] private GameObject _hellbourneTowerDeadGO;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("Attempting to instantiate another TowerManager instance.");
            return;
        }
        Instance = this;
    }

    void Start() {
        SpawnTowers();
    }

    private void SpawnTowers() {
        _legionTowerGO = Instantiate(_legionTowerPrefab, _legionTowerTransform);
        _hellbourneTowerGO = Instantiate(_hellbourneTowerPrefab, _hellbourneTowerTransform);
        NetworkServer.Spawn(_legionTowerGO);
        NetworkServer.Spawn(_hellbourneTowerGO);
    }

    public void DeadTower(GameManager.Factions faction) {
        if (faction == GameManager.Factions.Legion) {
            _legionTowerDeadGO = Instantiate(_legionTowerDeadPrefab, _legionTowerTransform);
            NetworkServer.Spawn(_legionTowerDeadGO);
            NetworkServer.Destroy(_legionTowerGO);
        }
        else if(faction == GameManager.Factions.Hellbourne) {
            _hellbourneTowerDeadGO = Instantiate(_hellbourneTowerPrefab, _hellbourneTowerTransform);
            NetworkServer.Spawn(_hellbourneTowerDeadGO);
            NetworkServer.Destroy(_hellbourneTowerDeadGO);
        }
    }

    public GameObject GetTower(GameManager.Factions type) {
        if (type == GameManager.Factions.Legion)
            return _legionTowerGO;
        else if (type == GameManager.Factions.Hellbourne) {
            return _hellbourneTowerGO;
        }
        return null;
    }
}
