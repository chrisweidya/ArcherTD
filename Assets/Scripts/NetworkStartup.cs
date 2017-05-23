using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkStartup : MonoBehaviour {
       
    private NetworkManager _networkManager;
    // Use this for initialization
    private void Awake() {
        _networkManager = gameObject.GetComponent<NetworkManager>();
    }
       
	void Start () {
        print(_networkManager.networkAddress);
	}
	
	// Update is called once per frame
	void Update () {

       // print(_networkManager.networkAddress);
    }
}
