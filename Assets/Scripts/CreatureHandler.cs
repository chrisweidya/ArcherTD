using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HealthNetwork))]

public class CreatureHandler : NetworkBehaviour {
    [SyncVar(hook = "OnIsDeadHook")]
    private bool isDead = false;

    [SerializeField]
    protected Animator _animator;
    protected HealthNetwork _healthNetwork;
    protected float _radius;

    protected void Awake() {
        if(_animator == null)
            _animator = GetComponent<Animator>();
        _healthNetwork = GetComponent<HealthNetwork>();
    }

    [Command]
    protected void CmdAnimationPlayWithLayer(string name, int layer) {
        print("animation paly");
        RpcAnimationPlayWithLayer(name, layer);
    }

    [ClientRpc]
    protected void RpcAnimationPlayWithLayer(string name, int layer) {
        _animator.Play(name, layer);
    }

    [Command]
    protected void CmdAnimationPlay(string name) {
        RpcAnimationPlay(name);
    }

    [ClientRpc]
    protected void RpcAnimationPlay(string name) {
        _animator.Play(name);
    }

    [Command]
    protected void CmdSetAnimationTrigger(string triggerString) {
        RpcSetAnimationTrigger(triggerString);
    }

    [ClientRpc]
    private void RpcSetAnimationTrigger(string triggerString) {
        print("Anination trigger string:" + triggerString);
        _animator.SetTrigger(triggerString);
    }

    [Command]
    protected void CmdSetActive(bool val) {
        RpcSetActive(val);
    }

    [ClientRpc]
    private void RpcSetActive(bool val) {
        gameObject.SetActive(val);
    }

    [Command]
    protected void CmdDoDamage(GameObject target, float amt) {
        if (!isDead && !target.GetComponent<CreatureHandler>().GetIsDead())
            target.GetComponent<CreatureHandler>().TakeDamage(amt);
    }

    [Command]
    protected void CmdDoDamageById(NetworkInstanceId id, float amt) {
        CreatureHandler targetHandler = NetworkServer.FindLocalObject(id).GetComponent<CreatureHandler>();
        if (!isDead && !targetHandler.GetIsDead())
            targetHandler.TakeDamage(amt);
    }

    private void TakeDamage(float amt) {
        if (!isServer)
            Debug.LogError("Taking damage on client side.");
        _healthNetwork.TakeDamage(amt);
        if (_healthNetwork.GetHealth() <= 0)
            SetIsDead(true);
    }

    public virtual void SetIsDead(bool isDead) {
        this.isDead = isDead;
    }
    
    public void ResetDeath() {
        isDead = false;
    }

    public bool GetIsDead() {
        return isDead;
    }

    private void OnIsDeadHook(bool dead) {
        isDead = dead;
        OnIsDead(dead);
    }

    public virtual void OnIsDead(bool isDead) {

    }

    public float GetRadius() {
        return _radius;
    }
}
