using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour {
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private List<AudioClip> audioClipList;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //search for the desired audio in the list
    private void searchForAudio(string s) {
        foreach (AudioClip c in audioClipList) {
            if (c.name == s) {
                audioSource.clip = c;
                break;
            }
        }   
    }

    //play the desired audio
    public void PlayAudio(string audioName) {
        //update audio if its not the same as the current audioclip
        if (audioSource.clip == null || audioSource.clip.name != audioName) {
            searchForAudio(audioName);
        }
        //play current audioclip
        if (audioSource.clip != null) {
            audioSource.Play();
        }
        else {
            Debug.Log(gameObject.name + "'s Audio Clip is null");
        }
    }
}
