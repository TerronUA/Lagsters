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
        {
            get
            {
                if ((splineData != null) && (splineData.points != null))
                    return splineData.points.Length;
                return 0;
            }
        }

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
                throw new IndexOutOfRangeException();
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
                    return splineData.points[index].points[indexCPoint].position;
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
                    splineData.points[index].SetPosition(-1, value);
            }
            else
                throw new Exception("Wrong index to set position");
        }        

        /// <summary>
        /// Returns list of next points after point in index
        /// </summary>
        /// <param name="index">Index of the point in array to return next points list</param>
        /// <returns></returns>
        public List<int> GetNextPointsIndexes(int index)
        {
            BezierPoint pt = GetPoint(index);

            List<int> result = new List<int>();

            for (int i = 0; i < pt.points.Length; i++)
            {
                if (pt.points[i].nextIndex >= 0)
                    result.Add(pt.points[i].nextIndex);
            }
            return result;
        }

        public List<BezierEdge> GetEdgesFrom(int index)
        {
            List<BezierEdge> result = new List<BezierEdge>();
            /*
            for (int i = 0; i < splineData.edges.Length; i++)
            {
                if (splineData.edges[i].indexFirst == index)
                {
                    BezierEdge edge = splineData.edges[i];
                    if (edge != null)
                        result.Add(edge);
                }
            }
            */
            return result;
        }

        public List<BezierEdge> GetEdgesTo(int index)
        {
            List<BezierEdge> result = new List<BezierEdge>();
            /*
            for (int i = 0; i < splineData.edges.Length; i++)
            {
                if (splineData.edges[i].indexSecond == index)
                {
                    BezierEdge edge = splineData.edges[i];
                    if (edge != null)
                        result.Add(edge);
                }
            }
            */
            return result;
        }

        public void AddFirstPoint()
        {
            if ((splineData != null) && (splineData.points != null))
            {
                Array.Resize(ref splineData.points, 1);
                splineData.points[0] = new BezierPoint();
            }
        }

        public int AddPointAfter(int index)
        {
            return 0;
            /*
            // To avoid any potential problems - add new points only if we have one point after current
            List<BezierEdge> edges = GetEdgesFrom(index);
            
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

            return newIndex;*/
        }
    }
}
