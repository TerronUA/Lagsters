using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoverCar
{
    public class Thruster : MonoBehaviour
    {
        public float strength = 200f;
        public float distance = 3f;
        public LayerMask layerMask;
        public Transform[] thrusters;

        private Rigidbody rBody;
        private Transform thisTransform;
        private GravityBody gBody;

        // Use this for initialization
        void Start()
        {
            rBody = GetComponent<Rigidbody>();
            thisTransform = GetComponent<Transform>();
            gBody = GetComponent<GravityBody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            RaycastHit hit;
            float distancePersentage;
            Vector3 downwardForce;
            Vector3 gravityNormal;
            Transform thruster;

            for (int i = 0; i < thrusters.Length; i++)
            {
                thruster = thrusters[i];
                gravityNormal = (thruster.position - gBody.gravityPosition).normalized;

                if (Physics.Raycast(thruster.position, /*gravityNormal /**/thruster.up, out hit, distance, layerMask))
                {
                    Debug.DrawLine(thruster.position, hit.point, Color.red);

                    distancePersentage = 1 - (hit.distance / distance);
                    downwardForce = thisTransform.up * strength * distancePersentage * Time.deltaTime * rBody.mass;

                    rBody.AddForceAtPosition(downwardForce, thruster.position);
                }
            }
        }
    }
}