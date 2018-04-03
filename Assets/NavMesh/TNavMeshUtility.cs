using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNavMeshUtility
{
    static public float determinant2D(Vector3 v0, Vector3 v1)
    {
        return v1.z * v0.x - v0.z * v1.x;
    }

    //        v2
    //        |
    //        |
    //v0---------------v1
    //        |
    //        |
    //        v3

    static public bool isectSegSeg(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, ref Vector3 v)
    {
        Vector3 segment0 = v1 - v0;
        Vector3 segment1 = v3 - v2;

        Vector3 w = v0 - v2;

        float d = TNavMeshUtility.determinant2D(segment0, segment1);
        if (Mathf.Abs(d) < 0.000001f) //面积为0，在一条直线上
        {
            return false;
        }

        d = 1.0f / d;
        //float s = determinant2D(segment1, w) * d;
        //if (s < 0 || s > 1) return false;
        float t = determinant2D(segment0, w) * d;
        if (t < 0 || t > 1) return false;

        t = Mathf.Clamp(t, 0.1f, 0.9f);

        v = v2 + segment1 * t;

        return true;
    }
}
