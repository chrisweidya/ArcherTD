using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillReward : MonoBehaviour {
    [SerializeField]
    private GameObject particlePrefab;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Reward(GameObject enemyHero) {
        GameObject particle = Instantiate<GameObject>(particlePrefab, transform.position, transform.rotation) as GameObject;
        Vector3 temp = particle.transform.position;
        temp.y += 2f;
        particle.transform.position = temp;
        particle.GetComponent<particleAttractorLinear>().target = enemyHero.GetComponent<PlayerHandler>().BowLocation();
        enemyHero.GetComponent<PlayerHandler>().PowerGain();
        Destroy(particle, 4f);
    }
}
