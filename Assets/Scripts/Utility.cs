using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {

    public static Vector3 BezierCurve(Vector3 p0, Vector3 p1, float t) {
        // Bezier = (1 - t)[(1 - t)P0 + tP1] + t[(1 - t)P1 + tPt]
        return p0 + t * (p1 - p0);
    }

    public static Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        // Bezier = (1 - t)[(1 - t)P0 + tP1] + t[(1 - t)P1 + tPt]
        return (1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2);
    }

    public static Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2,Vector3 p4, float t) {
        // Bezier = (1 - t)[(1 - t)P0 + tP1] + t[(1 - t)P1 + tPt]
        //return (1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2);
        throw new System.NotImplementedException("Yell at Josh to fix this...");
    }

    public static Vector3 CreatePeak(Vector3 p0, Vector3 p1, float t, float heightMult) {
        Vector3 point = BezierCurve(p0, p1, t);
        Vector3 forward = (p1 - p0).normalized;
        Vector3 up = Vector3.Cross(forward, Vector3.right);
        return up * heightMult;
    }

    public static Vector3 CreatePeak(Vector3 p0, Vector3 p1, float heightMult)
    {
        Vector3 midpoint = ((p1 - p0) * .5f) + p0;
        Vector3 peak = (Vector3.up * heightMult) + midpoint;
        return peak;
    }

}
