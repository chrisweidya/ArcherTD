using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private float _turnInput = 0;
    private float _moveInput = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetInputs();
        UpdateMovement();
        print("fads"); 
	}

    private void GetInputs() {
        _turnInput = Input.GetAxis("Horizontal");
        _moveInput = Input.GetAxis("Vertical");
    }

    private void UpdateMovement() {
        transform.Rotate(0, _turnInput * Time.deltaTime * 150f, 0);
        transform.Translate(0, 0, _moveInput * Time.deltaTime * 3f);
    }
}
