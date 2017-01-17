using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelSpline
{
    /// <summary>
    /// Class for point on Bezier spline
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

        public bool IsPrevCPointIndex(int index)
        {
            if ((0 <= index) && (index < points.Length))
                return points[index].prevIndex >= 0;

            return false;
        }

        public void UpdateNextCPoint(int oldIndex, int newIndex)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].nextIndex == oldIndex)
                    points[i].nextIndex = newIndex;
            }
        }

        public void UpdatePrevCPoint(int oldIndex, int newIndex)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].prevIndex == oldIndex)
                    points[i].prevIndex = newIndex;
            }
        }

        public void AddCPoint(Vector3 position, int prevIndex = -1, int nextIndex = -1, int weight = 1)
        {
            Array.Resize(ref points, points.Length + 1);
            points[points.Length - 1] = new BezierCPoint(position, prevIndex, nextIndex, weight);
        }
    }
}
