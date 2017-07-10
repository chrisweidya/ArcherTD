using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class TowerManager : NetworkBehaviour {

    public static TowerManager Instance;

    [SerializeField] private GameObject _legionTowerPrefab;
    [SerializeField] private GameObject _hellbourneTowerPrefab;
    
    [SerializeField] private Transform _legionTowerTransform;
    [SerializeField] private Transform _hellbourneTowerTransform;
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void SpawnTowers() {
        NetworkServer.Spawn(Instantiate(_legionTowerPrefab, _legionTowerTransform));
        NetworkServer.Spawn(Instantiate(_hellbourneTowerPrefab, _hellbourneTowerTransform));

    }
}
