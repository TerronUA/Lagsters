using UnityEngine;
using System.Collections;

[System.Serializable]
public class PathPoint
{
    public Vector3 position;
    public Quaternion rotation;
    public float gravity = 10f;
    public float raycastDistance = 5f;
    public BoxCollider collider;
    public BoxCollider colliderReverse;
}
