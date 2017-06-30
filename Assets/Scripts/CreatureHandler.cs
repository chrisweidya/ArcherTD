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

    [ClientRpc]
    public void RpcSetAnimationTrigger(string triggerString) {
        _animator.SetTrigger(triggerString);
    }

    public virtual void SetIsDead(bool isDead) {
        this.isDead = isDead;
    }

    public bool GetIsDead() {
        return isDead;
    }
    public void ResetDeath() {
        isDead = false;
    }
    private void OnIsDeadHook(bool dead) {
        isDead = dead;
        OnIsDead(dead);
    }
    public virtual void OnIsDead(bool isDead) {

    }


}
