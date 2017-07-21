using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour {

    [SerializeField] private SpriteRenderer _bloodSpriteRenderer;
    [SerializeField] private float _fadeTime;
    // Use this for initialization

    private void Awake() {
        if (_bloodSpriteRenderer == null)
            Destroy(this);
    }

    void Start () {
	}

    private void OnEnable() {
        EventManager.TakeDamageUIEffect += FlashBlood;
    }

    private void OnDisable() {
        EventManager.TakeDamageUIEffect -= FlashBlood;
    }

    private void FlashBlood() {
        StopAllCoroutines();
        StartCoroutine(StartBloodEffect());
    }

    private IEnumerator StartBloodEffect() {
        float startTime = Time.time;
        float endTime = startTime + _fadeTime;
        float fraction; 
        while(Time.time < endTime) {
            fraction = 1 - (Time.time - startTime) / _fadeTime;
            _bloodSpriteRenderer.color = new Color(_bloodSpriteRenderer.color.r, _bloodSpriteRenderer.color.g, _bloodSpriteRenderer.color.b, fraction);
            yield return new WaitForEndOfFrame();
        }
    }
}
