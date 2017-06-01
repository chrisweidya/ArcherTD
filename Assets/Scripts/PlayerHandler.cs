using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof (SpawnArrow))]

public class PlayerHandler : NetworkBehaviour {

    [SerializeField]
    private List<Renderer> _modelRenderers = new List<Renderer>();
    private SpawnArrow _spawnArrowScript = null;

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
        _spawnArrowScript = GetComponent<SpawnArrow>();
    }

    private void Start () {
        if (isLocalPlayer) {
            print(NetworkServer.active);
            GameManager.Instance.AssignCamera(transform.gameObject);
            foreach(Renderer r in _modelRenderers) {
                r.enabled = false;
            }
           // _spawnArrowScript.enabled = true;
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
