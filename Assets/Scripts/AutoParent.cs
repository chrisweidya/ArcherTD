using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem {
    public class AutoParent : MonoBehaviour {

        [SerializeField]
        private Hand hand;
        [SerializeField]
        private GameObject _primaryHandGameObject;

        public Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags;
        
        void Start() {
            if(gameObject == null || hand == null) {
                print("Hand or hand object missing.");
                return;
            }
            GameObject spawnedItem = GameObject.Instantiate(_primaryHandGameObject, transform);
            spawnedItem.SetActive(true);
            hand.AttachObject(spawnedItem, attachmentFlags, "");
        }

        // Update is called once per frame
        void Update() {

        }
    }
}