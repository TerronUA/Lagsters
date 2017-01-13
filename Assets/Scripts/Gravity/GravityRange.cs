using UnityEngine;
using System.Collections;

public class GravityRange
{
    public GravityPoint startPoint;
    public GravityPoint endPoint;

    public GravityRange()
    {
        startPoint = null;
        endPoint = null;
    }
    public GravityRange(GravityPoint sPoint, GravityPoint ePoint)
    {
        startPoint = sPoint;
        endPoint = ePoint;
    }
}
