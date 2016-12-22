using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    public GravityManager gm;

    [HideInInspector]
    public Vector3 startPoint;
    [HideInInspector]
    public Vector3 endPoint;
    [HideInInspector]
    public Vector3 gravityPosition;

    public float gravity = 10f;
    public GameObject gravityObject;
    public GameObject startObject;
    public GameObject endObject;
    private Rigidbody rbody;

    private Vector3 centerOfMass;
    private bool canPlay = false;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        rbody.useGravity = false;

        gm = FindObjectOfType<GravityManager>();

        if (gm != null)
            FindClosesPointOnSpline();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!canPlay)
            return;

        centerOfMass = rbody.transform.TransformPoint(rbody.centerOfMass);
        Vector3 vVector1 = centerOfMass - startPoint;
        Vector3 vVector2 = (endPoint - startPoint).normalized;

        float d = Vector3.Distance(startPoint, endPoint);
        float t = Vector3.Dot(vVector2, vVector1);

        if (t <= 0)
            gravityPosition = startPoint;
        else if (t >= d)
            gravityPosition = endPoint;
        else
        {
            Vector3 vVector3 = vVector2 * t;

            gravityPosition = startPoint + vVector3;
        }


        if (gravityObject != null)
            gravityObject.transform.position = gravityPosition;

        if (startObject != null)
            startObject.transform.position = startPoint;

        if (endObject != null)
            endObject.transform.position = endPoint;


        rbody.AddForce(gravity * rbody.mass * (gravityPosition - centerOfMass).normalized);
    }

    void FindClosesPointOnSpline()
    {
        if ((gm == null) || (gm.points.Count <= 0))
            return;

        Vector3 currentPosition = transform.position;

        startPoint = gm.points[0].position;

        for (int i = 0; i < gm.points.Count; i++)
        {
            if (Vector3.Distance(currentPosition, startPoint) > Vector3.Distance(currentPosition, gm.points[i].position))
            {
                startPoint = gm.points[i].position;
                if(i < gm.points.Count - 1)
                    endPoint = gm.points[i + 1].position;
                else
                    endPoint = gm.points[0].position;
            }
        }

        canPlay = true;
    }

    private void OnDrawGizmos()
    {/*
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(startPoint, 0.3f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(endPoint, 0.3f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(centerOfMass, 0.3f);*/
    }
}
