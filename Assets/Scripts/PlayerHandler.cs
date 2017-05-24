using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHandler : NetworkBehaviour {

    private float _turnInput = 0;
    private float _moveInput = 0;
    private Camera _camera = null;
    private AudioListener _audioListener = null;
    private Renderer _renderer = null;

    [SerializeField] private float _turnSpeed = 150f;
    [SerializeField] private float _moveSpeed = 10f;

    private void Awake() {
        _camera = gameObject.GetComponent<Camera>();
        _audioListener = gameObject.GetComponent<AudioListener>();
        _renderer = gameObject.GetComponent<Renderer>();
    }

    private void Start () {
        if (isLocalPlayer) {
            //print(transform.gameObject);
            GameManager.Instance.AssignCamera(transform.gameObject);
            Renderer[] rs = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rs)
                r.enabled = false;
            //_camera.enabled = true;
            //_audioListener.enabled = true;
        }
        else {
            Destroy(this);
        }
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
