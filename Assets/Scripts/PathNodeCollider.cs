using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class PathNodeCollider : MonoBehaviour
{
    public PathNodeReverseCollider reverseCollider;
    [HideInInspector]
    public int indexGravityNode = -1;
    [HideInInspector]
    public bool isTriggered = false;

    private GravityManager gm;
    private bool canPlay = false;

	void Start ()
    {
        if ((transform.parent != null))
            gm = transform.parent.gameObject.GetComponent<GravityManager>();

        canPlay = (reverseCollider != null) && (gm != null) && (gm.points.Count > 0) && (indexGravityNode >= 0) && (gm.points.Count > indexGravityNode);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!canPlay)
            return;

        Debug.Log("Triggered " + gameObject.name);

        GravityBody gBody = other.gameObject.GetComponentInParent<GravityBody>();
        if (gBody == null)
            return;

        gBody.startPoint = gm.points[indexGravityNode].position;

        if (reverseCollider.isTriggered)
        {
            if (indexGravityNode > 0 )
                gBody.endPoint = gm.points[indexGravityNode - 1].position;
            else
                gBody.endPoint = gm.points[gm.points.Count - 1].position;

            reverseCollider.isTriggered = false;
        }
        else
        { 
            if (indexGravityNode < (gm.points.Count - 1))
                gBody.endPoint = gm.points[indexGravityNode + 1].position;
            else
                gBody.endPoint = gm.points[0].position;
        }

        isTriggered = true;
    }
}
