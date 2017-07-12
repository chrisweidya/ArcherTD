using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerHandler : CreatureHandler {

    public static NetworkInstanceId localWardenNetId;
    public static GameObject localWardenGO;
    public static NetworkInstanceId enemyWardenNetId;
    public static GameObject enemyWardenGO;

    [SerializeField]
    private Vector3 _modelOffset = Vector3.zero;
    [SerializeField]
    private List<Renderer> _modelRenderers = new List<Renderer>();

    public enum PlayerState { Stand, BowPulled, BowReleased, Death}

    private PlayerState _playerState = PlayerState.Stand;


    private void OnEnable() {
        EventManager.ChangePlayerState += ChangeState;
        EventManager.DoDamageAction += DoDamage;
    }

    private void OnDisable() {
        EventManager.ChangePlayerState -= ChangeState;
        EventManager.DoDamageAction -= DoDamage;
    }

    private void Awake() {
        base.Awake();
    }

    private void Start() {
        if (isLocalPlayer) {
            GameManager.Instance.AssignCamera(transform.gameObject);
            transform.localPosition = _modelOffset;
            foreach (Renderer r in _modelRenderers) {
                r.enabled = false;
            }
            GameManager.SetLocalPlayerTeam(GetComponent<PlayerProperties>().GetTeam());
            localWardenNetId = netId;
            localWardenGO = gameObject;
            print(localWardenNetId);
        }
        else {
            enemyWardenNetId = netId;
            enemyWardenGO = gameObject;
            print(enemyWardenNetId);
        }
        _radius = GetComponent<CapsuleCollider>().radius;
    }

    private void ChangeState(PlayerState state, NetworkInstanceId netId) {
        if(isLocalPlayer && this.netId == netId)
            //if (this.netId == netId && _playerState != state) {
            CmdSetAnimationTrigger(state.ToString());
        //}
    }

    private void DoDamage(float dmg, NetworkInstanceId id) {
        if(isLocalPlayer) {
            CmdDoDamageById(id, dmg);
        }
    }

    //[ClientRpc]
    //private void RpcChangePlayerState(PlayerState state) {
    //    _playerState = state;
    //    RpcSetAnimationTrigger(state.ToString());
    //}
 
    //public void PlayerSetIsDead(bool isDead) {
    //    this.isDead = isDead;
    //    base.SetIsDead(isDead);
    //}

    //public bool PlayerGetIsDead() {
    //    return base.GetIsDead();
    //}

    public override void OnIsDead(bool isDead) {
        if (GetIsDead()) {
            transform.parent = null;
            ChangeState(PlayerState.Death, this.netId);            
            EventManager.FireGameEnd(netId);
        }
    }
}
