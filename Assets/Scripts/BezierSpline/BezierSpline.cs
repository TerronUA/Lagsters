using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelSpline
{
    public class BezierSpline : MonoBehaviour
    {
        public BezierSplineData splineData;
        
        public int PointsCount
        { get { return splineData.points.Length; } }

        /// <summary>
        /// Returns BezierPoint
        /// </summary>
        /// <param name="index">Index of requested point</param>
        /// <returns></returns>
        public BezierPoint GetPoint(int index)
        {
            if ((0 <= index) && (index < PointsCount))
                return splineData.points[index];
            else
                return null;
        }

        /// <summary>
        /// Returns coordinate for points
        /// </summary>
        /// <param name="index">Index of the point on spline</param>
        /// <param name="indexCPoint">Index of the contol point. If less 0 - method returns spline point position. If more - returns control point position </param>
        /// <returns></returns>
        public Vector3 GetPosition(int index, int indexCPoint)
        {
            if (0 <= index && index < PointsCount)
            {
                if (0 <= indexCPoint && indexCPoint < splineData.points[index].points.Length)
                    return splineData.points[index].points[indexCPoint];
                else
                    return splineData.points[index].Point;
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Set coordinate for points
        /// </summary>
        /// <param name="index">Index of the point on spline</param>
        /// <param name="indexCPoint">Index of the contol point. If less 0 - method sets spline point position. If more - sets control point position</param>
        /// <param name="value">New point/controlPoint position</param>
        public void SetPosition(int index, int indexCPoint, Vector3 value)
        {
            if (0 <= index && index < PointsCount)
            {
                if (0 <= indexCPoint && indexCPoint < splineData.points[index].points.Length)
                    splineData.points[index].SetPosition(indexCPoint, value);
                else
                    splineData.points[index].SetPosition(0, value);
            }
        }        

        /// <summary>
        /// Returns list of next points after point in index
        /// </summary>
        /// <param name="index">Index of the point in array to return next points list</param>
        /// <returns></returns>
        public List<BezierPoint> GetNextPoints(int index)
        {
            List<BezierPoint> result = new List<BezierPoint>();

            for (int i = 0; i < splineData.edges.Length; i++)
            {
                if (splineData.edges[i].indexFirst == index)
                {
                    BezierPoint pt = GetPoint(splineData.edges[i].indexSecond);
                    if (pt != null)
                        result.Add(pt);
                }
            }
            return result;
        }

        public List<BezierEdge> GetEdgesFrom(int index)
        {
            List<BezierEdge> result = new List<BezierEdge>();

            for (int i = 0; i < splineData.edges.Length; i++)
            {
                if (splineData.edges[i].indexFirst == index)
                {
                    BezierEdge edge = splineData.edges[i];
                    if (edge != null)
                        result.Add(edge);
                }
            }
            return result;
        }

        public List<BezierEdge> GetEdgesTo(int index)
        {
            List<BezierEdge> result = new List<BezierEdge>();

            for (int i = 0; i < splineData.edges.Length; i++)
            {
                if (splineData.edges[i].indexSecond == index)
                {
                    BezierEdge edge = splineData.edges[i];
                    if (edge != null)
                        result.Add(edge);
                }
            }
            return result;
        }

        public int AddPointAfter(int index)
        {
            // To avoid any potential problems - add new points only if we have one point after current
            List<BezierEdge> edges = GetEdgesFrom(index);

            //if (edges.Count > 1)
            //    return -1;

            Array.Resize(ref splineData.points, splineData.points.Length + 1);
            int newIndex = splineData.points.Length - 1;
            BezierPoint pt = splineData.points[index];
            BezierPoint ptNew = splineData.points[newIndex] = new BezierPoint();

            this.SetPosition(newIndex, -1, pt.Point + (pt.Point - pt.GetPosition(2)));

            // Start edges from new point;
            foreach (BezierEdge edge in edges)
                edge.indexFirst = newIndex;

            Array.Resize(ref splineData.edges, splineData.edges.Length + 1);
            splineData.edges[splineData.edges.Length - 1] = new BezierEdge(index, newIndex);

            return newIndex;
        }
    }
}
