using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ColliderCar
{
    interface IControllerStrategy
    {
        float DeviceAccelerometerSensitivity { get; set; }
        float DeadZone { get; set; }
        void GetInput(out float steering, out float acceleration, out float footbrake, out float handbrake);
    }

    class AccelerometerController : IControllerStrategy
    {
        private float deviceAccelerometerSensitivity = 2f; //how sensitive our mobile accelerometer will be
        public float deadZone = 0.001f; //controls mobile device tilting dead zone 
        private Vector3 accelerationInput;

        public float DeviceAccelerometerSensitivity
        {
            get { return deviceAccelerometerSensitivity; }
            set { deviceAccelerometerSensitivity = value; }
        }

        public float DeadZone
        {
            get { return deadZone; }
            set { deadZone = value; }
        }

        public void GetInput(out float steering, out float acceleration, out float footbrake, out float handbrake)
        {
            steering = acceleration = footbrake = handbrake = 0f;

            // we give the acceleration a little boost to make turning more sensitive
            accelerationInput = Input.acceleration * deviceAccelerometerSensitivity;

            if (accelerationInput.x > deadZone || accelerationInput.x < -deadZone)
            {
                steering = accelerationInput.x;
            }
            else
            {
                steering = 0;
            }

            foreach (Touch touch in Input.touches)
            {
                if (touch.position.x > Screen.width - Screen.width / 3 && touch.position.y < Screen.height / 3)
                {
                    acceleration = footbrake = 1f;
                }
                else if (touch.position.x < Screen.width / 3 && touch.position.y < Screen.height / 3)
                {
                    acceleration = footbrake = -1f;
                }
            }
        }
    }
    
    /// <summary>
    /// Class for getting input for devices with keyboard
    /// </summary>
    class KeyboardController : IControllerStrategy
    {
        public float DeviceAccelerometerSensitivity { get; set; }
        public float DeadZone { get; set; }

        public void GetInput(out float steering, out float acceleration, out float footbrake, out float handbrake)
        {
            //Use the Keyboard for all car input
            steering = Input.GetAxis("Horizontal");
            acceleration = footbrake = Input.GetAxis("Vertical");
            handbrake = Input.GetAxis("Jump");
        }
    }

    [RequireComponent(typeof(ColliderCar))]
    public class ColliderCarUserController : MonoBehaviour
    {
        public float deviceAccelerometerSensitivity = 2f; //how sensitive our mobile accelerometer will be
        public float deadZone = 0.001f; //controls mobile device tilting dead zone 
        public Text speedText;

        IControllerStrategy strategyController;
        ColliderCar car;
        float steering = 0;
        float acceleration = 0;
        float footbrake = 0;
        float handbrake = 0;

        // Use this for initialization
        void Start()
        {

            //Mobile platform turning input...testing to see if we are on a mobile device. 

#if UNITY_EDITOR
    #if !MOBILE_INPUT
            strategyController = new KeyboardController();
    #else
            strategyController = new AccelerometerController();
    #endif
#else
            if (Application.platform == RuntimePlatform.IPhonePlayer || (Application.platform == RuntimePlatform.Android))
                strategyController = new AccelerometerController();
            else
                strategyController = new KeyboardController();
#endif

            strategyController.DeviceAccelerometerSensitivity = deviceAccelerometerSensitivity;
            strategyController.DeadZone = deadZone;
        }

        void Awake()
        {
            // get the car controller
            car = GetComponent<ColliderCar>();
        }

        // Update is called once per frame
        void Update()
        {
            strategyController.GetInput(out steering, out acceleration, out footbrake, out handbrake);

            car.CarPhysicsUpdate(steering, acceleration, footbrake, handbrake);
        }

        private void FixedUpdate()
        {
            if (speedText)
                speedText.text = Math.Truncate(car.Velocity).ToString();
        }
    }
}