using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthBarUI : MonoBehaviour {

    private Slider healthBar;

	// Use this for initialization
	void Awake () {
        healthBar = GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetHealthBar(float health,float maxHealth)
    {
        healthBar.value =  ( health/maxHealth ) * 100;
    }
}
