using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerHandler : CreatureHandler {
    public static NetworkInstanceId localWardenNetId;
    
    [SerializeField] private Vector3 _modelOffset = Vector3.zero;
    [SerializeField] private List<Renderer> _modelRenderers = new List<Renderer>();

    public enum PlayerState { Stand, BowPulled, BowReleased, Death}
    private PlayerState _playerState = PlayerState.Stand;

    public string debugServer;
    public string debugIsLocal;

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
            localWardenNetId = netId;
            GameManager.SetLocalPlayerFaction(GetComponent<PlayerProperties>().GetFaction());
        }
        PlayerManager.Instance.SetHeroOnce(gameObject, GetComponent<PlayerProperties>().GetFaction(), netId);
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
    
    public override void OnIsDead(bool isDead) {
        if (GetIsDead()) {
            transform.parent = null;
            ChangeState(PlayerState.Death, this.netId);            
            EventManager.FireGameEnd(netId);
        }
    }
}
