using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton <GameManager> {
    
    private GameObject _cameraRigGO = null;
    private GameObject _cameraPlayer = null;

    protected GameManager() {
    }

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
        _cameraRigGO = GameObject.Find("Player");
        print(_cameraRigGO);
        //_cameraRig = Instantiate(_cameraRigPrefab);
    }

    private void SetCameraPos(Vector3 pos) {
        _cameraRigGO.transform.position = pos;
    }

    public void AssignCamera(GameObject player) {
        if(_cameraPlayer != null) {
            print("Camera already assigned");
            return;
        }
        else {
            _cameraPlayer = player;
            SetCameraPos(player.transform.position);
            player.transform.parent = _cameraRigGO.transform.Find("SteamVRObjects/VRCamera");
            if(player.transform.parent == null) {
                print("VR Camera not found, Vive not connected? Fallback.");
                player.transform.parent = _cameraRigGO.transform.Find("NoSteamVRFallbackObjects/FallbackObjects");
            }
            player.transform.position = player.transform.parent.position;
            player.transform.rotation = player.transform.parent.rotation;
            print(player.transform.position);
        }
    }
}
