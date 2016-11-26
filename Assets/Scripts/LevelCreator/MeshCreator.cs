using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshCreator : MonoBehaviour
{
    public float meshStep = 1f;
    public int pointsOnCircle = 20;
    public int splineSteps = 50;
    public float radius = 10f;
    public BezierSpline spline;

    private Vector3[] vertices;
    private Vector3[] splinePoints;
    private Vector3[] splineDirections;
    private List<Vector3> vrtList;
    private float progress;
    private Mesh mesh;


    private void Awake()
    {
        StartCoroutine(Generate());
    }

    private Vector3[] CreateEdge()
    {
        float rotateDeg = 360f / pointsOnCircle;

        Vector3[] v = new Vector3[pointsOnCircle];

        for (int i = 0; i < pointsOnCircle; i++)
            v[i] = Quaternion.Euler(0, 0, rotateDeg * i) * (radius * Vector3.up);

        return v;
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

        /*int[] triangles = new int[pointsOnCircle * splineSteps * 6];
        for (int ti = 0, vi = 0, y = 0; y < 2/*splineSteps; y++, vi++)
        {
            for (int x = 0; x < pointsOnCircle; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + pointsOnCircle;
                //if (x != pointsOnCircle - 1)
                triangles[ti + 5] = vi + pointsOnCircle + 1;
                //else
                //    triangles[ti + 5] = vi + pointsOnCircle;
            }
        }
        */
        /*
        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[3] = triangles[2] = 1;
        triangles[4] = triangles[1] = pointsOnCircle;
        triangles[5] = pointsOnCircle + 1;
        yield return wait;
        */
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
        if (vertices == null)
            return;

        /*
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
        */

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
        }
    }
}
