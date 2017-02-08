﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    public float acceleration = 10f;
    public float rotationRate = 3f;

    public float turnRotationAngle;
    public float turnRotationSeekSpeed;

    public LayerMask groundMask;

    private float rotationVelocity;
    private float groundAngleVelocity;

    private Transform thisTransform;
    private Rigidbody rBody;

    // Use this for initialization
    void Start ()
    {
        thisTransform = GetComponent<Transform>();
        rBody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        float inputVertical = Input.GetAxis("Vertical");
        float inputHorizontal = Input.GetAxis("Horizontal");

        // Acceleration
        if (Physics.Raycast(thisTransform.position, -1 * thisTransform.up, 1f, groundMask))
        {
            rBody.drag = 1;

            Vector3 forwardForce = thisTransform.forward * acceleration * inputVertical * Time.deltaTime * rBody.mass;

            rBody.AddForce(forwardForce);
        }
        else
        {
            rBody.drag = 0;
        }

        // Rotation
        Vector3 turnTorque = thisTransform.up * rotationRate * inputHorizontal * Time.deltaTime * rBody.mass;
        rBody.AddTorque(turnTorque);

        // Fake rotation ???
        Vector3 newRotation = thisTransform.eulerAngles;
        newRotation.z = Mathf.SmoothDampAngle(newRotation.z, inputHorizontal * - turnRotationAngle, ref rotationVelocity, turnRotationSeekSpeed);
        thisTransform.eulerAngles = newRotation;
    }
}
