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

        // Cashed references to components
        private Transform carTransform;
        private Rigidbody carRigidbody;
        private AudioSource carAudioSource;

        //Cashed parameters
        private float carMass;
        private Vector3 carRight; // World Right Vector for our car

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


        public void Move(float steering, float accel, float footbrake, float handbrake)
        {

        }
    }
}