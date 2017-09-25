using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : Singleton<Utils>
{
    public static float SignedAngle(Vector3 a, Vector3 b)
    {
        float angle = Vector3.Angle(a, b);
        Vector3 cross = Vector3.Cross(a, b);
        if (cross.y < 0) angle = -angle;

        return angle;
    }

    public void ActAfterDelay(float time, System.Action callback)
    {
        StartCoroutine(Delay(time, callback));
    }

    private IEnumerator Delay(float time, System.Action callback)
    {
        yield return new WaitForSeconds(time);

        callback();
    }
}
