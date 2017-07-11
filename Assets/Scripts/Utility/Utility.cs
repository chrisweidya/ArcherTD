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
}
