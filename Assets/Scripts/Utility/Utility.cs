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

    public static float InRangeGetDist(Vector3 position, Vector3 targetPosition, float range) {
        float distance = Vector3.SqrMagnitude(targetPosition - position);
        if (distance < range * range)
            return distance;
        else
            return -1;
    }
}
