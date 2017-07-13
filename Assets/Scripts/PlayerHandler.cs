using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerHandler : CreatureHandler {
    public static NetworkInstanceId localWardenNetId;
    
    [SerializeField] private Vector3 _modelOffset = Vector3.zero;
    [SerializeField] private List<Renderer> _modelRenderers = new List<Renderer>();
    [SerializeField] private float _respawnTimeSecs;

    public enum PlayerState { Stand, BowPulled, BowReleased, Death};
    private enum PlayerAnimationTrigger { Stand, BowPulled, BowReleased, Death};
    private enum PlayerAnimationState { Stand, Death};
    private enum PlayerAnimationLayer { BaseLayer, AttackLayer};
    private PlayerState _playerState = PlayerState.Stand;

    public GameManager.Factions faction;

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
        if (GameManager.Instance.GetCurrentSceneName() == GameManager.Scenes.MatchMaking.ToString()) {
            faction = GetComponent<PlayerProperties>().GetFaction();
            if (isLocalPlayer) {
                Spawn();
                foreach (Renderer r in _modelRenderers) {
                    r.enabled = false;
                }
                localWardenNetId = netId;
                GameManager.SetLocalPlayerFaction(faction);
            }
            PlayerManager.Instance.SetHeroOnce(gameObject, faction, netId);
            _radius = GetComponent<CapsuleCollider>().radius;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P) && isLocalPlayer) {
            DoDamage(99999, netId);
        }
    }

    private void ChangeState(PlayerState state, NetworkInstanceId netId) {
        if(isLocalPlayer && this.netId == netId)
            CmdSetAnimationTrigger(state.ToString());
    }

    private void DoDamage(float dmg, NetworkInstanceId id) {
        if(isLocalPlayer) {
            CmdDoDamageById(id, dmg);
        }
    }
    
    //client local player
    private void Spawn() {
        GameManager.Instance.AssignCamera(transform.gameObject, PlayerManager.Instance.GetSpawnPosition(faction));
        transform.localPosition = _modelOffset;
        EventManager.FirePlayerDeath(netId, false);
        CmdAnimationPlayWithLayer(PlayerAnimationState.Stand.ToString(), (int)PlayerAnimationLayer.BaseLayer);
        CmdAnimationPlayWithLayer(PlayerAnimationState.Stand.ToString(), (int)PlayerAnimationLayer.AttackLayer);
    }

    //Server
    private IEnumerator Respawn(float secs) {
        yield return new WaitForSeconds(secs);
        _healthNetwork.ResetHealth();
        SetIsDead(false);
    }

    //Server
    public override void SetIsDead(bool isDead) {
        base.SetIsDead(isDead);
        if (isDead) {
            CmdSetAnimationTrigger(PlayerAnimationTrigger.Death.ToString());
            StartCoroutine(Respawn(5));
        }
    }

    //Client local player
    public override void OnIsDead(bool isDead) {
        if (isLocalPlayer) {
            if (GetIsDead()) {
                transform.parent = null;
                EventManager.FirePlayerDeath(netId, true);
            }
            else
                Spawn();
        }
    }
}
