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
                    return splineData.points[index].position;
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
        
        /// <summary>
        /// Returns list of prev points before point in index
        /// </summary>
        /// <param name="index">Index of the point in array to return prev points list</param>
        /// <returns></returns>
        public List<int> GetPrevPointsIndexes(int index)
        {
            BezierPoint pt = GetPoint(index);

            List<int> result = new List<int>();

            for (int i = 0; i < pt.points.Length; i++)
            {
                if (pt.points[i].prevIndex >= 0)
                    result.Add(pt.points[i].prevIndex);
            }
            return result;
        }

        /// <summary>
        /// Creates first point in spline
        /// </summary>
        public void AddFirstPoint()
        {
            if ((splineData != null) && (splineData.points != null))
            {
                Array.Resize(ref splineData.points, 1);
                splineData.points[0] = new BezierPoint();
            }
        }

        /// <summary>
        /// Add new point before selected point
        /// </summary>
        /// <param name="index">index of point</param>
        /// <returns></returns>
        public int AddPointBefore(int index)
        {
            List<int> nextPoints = GetNextPointsIndexes(index);
            List<int> prevPoints = GetPrevPointsIndexes(index);

            BezierPoint pt = GetPoint(index);

            Vector3 newPosition = pt.position - Vector3.forward * 5;
            
            Array.Resize(ref splineData.points, splineData.points.Length + 1);
            int newIndex = splineData.points.Length - 1;
            BezierPoint ptNew = splineData.points[newIndex] = new BezierPoint();

            this.SetPosition(newIndex, -1, newPosition);

            // Update link for first prev point
            if (prevPoints.Count > 0)
            {
                BezierPoint prevPoint = GetPoint(prevPoints[0]);
                prevPoint.UpdateNextCPoint(index, newIndex);

                ptNew.AddPrevCPoint(ptNew.position + Vector3.forward * 2, prevPoints[0]);
            }

            if (prevPoints.Count == 0)
                pt.AddPrevCPoint(pt.position - Vector3.forward * 2, newIndex);
            else
                pt.UpdatePrevCPoint(prevPoints[0], newIndex);

            ptNew.AddNextCPoint(ptNew.position - Vector3.forward * 2, index);

            return newIndex;
        }

        /// <summary>
        /// Add new point after selected point
        /// </summary>
        /// <param name="index">index of point</param>
        /// <returns></returns>
        public int AddPointAfter(int index)
        {
            List<int> nextPoints = GetNextPointsIndexes(index);
            List<int> prevPoints = GetPrevPointsIndexes(index);

            BezierPoint pt = GetPoint(index);

            Vector3 newPosition = pt.position + Vector3.forward * 5;
            
            Array.Resize(ref splineData.points, splineData.points.Length + 1);
            int newIndex = splineData.points.Length - 1;
            BezierPoint ptNew = splineData.points[newIndex] = new BezierPoint();

            this.SetPosition(newIndex, -1, newPosition);

            // Update link for first next point
            if (nextPoints.Count > 0)
            {
                BezierPoint nextPoint = GetPoint(nextPoints[0]);
                nextPoint.UpdatePrevCPoint(index, newIndex);

                ptNew.AddNextCPoint(ptNew.position  + Vector3.forward * 2, nextPoints[0]);
            }

            if (nextPoints.Count == 0)
                pt.AddNextCPoint(pt.position + Vector3.forward * 2, newIndex);
            else
                pt.UpdateNextCPoint(nextPoints[0], newIndex);

            ptNew.AddPrevCPoint(ptNew.position - Vector3.forward * 2, index);

            return newIndex;
        }

        /// <summary>
        /// Create branch for selected point
        /// </summary>
        /// <param name="index">index of point</param>
        public int AddBranchAfter(int index)
        {
            List<int> nextPoints = GetNextPointsIndexes(index);
            if (nextPoints.Count == 0)
                return AddPointAfter(index);

            List<int> prevPoints = GetPrevPointsIndexes(index);

            BezierPoint pt = GetPoint(index);

            Vector3 newPosition = pt.position + Vector3.forward * 5;

            Array.Resize(ref splineData.points, splineData.points.Length + 1);
            int newIndex = splineData.points.Length - 1;
            BezierPoint ptNew = splineData.points[newIndex] = new BezierPoint();

            this.SetPosition(newIndex, -1, newPosition);
            
            pt.AddNextCPoint(pt.position + Vector3.forward * 2, newIndex);
            ptNew.AddPrevCPoint(ptNew.position - Vector3.forward * 2, index);

            return newIndex;
        }

        /// <summary>
        /// Adds edge between 2 points
        /// </summary>
        /// <param name="indexStart">start index of the edge</param>
        /// <param name="indexEnd">last index of the edge</param>
        public void AddEdgeBetween(int indexStart, int indexEnd)
        {
            BezierPoint ptStart = GetPoint(indexStart);
            BezierPoint ptEnd = GetPoint(indexEnd);

            ptStart.AddNextCPoint(ptStart.position + Vector3.forward * 2, indexEnd);
            ptEnd.AddPrevCPoint(ptEnd.position - Vector3.forward * 2, indexStart);
        }

        /// <summary>
        /// Deletes edge, connected to point defined in index
        /// </summary>
        /// <param name="index">Index of the point</param>
        /// <param name="indexCPoint">Index of CPoint, defined edge</param>
        public void DeleteEdge(int index, int indexCPoint)
        {
            BezierPoint pt = GetPoint(index);
            BezierCPoint cPoint = pt.GetCPoint(indexCPoint);
            int indexEnd;
            if (pt.IsPrevCPointIndex(indexCPoint))
                indexEnd = cPoint.prevIndex;
            else
                indexEnd = cPoint.nextIndex;

            pt.DeleteCPointsToPoint(indexEnd);

            pt = GetPoint(indexEnd);
            pt.DeleteCPointsToPoint(index);
        }

        public void RevertEdge(int index, int indexCPoint)
        {
            BezierPoint pt = GetPoint(index);
            BezierCPoint cPoint = pt.GetCPoint(indexCPoint);
            int indexEnd;
            bool prevPointSelected = pt.IsPrevCPointIndex(indexCPoint);
            if (prevPointSelected)
            {
                cPoint.nextIndex = indexEnd = cPoint.prevIndex;
                cPoint.prevIndex = -1;
            }
            else
            {
                cPoint.prevIndex = indexEnd = cPoint.nextIndex;
                cPoint.nextIndex = -1;
            }

            pt = GetPoint(indexEnd);
            if (prevPointSelected)
            {
                cPoint = pt.GetNextPointTo(index);
                cPoint.prevIndex = index;
                cPoint.nextIndex = -1;
            }
            else
            {
                cPoint = pt.GetPrevPointTo(index);
                cPoint.prevIndex = -1;
                cPoint.nextIndex = index;
            }
        }

        /// <summary>
        /// Update next/prev indexes in CPoints after point removal
        /// </summary>
        /// <param name="index">index of the point that will be removed</param>
        private void UpdateIndexesAfterRemove(int index)
        {
            for (int i = 0; i < splineData.points.Length; i++)
            {
                BezierPoint pt = splineData.points[i];
                for (int j = 0; j < pt.points.Length; j++)
                {
                    if (pt.points[j].nextIndex > index)
                        pt.points[j].nextIndex--;
                    if (pt.points[j].prevIndex > index)
                        pt.points[j].prevIndex--;
                }
            }
        }

        /// <summary>
        /// Removes point from array and reconnect all points
        /// </summary>
        /// <param name="index">index of point in array to delete</param>
        private void DeletePointFromArray(int index)
        {
            Array.Copy(splineData.points, index + 1, splineData.points, index, splineData.points.Length - index - 1);
            
            Array.Resize(ref splineData.points, splineData.points.Length - 1);

            UpdateIndexesAfterRemove(index);
        }

        private void ReconnectPointsTo(List<int> indexes, int indexOld, int indexNew, bool edgeFromNewIndex)
        {
            if ((indexes == null) || (indexes.Count == 0))
                return;

            // start from index 1, because we connect all other points to the first one
            for (int i = 0; i < indexes.Count; i++)
            {
                BezierPoint pt = splineData.points[indexes[i]];
                pt.DeleteCPointsToPoint(indexOld);

                if (indexes[i] != indexNew)
                    if (edgeFromNewIndex)
                        AddEdgeBetween(indexNew, indexes[i]);
                    else
                        AddEdgeBetween(indexes[i], indexNew);
            }
        }

        public void DeletePoint(int index)
        {
            List<int> nextPoints = GetNextPointsIndexes(index);
            List<int> prevPoints = GetPrevPointsIndexes(index);

            if (prevPoints.Count == 0 && nextPoints.Count == 0)
            {
                if(splineData.points.Length == 1)
                    Array.Resize(ref splineData.points, 0);
                else
                    DeletePointFromArray(index);
            }
            else if (prevPoints.Count != 0 && nextPoints.Count == 0)
            {
                ReconnectPointsTo(prevPoints, index, prevPoints[0], false);

                DeletePointFromArray(index);
            }
            else if (prevPoints.Count == 0 && nextPoints.Count != 0)
            {
                ReconnectPointsTo(nextPoints, index, nextPoints[0], false);

                DeletePointFromArray(index);    
            }
            else if (prevPoints.Count == 1 && nextPoints.Count == 1)
            {
                splineData.points[prevPoints[0]].UpdateNextCPoint(index, nextPoints[0]);
                splineData.points[nextPoints[0]].UpdatePrevCPoint(index, prevPoints[0]);

                DeletePointFromArray(index);
            }
            else if (prevPoints.Count == 1 && nextPoints.Count > 1)
            {
                splineData.points[prevPoints[0]].DeleteCPointsToPoint(index);

                ReconnectPointsTo(nextPoints, index, prevPoints[0], true);

                DeletePointFromArray(index);
            }
            else if (prevPoints.Count > 1 && nextPoints.Count == 1)
            {
                splineData.points[nextPoints[0]].DeleteCPointsToPoint(index);

                ReconnectPointsTo(prevPoints, index, nextPoints[0], false);

                DeletePointFromArray(index);
            }
            else
            {
                splineData.points[prevPoints[0]].DeleteCPointsToPoint(index);

                ReconnectPointsTo(prevPoints, index, prevPoints[0], false);
                ReconnectPointsTo(nextPoints, index, prevPoints[0], true);

                DeletePointFromArray(index);
            }
        }

        public void RepairData()
        {
            for (int i = 0; i < splineData.points.Length; i++)
            {
                BezierPoint pt = splineData.points[i];
                
                // clean self-loops to point
                pt.DeleteCPointsToPoint(i);

                // Delete cPoints connected to indexes out of range
                for (int j = pt.points.Length - 1; j >= 0; j--)
                {
                    if (pt.points[j].nextIndex >= splineData.points.Length)
                        pt.DeleteCPointsToPoint(pt.points[j].nextIndex);
                    else if (pt.points[j].prevIndex >= splineData.points.Length)
                        pt.DeleteCPointsToPoint(pt.points[j].prevIndex);
                }
            }
        }

        public void EnforceTangent(int index)
        {
            BezierPoint pt = splineData.points[index];
            for (int i = 0; i < pt.points.Length; i++)
            {
                if (pt.IsPrevCPointIndex(i))
                {
                    pt.EnforceTangent(i);
                    return;
                }
            }
        }

        public void EnforceTangent(int index, int indexCPoint, int indexForceTangent)
        {
            BezierPoint pt = splineData.points[index];
            pt.EnforceTangent(indexCPoint, indexForceTangent);
        }
    }
}
