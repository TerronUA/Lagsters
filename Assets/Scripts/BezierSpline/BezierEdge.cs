using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelSpline
{
    [System.Serializable]
    public class BezierEdge 
    {
        public int indexFirst = -1;
        public int indexSecond = -1;
        public int weight = 1;

        public BezierEdge(int index0, int index1)
        {
            indexFirst = index0;
            indexSecond = index1;
        }
    }
}
