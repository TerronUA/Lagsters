using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshEditor : MonoBehaviour
{
    public float positionOnSpline = 0f;
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
    private Vector3[] splinePoints;
    private Vector3[] splineDirections;
    private List<Vector3> vrtList;
    private float progress;
    private Mesh mesh;
    private Vector3[] meshVertices;
    private int[] meshTriangles;
    private MeshExtrusion.Edge[] meshEdges;
    public Mesh extrudeMesh;


    private void Awake()
    {
        //StartCoroutine(Generate());
        LoadMesh();
    }

    public void LoadMesh()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshVertices = mesh.vertices;
        meshTriangles = mesh.triangles;
        if (extrudeMesh == null)
            extrudeMesh = new Mesh();
}

    public bool isMeshEmpty
    {
        get
        {
            if (mesh != null)
                return mesh.vertexCount > 0;
            else
                return true;
        }
    }

    private Vector3[] CreateEdge()
    {
        float rotateDeg = 360f / pointsOnCircle;

        Vector3[] v = new Vector3[pointsOnCircle];

        for (int i = 0; i < pointsOnCircle; i++)
            v[i] = Quaternion.Euler(0, 0, rotateDeg * i) * (radius * Vector3.up);

        return v;
    }

    public Vector3[] GenerateEdgeOnSpline(float pos)
    {
        Vector3[] v = CreateEdge();
        

        Vector3 directionStart = spline.GetDirection(0);
        Vector3 position = spline.GetPoint(pos);
        Vector3 direction = spline.GetDirection(pos);
        Quaternion FromToRotation = Quaternion.LookRotation(direction);// Quaternion.FromToRotation(directionStart, direction);

        //Quaternion.LookRotation
        Debug.Log( FromToRotation );
        
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
            prevEdge = GenerateEdgeOnSpline(pos - 1);
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
        Vector3 positionStart = spline.GetPoint(0);
        Vector3 directionStart = spline.GetVelocity(0).normalized;
        Vector3 position;
        Vector3 direction;
        Vector3 pt;
        Quaternion FromToRotation;

        vertices = CreateEdge();

        progress = 0f;

        mesh.Clear();

        Array.Resize(ref meshVertices, pointsOnCircle * 2);

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

        meshEdges = MeshExtrusion.BuildManifoldEdges(mesh);
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

            meshTriangles[ti + 4] = meshTriangles[ti + 1] = si;//vi + pointsOnCircle;

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

        meshEdges = MeshExtrusion.BuildManifoldEdges(mesh);
        Debug.Log("meshEdges.Length: " + meshEdges.Length);

    }

    public void CreateMesh()
    {
        Vector3 positionStart = spline.GetPoint(0);
        Vector3 directionStart = spline.GetVelocity(0).normalized;
        Vector3 position;
        Vector3 direction;
        Vector3 pt;
        Quaternion FromToRotation;

        vertices = CreateEdge();

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

        if (extrudeMesh == null)
            extrudeMesh = new Mesh();
        else
            extrudeMesh.Clear();

        meshEdges = MeshExtrusion.BuildManifoldEdges(mesh);
    }

    public void ExtrudeMesh()
    {
        int prevSplineStep = (currentSplineStep > 0) ? currentSplineStep - 1 : (splineSteps - 1);

        Vector3 position = spline.GetPoint(1 / splineSteps * currentSplineStep);
        Vector3 direction = spline.GetVelocity(1 / splineSteps * currentSplineStep);

        Vector3 prevPosition = spline.GetPoint(1 / splineSteps * prevSplineStep);
        Vector3 prevDirection = spline.GetVelocity(1 / splineSteps * prevSplineStep);

        meshEdges = MeshExtrusion.BuildManifoldEdges(mesh);

        Matrix4x4 worldToLocal = transform.worldToLocalMatrix;
        Quaternion rotation = Quaternion.LookRotation(direction, prevDirection);
        Debug.DrawLine(position, position + direction, Color.red, 50f);
        

        Matrix4x4[] finalSections = new Matrix4x4[2];

        finalSections[0] = worldToLocal * Matrix4x4.TRS(prevPosition, Quaternion.identity, Vector3.one);
        finalSections[1] = worldToLocal * Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);

        Mesh srcMesh = GetComponent<MeshFilter>().sharedMesh;
        if (extrudeMesh == null)
            extrudeMesh = new Mesh();
        else
            extrudeMesh.Clear();

        MeshExtrusion.ExtrudeMesh(srcMesh, extrudeMesh, finalSections, meshEdges, false);
    }

    private IEnumerator Generate()
    {
        if (vrtList == null)
            vrtList = new List<Vector3>();
        else
            vrtList.Clear();

        float rotateDeg = 360f / pointsOnCircle;

        Vector3 positionStart = spline.GetPoint(0);
        Vector3 directionStart = spline.GetVelocity(0).normalized;

        vertices = CreateEdge();

        Vector3 position = spline.GetPoint(0);
        Vector3 direction;
        Vector3 pt;
        Quaternion FromToRotation;


        splinePoints = new Vector3[splineSteps];
        splineDirections = new Vector3[splineSteps];
        progress = 0f;

        for (int j = 0; j < splineSteps; j++)
        {
            position = spline.GetPoint(progress);
            direction = spline.GetVelocity(progress);
            FromToRotation = Quaternion.FromToRotation(directionStart, direction);

            splinePoints[j] = position;
            splineDirections[j] = direction;


            Debug.DrawLine(position, position + direction);

            for (int i = 0; i < pointsOnCircle; i++)
            {
                pt = FromToRotation * transform.rotation * vertices[i];
                pt = pt + position;

                vrtList.Add(pt);
                //yield return wait;
            }

            progress += 1f / splineSteps;
        }

        Vector3[] v = new Vector3[vrtList.Count];

        for (int i = 0; i < vrtList.Count; i++)
        {
            v[i] = vrtList[i];
        }

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";
        mesh.vertices = v;

        // Generate Triangles

        WaitForSeconds wait = new WaitForSeconds(0.5f);

        int[] triangles = new int[pointsOnCircle * 6];
        for (int ti = 0, vi = 0, x = 0; x < pointsOnCircle; x++, ti += 6, vi++)
        {
            triangles[ti] = vi;

            triangles[ti + 4] = triangles[ti + 1] = vi + pointsOnCircle;

            if (vi < pointsOnCircle - 1)
            {
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 5] = vi + pointsOnCircle + 1;
            }
            else
            {
                triangles[ti + 3] = triangles[ti + 2] = 0;
                triangles[ti + 5] = pointsOnCircle;
            }


            yield return wait;
        }
        // Close surface


        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (edge == null)
            return;

        for (int i = 0; i < edge.Length; i++)
        {
            switch (i)
            {
                case 0 :
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
            Gizmos.DrawSphere(edge[i], 0.1f);

        }
        if (prevEdge == null)
            return;

        for (int i = 0; i < prevEdge.Length; i++)
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
            Gizmos.DrawSphere(prevEdge[i], 0.1f);

        }

        if (vertices == null)
            return;
        /*
        Gizmos.color = Color.green;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
        Gizmos.color = Color.yellow;
        for (int i = 0; i < meshVertices.Length; i++)
        {
            Gizmos.DrawSphere(meshVertices[i], 0.1f);
        }
        /*
        if (meshEdges != null)
        {
            Gizmos.color = Color.magenta;
            for (int i = 0; i < meshEdges.Length; i++)
            {
                for (int j = 0; j < meshEdges[i].vertexIndex.Length; j++)
                {
                    Gizmos.DrawSphere(meshVertices[meshEdges[i].vertexIndex[j]], 0.15f);
                }
            }

        }
        */

        /*
        for (int i = 0; i < splineSteps; i++)
        {
            if (i == 0)
                Gizmos.color = Color.blue;
            else
                Gizmos.color = Color.green;
            Gizmos.DrawSphere(splinePoints[i], 0.5f);
            Gizmos.DrawRay(splinePoints[i], splineDirections[i]);
        }

        for (int i = 0; i < vrtList.Count; i++)
        {
            if (i == 0)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(vrtList[i], 0.3f);
            }
            else if (i == 1)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(vrtList[i], 0.3f);
            }
            else if (i == pointsOnCircle)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(vrtList[i], 0.3f);
            }
            else if (i == pointsOnCircle + 1)
            {
                Gizmos.color = Color.grey;
                Gizmos.DrawSphere(vrtList[i], 0.3f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(vrtList[i], 0.1f);
            }
        }*/
    }
}