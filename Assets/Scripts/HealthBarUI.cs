using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthBarUI : MonoBehaviour {

    private Slider healthBar;

	// Use this for initialization
	void Start () {
        healthBar = GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetHealthBar(float health)
    {
        healthBar.value = health;
    }
}
