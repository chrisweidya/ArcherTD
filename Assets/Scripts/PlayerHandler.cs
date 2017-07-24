using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerHandler : CreatureHandler {
    public static NetworkInstanceId LocalWardenNetId;
    public static GameManager.Factions LocalFaction;
    public static float RespawnTimeSecs = 12;
    
    [SerializeField] private Vector3 _modelOffset = Vector3.zero;
    [SerializeField] private List<Renderer> _modelRenderers = new List<Renderer>();

    public enum PlayerState { Stand, BowPulled, BowReleased, Death};
    private enum PlayerAnimationTrigger { Stand, BowPulled, BowReleased, Death};
    private enum PlayerAnimationState { Stand, Death};
    private enum PlayerAnimationLayer { BaseLayer, AttackLayer};
    private PlayerState _playerState = PlayerState.Stand;

    public GameManager.Factions faction;

    [SerializeField]
    private float baseTowerDamage;
    [SerializeField]
    private float baseCreatureDamage;
    [SerializeField]
    private float towerDamage;
    [SerializeField]
    private float creatureDamage;

    private float incrementPercentage = 0.1f;
    private float currentIncrement = 1;
    private float maxIncrement = 3f;

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
                LocalFaction = faction;
                Spawn();
                foreach (Renderer r in _modelRenderers) {
                    r.enabled = false;
                }
                LocalWardenNetId = netId;
                GameManager.SetLocalPlayerFaction(faction);
            }
            if(isServer)
                PlayerManager.Instance.SetHeroOnce(gameObject, faction, netId);
            _radius = GetComponent<CapsuleCollider>().radius;
        }
        towerDamage = baseTowerDamage;
        creatureDamage = baseCreatureDamage;
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
        print("fdasf1");
        GameManager.Instance.AssignCamera(transform.gameObject, PlayerManager.Instance.GetSpawnPosition(faction));
        print("fdasf2");
        transform.localPosition = _modelOffset;
        EventManager.FirePlayerDeath(netId, false);
        CmdAnimationPlayWithLayer(PlayerAnimationState.Stand.ToString(), (int)PlayerAnimationLayer.BaseLayer);
        CmdAnimationPlayWithLayer(PlayerAnimationState.Stand.ToString(), (int)PlayerAnimationLayer.AttackLayer);
        print("fdasf3");
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

            CmdAnimationPlayWithLayer(PlayerAnimationState.Death.ToString(), (int)PlayerAnimationLayer.BaseLayer);
            CmdAnimationPlayWithLayer(PlayerAnimationState.Death.ToString(), (int)PlayerAnimationLayer.AttackLayer);
            StartCoroutine(Respawn(RespawnTimeSecs));
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

    public float GetTowerDamage() {
        return towerDamage;
    }
    public float GetCreatureDamage() {
        return creatureDamage;
    }
    public void PowerGain() {
        if (currentIncrement < maxIncrement) {
            currentIncrement += incrementPercentage;
            towerDamage = baseTowerDamage * currentIncrement;
            creatureDamage = baseCreatureDamage * currentIncrement;
        }
        Debug.Log("towerDamage " + towerDamage + " creature damage " + creatureDamage);
    }
}
