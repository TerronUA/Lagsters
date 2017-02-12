using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeCar
{
    public class TireToWheel : MonoBehaviour
    {
        public WheelCollider wheelCollider;

        private Transform thisTransform;

        // Use this for initialization
        void Start()
        {
            thisTransform = GetComponent<Transform>();
        }
        
        void FixedUpdate()
        {
            Vector3 localPosition = thisTransform.localPosition;

            WheelHit hit = new WheelHit();
            
            // see if we have contact with ground
            if (wheelCollider.GetGroundHit(out hit))
            {
                float hitY = wheelCollider.transform.InverseTransformPoint(hit.point).y;

                localPosition.y = hitY + wheelCollider.radius;
            }
            else
            {
                // no contact with ground, just extend wheel position with suspension distance
                localPosition = Vector3.Lerp(localPosition, -Vector3.up * wheelCollider.suspensionDistance, .05f);
            }

            // actually update the position
            thisTransform.localPosition = localPosition;
            thisTransform.localRotation = Quaternion.Euler(wheelCollider.transform.rotation.eulerAngles.x, wheelCollider.steerAngle, 0);

            thisTransform.Rotate(wheelCollider.rpm / 60 * 360 * Time.fixedDeltaTime, 0, 0);
        }
    }
}
