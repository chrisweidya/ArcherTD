using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class CreatureHandler : NetworkBehaviour {
    [SyncVar(hook = "OnIsDeadHook")]
    private bool isDead = false;

    [SerializeField]
    protected Animator _animator;

    protected void Awake() {
        if(_animator == null)
            _animator = GetComponent<Animator>();
    }

    [Command]
    protected void CmdSetAnimationTrigger(string triggerString) {
        RpcSetAnimationTrigger(triggerString);
    }

    [ClientRpc]
    public void RpcSetAnimationTrigger(string triggerString) {
        print("Anination trigger string:" + triggerString);
        _animator.SetTrigger(triggerString);
    }

    [Command]
    protected void CmdSetActive(bool val) {
        RpcSetActive(val);
    }

    [ClientRpc]
    public void RpcSetActive(bool val) {
        gameObject.SetActive(val);
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
}
