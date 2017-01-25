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

        public BezierCPoint GetCPoint(int index)
        {
            if (0 <= index && index < points.Length)
                return points[index];

            throw new IndexOutOfRangeException();
        }

        public BezierCPoint GetPrevPointTo(int index)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].prevIndex == index)
                    return points[i];
            }

            return null;
        }

        public BezierCPoint GetNextPointTo(int index)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].nextIndex == index)
                    return points[i];
            }

            return null;
        }

        public Vector3 GetPrevPointPositionTo(int index)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].prevIndex == index)
                    return points[i].position;
            }

            throw new EdgeNotExist();
        }

        public Vector3 GetNextPointPositionTo(int index)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].nextIndex == index)
                    return points[i].position;
            }

            throw new EdgeNotExist();
        }

        /// <summary>
        /// Returns list of next points after point in index
        /// </summary>
        /// <param name="index">Index of the point in array to return next points list</param>
        /// <returns></returns>
        public List<int> GetNextPointsIndexes()
        {
            List<int> result = new List<int>();

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].nextIndex >= 0)
                    result.Add(points[i].nextIndex);
            }
            return result;
        }

        /// <summary>
        /// Returns list of prev points before point in index
        /// </summary>
        /// <param name="index">Index of the point in array to return prev points list</param>
        /// <returns></returns>
        public List<int> GetPrevPointsIndexes()
        {
            List<int> result = new List<int>();

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].prevIndex >= 0)
                    result.Add(points[i].prevIndex);
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

        public void AddNextCPoint(Vector3 position, int nextIndex, int weight = 1)
        {
            AddCPoint(position, -1, nextIndex, weight);
        }

        public void AddPrevCPoint(Vector3 position, int prevIndex, int weight = 1)
        {
            AddCPoint(position, prevIndex, -1, weight);
        }

        private void AddCPoint(Vector3 position, int prevIndex = -1, int nextIndex = -1, int weight = 1)
        {
            Array.Resize(ref points, points.Length + 1);
            points[points.Length - 1] = new BezierCPoint(position, prevIndex, nextIndex, weight);
        }

        private void DeleteCPoint(int index)
        {
            if (0 <= index && index < points.Length)
            {
                if (index < points.Length - 1)
                    Array.Copy(points, index + 1, points, index, points.Length - index - 1);

                Array.Resize(ref points, points.Length - 1);
            }
        }

        /// <summary>
        /// Removes all CPoints to/from point, defined in index
        /// </summary>
        /// <param name="index">index pf the point</param>
        public void DeleteCPointsToPoint(int index)
        {
            for (int i = points.Length - 1; i >= 0; i--)
            {
                if ((points[i].nextIndex == index) || (points[i].prevIndex == index))
                {
                    if (i != points.Length - 1)
                        Array.Copy(points, i + 1, points, i, points.Length - i - 1);

                    Array.Resize(ref points, points.Length - 1);
                }
            }
        }
     
        public void EnforceTangent(int indexCPoint)
        {
            BezierCPoint cp = points[indexCPoint];
            Vector3 cpPosition = cp.position;
            Vector3 tangent = position - cpPosition;

            bool isPrevPoint = IsPrevCPointIndex(indexCPoint);
            for (int i = 0; i < points.Length; i++)
            {
                if (i != indexCPoint & isPrevPoint && !IsPrevCPointIndex(i))
                {
                    tangent = tangent.normalized * Vector3.Distance(position, points[i].position);
                    SetPosition(i, position + tangent);
                }
            }
        }

        public void EnforceTangent(int indexCPoint, int indexForceTangent)
        {
            BezierCPoint cp = points[indexCPoint];
            Vector3 cpPosition = cp.position;
            Vector3 tangent = position - cpPosition;
            tangent = tangent.normalized * Vector3.Distance(position, points[indexForceTangent].position);
            SetPosition(indexForceTangent, position + tangent);
        }

        public void DeleteEdge(int index)
        {

        }
    }
}
