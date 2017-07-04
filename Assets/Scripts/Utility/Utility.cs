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
}
