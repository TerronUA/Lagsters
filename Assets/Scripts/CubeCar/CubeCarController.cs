using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeCar
{
    internal enum CarDriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive // TODO: add types for 3 and 6 wheel drives
    }

    internal enum SpeedType
    {
        MPH,
        KPH
    }

    public class CubeCarController : MonoBehaviour
    {
        [SerializeField]
        private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive;
        [SerializeField]
        private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField]
        public Transform m_CenterOfMass;

        [SerializeField]
        private float m_FullTorqueOverAllWheels;
        [SerializeField]
        private float m_ReverseTorque;
        [SerializeField]
        private float m_BrakeTorque;
        [SerializeField]
        private float m_MaxHandbrakeTorque;
        [SerializeField]
        private float m_MaximumSteerAngle;
        [Range(0, 1)]
        [SerializeField]
        private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range(0, 1)]
        [SerializeField]
        private float m_TractionControl; // 0 is no traction control, 1 is full interference

        private Rigidbody m_Rigidbody;
        private Transform m_Transform;
        private float m_CurrentTorque;
        private float m_SteerAngle;
        private float m_OldRotation;

        public float CurrentSpeed
        {
            get
            {
                return m_Rigidbody.velocity.magnitude * 2.23693629f;
            }
        }

        // Use this for initialization
        void Start()
        {
            m_WheelColliders[0].ConfigureVehicleSubsteps(2, 10, 15);

            m_MaxHandbrakeTorque = float.MaxValue;

            m_Transform = transform;
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Rigidbody.centerOfMass = m_CenterOfMass.transform.localPosition;
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);
        }

        // Update is called once per frame
        void FixedUpdate()
        {

        }

        public void Move(float steering, float accel, float footbrake, float handbrake)
        {
            //clamp input values
            steering = Mathf.Clamp(steering, -1, 1);
            /*AccelInput = */accel = Mathf.Clamp(accel, 0, 1);
            /*BrakeInput = */footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
            handbrake = Mathf.Clamp(handbrake, 0, 1);

            //Debug.Log("steering = " + steering + ", accel = " + accel + ", footbrake = " + footbrake + ", handbrake = handbrake");

            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            m_SteerAngle = steering * m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;

            SteerHelper();

            ApplyDrive(accel, footbrake);
        }

        private void SteerHelper()
        {
            WheelHit wheelHit;
            for (int i = 0; i < 4; i++)
            {
                m_WheelColliders[i].GetGroundHit(out wheelHit);
                if (wheelHit.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }
            /*
            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(m_OldRotation - m_Transform.eulerAngles.y) < 10f)
            {
                var turnadjust = (m_Transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                Debug.Log(m_OldRotation + ", " + m_Transform.eulerAngles.y);
                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
            }
            */
            m_OldRotation = m_Transform.eulerAngles.y;
        }


        private void ApplyDrive(float accel, float footbrake)
        {
            float thrustTorque;
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 4f);
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].motorTorque = thrustTorque;
                    }
                    break;

                case CarDriveType.FrontWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
                    break;

                case CarDriveType.RearWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
                    break;

            }

            for (int i = 0; i < 4; i++)
            {
                if (CurrentSpeed > 1 && Vector3.Angle(m_Transform.forward, m_Rigidbody.velocity) < 50f)
                {
                    m_WheelColliders[i].brakeTorque = m_BrakeTorque * footbrake;
                }
                else if (footbrake > 0)
                {
                    m_WheelColliders[i].brakeTorque = 0f;
                    m_WheelColliders[i].motorTorque = -m_ReverseTorque * footbrake;
                }
            }
        }
    }
}
