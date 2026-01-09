//Q
/*
+ Needs an Empty with a camera as a child to work
+ Camera (MainCamera) near head level
Add this script to the Empty
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float crouchMultiplier = 0.5f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.81f;


    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownRange = 80.0f;


    [Header("Inputs Customization")]
    [SerializeField] private string verticalInput = "Vertical";
    [SerializeField] private string horizontalInput = "Horizontal";
    [SerializeField] private string MouseXInput = "Mouse X";
    [SerializeField] private string MouseYInput = "Mouse Y";
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.RightShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;


    private Camera mainCamera;
    private float verticalRotation;
    private Vector3 currentMovement = Vector3.Zero;
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }
    void HandleMovement()
    {
        // Get input
        float sprintMultiplier = Input.GetKey(sprintKey) ? sprintMultiplier : 1f;
        float crouchMultiplier = Input.GetKey(crouchKey) ? crouchMultiplier : 1f;
        float speedMultiplier = sprintMultiplier * crouchMultiplier;

        float verticalSpeed = Input.GetAxis(verticalInput) * walkSpeed * speedMultiplier;
        float horizontalSpeed = Input.GetAxis(horizontalInput) * walkSpeed * speedMultiplier;

        Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0, verticalSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;// Make sure is facing right

        HandleGravityAndJumping();

        currentMovement.X = horizontalMovement.X;
        currentMovement.Z = horizontalMovement.Z;

        characterController.Move(currentMovement * Time.deltaTime);
    }

    void HandleGravityAndJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.Y = -0.5f;
            if (Input.GetKeyDown(jumpKey))
            {
                currentMovement.Y = jumpForce;
            }
        }
        else
        {
            currentMovement.Y -= gravity * Time.deltaTime;
        }
    }


    void HandleRotation()
    {
        float mouseXRotation = Input.GetAxis(mouseXInput) * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= Input.GetAxis(mouseYInput) * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}