using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class PathNodeCollider : MonoBehaviour
{
    [HideInInspector]
    public int indexGravityNode = -1;
    
	void Start ()
    {
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "COM")
            return;

        GravityBody gBody = other.gameObject.GetComponentInParent<GravityBody>();
        if (gBody == null)
            return;
        
        StartCoroutine(gBody.CheckEdgeCoroutine(indexGravityNode));
    }
}
