using UnityEngine;
using System.Collections;

namespace VP
{


    public class CarCamera : MonoBehaviour
    {

        public Transform target;
        public float distance = 20.0f;
        public float height = 5.0f;
        public float heightDamping = 2.0f;

        private float lookAtHeight = 0.0f;
        private GameObject parentGameObject;
        public Rigidbody parentRigidbody;

        public float rotationSnapTime = 0.3F;

        public float distanceSnapTime;
        public float distanceMultiplier;

        private Vector3 lookAtVector;

        private float usedDistance;

        float wantedRotationAngle;
        float wantedHeight;

        float currentRotationAngle;
        float currentHeight;

        Quaternion currentRotation;
        Vector3 wantedPosition;

        private float yVelocity = 0.0F;
        private float zVelocity = 0.0F;

        void Awake()
        {

            lookAtVector = new Vector3(0f, lookAtHeight, 0f);
            StartCoroutine("FindPlayerObject");
        }

        IEnumerator FindPlayerObject()
        {
            yield return new WaitForSeconds(0.1f);

            if (target == null)
            {
                target = GameObject.Find("CameraTarget").transform;
                parentGameObject = GameObject.FindWithTag("Player");
                parentRigidbody = parentGameObject.GetComponent<Rigidbody>();
            }
        }

        void LateUpdate()
        {
            if (target != null && parentRigidbody != null)
            {
                wantedHeight = target.position.y + height;
                currentHeight = transform.position.y;

                wantedRotationAngle = target.eulerAngles.y;
                currentRotationAngle = transform.eulerAngles.y;

                currentRotationAngle = Mathf.SmoothDampAngle(currentRotationAngle, wantedRotationAngle, ref yVelocity, rotationSnapTime);

                currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

                wantedPosition = target.position;
                wantedPosition.y = currentHeight;

                usedDistance = Mathf.SmoothDampAngle(usedDistance, distance + (parentRigidbody.velocity.magnitude * distanceMultiplier), ref zVelocity, distanceSnapTime);

                wantedPosition += Quaternion.Euler(0, currentRotationAngle, 0) * new Vector3(0, 0, -usedDistance);

                transform.position = wantedPosition;

                transform.LookAt(target.position + lookAtVector);
            }
        }
    }

}