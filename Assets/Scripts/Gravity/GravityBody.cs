using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    public GravityManager gm;

    [HideInInspector]
    public Vector3 ptStart;
    [HideInInspector]
    public Vector3 ptEnd;
    [HideInInspector]
    public Vector3 ptGravity;

    public float gravity = -10f;
    public GameObject objGravity;
    public GameObject objStart;
    public GameObject objEnd;
    private Rigidbody rbody;
    private Transform gTransform;

    private Vector3 centerOfMass;
    private bool canPlay = false;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponentInParent<Rigidbody>();
        rbody.useGravity = false;

        gTransform = rbody.transform;

        gm = FindObjectOfType<GravityManager>();

        DefineGravityPosition();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!canPlay)
            return;

        centerOfMass = gTransform.position;
        ptGravity = MathUtils.ClosesPointOnLine(centerOfMass, ptStart, ptEnd);


        if (objGravity)
            objGravity.transform.position = ptGravity;
        if (objStart)
            objStart.transform.position = ptStart;
        if (objEnd)
            objEnd.transform.position = ptEnd;

    rbody.AddForce(gravity * rbody.mass * (ptGravity - centerOfMass).normalized); //AddForceAtPosition(gravity * rbody.mass * (gravityPosition - centerOfMass).normalized, centerOfMass);
    }

    void DefineGravityPosition()
    {
        canPlay = false;

        if ((gm == null) || (gm.points.Count <= 0))
            return;
        
        canPlay = gm.FindClosestEdgeToPosition(gTransform.position, ref ptStart, ref ptEnd);
    }

    public IEnumerator CheckEdgeCoroutine(int index)
    {
        if (!canPlay)
            yield break;

        gm.FindStartEndPoints(index, gTransform.position, ref ptStart, ref ptEnd);

        yield return new WaitForSeconds(0.1f);

        gm.FindStartEndPoints(index, gTransform.position, ref ptStart, ref ptEnd);

        //Debug.Log("CheckEdgeCoroutine(" + index + ")");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(ptStart, 0.5f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(ptEnd, 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(ptGravity, 0.7f);
    }
}
