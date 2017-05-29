using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHandler : NetworkBehaviour {

    [SerializeField]
    List<Renderer> _modelRenderers = new List<Renderer>();
    [SerializeField]
    private Renderer _weaponRenderer = null;

    [SyncVar(hook="TriggerPlayerAnimation")]
    private Enums.PlayerState _playerState = Enums.PlayerState.Stand;

    private void OnEnable() {
        EventManager.ChangePlayerState += ChangeState;
    }

    private void OnDisable() {
        EventManager.ChangePlayerState -= ChangeState;
    }

    private void ChangeState(Enums.PlayerState state) {
        _playerState = state;
    }

    private void TriggerPlayerAnimation(Enums.PlayerState playerstate) {
        print(playerstate);

    }

    private void Awake() {
        //_modelRenderer = gameObject.GetComponent<Renderer>();
    }

    private void Start () {
        if (isLocalPlayer) {
            GameManager.Instance.AssignCamera(transform.gameObject);
            foreach(Renderer r in _modelRenderers) {
                r.enabled = false;
            }
            //Renderer[] rs = GetComponentsInChildren<Renderer>();
            //foreach (Renderer r in rs)
             //   r.enabled = false;
        }
        else {
            Destroy(this);
        }
    }


    // Update is called once per frame
    private void Update () {
	}
    
}
