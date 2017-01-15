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
    public class BezierPoint 
    {
        /// <summary>
        /// Save all points in one array
        /// </summary>
        public Vector3[] points;

        // Use this for initialization
        public BezierPoint()
        {
            points = new Vector3[3];
            points[0] = Vector3.forward * 3; 
            points[1] = points[0] - Vector3.forward;
            points[2] = points[0] + Vector3.forward;
        }

        public Vector3 Point
        {
            get
            { return GetPosition(0); }
            set
            { SetPosition(0, value); }
        }

        public Vector3 GetPosition(int index)
        {
            if ((index < 0) && (index > points.Length - 1))
                return Vector3.zero;
            return points[index];
        }

        public void SetPosition(int index, Vector3 value)
        {
            if ((0 <= index) && (index < points.Length))
            {
                Vector3 delta = value - points[index];
                if (index == 0)
                    for (int i = 1; i < points.Length; i++)
                        points[i] += delta;

                points[index] = value;
            }
        }

        public Vector3 GetLeftPoint(int index)
        {
            if ((index < 0) && (index > NextPointsCount - 1))
                return Vector3.zero;
            return points[2 * index + 1];
        }

        public Vector3 GetRightPoint(int index)
        {
            if ((index < 0) && (index > NextPointsCount - 1))
                return Vector3.zero;
            return points[2 * (index + 1)];
        }

        public int NextPointsCount
        { get { return (points.Length - 1) / 2; } }

        public bool IsLeftPointIndex(int index)
        {
            return index % 2 != 0;
        }
    }
}
