using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton <GameManager> { 

    protected GameManager() {
    }

    private GameObject _cameraPlayer = null;

    [SerializeField] private Transform _cameraRig = null;

    protected override void Awake() {
        base.Awake();
        _cameraRig = Instantiate(_cameraRig);
        DontDestroyOnLoad(_cameraRig);
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
            player.transform.parent = _cameraRig;
        }
    }
}
