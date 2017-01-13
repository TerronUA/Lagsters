using System;
using UnityEngine;

[Serializable]
public class GravityPoint// : UnityEngine.Object
{
    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;
    public float gravity;

    //public GravityPoint()
    //{
    //    position = Vector3.zero;
    //    gravity = 9.8f;
    //}

    //public GravityPoint(GravityPoint other)
    //{
    //    position = other.position;
    //    gravity = other.gravity;
    //}
}
