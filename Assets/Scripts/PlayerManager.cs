using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerManager : NetworkBehaviour {

    public static PlayerManager Instance;

    private GameObject _legionHeroGO;
    private NetworkInstanceId _legionHeroNetId;
    [SerializeField] private Transform _legionSpawnPoint;
    private GameObject _hellbourneHeroGO;
    private NetworkInstanceId _hellbourneHeroNetId;
    [SerializeField] private Transform _hellbourneSpawnPoint;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("Attempting to instantiate another PlayerManager instance.");
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
    }

    public void SetHeroOnce(GameObject obj, GameManager.Factions faction, NetworkInstanceId id) {
        if (faction == GameManager.Factions.Legion) {
            print("set hero le");
            if (_legionHeroGO != null) {
                Debug.LogError("Attempting to set hero more than once.");
                return;
            }
            _legionHeroNetId = id;
            _legionHeroGO = obj;
        }
        else if (faction == GameManager.Factions.Hellbourne) {
            print("set hero he");
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

    public Vector3 GetSpawnPosition(GameManager.Factions type) {
        if (type == GameManager.Factions.Legion) {
            return _legionSpawnPoint.position;
        }
        else if(type == GameManager.Factions.Hellbourne) {
            return _hellbourneSpawnPoint.position;
        }
        return Vector3.zero;
    }
}
