using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelSpline
{
    [System.Serializable]
    public class BezierSplineData : ScriptableObject
    {
        public BezierPoint[] points;
        public BezierEdge[] edges;

        // Use this for initialization
        void Start()
        {
            points = new BezierPoint[0];
            edges = new BezierEdge[0];
        }
    }
}
