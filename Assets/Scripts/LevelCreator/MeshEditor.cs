using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshEditor : MonoBehaviour
{
    public int currentSplineStep = 0;
    public int generatedSplineStep = 0;
    public int pointsOnCircle = 20;
    public int splineSteps = 50;
    public float radius = 10f;
    public BezierSpline spline;

    [HideInInspector]
    public Vector3[] edge;
    public Vector3[] prevEdge;

    private Vector3[] vertices;
    private float progress;
    private Mesh mesh;
    private Vector3[] meshVertices;
    private int[] meshTriangles;

    private void Awake()
    {
        LoadMesh();
    }

    public void LoadMesh()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        meshVertices = mesh.vertices;
        meshTriangles = mesh.triangles;
    }

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

        Vector3 position = spline.GetPoint(pos);
        Vector3 direction = spline.GetDirection(pos);
        Quaternion FromToRotation = Quaternion.LookRotation(direction);
                
        Vector3 pt;
        for (int i = 0; i < pointsOnCircle; i++)
        {
            pt = FromToRotation * transform.rotation * v[i];
            v[i] = pt + position;
        }

        return v;
    }

    public void CreateNewEdge(float pos)
    {
        edge = GenerateEdgeOnSpline(pos);

        if (pos > 0)
            prevEdge = GenerateEdgeOnSpline(pos - 1);
        else
            prevEdge = GenerateEdgeOnSpline(splineSteps - 1);
    }

    public void CreateNewEdge(int pos)
    {
        float p = 1f / splineSteps * pos;
        edge = GenerateEdgeOnSpline(p);

        if (pos > 0)
            p = 1f / splineSteps *  (pos - 1);
        else
            p = 1f / splineSteps * (splineSteps - 1);
        prevEdge = GenerateEdgeOnSpline(p);
    }

    public void CreateFirstStepMesh()
    {
        Vector3 directionStart = spline.GetVelocity(0).normalized;
        Vector3 position;
        Vector3 direction;
        Vector3 pt;
        Quaternion FromToRotation;

        vertices = CreateCircleEdge();

        progress = 0f;

        mesh.Clear();

        Array.Resize(ref meshVertices, pointsOnCircle * 2);
        Vector2[] uv = new Vector2[pointsOnCircle * 2];

        for (int j = 0, k = 0; j < 2; j++)
        {
            position = spline.GetPoint(progress);
            direction = spline.GetVelocity(progress);
            FromToRotation = Quaternion.FromToRotation(directionStart, direction);
            
            for (int i = 0; i < pointsOnCircle; i++, k++)
            {
                pt = FromToRotation * transform.rotation * vertices[i];
                meshVertices[k] = pt + position;
            }

            progress += 1f / splineSteps;
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

        int nextSplineStep = generatedSplineStep + 1;

        progress = 1f / splineSteps * nextSplineStep;

        meshVertices = mesh.vertices;
        meshTriangles = mesh.triangles;

        Array.Resize(ref meshVertices, meshVertices.Length + pointsOnCircle);

        vertices = GenerateEdgeOnSpline(progress);
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

    public void CreateLastStepMesh()
    {

        if (generatedSplineStep == 0)
        {
            CreateFirstStepMesh();
            return;
        }

        if (generatedSplineStep < splineSteps - 1)
        {
            CreateNextStepMesh();
            return;
        }

        Array.Resize(ref meshTriangles, meshTriangles.Length + pointsOnCircle * 6);

        for (int ti = meshTriangles.Length - pointsOnCircle * 6, vi = meshVertices.Length - pointsOnCircle, si = 0, x = 0; x < pointsOnCircle; x++, ti += 6, vi++, si++)
        {
            meshTriangles[ti] = vi;

            meshTriangles[ti + 4] = meshTriangles[ti + 1] = si;

            if (vi < meshVertices.Length - 1)
            {
                meshTriangles[ti + 3] = meshTriangles[ti + 2] = vi + 1;
                meshTriangles[ti + 5] = si + 1;
            }
            else
            {
                meshTriangles[ti + 3] = meshTriangles[ti + 2] = meshVertices.Length - pointsOnCircle;
                meshTriangles[ti + 5] = 0;
            }
        }

        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;

        mesh.RecalculateNormals();

        generatedSplineStep += 1;
    }

    public void CreateMeshWithClosedStart()
    {
        Vector3 positionStart = spline.GetPoint(0);
        Vector3 directionStart = spline.GetVelocity(0).normalized;
        Vector3 position;
        Vector3 direction;
        Vector3 pt;
        Quaternion FromToRotation;

        vertices = CreateCircleEdge();

        progress = 0f;

        mesh.Clear();

        Array.Resize(ref meshVertices, pointsOnCircle * 2 + 1); // +1 to close start point side

        for (int j = 0, k = 0; j < 2; j++)
        {
            position = spline.GetPoint(progress);
            direction = spline.GetVelocity(progress);
            FromToRotation = Quaternion.FromToRotation(directionStart, direction);

            for (int i = 0; i < pointsOnCircle; i++, k++)
            {
                pt = FromToRotation * transform.rotation * vertices[i];
                meshVertices[k] = pt + position;
            }

            progress += 1f / splineSteps;
        }
        meshVertices[meshVertices.Length - 1] = positionStart;


        Array.Resize(ref meshTriangles, pointsOnCircle * 9);// old *6

        for (int ti = 0, vi = 0, x = 0; x < pointsOnCircle; x++, ti += 9, vi++)
        {
            meshTriangles[ti + 6] = meshTriangles[ti] = vi;

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
            meshTriangles[ti + 7] = meshTriangles[ti + 2];
            meshTriangles[ti + 8] = meshVertices.Length - 1;
        }

        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        DrawEdge(edge);
        DrawEdge(prevEdge);
    }

    private void DrawEdge(Vector3[] e)
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
            Gizmos.DrawSphere(e[i], 0.1f);
        }

    }
}