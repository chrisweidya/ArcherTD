using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Valve.VR.InteractionSystem {
    [RequireComponent(typeof(Interactable))]
    public class HandEventHooks : MonoBehaviour {

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        private void OnAttachedToHand(Hand hand) {
            print("trigger");
        }
    }
}