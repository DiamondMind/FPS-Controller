using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Prototypes.Characters.FPS
{
    public enum MovementState
    {
        Idle, Walking, Running, Crouching, Jumping, CustomAction
    }

    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class FirstPersonMovement : MonoBehaviour
    {
        [Header("---------- Height Settings ----------")]
        [SerializeField] private float playerHeight = 2f;
        public Transform camTransform;
        [SerializeField] private float standCamPos;
        [SerializeField] private float crouchCamPos;

        [Header("---------- Movement Settings ----------")]
        public MovementState movementState = MovementState.Idle;
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 10f;
        [SerializeField] private bool canCrouch = true;
        [SerializeField] private float crouchSpeed = 2f;
        [SerializeField] private float crouchHeight = 1f;
        [SerializeField, Range(0.1f, 0.5f)] private float crouchSmooth = 0.25f;
        [SerializeField] private float jumpHeight = 3f;
        [SerializeField, Range(0f, 5f)] private float jumpCoolDown = 2f;

        [Header("---------- Grounded Settings ----------")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Transform groundCheck;

        public bool isEnabled { get; set; }
        public bool isGrounded { get; private set; }
        public bool isInMotion { get; private set; }
        public bool isRunning { get; private set; }
        public bool isCrouching { get; private set; }
        public bool isChangingStance { get; private set; }
        public float jumpDelay { get; private set; }
        public bool customAction { get; set; }
        public Vector3 moveDirection { get; private set; }

        PlayerInput playerInput;
        CharacterController controller;
        FirstPersonController playerController;

        float inputX;
        float inputZ;

        Vector3 velocity;
        Vector3 crouchCenter;
        Vector3 standCenter = Vector3.zero;
        float gravity = -9.81f;
        bool wasGrounded;

        bool isJumping;
        bool isInJumpCoolDown;


        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            controller = playerInput.controller;
            playerController = playerInput.playerController;

            controller.height = playerHeight;
            isEnabled = true;
            standCenter = controller.center;
            crouchCenter = new Vector3(controller.center.x, crouchHeight / 2, controller.center.z);

        }

        private void FixedUpdate()
        {
            SetMovementState();
            CheckGrounded();

            if (!wasGrounded && isGrounded)
            {
                playerController.PlayLandSfx();
            }

            if (isEnabled)
            {
                MoveCharacter();
                Jump();
                HandleCrouch();
                ApplyGravity();
            }

            wasGrounded = isGrounded;
        }

        private void SetMovementState()
        {
            if (isCrouching)
            {
                movementState = MovementState.Crouching;
            }
            else if (isJumping)
            {
                movementState = MovementState.Jumping;
            }
            else if(customAction)
            {
                movementState = MovementState.CustomAction;
            }
            else if (isRunning)
            {
                movementState = MovementState.Running;
            }
            else if (isInMotion)
            {
                movementState = MovementState.Walking;
            }
            else
            {
                movementState = MovementState.Idle;
            }
        }

        private void MoveCharacter()
        {
            if (isGrounded)
            {
                inputX = playerInput.horizontalInput;
                inputZ = playerInput.verticalInput;
            }

            if (inputX != 0 || inputZ != 0)
            {
                isInMotion = true;
            }
            else
            {
                isInMotion = false;
            }

            // movement
            moveDirection = transform.right * inputX + transform.forward * inputZ;

            if (isInMotion && !isCrouching && !isRunning)
            {
                controller.Move(moveDirection * walkSpeed * Time.deltaTime);
            }
            // crouchwalk
            if (isInMotion && isCrouching && !isRunning)
            {
                controller.Move(moveDirection * crouchSpeed * Time.deltaTime);
            }
            // Run
            if (playerInput.sprintInput && isInMotion && playerController.canRun)
            {
                if (isCrouching && !isChangingStance && CanExitCrouch())
                {
                    StartCoroutine(ChangeStance(false));
                }

                if(!isChangingStance)
                {
                    isRunning = true;
                    controller.Move(moveDirection * runSpeed * Time.deltaTime);
                }
            }
            else
                isRunning = false;

        }

        private void Jump()
        {
            if (!isInJumpCoolDown && playerInput.jumpInput && isGrounded && !isCrouching && playerController.canJump)
            {
                float initialJumpVelocity = Mathf.Sqrt(-2f * jumpHeight * gravity); // based on formula
                velocity.y = initialJumpVelocity;  

                playerController.JumpAction();
                StartCoroutine(StartJumpCooldown());
            }
        }

        private IEnumerator StartJumpCooldown()
        {
            isInJumpCoolDown = true;
            yield return new WaitForSeconds(jumpCoolDown);
            isInJumpCoolDown = false;
        }

        private void HandleCrouch()
        {
            if (!isCrouching && !isChangingStance  && playerInput.crouchInput && isGrounded && !isRunning)
            {
                StartCoroutine(ChangeStance(true));
            }
            else if(isCrouching && !isChangingStance && playerInput.crouchInput && CanExitCrouch())
            {
                StartCoroutine(ChangeStance(false));
            }
        }

        private bool CanExitCrouch()
        {
            Vector3 castOrigin = transform.position + Vector3.up * controller.height;
            Ray ray = new Ray(castOrigin, Vector3.up);

            if (Physics.SphereCast(ray, controller.radius, controller.radius))
                return false;
            else
                return true;
        }

        private IEnumerator ChangeStance(bool shouldCrouch)
        {
            isChangingStance = true;
            if (playerController)
                playerController.PlayStanceChangeSfx();

            float timeElapsed = 0f;
            float targetHeight = shouldCrouch ? crouchHeight : playerHeight;
            float currentHeight = controller.height;
            float targetCamPos = shouldCrouch ? crouchCamPos : standCamPos;
            float currentCamPos = camTransform.position.y;
            Vector3 targetCenter = shouldCrouch ? crouchCenter : standCenter;
            Vector3 currentCenter = controller.center;
            isCrouching = shouldCrouch;

            while (timeElapsed < crouchSmooth)
            {
                controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / crouchSmooth);
                controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / crouchSmooth);

                // Lerp the camera position
                Vector3 newCamPos = camTransform.position;
                newCamPos.y = Mathf.Lerp(currentCamPos, targetCamPos, timeElapsed / crouchSmooth);
                camTransform.position = newCamPos;

                timeElapsed += Time.deltaTime;

                yield return null;
            }

            // Set the final values to ensure consistency
            controller.height = targetHeight;
            controller.center = targetCenter;

            // Set the final camera position
            Vector3 finalCamPos = camTransform.position;
            finalCamPos.y = targetCamPos;
            camTransform.position = finalCamPos;

            isChangingStance = false;
        }

        private void ApplyGravity()
        {
            // reset velocity at the right conditions
            if (isGrounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }

            controller.Move(velocity * Time.deltaTime);
        }

        private void CheckGrounded()
        {
            if(Physics.CheckSphere(groundCheck.position, controller.radius, groundLayer, QueryTriggerInteraction.Ignore))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }

    }
}

