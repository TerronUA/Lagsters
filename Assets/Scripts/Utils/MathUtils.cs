using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Summary:
//     ///
//     A collection of common math functions.
//     ///
public struct MathUtils
{
    public static Vector3 ClosesPointOnLine(Vector3 position, Vector3 start, Vector3 end)
    {
        Vector3 vVector1 = position - start;
        Vector3 vVector2 = (end - start).normalized;

        float d = Vector3.Distance(start, end);
        float t = Vector3.Dot(vVector2, vVector1);

        if (t <= 0)
            return start;
        else if (t >= d)
            return end;
        else
            return start + vVector2 * t;
    }
}
