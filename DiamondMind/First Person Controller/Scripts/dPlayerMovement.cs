using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Prototypes.FPSPlayer
{
    public class dPlayerMovement : MonoBehaviour
    {
        [SerializeField] private dPlayerInput _playerInput;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform mainCamera;

        [SerializeField] private float playerHeight = 2f;
        [SerializeField] private bool isWalking;
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float crouchSpeed = 2f;
        [SerializeField] private float sprintSpeed = 10f;
        [SerializeField] private float jumpHeight = 3f;
       
        [SerializeField] private bool isCrouching = false;
        [SerializeField] private float crouchHeight = 1f;
        [SerializeField] [Range(0f, 3f)] private float crouchTimer = 2f;
        [SerializeField] [Range(0f, 5f)] private float jumpTimer = 3f;

        [SerializeField] private bool isGrounded;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float gravity = -9.81f;

        Vector3 velocity;
        float jumpDelay;
        float crouchDelay;
        
        private void Start()
        {
            // get relevant components
            _controller = GetComponent<CharacterController>();
            _playerInput = GetComponent<dPlayerInput>();

            _controller.height = playerHeight;
        }
        void FixedUpdate()
        {
            // set isGrounded bool
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
            // reset velocity at the right conditions
            if(isGrounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }

            Movement();
            Jump();
            Crouch();
            Gravity();
        }
        void Movement()
        {
            // gather keyboard input and store in float variables
            float x = _playerInput.horizontalInput;
            float z = _playerInput.verticalInput;
            // check isWalking bool
            if (x != 0 || z != 0)
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }
            // movement
            Vector3 move = transform.right * x + transform.forward * z; // get the local direction of the player and store in a vector3 variable
            // walk
            if (isWalking && !isCrouching && !_playerInput.sprintInput)  
            {
                _controller.Move(move * walkSpeed * Time.deltaTime);
            }
            // crouchwalk
            if (isWalking && isCrouching && !_playerInput.sprintInput)
            {
                _controller.Move(move * crouchSpeed * Time.deltaTime);
            }
            // sprint
            else if (_playerInput.sprintInput && isWalking)    
            {
                _controller.Move(move * sprintSpeed * Time.deltaTime);
                // stand up while sprinting
                if (isCrouching) 
                {
                    float standCameraHeight = playerHeight / 2;
                    _controller.height = playerHeight;
                    SetCameraHeight(standCameraHeight);
                    isCrouching = false;
                }
            } 
        }
        void Jump()
        {
            // jump 
            if (_playerInput.jumpImput && isGrounded && Time.time > jumpDelay)
            {
                if (isCrouching)
                {
                    float standCameraHeight = playerHeight / 2;
                    _controller.height = playerHeight;
                    SetCameraHeight(standCameraHeight);
                    isCrouching = false;
                }
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);    // based on formula
                isGrounded = false;
                jumpDelay = Time.time + jumpTimer;  // prevent jump until delay is completed
            }
        }
        void SetCameraHeight(float desiredHeight)
        {
            // set camera height based on stance
            mainCamera.transform.localPosition = new Vector3(0, desiredHeight, 0);
        }
        void Crouch()
        {
            float standCameraHeight = playerHeight / 2;
            float crouchCameraHeight = crouchHeight / 2;
            // crouch
            if (!isCrouching && _playerInput.crouchInput && isGrounded  && Time.time > crouchDelay)
            {
                _controller.height = crouchHeight;   // set controller height to crouch height
                SetCameraHeight(crouchCameraHeight);
                isCrouching = true;
                crouchDelay = Time.time + crouchTimer;  // prevent crouch until delay is completed 
            }
            else if (isCrouching && _playerInput.crouchInput && isGrounded && Time.time > crouchDelay)
            {
                _controller.height = playerHeight;   // set controller height to stand height
                SetCameraHeight(standCameraHeight);
                isCrouching = false;
                crouchDelay = Time.time + crouchTimer;
            }
        }
        void Gravity()
        {
            // gravity
            velocity.y += gravity * Time.deltaTime;
            _controller.Move(velocity * Time.deltaTime);
        }
    }
}
