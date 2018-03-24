using System.Collections.Generic;
using UnityEngine;


//http://digestingduck.blogspot.jp/2010/03/simple-stupid-funnel-algorithm.html
public class Funnel
{
    private static float TriangleArea2(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        float ax = v1.x - v0.x;
        float ay = v1.z - v0.z;
        float bx = v2.x - v0.x;
        float by = v2.z - v0.z;
        return bx * ay - ax * by;
    }

    public static void StringPull(Vector3 start, Vector3 target, List<Vector3> portals, List<Vector3> path)
    {
        path.Add(start);

        Vector3 vApex = start;
        Vector3 vLeft = vApex;
        Vector3 vRight = vApex;
        int apexIndex = 0;
        int leftIndex = 0;
        int rightIndex = 0;

        for (int i = 0; i < portals.Count; i += 2)
        {
            Vector3 vEdgeLeft = portals[i];
            if (TriangleArea2(vApex, vLeft, vEdgeLeft) >= 0.0f)
            {
                if (vApex == vLeft || TriangleArea2(vApex, vRight, vEdgeLeft) < 0.0f)
                {
                    vLeft = vEdgeLeft;
                    leftIndex = i;
                }
                else  //vEdgeLeft在vRight的右边了
                {
                    path.Add(vRight);
                    vApex = vRight;
                    apexIndex = rightIndex;

                    vRight = vApex;
                    vLeft = vApex;
                    leftIndex = apexIndex;
                    rightIndex = apexIndex;
                    i = apexIndex;
                    continue;
                }
            }

            Vector3 vEdgeRight = portals[i + 1];
            if (TriangleArea2(vApex, vRight, vEdgeRight) <= 0.0f)
            {
                if (vApex == vRight || TriangleArea2(vApex, vLeft, vEdgeRight) > 0.0f)
                {
                    vRight = vEdgeRight;
                    rightIndex = i;
                }
                else  //vEdgeRight在vLeft的左边了
                {
                    path.Add(vLeft);
                    vApex = vLeft;
                    apexIndex = leftIndex;

                    vRight = vApex;
                    vLeft = vApex;

                    leftIndex = apexIndex;
                    rightIndex = apexIndex;
                    // Restart scan
                    i = apexIndex;

                    continue;
                }
            }
        }

        path.Add(target);
    }
}
