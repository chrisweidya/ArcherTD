using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton <GameManager> { 

    protected GameManager() {
    }

    private GameObject _cameraPlayer = null;

    [SerializeField] private Transform _cameraRigPrefab = null;
    private Transform _cameraRig = null;

    protected override void Awake() {
        base.Awake();
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        print("Scene Loaded.");
        _cameraRig = Instantiate(_cameraRigPrefab);
    }

    public void AssignCamera(GameObject player) {
        if(_cameraPlayer != null) {
            print("Camera already assigned");
            return;
        }
        else {
            _cameraPlayer = player;
            _cameraRig.position = player.transform.position;
            _cameraRig.rotation = player.transform.rotation;
            player.transform.parent = _cameraRig.transform.Find("Camera (head)/Camera (eye)");
            print(player.transform.parent);
        }
    }
}
