using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSpline;
using System;

[RequireComponent(typeof(LevelSpline.BezierSpline), typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshBuilder : MonoBehaviour
{
    #region Main properties
    public int currentSplineStep = 0;
    public int generatedSplineStep = 0;
    public int pointsOnCircle = 20;
    public int splineSteps = 50;
    public float radius = 10f;
    #endregion
    #region Selected edge
    public int selectedIndex = -1;
    public int selectedEdge = -1;
    public float positionOnEdge = 0f;
    [SerializeField]
    private int selectedIndexEndPoint = -1;
    #endregion

    public LevelSpline.BezierSplineData spline;
    
    public Mesh mesh;
    private Vector3[] meshVertices;
    private int[] meshTriangles;

    #region Mesh generation properties
    [SerializeField]
    private List<int> boundaryFirst;
    [SerializeField]
    private List<int> boundaryLast;
    [SerializeField]
    private int indexFirstEdgeStarts = -1;
    [SerializeField]
    private int indexFirstEdgeEnds = -1;
    [SerializeField]
    private int indexLastEdgeStarts = -1;
    [SerializeField]
    private int indexLastEdgeEnds = -1;
    #endregion
    
    #region Spline selection properties
    [SerializeField]
    private Vector3 ptSelectedStart;
    [SerializeField]
    private Vector3 ptSelectedEnd;
    [SerializeField]
    private Vector3 ptSelectedStartCPoint;
    [SerializeField]
    private Vector3 ptSelectedEndCPoint;
    [SerializeField]
    private Color colorSelectedEdge = Color.yellow;
    #endregion

    private Vector3[] vertices;


    void OnEnable()
    {
        if (mesh != null)
        {
            meshVertices = mesh.vertices;
            meshTriangles = mesh.triangles;
        }
    }

    public int PointsOnSpline
    {
        get
        {
            if (spline != null)
                return spline.points.Length;
            return 0;
        }
    }

    public int EdgesFromSelectedPoint
    {
        get
        {
            if (spline != null && spline.IndexInRange(selectedIndex))
                return spline.points[selectedIndex].points.Length;
            return -1;
        }
    }

    public int SelectedPoint
    {
        get
        {
            return selectedIndex;
        }
        set
        {
            selectedIndex = value;
            UpdateSelectedPoint();
        }
    }

    public int SelectedEdge
    {
        get
        {
            return selectedEdge;
        }
        set
        {
            selectedEdge = value;
            UpdateSelectedPoint();
        }
    }

    public float PositionOnEdge
    {
        get
        {
            return positionOnEdge;
        }
        set
        {
            positionOnEdge = value;
            UpdateSelectedPoint();
        }
    }

    /// <summary>
    /// Updates points positions for active edge
    /// </summary>
    /// <returns>True if selected point and edge exists</returns>
    private bool UpdateSelectedPoint()
    {
        ptSelectedStart = ptSelectedEnd = ptSelectedStartCPoint = ptSelectedEndCPoint = Vector3.zero;
        colorSelectedEdge = Color.yellow;

        if (!spline.IndexInRange(selectedIndex))
            return false;

        BezierPoint pt = spline.points[selectedIndex];

        if ((selectedEdge < 0) || (selectedEdge >= pt.points.Length))
            return false;

        BezierPoint endPoint;

        if (pt.IsPrevCPointIndex(selectedEdge))
        {
            selectedIndexEndPoint = pt.points[selectedEdge].prevIndex;

            endPoint = spline.points[selectedIndexEndPoint];

            ptSelectedStartCPoint = pt.GetPrevPointPositionTo(selectedIndexEndPoint);
            ptSelectedEndCPoint = endPoint.GetNextPointPositionTo(selectedIndex);

            colorSelectedEdge = Color.red;
        }
        else
        {
            selectedIndexEndPoint = pt.points[selectedEdge].nextIndex;

            endPoint = spline.points[selectedIndexEndPoint];

            ptSelectedStartCPoint = pt.GetNextPointPositionTo(selectedIndexEndPoint);
            ptSelectedEndCPoint = endPoint.GetPrevPointPositionTo(selectedIndex);

            colorSelectedEdge = Color.green;
        }

        ptSelectedStart = pt.position;
        ptSelectedEnd = endPoint.position;

        vertices = GenerateEdgeOnSpline(positionOnEdge);

        return true;
    }

    public bool GetSelectedEdgePoints(out Vector3 ptStart, out Vector3 ptEnd, out Vector3 startCPoint, out Vector3 endCPoint, out Color drawColor)
    {
        bool result = UpdateSelectedPoint();

        ptStart = ptSelectedStart;
        ptEnd = ptSelectedEnd;
        startCPoint = ptSelectedStartCPoint;
        endCPoint = ptSelectedEndCPoint;
        drawColor = colorSelectedEdge;

        return result;
    }

    /// <summary>
    /// Creates vector3 array with points on circle around [0, 0, 0] point
    /// </summary>
    /// <returns>Genearated points on circle</returns>
    private Vector3[] CreateCircleEdge()
    {
        float rotateDeg = 360f / pointsOnCircle;

        Vector3[] v = new Vector3[pointsOnCircle];

        for (int i = 0; i < pointsOnCircle; i++)
            v[i] = Quaternion.Euler(0, 0, rotateDeg * i) * (radius * Vector3.up);

        return v;
    }
    
    public Vector3[] GenerateEdgeOnSpline(float pos)
    {
        Vector3[] v = CreateCircleEdge();

        Vector3 position = spline.GetPoint(selectedIndex, selectedIndexEndPoint, pos);
        Vector3 direction = spline.GetDirection(selectedIndex, selectedIndexEndPoint, pos);
        
        //Vector3 directionStart = spline.GetDirection(0);
        //Quaternion FromToRotation = Quaternion.FromToRotation(directionStart, direction);
        Quaternion FromToRotation = Quaternion.LookRotation(direction);

        Vector3 pt;
        for (int i = 0; i < pointsOnCircle; i++)
        {
            pt = FromToRotation * transform.rotation * v[i];
            v[i] = pt + position;
        }

        return v;
    }
    
    private void OnDrawGizmos()
    {
        if (spline != null && 0 <= selectedIndex && selectedIndex < spline.points.Length)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(spline.points[selectedIndex].position, 2f);

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(spline.GetPoint(selectedIndex, selectedIndexEndPoint, positionOnEdge), 2f);
        }

        DrawVertices(vertices, 1f);
    }

    /// <summary>
    /// Draws points in array. First 5 are colored, others - black
    /// </summary>
    /// <param name="e">points array</param>
    /// <param name="diameter">point diameter</param>
    private void DrawVertices(Vector3[] e, float diameter = 0.1f)
    {
        if (e == null)
            return;

        for (int i = 0; i < e.Length; i++)
        {
            switch (i)
            {
                case 0:
                    Gizmos.color = Color.red;
                    break;
                case 1:
                    Gizmos.color = Color.yellow;
                    break;
                case 2:
                    Gizmos.color = Color.yellow;
                    break;
                case 3:
                    Gizmos.color = Color.green;
                    break;
                case 4:
                    Gizmos.color = Color.green;
                    break;
                case 5:
                    Gizmos.color = Color.green;
                    break;
                default:
                    Gizmos.color = Color.black;
                    break;
            }
            Gizmos.DrawSphere(e[i], diameter);
        }

    }
    
    public void CreateFirstStepMesh()
    {
        positionOnEdge = 0f;

        mesh.Clear();

        Array.Resize(ref meshVertices, pointsOnCircle * 2);
        Vector2[] uv = new Vector2[pointsOnCircle * 2];

        for (int j = 0, k = 0; j < 2; j++)
        {
            vertices = GenerateEdgeOnSpline(positionOnEdge);

            for (int i = 0; i < pointsOnCircle; i++, k++)
                meshVertices[k] = vertices[i];

            positionOnEdge = 1f / splineSteps;
        }

        Array.Resize(ref meshTriangles, pointsOnCircle * 6);

        for (int ti = 0, vi = 0, x = 0; x < pointsOnCircle; x++, ti += 6, vi++)
        {
            meshTriangles[ti] = vi;

            meshTriangles[ti + 4] = meshTriangles[ti + 1] = vi + pointsOnCircle;

            if (vi < pointsOnCircle - 1)
            {
                meshTriangles[ti + 3] = meshTriangles[ti + 2] = vi + 1;
                meshTriangles[ti + 5] = vi + pointsOnCircle + 1;
            }
            else
            {
                meshTriangles[ti + 3] = meshTriangles[ti + 2] = 0;
                meshTriangles[ti + 5] = pointsOnCircle;
            }
        }

        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;

        mesh.RecalculateNormals();

        generatedSplineStep = 1;
    }
    
    public void CreateNextStepMesh()
    {
        if (generatedSplineStep == 0)
            CreateFirstStepMesh();

        positionOnEdge = 1f / splineSteps * (generatedSplineStep + 1);

        meshVertices = mesh.vertices;
        meshTriangles = mesh.triangles;

        Array.Resize(ref meshVertices, meshVertices.Length + pointsOnCircle);

        vertices = GenerateEdgeOnSpline(positionOnEdge);
        for (int i = 0, k = meshVertices.Length - pointsOnCircle; i < pointsOnCircle; i++, k++)
            meshVertices[k] = vertices[i];


        Array.Resize(ref meshTriangles, meshTriangles.Length + pointsOnCircle * 6);

        for (int ti = meshTriangles.Length - pointsOnCircle * 6, vi = meshVertices.Length - pointsOnCircle * 2, x = 0; x < pointsOnCircle; x++, ti += 6, vi++)
        {
            meshTriangles[ti] = vi;

            meshTriangles[ti + 4] = meshTriangles[ti + 1] = vi + pointsOnCircle;

            if (vi < meshVertices.Length - pointsOnCircle - 1)
            {
                meshTriangles[ti + 3] = meshTriangles[ti + 2] = vi + 1;
                meshTriangles[ti + 5] = vi + pointsOnCircle + 1;
            }
            else
            {
                meshTriangles[ti + 3] = meshTriangles[ti + 2] = meshVertices.Length - pointsOnCircle * 2;
                meshTriangles[ti + 5] = meshVertices.Length - pointsOnCircle;
            }
        }

        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;

        mesh.RecalculateNormals();

        generatedSplineStep += 1;
    }
}
