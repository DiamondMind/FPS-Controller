using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Prototypes.Characters.FPS
{
    public class HeadBobController : MonoBehaviour
    {
        public bool isEnabled = true;

        [Header("---------- HeadBob Parameters ----------")]
        [SerializeField] private float walkBobSpeed = 10f;
        [SerializeField] [Range(0.01f, 0.1f)] private float walkMagnitude = 0.05f;
        [SerializeField] private float crouchBobSpeed = 5f;
        [SerializeField] [Range(0.01f, 0.1f)] private float crouchMagnitude = 0.025f;
        [SerializeField] private float sprintBobSpeed = 15f;
        [SerializeField] [Range(0.05f, 0.5f)] private float sprintMagnitude = 0.1f;
        [SerializeField] private float idleBobSpeed = 2f;
        [SerializeField] [Range(0.01f, 0.1f)] private float idleMagnitude = 0.03f;

        PlayerInput playerInput;
        FirstPersonMovement playerMovement;
        Transform camHolder;
        Vector3 originalPosition;
        float startYPos;
        float timer;
        public bool startEnabled { get; private set; }

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            camHolder = playerInput.playerMovement.camTransform;
            startEnabled = isEnabled;
            startYPos = camHolder.localPosition.y;
            originalPosition = camHolder.localPosition;
        }

        private void Update()
        {
            if(playerMovement.isChangingStance)
            {
                startYPos = camHolder.localPosition.y;
                originalPosition = camHolder.localPosition;
            }

            if (isEnabled && !playerMovement.isChangingStance)
            {
                DoHeadBob();
            }
        }

        private void DoHeadBob()
        {
            if (playerMovement.isGrounded && playerMovement.moveDirection.magnitude > 0)
            {
                float bobSpeed = playerMovement.isCrouching ? crouchBobSpeed :
                                playerMovement.isRunning ? sprintBobSpeed : walkBobSpeed;

                timer += Time.deltaTime * bobSpeed;
                float magnitude = playerMovement.isCrouching ? crouchMagnitude :
                                 playerMovement.isRunning ? sprintMagnitude : walkMagnitude;

                camHolder.localPosition = new Vector3(camHolder.localPosition.x,
                    startYPos + Mathf.Sin(timer) * magnitude,
                    camHolder.transform.localPosition.z);
            }
            else
            {
                // Apply idle head bob when not moving
                timer += Time.deltaTime * idleBobSpeed;
                camHolder.localPosition = new Vector3(camHolder.localPosition.x,
                    startYPos + Mathf.Sin(timer) * idleMagnitude,
                    camHolder.transform.localPosition.z);
            }
        }
    }

}
