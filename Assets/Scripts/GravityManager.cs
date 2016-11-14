using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

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

    public int activeIndex = -1;

    public PathPoints pathPoints;

    public PathPoint activePoint = null;

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

        Vector3 otherPoint;
        if (activeIndex > 0)
            otherPoint = points[activeIndex - 1].position;
        else
            otherPoint = points[activeIndex + 1].position;

        activePoint.position = CalculateNewPointPosition(activePoint.position, otherPoint);

        // Calculate new forward direction
        otherPoint = activePoint.position + (activePoint.position - otherPoint).normalized;
        Debug.DrawLine(activePoint.position, otherPoint * 3);
        otherPoint = CalculateNewPointPosition(otherPoint, activePoint.position);
        Debug.DrawLine(activePoint.position, otherPoint * 3, Color.cyan);
        activePoint.rotation = Quaternion.LookRotation(otherPoint);
    }

    Vector3 CalculateNewPointPosition(Vector3 point, Vector3 previousPoint)
    {
        const int countRealignPoint = 15;
        Vector3 newPos;
        
        for (int i = 0; i < countRealignPoint; i++)
        {
            Vector3 p1 = Vector3.Cross(point - previousPoint, Vector3.one).normalized;
            Vector3 p2 = Vector3.Cross(point - previousPoint, point - p1).normalized;

            RaycastHit hit;

            List<Vector3> hitPoints = new List<Vector3>();

            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(point, p1, out hit, Mathf.Infinity, layerMask))
                hitPoints.Add(hit.point);
            if (Physics.Raycast(point, -p1, out hit, Mathf.Infinity, layerMask))
                hitPoints.Add(hit.point);
            if (Physics.Raycast(point, p2, out hit, Mathf.Infinity, layerMask))
                hitPoints.Add(hit.point);
            if (Physics.Raycast(point, -p2, out hit, Mathf.Infinity, layerMask))
                hitPoints.Add(hit.point);

            newPos = Vector3.zero;
            foreach (var pt in hitPoints)
                newPos += pt;
            if (hitPoints.Count > 0)
                newPos /= hitPoints.Count;

            point = newPos;
        }

        return point;
    }

    // Use this for initialization
    void Start()
    {
        //if (points == null)
        //  points = new List<GravityPoint>();
    }

    void Update()
    {
    }
}
