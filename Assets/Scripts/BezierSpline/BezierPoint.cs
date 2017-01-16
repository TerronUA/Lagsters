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
        /// 
        public Vector3 position;
        public BezierCPoint[] points;

        // Use this for initialization
        public BezierPoint()
        {
            position = Vector3.forward * 3;
            points = new BezierCPoint[0];
        }

        public Vector3 Point
        {
            get
            { return GetPosition(-1); }
            set
            { SetPosition(-1, value); }
        }

        public Vector3 GetPosition(int index)
        {
            if ((index < 0) || (index > points.Length - 1))
                return position;
            return points[index].position;
        }

        public void SetPosition(int index, Vector3 value)
        {
            if (index < 0 )
            {
                Vector3 delta = value - position;
                for (int i = 0; i < points.Length; i++)
                    points[i].position += delta;

                position = value;
            }
            else if (index < points.Length)
                points[index].position = value;
        }

        public Vector3 GetPrevPointTo(int index)
        {
            Vector3 result = Vector3.zero;

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].prevIndex == index)
                {
                    result = points[i].position;
                    break;
                }
            }

            return result;
        }

        public Vector3 GetNextPointTo(int index)
        {
            Vector3 result = Vector3.zero;

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].nextIndex == index)
                {
                    result = points[i].position;
                    break;
                }
            }

            return result;
        }
        /*
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
        {
            get
            {
                int count = 0;
                for (int i = 0; i < points.Length; i++)
                    if (points[i].nextIndex >= 0)
                        count++;
                return count;
            }
        }
        */

        public bool IsPrevCPointIndex(int index)
        {
            if ((0 <= index) && (index < points.Length))
                return points[index].prevIndex >= 0;

            return false;
        }
    }
}
