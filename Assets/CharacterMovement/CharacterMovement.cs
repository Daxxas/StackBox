using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public Vector3 velocity;
    private Vector2 inputVector;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float accelerationTimeAirborne = 0f;
    [SerializeField] private float accelerationTimeGrounded = 0f;
    [Header("Jump")]
    [SerializeField] private float MaxjumpHeight = 4f;
    [SerializeField] private float MinjumpHeight = 1f;
    [SerializeField] private float timeJumpApex = .4f;
    [SerializeField] private float gravityFallScale = 1;
    
    private CharacterController characterController;

    private bool isRunning = false;
    
    private float gravity => -2 * MaxjumpHeight / (timeJumpApex * timeJumpApex);
    private float MaxJumpVelocity => 2 * MaxjumpHeight / timeJumpApex;
    private float MinJumpVelocity => Mathf.Sqrt(2 * Mathf.Abs(gravity) * MinjumpHeight);
    private float maxHeightReached = Mathf.NegativeInfinity;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        
        //Movement stuff
        if (!characterController.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        if (isRunning)
        {
            applyMovement(runSpeed);
        }
        else
        {
            applyMovement(moveSpeed);
        }
        

        if (Time.timeScale == 1)
        {
            //Rotation
            transform.LookAt(transform.position + new Vector3(inputVector.x, 0, inputVector.y));
            //Move
            characterController.Move(velocity * Time.deltaTime);
        }

        
    }

    private void applyMovement(float speed)
    {
        
        velocity.x = inputVector.x * speed;
        velocity.z = inputVector.y * speed;
    }
    
    public void InputMovement(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }
    

    public void Jump(InputAction.CallbackContext context)
    {
        if (characterController.isGrounded)
        {
            velocity.y = MaxJumpVelocity;
        }
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRunning = true;
        }

        if (context.canceled)
        {
            isRunning = false;
        }
    }
} 
