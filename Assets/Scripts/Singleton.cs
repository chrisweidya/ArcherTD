using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour{

    private static T _instance;

    public static T Instance { get { print("get"); return _instance; } }

    protected virtual void Awake() {
        if (_instance != null) {
            print("Singleton exists of type: " + typeof(T));
            Destroy(gameObject);
        }
        else {
            print("Created new Singleton");
            _instance = gameObject.GetComponent<T>();
            DontDestroyOnLoad(gameObject);
        }
    }	
}
