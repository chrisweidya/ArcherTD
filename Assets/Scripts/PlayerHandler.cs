using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerHandler : NetworkBehaviour {

    [SerializeField]
    private Vector3 _modelOffset = Vector3.zero;
    [SerializeField]
    private List<Renderer> _modelRenderers = new List<Renderer>();
    [SerializeField]
    private Animator _animator = null;
    
    private Enums.PlayerState _playerState = Enums.PlayerState.Stand;

    private float CooldownStateChange = 0.5f;
    private float CurrentTimeElapsed = 0;

    private void OnEnable() {
        print("enabled player state change event");
        EventManager.ChangePlayerState += ChangeState;
    }

    private void OnDisable() {
        print("disable player state changed event");
        EventManager.ChangePlayerState -= ChangeState;
    }
    
    private void TriggerPlayerAnimation(Enums.PlayerState playerstate) {
        _animator.SetTrigger(playerstate.ToString());
    }

    private void ChangeState(Enums.PlayerState state) {
        if (isLocalPlayer && _playerState != state) {
            print("Dissimilar states");
            CmdChangePlayerState(state);
        }
    }

    [Command]
    private void CmdChangePlayerState(Enums.PlayerState state) {
        RpcChangePlayerState(state);
    }

    [ClientRpc]
    private void RpcChangePlayerState(Enums.PlayerState state) {
        print("Changed state to: " + state + " at client.");
        _playerState = state;
        TriggerPlayerAnimation(state);
    }

    private void Awake() {
        //Gets first animator component in children.
        if(_animator == null) {
            _animator = GetComponentsInChildren<Animator>()[0];
        }
    }

    private void Start () {
        if (isLocalPlayer) {
            GameManager.Instance.AssignCamera(transform.gameObject);
            //transform.localPosition = new Vector3(0.052f, -1.947f, -0.57f);
            //foreach(Renderer r in _modelRenderers) {
            //    r.enabled = false;
            //}
        }
    }

    // Update is called once per frame
    private void Update () {
	}
    
}
