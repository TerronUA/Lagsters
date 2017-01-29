using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LevelSpline;

public class GravityManager : MonoBehaviour
{
    public LayerMask layerMask;
    public float offset = 10f;

    public bool useLastPointGravity = false;

    public float gravity = 9.8f;

    public List<PathPoint> points
    {
        get
        {
            if (pathPoints != null)
                return pathPoints.pointsList;
            else
                return null;
        }
    }

    public LevelSpline.BezierSplineData spline;
    public int splinePoinsAmount = 50;

    public int activeIndex = -1;

    public PathPoints pathPoints;

    public PathPoint activePoint = null;

    private List<Vector3> hitPoints;

    public PathPoint ActivePoint
    {
        get
        {
            UpdateActivePoint();
            return activePoint;
        }
        set
        {
            UpdateActivePoint();
            if (activePoint != null)
                activePoint = value;
        }
    }

    public void UpdateActivePoint()
    {
        if ((-1 < activeIndex) && (activeIndex < points.Count))
            activePoint = points[activeIndex];
        else
            activePoint = null;
    }

    public int AddPoint()
    {
        if  (pathPoints == null)
          return -1;

        PathPoint newPoint = new PathPoint();

        switch (points.Count)
        {
            case 0:
                break;
            case 1:
                newPoint.position = points[points.Count - 1].position;
                newPoint.position += Vector3.forward * offset;
                break;
            default:
                newPoint.position = points[points.Count - 1].position;
                newPoint.position += (points[points.Count - 1].position - points[points.Count - 2].position).normalized * offset;
                break;
        }

        if (useLastPointGravity && (points.Count > 0))
            newPoint.gravity = points[points.Count - 1].gravity;
        else
            newPoint.gravity = gravity;

        pathPoints.pointsList.Add(newPoint);

        UpdateActivePoint();

        return points.Count - 1;
    }

    public void DeletePoint()
    {
        if ((points.Count > 0) && (-1 < activeIndex) && (activeIndex < points.Count))
        {
            pathPoints.pointsList.RemoveAt(activeIndex);
            if (activeIndex >= points.Count)
                activeIndex = points.Count - 1;
        }

        UpdateActivePoint();
    }

    public void DeleteAllPoints()
    {
        pathPoints.pointsList.Clear();

        // Delete all child objects with colliders
        int j = 0;
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            j++;
        }

        activeIndex = -1;
        UpdateActivePoint();

    }

    public void RealignPoint()
    {
        //const int countRealignPoint = 15;

        // Dont calculate position if less then 2 points - we dont have direction
        if (points.Count < 2)
            return;

        UpdateActivePoint();

        if (activePoint == null)
            return;
/*
        Vector3 otherPoint;
        if (activeIndex > 0)
            otherPoint = points[activeIndex - 1].position;
        else
            otherPoint = points[activeIndex + 1].position;
*/
        activePoint.position = CalculateNewPointPosition(activePoint.position, activePoint.raycastDistance);
/*
        // Calculate new forward direction
        otherPoint = activePoint.position + (activePoint.position - otherPoint).normalized;
        Debug.DrawLine(activePoint.position, otherPoint * 3);
        otherPoint = CalculateNewPointPosition(otherPoint, activePoint.position, activePoint.raycastDistance);
        Debug.DrawLine(activePoint.position, otherPoint * 3, Color.cyan);
        activePoint.rotation = Quaternion.LookRotation(otherPoint);
*/
    }

    public void GeneratePointsOnBezier()
    {
        
        if (pathPoints == null)
            return;

        if (spline == null)
            return;

        DeleteAllPoints();

        float progress = 0;
        Vector3 position, direction, reversePosition, reverseDirection;

        for (int i = 0; i < spline.points.Length; i++)
        {
            BezierPoint pt = spline.points[i];
            for (int j = 0; j < pt.points.Length; j++)
            {
                if (pt.IsPrevCPointIndex(j))
                    continue;

                BezierCPoint cPoint = pt.points[j];

                progress = 0;
                for (int k = 0; k < splinePoinsAmount; k++)
                {
                    if (k > 0)
                        progress = 1f / splinePoinsAmount * (k);
                    position = spline.GetPoint(i, cPoint.nextIndex, progress);
                    direction = spline.GetDirection(i, cPoint.nextIndex, progress);
                    reversePosition = spline.GetPoint(i, cPoint.nextIndex, progress + 0.001f);
                    reverseDirection = spline.GetDirection(i, cPoint.nextIndex, progress + 0.001f);

                    CreateGravityPoint(position, direction, reversePosition, reverseDirection);
                }
            }
        }

        if (pathPoints.pointsList.Count > 0)
            activeIndex = 0;

        UpdateActivePoint();
    }

    private void CreateGravityPoint(Vector3 position, Vector3 direction, Vector3 reversePosition, Vector3 reverseDirection)
    {
        PathPoint newPoint = new PathPoint();

        newPoint.position = position;
        newPoint.rotation = Quaternion.LookRotation(direction);
        newPoint.gravity = gravity;

        int index = pathPoints.pointsList.Count;
        // Create PathPoing Collider
        GameObject go = new GameObject("Path collider " + index);

        go.transform.parent = gameObject.transform;
        go.transform.position = newPoint.position;
        go.transform.rotation = newPoint.rotation;

        BoxCollider colliderNormal = go.AddComponent<BoxCollider>();
        colliderNormal.size = new Vector3(20f, 20f, 0.05f);
        colliderNormal.isTrigger = true;

        PathNodeCollider pn = go.AddComponent<PathNodeCollider>();
        pn.indexGravityNode = pathPoints.pointsList.Count;

        // Create Reverse collider
        GameObject goReverse = new GameObject("Reverse Path collider " + index);

        goReverse.transform.parent = gameObject.transform;
        goReverse.transform.position = reversePosition;
        goReverse.transform.rotation = Quaternion.LookRotation(reverseDirection);

        BoxCollider colliderReverse = goReverse.AddComponent<BoxCollider>();
        colliderReverse.size = new Vector3(20f, 20f, 0.05f);
        colliderReverse.isTrigger = true;

        PathNodeReverseCollider pnReverse = goReverse.AddComponent<PathNodeReverseCollider>();

        pn.reverseCollider = pnReverse;
        pnReverse.normalCollider = pn;

        newPoint.collider = colliderNormal;
        newPoint.colliderReverse = colliderReverse;

        pathPoints.pointsList.Add(newPoint);
    }


    Vector3 CalculateNewPointPosition(Vector3 point, float rayCastDistance = Mathf.Infinity)
    {
        Vector3 newPos = point;
        RaycastHit hit;

        Vector3[] directions = GetSphereDirections(16);

        for (int i = 0; i < 15; i++)
        {
            if (hitPoints == null)
                hitPoints = new List<Vector3>();
            else
                hitPoints.Clear();

            foreach (var direction in directions)
            {
                if (Physics.Raycast(point, direction, out hit, rayCastDistance, layerMask))
                {
                    hitPoints.Add(hit.point);
                    //Debug.DrawLine(point, hit.point, Color.blue);
                }
            }

            Debug.Log("hitPoints.Count = " + hitPoints.Count);

            if (hitPoints.Count == 0)
                return point;

            newPos = Vector3.zero;
            foreach (var pt in hitPoints)
                newPos += pt;
            if (hitPoints.Count > 0)
                newPos /= hitPoints.Count;

            point = newPos;
        }

        return newPos;
    }

    /// <summary>
    /// Create random vectors to raycast in 360 deg around point;
    /// </summary>
    /// <param name="numDirections">Number of required directions</param>
    /// <returns></returns>
    private Vector3[] GetSphereDirections(int numDirections)
    {
        var pts = new Vector3[numDirections];
        var inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        var off = 2f / numDirections;

        foreach (var k in Enumerable.Range(0, numDirections))
        {
            var y = k * off - 1 + (off / 2);
            var r = Mathf.Sqrt(1 - y * y);
            var phi = k * inc;
            var x = (float)(Mathf.Cos(phi) * r);
            var z = (float)(Mathf.Sin(phi) * r);
            pts[k] = new Vector3(x, y, z);
        }

        return pts;
    }
    
    private void OnDrawGizmos()
    {
        if (points.Count <= 0)
            return;

        PathPoint nextPoint;
        Gizmos.color = Color.blue;
        for (int i = 0; i < points.Count; i++)
        {
            if (i < points.Count - 1)
                nextPoint = points[i + 1];
            else
                nextPoint = points[0];

            Gizmos.DrawLine(points[i].position, nextPoint.position);
        }

        if ((hitPoints == null) || (hitPoints.Count <= 0))
            return;

        Gizmos.color = Color.red;
        for (int i = 0; i < hitPoints.Count; i++)
        {
            Gizmos.DrawSphere(hitPoints[i], 0.1f);
        }
    }
}

