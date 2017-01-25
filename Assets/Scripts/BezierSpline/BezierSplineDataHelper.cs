﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelSpline
{
    public static class BezierSplineDataHelper
    {
        public static bool IndexInRange(this BezierSplineData splineData, int index)
        {
            return (0 <= index && index < splineData.points.Length);
        }

        /// <summary>
        /// Update next/prev indexes in CPoints after point removal
        /// </summary>
        /// <param name="splineData">splineData variable</param>
        /// <param name="index">index of the point that will be removed</param>
        private static void UpdateIndexesAfterRemove(this BezierSplineData splineData, int index)
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
        /// <param name="splineData">splineData variable</param>
        /// <param name="index">index of point in array to delete</param>
        public static void DeletePointFromArray(this BezierSplineData splineData, int index)
        {
            Array.Copy(splineData.points, index + 1, splineData.points, index, splineData.points.Length - index - 1);

            Array.Resize(ref splineData.points, splineData.points.Length - 1);

            splineData.UpdateIndexesAfterRemove(index);
        }

        /// <summary>
        /// Adds edge between 2 points
        /// </summary>
        /// <param name="splineData">splineData variable</param>
        /// <param name="indexStart">start index of the edge</param>
        /// <param name="indexEnd">last index of the edge</param>
        public static void AddEdgeBetween(this BezierSplineData splineData, int indexStart, int indexEnd)
        {
            BezierPoint ptStart = splineData.points[indexStart];
            BezierPoint ptEnd = splineData.points[indexEnd];

            ptStart.AddNextCPoint(ptStart.position + Vector3.forward * 2, indexEnd);
            ptEnd.AddPrevCPoint(ptEnd.position - Vector3.forward * 2, indexStart);
        }

        public static void ReconnectPointsTo(this BezierSplineData splineData, List<int> indexes, int indexOld, int indexNew, bool edgeFromNewIndex)
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
                        splineData.AddEdgeBetween(indexNew, indexes[i]);
                    else
                        splineData.AddEdgeBetween(indexes[i], indexNew);
            }
        }

        private static void GetBezierPoints(this BezierSplineData splineData, int indexStart, int indexEnd, out Vector3 ptStart, out Vector3 ptEnd, out Vector3 ptStartCPoint, out Vector3 ptEndCPoint)
        {
            BezierPoint pt = splineData.points[indexStart];
            BezierCPoint ptC = pt.GetNextPointTo(indexEnd);
            if (ptC == null)
            {
                ptC = pt.GetPrevPointTo(indexEnd);
                if (ptC == null)
                    throw new EdgeNotExist();
            }

            ptStart = pt.position;
            ptStartCPoint = ptC.position;

            pt = splineData.points[indexEnd];
            ptC = pt.GetPrevPointTo(indexStart);
            if (ptC == null)
            {
                ptC = pt.GetNextPointTo(indexStart);
                if (ptC == null)
                    throw new EdgeNotExist();
            }

            ptEnd = pt.position;
            ptEndCPoint = ptC.position;
        }

        public static Vector3 GetPoint(this BezierSplineData splineData, int indexStart, int indexEnd, float t)
        {
            Vector3 ptStart;
            Vector3 ptEnd;
            Vector3 ptStartCPoint;
            Vector3 ptEndCPoint;

            splineData.GetBezierPoints(indexStart, indexEnd,
                out ptStart, out ptEnd, out ptStartCPoint, out ptEndCPoint);

            return Bezier.GetPoint(ptStart, ptStartCPoint, ptEndCPoint, ptEnd, t);
        }

        public static Vector3 GetVelocity(this BezierSplineData splineData, int indexStart, int indexEnd, float t)
        {
            Vector3 ptStart;
            Vector3 ptEnd;
            Vector3 ptStartCPoint;
            Vector3 ptEndCPoint;

            splineData.GetBezierPoints(indexStart, indexEnd,
                out ptStart, out ptEnd, out ptStartCPoint, out ptEndCPoint);

            return Bezier.GetFirstDerivative(ptStart, ptStartCPoint, ptEndCPoint, ptEnd, t);
        }

        public static Vector3 GetDirection(this BezierSplineData splineData, int indexStart, int indexEnd, float t)
        {
            return splineData.GetVelocity(indexStart, indexEnd, t).normalized;
        }
    }
}