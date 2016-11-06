using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    public GravityManager gm;
    public Transform startPoint;
    public Transform endPoint;
    public float gravity = 10f;
    private Rigidbody rbody;
    private Vector3 gravityPosition;
    private Vector3 centerOfMass;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        rbody.useGravity = false;

        gm = FindObjectOfType<GravityManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        centerOfMass = rbody.transform.TransformPoint(rbody.centerOfMass);
        Vector3 vVector1 = centerOfMass - startPoint.position;
        Vector3 vVector2 = (endPoint.position - startPoint.position).normalized;

        float d = Vector3.Distance(startPoint.position, endPoint.position);
        float t = Vector3.Dot(vVector2, vVector1);

        if (t <= 0)
            gravityPosition = startPoint.position;
        else if (t >= d)
            gravityPosition = endPoint.position;
        else
        {
            Vector3 vVector3 = vVector2 * t;

            gravityPosition = startPoint.position + vVector3;
        }

        rbody.AddForce(gravity * rbody.mass * (gravityPosition - centerOfMass).normalized);
    }
}
