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


    public enum PlayerState { Stand, BowPulled, BowReleased, Death}

    private PlayerState _playerState = PlayerState.Stand;

    private void OnEnable() {
        print("enabled player state change event");
        EventManager.ChangePlayerState += ChangeState;
    }

    private void OnDisable() {
        print("disable player state changed event");
        EventManager.ChangePlayerState -= ChangeState;
    }
    
    private void TriggerPlayerAnimation(PlayerState playerstate) {
        _animator.SetTrigger(playerstate.ToString());
    }

    private void ChangeState(PlayerState state) {
        if (isLocalPlayer && _playerState != state) {
            CmdChangePlayerState(state);
        }
    }

    [Command]
    private void CmdChangePlayerState(PlayerState state) {
        RpcChangePlayerState(state);
    }

    [ClientRpc]
    private void RpcChangePlayerState(PlayerState state) {
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
            //print("fdsf");
            //transform.localPosition = new Vector3(0.052f, -1.947f, -0.57f);
            //foreach(Renderer r in _modelRenderers) {
            //    r.enabled = false;
           // }
        }
    }
    
    private void Update () {
        if(Input.GetKeyDown(KeyCode.K)) {
            print("pressed");
            
        TriggerPlayerAnimation(PlayerState.Death);
            //EventManager.FirePlayerStateChange(PlayerState.Death);
        }
	}
    
}
