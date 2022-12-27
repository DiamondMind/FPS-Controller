using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Characters.FirstPerson
{
    public class dPlayerMovement : MonoBehaviour
    {
        dPlayerInput _playerInput;
        CharacterController _controller;
        [SerializeField] private Transform mainCamera;

        public bool isEnabled;
        [Header("Basic Movement")]
        [SerializeField] public bool isGrounded;
        [SerializeField] public bool useGravity = true;
        [SerializeField] public float playerHeight = 2f;
        [SerializeField] public bool isInMotion;    //{ get; private set; }
        [SerializeField] public bool isSprinting;
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 10f;
        [SerializeField] public bool isCrouching = false;
        [SerializeField] private float crouchSpeed = 2f;
        [SerializeField] private float crouchHeight = 1f;
        [SerializeField] [Range(0.1f, 0.5f)] private float crouchSmooth = 0.25f;
        [SerializeField] private bool isInTransition;
        [SerializeField] private bool canCrouch = true;
        [SerializeField] private float jumpHeight = 3f;
        [SerializeField] [Range(0f, 5f)] private float jumpTimer = 3f;

        public Vector3 moveDirection { get; private set; }

        float gravity = -9.81f;
        Vector3 velocity;
        Vector3 crouchCenter = new Vector3(0, 0.5f, 0);
        Vector3 standCenter = new Vector3(0, 0, 0);
        float jumpDelay;

        
        private void Start()
        {
            isEnabled = true;
            _controller = GetComponent<CharacterController>();
            _playerInput = GetComponent<dPlayerInput>();
            _controller.height = playerHeight;

            useGravity = true;
        }

        private void FixedUpdate()
        {
            
            isGrounded = _controller.isGrounded;
            
            // reset velocity at the right conditions
            if (isGrounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }
            // avoid standing if crouching and close to ceiling
            if (isCrouching && Physics.Raycast(mainCamera.transform.position, Vector3.up, 1f))
            {
                canCrouch = false;
            }
            else
                canCrouch = true;
           
            if(isEnabled)
            {
                Movement();
                Jump();
                Crouch();
                Gravity();
            }
        }

        private void Movement()
        {
            float x = _playerInput.horizontalInput;
            float z = _playerInput.verticalInput;
            // check isWalking bool
            if (x != 0 || z != 0)
            {
                isInMotion = true;
            }
            else
            {
                isInMotion = false;
            }
            
            // movement
            moveDirection = transform.right * x + transform.forward * z; // get the local direction of the player and store in a vector3 variable
            // walk
            if (isInMotion && !isCrouching && !_playerInput.sprintInput)  
            {
                _controller.Move(moveDirection * walkSpeed * Time.deltaTime);
            }
            // crouchwalk
            if (isInMotion && isCrouching && !_playerInput.sprintInput)
            {
                _controller.Move(moveDirection * crouchSpeed * Time.deltaTime);
            }
            // sprint
            if (_playerInput.sprintInput && isInMotion)    
            {
                isSprinting = true;
                isInMotion = false;
                _controller.Move(moveDirection * sprintSpeed * Time.deltaTime);
                // stand up while sprinting
                if (isCrouching) 
                {
                    float standCameraHeight = playerHeight / 2;
                    _controller.height = playerHeight;
                    isCrouching = false;
                }
            } 
            else
            {
                isSprinting = false;
            }
        }
        private void Jump()
        {
            if (_playerInput.jumpInput && isGrounded && !isCrouching && Time.time > jumpDelay)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);    // based on formula
                //isGrounded = false;
                jumpDelay = Time.time + jumpTimer;  // prevent jump until delay is completed
            }
        }
        private void Crouch()
        {
            if( canCrouch && !isInTransition && _playerInput.crouchInput && !_playerInput.sprintInput && isGrounded)
            {
                StartCoroutine(ChangeStance());
            }
        }
        private IEnumerator ChangeStance()
        {
            isInTransition = true;
            float timeElapsed = 0f;
            float targetHeight = isCrouching ? playerHeight : crouchHeight;
            float currentHeight = _controller.height;
            Vector3 targetCenter = isCrouching ? standCenter : crouchCenter;
            Vector3 currentCenter = _controller.center;

            while(timeElapsed < crouchSmooth)
            {
                    // toggle height & center
                    _controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / crouchSmooth);
                    _controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / crouchSmooth);
                    timeElapsed += Time.deltaTime;
                
                yield return null;
            }
            // avoid inconsistency in values
            _controller.height = targetHeight;
            _controller.center = targetCenter;

            isCrouching = !isCrouching;

            isInTransition = false;
        }
        private void Gravity()
        {
            if(useGravity == true)
            {
                // gravity
                velocity.y += gravity * Time.deltaTime;
                _controller.Move(velocity * Time.deltaTime);
            }
        }
    }
}
