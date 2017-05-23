using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    private float _turnInput = 0;
    private float _moveInput = 0;
    private Camera _camera = null;
    private AudioListener _audioListener = null;

	// Use this for initialization
	private void Start () {
        if (isLocalPlayer) {
            _camera.enabled = true;
            _audioListener.enabled = true;
        }

        print(_camera.enabled);
    }

    private void Awake() {
        _camera = gameObject.GetComponent<Camera>();
        _audioListener = gameObject.GetComponent<AudioListener>();
    }

    // Update is called once per frame
    private void Update () {
        if (isLocalPlayer) {
            GetInputs();
            UpdateMovement();
        }
	}

    private void GetInputs() {
        _turnInput = Input.GetAxis("Horizontal");
        _moveInput = Input.GetAxis("Vertical");
    }

    private void UpdateMovement() {
        transform.Rotate(0, _turnInput * Time.deltaTime * 150f, 0);
        transform.Translate(0, 0, _moveInput * Time.deltaTime * 3f);
    }

}
