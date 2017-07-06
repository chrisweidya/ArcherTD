using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour {

    [SerializeField] private float radius;
    private GameObject _cylinder;
    private Rigidbody _rb;

	private void Start () {
        CreateCylinder();
    }

    private void OnEnable() {
    }

    private void CreateCylinder() {
        _cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        _cylinder.GetComponent<CapsuleCollider>().enabled = false;
        _rb = _cylinder.AddComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = true;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        _cylinder.transform.parent = transform;
        _cylinder.transform.localPosition = new Vector3(0, 1f, 0);
        _cylinder.transform.localScale = new Vector3(radius, 0.01f, radius);
    }

    void Update () {
		
	}
}
