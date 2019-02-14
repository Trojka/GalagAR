using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SortClockwise {

    public static List<Vector4> Sort(List<Vector4> pointList) {
        Vector3 center = new Vector3(
            pointList.Select(v => v.x).Average(),
            pointList.Select(v => v.y).Average(),
            pointList.Select(v => v.y).Average()
        );
        pointList.Sort((a, b) => ClockwiseCompareAroundCenter(a, b, center));

        return pointList;
    }

    public static int ClockwiseCompareAroundCenter(Vector4 a, Vector4 b, Vector4 center) {
        return IsLessAroundCenter(a, b, center) ? -1 : 1;
    }

    //https://stackoverflow.com/questions/6989100/sort-points-in-clockwise-order
    public static bool IsLessAroundCenter(Vector4 a, Vector4 b, Vector4 center) {
        if (a.x - center.x >= 0 && b.x - center.x < 0)
            return true;
        if (a.x - center.x < 0 && b.x - center.x >= 0)
            return false;
        if (a.x - center.x == 0 && b.x - center.x == 0)
        {
            if (a.y - center.y >= 0 || b.y - center.y >= 0)
                return a.y > b.y;
            return b.y > a.y;
        }

        // compute the cross product of vectors (center -> a) x (center -> b)
        float det = (a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y);
        if (det < 0)
            return true;
        if (det > 0)
            return false;

        // points a and b are on the same line from the center
        // check which point is closer to the center
        float d1 = (a.x - center.x) * (a.x - center.x) + (a.y - center.y) * (a.y - center.y);
        float d2 = (b.x - center.x) * (b.x - center.x) + (b.y - center.y) * (b.y - center.y);
        return d1 > d2;

    }
}
