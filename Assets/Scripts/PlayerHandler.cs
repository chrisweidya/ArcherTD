using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHandler : NetworkBehaviour {
    
    private Renderer _renderer = null;

    [SyncVar(hook="TriggerPlayerAnimation")]
    private Enums.PlayerState _playerState = Enums.PlayerState.Stand;

    private void OnEnable() {
        EventManager.ChangePlayerState += ChangeState;
    }

    private void OnDisable() {
        EventManager.ChangePlayerState -= ChangeState;
    }

    private void ChangeState(Enums.PlayerState state) {
        _playerState = state;
    }

    private void TriggerPlayerAnimation(Enums.PlayerState playerstate) {
        print(playerstate);
    }

    private void Awake() {
        _renderer = gameObject.GetComponent<Renderer>();
    }

    private void Start () {
        if (isLocalPlayer) {
            GameManager.Instance.AssignCamera(transform.gameObject);
            _renderer.enabled = false;
            //Renderer[] rs = GetComponentsInChildren<Renderer>();
            //foreach (Renderer r in rs)
             //   r.enabled = false;
        }
        else {
            Destroy(this);
        }
    }


    // Update is called once per frame
    private void Update () {
	}
    
}
