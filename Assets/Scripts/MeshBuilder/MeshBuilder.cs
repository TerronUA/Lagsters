using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSpline;

[RequireComponent(typeof(LevelSpline.BezierSpline), typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshBuilder : MonoBehaviour
{
    public int currentSplineStep = 0;
    public int generatedSplineStep = 0;
    public int pointsOnCircle = 20;
    public int splineSteps = 50;
    public float radius = 10f;
    public int selectedIndex = -1;

    public LevelSpline.BezierSplineData spline;

    private Mesh mesh;
    private Vector3[] meshVertices;
    private int[] meshTriangles;

    private Vector3[] vertices;


    void OnEnable()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        meshVertices = mesh.vertices;
        meshTriangles = mesh.triangles;
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

    public int SelectedPoint
    {
        get
        {
            return selectedIndex;
        }
        set
        {
            selectedIndex = value;
            vertices = CreateCircleEdge();
        }
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

    private void OnDrawGizmos()
    {
        if (spline != null && 0 <= selectedIndex && selectedIndex < spline.points.Length)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(spline.points[selectedIndex].position, 2f);
        }
        DrawEdge(vertices, 0.2f);
    }

    private void DrawEdge(Vector3[] e, float diameter = 0.1f)
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
}
