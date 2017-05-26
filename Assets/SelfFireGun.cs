using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SelfFireGun : NetworkBehaviour {

    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;

    private float timer;

	// Use this for initialization
	void Start () {
        timer = 2.0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (timer < 0)
        {
            timer = 2.0f;
            CmdFire();
        }
        timer -= Time.deltaTime;
	}
    
    [Command]
    private void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        GameObject bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawnPoint.position,
            bulletSpawnPoint.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }



}
