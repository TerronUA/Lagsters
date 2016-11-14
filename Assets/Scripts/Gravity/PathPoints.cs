using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathPoints : ScriptableObject
{
    public int layer;
    public int drawPointsCount = 3;
    public List<PathPoint> pointsList; 
}
