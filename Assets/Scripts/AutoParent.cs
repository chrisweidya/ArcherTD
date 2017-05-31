using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Valve.VR.InteractionSystem {
    public class AutoParent : NetworkBehaviour {

        [SerializeField]
        private Hand hand;
        [SerializeField]
        private GameObject _primaryHandGameObject;

        public Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags;
        
        void Start() {
            CmdSpawnWeapons();

        }

        // Update is called once per frame
        void Update() {

        }

        [Command]
        void CmdSpawnWeapons()
        {
            SpawnWeapons();
            RpcSpawnWeapons();
        }
        [ClientRpc]
        void RpcSpawnWeapons()
        {
            SpawnWeapons();
            Debug.Log("RPC Spawned " + gameObject.name);
        }
        void SpawnWeapons()
        {
            if (gameObject == null || hand == null)
            {
                print("Hand or hand object missing.");
                return;
            }
            GameObject spawnedItem = GameObject.Instantiate(_primaryHandGameObject, transform);
            spawnedItem.SetActive(true);
            hand.AttachObject(spawnedItem, attachmentFlags, "");
            NetworkServer.Spawn(spawnedItem);
        }
       

    }
}