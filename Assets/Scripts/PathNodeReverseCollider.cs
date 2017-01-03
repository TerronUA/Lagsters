using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PathNodeReverseCollider : MonoBehaviour
{
    [HideInInspector]
    public bool isTriggered = false;
    public PathNodeCollider normalCollider;

    private BoxCollider bc;
    private bool canPlay = false;

    // Use this for initialization
    void Start ()
    {
        bc = GetComponent<BoxCollider>();
        canPlay = (normalCollider != null);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!canPlay)
            return;

        if (normalCollider.isTriggered)
            normalCollider.isTriggered = false;
        else
            isTriggered = true;

        Debug.Log("Rev.Collider " + gameObject.name);
    }
}
