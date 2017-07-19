using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpAnimator : MonoBehaviour {

    public IEnumerator StartMove(GameObject obj, Vector3 endPos, float secs, bool bobAtEnd) {
        Vector3 startPos = obj.transform.position;
        float startTime = Time.time;
        float fracJourney = (Time.time - startTime) / secs;
        while (fracJourney < 1) {
            fracJourney = (Time.time - startTime) / secs;
            obj.transform.position = Vector3.Lerp(startPos, endPos, fracJourney);
            yield return new WaitForEndOfFrame();
        }
        if(bobAtEnd)
            StartCoroutine(Bob(obj, bobAtEnd));
    }

    public IEnumerator Bob(GameObject obj, bool upwards) {
        Vector3 _startPos = obj.transform.position;
        Vector3 _bobTargetPos;
        if (upwards)
            _bobTargetPos = new Vector3(_startPos.x, _startPos.y + 4, _startPos.z);
        else
            _bobTargetPos = new Vector3(_startPos.x, _startPos.y - 4, _startPos.z);
        float _startTime = Time.time;
        float fracJourney = (Time.time - _startTime) / 1;
        while (fracJourney < 1) {
            fracJourney = (Time.time - _startTime) / 1;
            obj.transform.position = Vector3.Lerp(_startPos, _bobTargetPos, fracJourney);
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(Bob(obj, !upwards));
    }


}