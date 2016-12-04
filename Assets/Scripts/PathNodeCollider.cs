using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class PathNodeCollider : MonoBehaviour
{
    [HideInInspector]
    public int indexGravityNode = -1;
    private BoxCollider bc;
    private GravityManager gm;
    private bool canPlay = false;

	void Start ()
    {
        bc = GetComponent<BoxCollider>();
        if ((transform.parent != null))
        {
            gm = transform.parent.gameObject.GetComponent<GravityManager>();
            canPlay = (gm != null) && (gm.points.Count > 0) && (indexGravityNode >= 0);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (!canPlay)
            return;

        Debug.Log("Triggered " + gameObject.name);

        GravityBody gBody = other.gameObject.GetComponentInParent<GravityBody>();
        if ((gBody != null) && (gm.points.Count > indexGravityNode))
        {
            gBody.startPoint = gm.points[indexGravityNode].position;
            if (indexGravityNode < (gm.points.Count - 1))
                gBody.endPoint = gm.points[indexGravityNode + 1].position;
            else
                gBody.endPoint = gm.points[0].position;
        }

    }
}
