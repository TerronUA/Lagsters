using UnityEngine;
using System.Collections;

public enum SplineType
{
    stLinear,
    stQuadratic,
    stCubic
}

[System.Serializable]
public class LevelSplineSubPoint 
{
    public Vector3 position;
    public SplineType splineType = SplineType.stLinear; 
}