using UnityEngine;
using System.Collections;

public class GravityAttractor : MonoBehaviour
{
    public float gravity = -10f;

    public void Attract(GameObject body)
    {
        Vector3 gravityDirection = (body.transform.position - transform.position).normalized;
        Vector3 bodyUp = -body.transform.up;

        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityDirection) * body.transform.rotation;

        Rigidbody rbody = body.GetComponent<Rigidbody>();

        rbody.AddForce(gravityDirection * gravity);
        Debug.DrawLine(body.transform.position, body.transform.position + bodyUp, Color.red);
        Debug.DrawLine(body.transform.position, body.transform.position + gravityDirection, Color.blue);
        //body.transform.rotation = Quaternion.Slerp(body.transform.rotation, targetRotation, 50 * Time.deltaTime);
    }

    public void AttractToPlane(GameObject body)
    {
        //note, the code here suppose our character use a capsuleCollider and the floors' layerMask is "floor"..
        //..if yours' is not, you should make some change.

        RaycastHit hitInfo;
        Vector3 downDirection = -body.transform.up;
        //Vector3 downD = new Vector3(0f, -1f, 0f);
        Rigidbody bodyRigidbody = body.GetComponent<Rigidbody>();
        BoxCollider bodyCollider = body.GetComponentInChildren<BoxCollider>();
        
        Vector3 bodyColliderCenterInWorldSpace = bodyCollider.transform.TransformPoint(bodyCollider.center);

        //Debug.DrawLine(body.transform.position, body.transform.position + downD, Color.blue);

        bool isHit = Physics.Raycast(bodyColliderCenterInWorldSpace, downDirection, out hitInfo, 100f, LayerMask.GetMask("Tunnel"));

        Vector3 forward = bodyRigidbody.transform.forward;

        Vector3 newUp;
        if (isHit)
            newUp = hitInfo.normal;
        else
            newUp = Vector3.up;

        Vector3 left = Vector3.Cross(forward, newUp);//note: unity use left-hand system, and Vector3.Cross obey left-hand rule.
        Vector3 newForward = Vector3.Cross(newUp, left);
        Quaternion oldRotation = bodyRigidbody.transform.rotation;
        Quaternion newRotation = Quaternion.LookRotation(newForward, newUp);

        float kSoftness = 0.1f;//if do not want softness, change the value to 1.0f
        bodyRigidbody.MoveRotation(Quaternion.Lerp(oldRotation, newRotation, kSoftness));
        
        Debug.DrawLine(body.transform.position, body.transform.position + downDirection, Color.red);
        bodyRigidbody.AddForce(downDirection * gravity);
    }
}
