using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColliderCar
{
    public class ColliderCar : MonoBehaviour
    {
        public Transform centerOfMass;

        // the physical transforms for the car's wheels
        public Transform[] wheels;
        //wheels transforms to turn for steering/separates steering turn from acceleration turning
        public Transform[] steeringWheels;

        public float maxSteeringAngle = 30f;

        // Cashed references to components
        private Transform carTransform;
        private Rigidbody carRigidbody;
        private AudioSource carAudioSource;

        //Cashed parameters
        private float carMass;
        private Vector3 carRight; // World Right Vector for our car
        private int steeringAxes;

        // Params from input
        private float inputSteering = 0;
        private float inputAcceleration = 0;
        private float inputBrake = 0;
        private float inputHandBrake = 0;

        // Movement params
        private Vector3 relativeVelocity;
        private Vector3 carRotationAmount;
        private Vector3 carCurrentRight;
        private Vector3 carCurrentVelocity;
        private Vector3 carCurrentDirection;
        private float carCurrentSpeed;
        private float carSlideSpeed;
        private float carSteering;
        private bool inReverseMode;

        // Cashed transforms for our wheels
        private Transform[] wheelTransform;

        // Use this for initialization
        void Start()
        {
            InitializeCar();
        }

        void InitializeCar()
        {
            // Cache a reference to our car's transform
            carTransform = GetComponent<Transform>();
            // cache the rigidbody for our car
            carRigidbody = GetComponent<Rigidbody>();
            // cache a reference to the AudioSource
            carAudioSource = GetComponent<AudioSource>();
            // cache the mass of our vehicle 
            carMass = carRigidbody.mass;
            // cache the World Right Vector for our car
            carRight = Vector3.right;
            // call to set up our wheels array
            SetUpWheels();
            // we set a COG here and lower the center of mass to a
            //negative value in Y axis to prevent car from flipping over
            carRigidbody.centerOfMass = centerOfMass.localPosition;
            // 
            steeringAxes = steeringWheels.Length / 2;
        }

        void SetUpWheels()
        {
            // Check if all wheels been assigned
            for (int i = 0; i < wheels.Length; i++)
            {
                if (! wheels[i])
                {
                    Debug.LogError("One or more of the wheel transforms have not been assigned on the car");
                    Debug.Break();
                }
            }

            wheelTransform = new Transform[wheels.Length];
            for (int i = 0; i < wheels.Length; i++)
                wheelTransform[i] = wheels[i];
        }

        // Update is called once per frame
        void Update()
        {

        }
        
        void LateUpdate()
        {
            // this function makes the visual 3d wheels rotate and turn
            RotateVisualWheels();

            //this is where we send to a function to do engine sounds
            EngineSound();
        }


        public void CarPhysicsUpdate(float steering, float accel, float footbrake, float handbrake)
        {
            //clamp input values
            inputSteering = Mathf.Clamp(steering, -1, 1);
            inputAcceleration = Mathf.Clamp(accel, 0, 1);
            inputBrake = -1 * Mathf.Clamp(footbrake, -1, 0);
            inputHandBrake = Mathf.Clamp(handbrake, 0, 1);


            //grab all the physics info we need to calc everything
            carCurrentRight = carTransform.right;

            // figure out our velocity without y movement - our flat velocity
            carCurrentVelocity = carRigidbody.velocity;

            // find out which direction we are moving in
            carCurrentDirection = Vector3.Normalize(carTransform.forward);

            //Debug.DrawRay(carTransform.position, carTransform.forward * 10, Color.yellow);

            //Debug.DrawRay(carTransform.position, dir * 10, Color.green);
            
            // calculate relative velocity
            relativeVelocity = carTransform.InverseTransformDirection(carCurrentVelocity);

            // calculate how much we are sliding (find out movement along our x axis)
            carSlideSpeed = Vector3.Dot(carCurrentRight, carCurrentVelocity);

            // calculate current speed (the magnitude of the flat velocity)
            carCurrentSpeed = carCurrentVelocity.magnitude;

            // check to see if we are moving in reverse
            inReverseMode = Mathf.Sign(Vector3.Dot(carCurrentVelocity, carCurrentDirection)) < 0.1f;

            // calculate engine force with our flat direction vector and acceleration
            engineForce = (carCurrentDirection * (power * throttle) * carMass);

            // do turning
            carSteering = inputSteering;

            // if we're in reverse, we reverse the turning direction too
            if (inReverseMode)
            {
                carSteering = -carSteering;
            }

            // calculate torque for applying to our rigidbody
            turnVec = (((carTransform.up * turnSpeed) * carSteering) * carMass) * 800f;

            // calculate impulses to simulate grip by taking our right vector, reversing the slidespeed and
            // multiplying that by our mass, to give us a completely 'corrected' force that would completely
            // stop sliding. we then multiply that by our grip amount (which is, technically, a slide amount) which
            // reduces the corrected force so that it only helps to reduce sliding rather than completely
            // stop it 

            actualGrip = Mathf.Lerp(100f, carGrip, mySpeed * 0.02f);
            imp = myRight * (-carSlideSpeed * carMass * actualGrip);
        }

        /// <summary>
        /// Method rotates wheel meshes
        /// </summary>
        void RotateVisualWheels()
        {
            if (steeringWheels.Length > 0)
            {
                Vector3 tmpEulerAngles = steeringWheels[0].localEulerAngles;
                int rotatingAxes = steeringWheels.Length / 2;

                // front wheels visual rotation while steering the car
                for (int i = 0; i < steeringWheels.Length; i++)
                {
                    rotatingAxes = i / 2 + 1;
                    tmpEulerAngles.y = inputSteering * maxSteeringAngle / rotatingAxes;
                    steeringWheels[i].localEulerAngles = tmpEulerAngles;
                }
                //LFWheelTransform.localEulerAngles = tmpEulerAngles;
                //RFWheelTransform.localEulerAngles = tmpEulerAngles;
            }

            carRotationAmount = carRight * (relativeVelocity.z * 1.6f * Time.deltaTime * Mathf.Rad2Deg); ;

            for (int i = 0; i < steeringWheels.Length; i++)
                wheelTransform[i].Rotate(carRotationAmount);
        }

        //this controls the sound of the engine audio by adjusting the pitch of our sound file
        void EngineSound()
        {
/*            carAudioSource.pitch = 0.30f + mySpeed * 0.025f;

            if (mySpeed > 30f)
            {
                carAudioSource.pitch = 0.25f + mySpeed * 0.015f;
            }
            if (mySpeed > 40f)
            {
                carAudioSource.pitch = 0.20f + mySpeed * 0.013f;
            }
            if (mySpeed > 49f)
            {
                carAudioSource.pitch = 0.15f + mySpeed * 0.011f;
            }
            //ensures we dont exceed to crazy of a pitch by resetting it back to default 2
            if (carAudioSource.pitch > 2.0f)
            {
                carAudioSource.pitch = 2.0f;
            }
*/        }
    }
}