using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLerp : MonoBehaviour {
    [SerializeField] private Vector3 _endPos;
    [SerializeField] private float _timeSecs;
    [SerializeField] private bool _bobAndMoveAway;

    LerpAnimator _lerpAnimator;

    private void Awake() {
        _lerpAnimator = GetComponent<LerpAnimator>();
    }
    
	void Start () {
        if (_bobAndMoveAway)
            StartCoroutine(BobAndMove());
        else
            StartCoroutine(_lerpAnimator.StartMove(gameObject, _endPos, _timeSecs, true));
    }

    private IEnumerator BobAndMove() {
        float startTime = Time.time;
        Coroutine bobber = StartCoroutine(_lerpAnimator.Bob(gameObject, true));
        while(Time.time - startTime < _timeSecs) {
            yield return new WaitForSeconds(0.5f);
        }
        StopCoroutine(bobber);
        StartCoroutine(_lerpAnimator.StartMove(gameObject, _endPos, 1, false));
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
