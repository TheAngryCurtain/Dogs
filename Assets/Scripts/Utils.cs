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

    public void SpawnObject(GameObject prefab, Vector3? position, System.Action<GameObject> callback)
    {
        GameObject obj = (GameObject)Instantiate(prefab, null);
        if (position != null)
        {
            obj.transform.position = position.Value;
        }

        if (callback != null)
        {
            callback(obj);
        }
    }

    private IEnumerator Delay(float time, System.Action callback)
    {
        yield return new WaitForSeconds(time);

        callback();
    }
}
