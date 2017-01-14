using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelSpline
{
    /// <summary>
    /// Class for point on Bezier spline
    /// points[0]  - point on spline
    /// points[2 * index + 1 ] - left points, points to calculate Bezier spline with prev point
    /// points[2 * (index + 1) ] - rignt points, points to calculate Bezier spline with next point
    /// </summary>
    [System.Serializable]
    public class BezierPoint : ScriptableObject
    {
        /// <summary>
        /// Save all points in one array
        /// </summary>
        public Vector3[] points;

        // Use this for initialization
        void Start()
        {
            points = new Vector3[3];
        }

        public Vector3 Point
        {
            get
            { return points[0]; }
            set
            { points[0] = value; }
        }

        public Vector3 LeftPoint(int index)
        {
            if (index < 0)
                return Vector3.zero;
            return points[2 * index + 1];
        }

        public Vector3 RigntPoint(int index)
        {
            if (index < 0)
                return Vector3.zero;
            return points[2 * (index + 1)];
        }
    }
}
