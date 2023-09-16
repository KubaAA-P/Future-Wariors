using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 3f;
    public float rotationSpeed = 2f;
    public float jumpForce = 2f;
    public float inAirMultiplicator = 0.7f;

    public bool isSprinting;
    public bool isCrouching;

    public Camera playerCamera;

    public float playerHeight = 2f;

    private Rigidbody playerRigidBody;

    private float rotationY;
    private float rotationX;

    private KeyCode jumpKey = KeyCode.Space;
    private KeyCode sprintKey = KeyCode.LeftShift;
    private KeyCode crouchKey = KeyCode.LeftControl;

    private void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();

        if (playerCamera == null && playerCamera.enabled)
        {
            Camera.SetupCurrent(playerCamera); //fp mode
        }

        Cursor.lockState = CursorLockMode.Locked; //hiding cursor (3d fps game)
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector3 movement = XYMovementCalculations();

        if (Input.GetKey(sprintKey))
        {
            playerRigidBody.velocity = movement * sprintSpeed;
            isSprinting = true;
        }
        else if (Input.GetKey(crouchKey))
        {
            playerRigidBody.velocity = movement * crouchSpeed;
            isCrouching = true;
        } else
        {
            playerRigidBody.velocity = movement * moveSpeed;
            isSprinting = false;
            isCrouching = false;
        }

        RotationCalculations();
        transform.localRotation = Quaternion.Euler(0f, rotationY, 0f);

        if (playerCamera != null) 
        {
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }

        Jumping();
    }

    Vector3 XYMovementCalculations()
    {
        float moveX = Input.GetAxis("Horizontal"); // A and D keys
        float moveY = Input.GetAxis("Vertical");   // W and S keys

        Vector3 movement = new Vector3(moveX, 0f, moveY);

        Vector3 output = transform.TransformDirection(movement); //local -> gloabal rotation

        return output;
    }


    void Jumping()
    {
        if (Input.GetKeyDown(jumpKey) && IsGrounded())
        {
            Vector3 jumpVector = Vector3.up * jumpForce; //jump upwards

            playerRigidBody.AddForce(jumpVector, ForceMode.Impulse);
        }
    }

    void RotationCalculations()
    {
        float airMultiplicator = 1f;

        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed; //mouse movement
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        if (!IsGrounded())
        {
            airMultiplicator = inAirMultiplicator; //in air worst movement
        }

        rotationY += mouseX * airMultiplicator; 
        rotationX -= mouseY * airMultiplicator; //down mouse = down cam
    }

    bool IsGrounded()
    {
        float raycastDistance = playerHeight; 

        return Physics.Raycast(transform.position, Vector3.down, raycastDistance);
    }
}
