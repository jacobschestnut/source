using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;

    [Header("Look Parameters")]
    [SerializeField, Range(1,10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1,10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1,100)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1,100)] private float lowerLookLimit = 80.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpHeight = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouch Parameters")]
    [SerializeField] private bool toggleCrouch = false;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new(0,0.5f,0); 
    [SerializeField] private Vector3 standingCenter = new(0,0,0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    private Camera playerCamera;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector2 currentInput;
    private float rotationX = 0;

    // Awake is called before Start()
    void Awake()
    { 
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if(CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if(canJump)
            {
                HandleJump();
            }

            if(canCrouch)
            {
                HandleCrouch();
            }

            ApplyFinalMovement();
        }
    }

    // Update is called once per frame
    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching ? crouchSpeed : (IsSprinting ? sprintSpeed : walkSpeed)) * Input.GetAxis("Vertical"), (IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleJump()
    {
        if(ShouldJump)
        {
            moveDirection.y = jumpHeight;
        }
    }

    private void HandleCrouch()
    {
        if (toggleCrouch && Input.GetKeyDown(crouchKey) && canCrouch)
        {
            isCrouching = !isCrouching;
            StartCoroutine(CrouchStand(isCrouching));
        }
        else if (!toggleCrouch && Input.GetKey(crouchKey) && canCrouch)
        {
            if (!isCrouching)
            {
                StartCoroutine(CrouchStand(true));
            }
        }
        else if (!toggleCrouch && isCrouching && !Input.GetKey(crouchKey))
        {
            StartCoroutine(CrouchStand(false));
        }
    }

    private void ApplyFinalMovement()
    {
        if(!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand(bool shouldCrouch)
    {
        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = shouldCrouch ? crouchHeight : standingHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = shouldCrouch ? crouchingCenter : standingCenter;
        Vector3 currentCenter = characterController.center;

        while(timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime; 
            yield return null;
        }

        isCrouching = shouldCrouch;
        
        characterController.height = targetHeight;
        characterController.center = targetCenter;

        duringCrouchAnimation = false;
    }
}
