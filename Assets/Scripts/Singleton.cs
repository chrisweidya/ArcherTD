using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour {

    private static Singleton _instance;

    public static Singleton Instance { get { print("get"); return _instance; } }

    private void Awake() {
        print("created singelton");
        if(_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
    }
	
}
