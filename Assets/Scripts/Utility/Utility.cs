using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {

	public static bool InRange(Vector3 position, Vector3 targetPosition, float range) {
        if (Vector3.SqrMagnitude(targetPosition - position) < range * range)
            return true;
        else
            return false;
    }

    public static bool InRange(Vector3 position, Vector3 targetPosition, float range, float targetOffsetRadius) {
        float distance = Vector3.SqrMagnitude(targetPosition - position);
        if (distance < (range + targetOffsetRadius) * (range + targetOffsetRadius))
            return true;
        else
            return false;
    }

    public static float InRangeGetDist(Vector3 position, Vector3 targetPosition, float range) {
        float distance = Vector3.SqrMagnitude(targetPosition - position);
        if (distance < range * range)
            return distance;
        else
            return -1;
    }

    public static IEnumerator RotateLerp(GameObject obj, Quaternion curr, Vector3 targetPos, float secs) {
        var originalTime = secs;
        Vector3 relativePos = targetPos - obj.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(relativePos);
        Quaternion originalRotation = obj.transform.rotation;
        while (secs > 0.0f) {
            secs -= Time.deltaTime;
            obj.transform.rotation = Quaternion.Lerp(originalRotation, lookRotation, 1 - (secs / originalTime));
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public static bool IsAliveAndInRange(GameObject currGO, GameObject targetGO, float range) {
        if (targetGO.GetComponent<CreatureHandler>().GetIsDead() || !InRange(currGO.transform.position, targetGO.transform.position,
            range, targetGO.GetComponent<CreatureHandler>().GetRadius())) {
            return false;
        }
        return true;
    }

    /*
    private static IEnumerator Bob(GameObject obj, bool upwards) {
        Vector3 _startPos = obj.transform.position;
        Vector3 _bobTargetPos;
        if (upwards)
            _bobTargetPos = new Vector3(_startPos.x, _startPos.y + 4, _startPos.z);
        else
            _bobTargetPos = new Vector3(_startPos.x, _startPos.y - 4, _startPos.z);
        float _startTime = Time.time;
        float fracJourney = (Time.time - _startTime) / 2;
        while (fracJourney < 1) {
            fracJourney = (Time.time - _startTime) /2;
            obj.transform.position = Vector3.Lerp(_startPos, _bobTargetPos, fracJourney);
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(Bob(obj, !upwards));
    }
    */
}
