using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CubeCar
{
    [RequireComponent(typeof(CubeCarController))]
    public class CubeCarUserController : MonoBehaviour
    {
        public Text speedDisplay;
        private CubeCarController m_Car; // the car controller we want to use

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CubeCarController>();
        }
        
        void FixedUpdate()
        {
            // pass the input to the car!
            float inputVertical = Input.GetAxis("Vertical");
            float inputHorizontal = Input.GetAxis("Horizontal");

            //            if ((h != 0) || (v != 0))
            //                Debug.Log("h = " + h + "; v = " + v);
#if !MOBILE_INPUT
            float handbrake = Input.GetAxis("Jump");
            m_Car.Move(inputHorizontal, inputVertical, inputVertical, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
            speedDisplay.text = Math.Truncate(m_Car.CurrentSpeed).ToString();
        }
    }
}