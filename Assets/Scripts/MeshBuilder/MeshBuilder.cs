using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSpline;

public class MeshBuilder : MonoBehaviour
{
    public int currentSplineStep = 0;
    public int generatedSplineStep = 0;
    public int pointsOnCircle = 20;
    public int splineSteps = 50;
    public float radius = 10f;
    public LevelSpline.BezierSpline spline;

    private Mesh mesh;
    private Vector3[] meshVertices;
    private int[] meshTriangles;


    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        meshVertices = mesh.vertices;
        meshTriangles = mesh.triangles;
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
}
