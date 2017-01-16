using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelSpline
{
    [System.Serializable]
    public class BezierSplineData : ScriptableObject
    {
        public BezierPoint[] points;

        public BezierSplineData()
        {
            points = new BezierPoint[0];
        }
    }
}
