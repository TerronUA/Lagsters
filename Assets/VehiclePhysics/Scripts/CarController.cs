using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour
{
    public float deviceAccelerometerSensitivity = 2f; //how sensitive our mobile accelerometer will be
    public float deadZone = .001f; //controls mobile device tilting dead zone 
    public float horizontal; //horizontal input control, either mobile control or keyboard
    public float maxSpeedToTurn = 0.25f; //keeps car from turning until it's reached this value 

    // the physical transforms for the car's wheels
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    //these transform parents will allow wheels to turn for steering/separates steering turn from acceleration turning
    public Transform LFWheelTransform;
    public Transform RFWheelTransform;

    //do not adjust this, but you will be able to read it in the inspector, and read it's value for a speedometer 
    public float mySpeed;

    // car physics adjustments
    public float power = 1200f;
    public float maxSpeed = 50f;
    public float carGrip = 70f;
    public float turnSpeed = 2.5f; //keep this value somewhere between 2.5 and 6.0

    //bunch of variables we do not adjust, script handles these internally 
    private float slideSpeed;
    private Vector3 carRight;
    private Vector3 carFwd;
    private Vector3 tempVEC;
    private Vector3 rotationAmount;
    private Vector3 accel;
    private float throttle;
    private Vector3 myRight;
    private Vector3 velo;
    private Vector3 flatVelo;
    private Vector3 relativeVelocity;
    private Vector3 dir;
    private Vector3 flatDir;
    private Vector3 carUp;
    private Transform carTransform;
    private Rigidbody thisRigidbody;
    private Vector3 engineForce;
    private float actualGrip;
    private Vector3 turnVec;
    private Vector3 imp;
    private float rev;
    private float actualTurn;
    private float carMass;
    private Transform[] wheelTransform = new Transform[4]; //these are the transforms for our 4 wheels
    private AudioSource myAudioSource;

    void Start()
    {
        InitializeCar();
    }

    void InitializeCar()
    {
        // Cache a reference to our car's transform
        carTransform = GetComponent<Transform>();
        // cache the rigidbody for our car
        thisRigidbody = GetComponent<Rigidbody>();
        // cache a reference to the AudioSource
        myAudioSource = GetComponent<AudioSource>();
        // cache our vector up direction
        carUp = carTransform.up;
        // cache the mass of our vehicle 
        carMass = thisRigidbody.mass;
        // cache the Forward World Vector for our car
        carFwd = Vector3.forward;
        // cache the World Right Vector for our car
        carRight = Vector3.right;
        // call to set up our wheels array
        SetUpWheels();
        // we set a COG here and lower the center of mass to a
        //negative value in Y axis to prevent car from flipping over
        thisRigidbody.centerOfMass = new Vector3(0f, -0.75f, .35f);
    }

    void Update()
    {
        // call the function to start processing all vehicle physics
        CarPhysicsUpdate();

        //call the function to see what input we are using and apply it
        CheckInput();
    }

    void LateUpdate()
    {
        // this function makes the visual 3d wheels rotate and turn
        RotateVisualWheels();

        //this is where we send to a function to do engine sounds
        EngineSound();
    }

    void SetUpWheels()
    {
        if ((null == frontLeftWheel || null == frontRightWheel || null == rearLeftWheel || null == rearRightWheel))
        {
            Debug.LogError("One or more of the wheel transforms have not been assigned on the car");
            Debug.Break();
        }
        else
        {
            //set up the car's wheel transforms
            wheelTransform[0] = frontLeftWheel;
            wheelTransform[1] = rearLeftWheel;
            wheelTransform[2] = frontRightWheel;
            wheelTransform[3] = rearRightWheel;
        }
    }

    void RotateVisualWheels()
    {
        Vector3 tmpEulerAngles = LFWheelTransform.localEulerAngles;
        tmpEulerAngles.y = horizontal * 30f;

        // front wheels visual rotation while steering the car
        LFWheelTransform.localEulerAngles = tmpEulerAngles;
        RFWheelTransform.localEulerAngles = tmpEulerAngles;

        rotationAmount = carRight * (relativeVelocity.z * 1.6f * Time.deltaTime * Mathf.Rad2Deg);

        wheelTransform[0].Rotate(rotationAmount);
        wheelTransform[1].Rotate(rotationAmount);
        wheelTransform[2].Rotate(rotationAmount);
        wheelTransform[3].Rotate(rotationAmount);
    }

    void CheckInput()
    {
        //Mobile platform turning input...testing to see if we are on a mobile device.              
        if (Application.platform == RuntimePlatform.IPhonePlayer || (Application.platform == RuntimePlatform.Android))
        {
            // we give the acceleration a little boost to make turning more sensitive
            accel = Input.acceleration * deviceAccelerometerSensitivity;

            if (accel.x > deadZone || accel.x < -deadZone)
            {
                horizontal = accel.x;
            }
            else
            {
                horizontal = 0;
            }
            throttle = 0;

            foreach (Touch touch in Input.touches)
            {
                if (touch.position.x > Screen.width - Screen.width / 3 && touch.position.y < Screen.height / 3)
                {
                    throttle = 1f;
                }
                else if (touch.position.x < Screen.width / 3 && touch.position.y < Screen.height / 3)
                {
                    throttle = -1f;
                }
            }
        }
        else //this input is for the Unity editor 
        {
            //Use the Keyboard for all car input
            horizontal = Input.GetAxis("Horizontal");
            throttle = Input.GetAxis("Vertical");
        }
    }

    void CarPhysicsUpdate()
    {
        //grab all the physics info we need to calc everything
        myRight = carTransform.right;

        Debug.DrawRay(carTransform.position, myRight * 3, Color.red);

        // find our velocity
        velo = thisRigidbody.velocity;

        tempVEC = new Vector3(velo.x, 0f, velo.z);

        // figure out our velocity without y movement - our flat velocity
        flatVelo = tempVEC;

        // find out which direction we are moving in
        dir = transform.TransformDirection(carFwd);
        
        Debug.DrawRay(carTransform.position, carFwd * 5, Color.yellow);

        Debug.DrawRay(carTransform.position, dir * 3, Color.green);

        tempVEC = new Vector3(dir.x, 0, dir.z);

        // calculate our direction, removing y movement - our flat direction
        flatDir = Vector3.Normalize(tempVEC);

        // calculate relative velocity
        relativeVelocity = carTransform.InverseTransformDirection(flatVelo);

        // calculate how much we are sliding (find out movement along our x axis)
        slideSpeed = Vector3.Dot(myRight, flatVelo);

        // calculate current speed (the magnitude of the flat velocity)
        mySpeed = flatVelo.magnitude;

        // check to see if we are moving in reverse
        rev = Mathf.Sign(Vector3.Dot(flatVelo, flatDir));

        // calculate engine force with our flat direction vector and acceleration
        engineForce = (flatDir * (power * throttle) * carMass);

        // do turning
        actualTurn = horizontal;

        // if we're in reverse, we reverse the turning direction too
        if (rev < 0.1f)
        {
            actualTurn = -actualTurn;
        }

        // calculate torque for applying to our rigidbody
        turnVec = (((carUp * turnSpeed) * actualTurn) * carMass) * 800f;

        // calculate impulses to simulate grip by taking our right vector, reversing the slidespeed and
        // multiplying that by our mass, to give us a completely 'corrected' force that would completely
        // stop sliding. we then multiply that by our grip amount (which is, technically, a slide amount) which
        // reduces the corrected force so that it only helps to reduce sliding rather than completely
        // stop it 

        actualGrip = Mathf.Lerp(100f, carGrip, mySpeed * 0.02f);
        imp = myRight * (-slideSpeed * carMass * actualGrip);

    }

    //this controls the sound of the engine audio by adjusting the pitch of our sound file
    void EngineSound()
    {
        myAudioSource.pitch = 0.30f + mySpeed * 0.025f;

        if (mySpeed > 30f)
        {
            myAudioSource.pitch = 0.25f + mySpeed * 0.015f;
        }
        if (mySpeed > 40f)
        {
            myAudioSource.pitch = 0.20f + mySpeed * 0.013f;
        }
        if (mySpeed > 49f)
        {
            myAudioSource.pitch = 0.15f + mySpeed * 0.011f;
        }
        //ensures we dont exceed to crazy of a pitch by resetting it back to default 2
        if (myAudioSource.pitch > 2.0f)
        {
            myAudioSource.pitch = 2.0f;
        }
    }

    void FixedUpdate()
    {
        if (mySpeed < maxSpeed)
        {
            // apply the engine force to the rigidbody
            thisRigidbody.AddForce(engineForce * Time.deltaTime);
        }
        //if we're going to slow to allow car to rotate around 
        if (mySpeed > maxSpeedToTurn)
        {
            // apply torque to our rigidbody
            thisRigidbody.AddTorque(turnVec * Time.deltaTime);
        }
        else if (mySpeed < maxSpeedToTurn)
        {
            return;
        }
        // apply forces to our rigidbody for grip
        thisRigidbody.AddForce(imp * Time.deltaTime);
    }
}
