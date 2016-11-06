using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public float movementSpeed = 10f;

//    public float mouseSensitivity = 5.0f;

//    public float jumpSpeed = 5.0f;

    public bool isTiltAxisInverted = false;

//    public float upDownRange = 60.0f;

    
    float verticalRotation = 0.0f;
    float verticalVelocity = 0.0f;
    
    //todo: uncomment for double jumping
    //bool doubleJumping = false;

    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        LockCursor(true);
    }

    void Update()
    {
        DoMove();

        // if ESCAPE key is pressed, then unlock the cursor
        if (Input.GetButtonDown("Cancel"))
        {
            LockCursor(false);
        }

        // if the player fires, then relock the cursor
        if (Input.GetButtonDown("Fire1"))
        {
            LockCursor(true);
        }
    }

    private void LockCursor(bool isLocked)
    {
        if (isLocked)
        {
            // make the mouse pointer invisible
            Cursor.visible = false;

            // lock the mouse pointer within the game area
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // make the mouse pointer visible
            Cursor.visible = true;

            // unlock the mouse pointer so player can click on other windows
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void DoMove()
    {
        //float rotateLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
        //transform.Rotate(0, rotateLeftRight, 0);

        //if (isTiltAxisInverted)
        //    verticalRotation += Input.GetAxis("Mouse Y") * mouseSensitivity;
        //else
        //    verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;


        //verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);

        //Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        float forwardSpeed = Input.GetAxis("Vertical") * movementSpeed;
        float sideSpeed = Input.GetAxis("Horizontal") * movementSpeed;

        verticalVelocity += Physics.gravity.y * Time.deltaTime;
        
        // todo: add double jumping
        //if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        //{
        //    verticalVelocity = jumpSpeed;
        //}
        transform.Rotate(0, sideSpeed, 0);
        Vector3 speed = new Vector3(0, verticalVelocity, forwardSpeed);

        speed = transform.rotation * speed;

        characterController.Move(speed * Time.deltaTime);
    }
}
