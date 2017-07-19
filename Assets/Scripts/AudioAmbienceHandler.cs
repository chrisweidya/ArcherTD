using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAmbienceHandler : MonoBehaviour {

    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private float minRange;
    [SerializeField]
    private float maxRange;

    // Use this for initialization
    void Start () {
        if (source == null) {
            source = GetComponent<AudioSource>();
        }
        StartCoroutine(RandomAmbience());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator RandomAmbience() {

        while (true) {
            source.Play();
            yield return new WaitForSeconds(Random.Range(minRange, maxRange));
        }
       
    }
}
