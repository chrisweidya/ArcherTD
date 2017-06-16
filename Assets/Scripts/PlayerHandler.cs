using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerHandler : NetworkBehaviour {

    public static NetworkInstanceId localWardenNetId;

    [SerializeField]
    private Vector3 _modelOffset = Vector3.zero;
    [SerializeField]
    private List<Renderer> _modelRenderers = new List<Renderer>();
    [SerializeField]
    private Animator _animator = null;

    public enum PlayerState { Stand, BowPulled, BowReleased, Death}

    private PlayerState _playerState = PlayerState.Stand;
    [SyncVar(hook = "OnIsDead")]
    private bool isDead = false;

    private void OnEnable() {
        EventManager.ChangePlayerState += ChangeState;
    }

    private void OnDisable() {
        EventManager.ChangePlayerState -= ChangeState;
    }

    private void Awake() {
        //Gets first animator component in children.
        if (_animator == null) {
            _animator = GetComponentsInChildren<Animator>()[0];
        }
    }

    private void Start() {
        if (isLocalPlayer) {
            GameManager.Instance.AssignCamera(transform.gameObject);
            //print("fdsf");
            transform.localPosition = _modelOffset;
            foreach (Renderer r in _modelRenderers) {
                r.enabled = false;
            }
            localWardenNetId = netId;
        }
    }

    private void TriggerPlayerAnimation(PlayerState playerstate) {
        _animator.SetTrigger(playerstate.ToString());
    }

    private void ChangeState(PlayerState state, NetworkInstanceId netId) {
        if (this.netId == netId && _playerState != state) {
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
    
    private void Update () {
	}
 
    public void SetIsDead(bool isDead) {
        this.isDead = isDead;
    }

    public bool GetIsDead() {
        return isDead;
    }

    private void OnIsDead(bool isDead) {
        if (isDead) {
            transform.parent = null;
            EventManager.FirePlayerStateChange(PlayerState.Death, this.netId);
            EventManager.FireGameEnd(netId);
        }
    }
}
