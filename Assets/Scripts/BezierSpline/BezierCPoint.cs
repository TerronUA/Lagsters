using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelSpline
{
    [System.Serializable]
    public class BezierCPoint
    {
        public Vector3 position;
        public int prevIndex;
        public int nextIndex;
        public int weight;

        public BezierCPoint()
        {
            position = Vector3.zero;
            nextIndex = -1;
            prevIndex = -1;
            weight = 1;
        }

        public BezierCPoint(Vector3 position, int prevIndex = -1, int nextIndex = -1, int weight = 1)
        {
            this.position = position;
            this.prevIndex = prevIndex;
            this.nextIndex = nextIndex;
            this.weight = weight;
        }
    }
}
