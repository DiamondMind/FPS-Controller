using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Prototypes.Characters.FPS
{
    public class LadderAction : MonoBehaviour
    {
        [Tooltip(" How much the player should face the ladder before climbing is possible")]
        [Range(0, 1), SerializeField] private float alignmentThreshold = 0.9f;
        public float climbSpeed = 3f;
        [SerializeField] private string ladderTag = "Ladder";
        [SerializeField] private string ladderEndTag = "ladderEndPoint";

        PlayerInput playerInput;

        bool isOnLadder;
        bool isClimbing;
        bool isInsideLadderZone = false;
        Vector3 climbDirection;
        Transform ladderTransform;

        void Start()
        {
            playerInput = GetComponent<PlayerInput>();

            isOnLadder = false;
        }

        private void Update()
        {
            if (isInsideLadderZone && ladderTransform && !isOnLadder)
            {
                float dotProduct = Vector3.Dot(transform.forward, ladderTransform.forward);

                if (dotProduct > alignmentThreshold)
                {
                    isOnLadder = true;
                    playerInput.playerMovement.isEnabled = false;
                    playerInput.playerMovement.customAction = true;
                    playerInput.playerLook.lockSidewaysRotation = true;

                    if (playerInput.headBobController.startEnabled)
                    {
                        playerInput.headBobController.isEnabled = false;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            LadderMovement();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == ladderTag && playerInput.playerMovement.isInMotion)
            {
                isInsideLadderZone = true;
                ladderTransform = other.transform;
            }


            if (other.gameObject.tag == ladderEndTag)
            {
                playerInput.playerMovement.isEnabled = true;
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == ladderTag)
            {
                ladderTransform = null;
                isInsideLadderZone = false;
                isOnLadder = false;
                playerInput.playerMovement.isEnabled = true;
                playerInput.playerMovement.customAction = false;
                playerInput.playerLook.lockSidewaysRotation = false;

                if (playerInput.headBobController.startEnabled)
                {
                    playerInput.headBobController.isEnabled = true;
                }
            }
        }

        void LadderMovement()
        {
            float z = playerInput.verticalInput;

            if (z != 0 && isOnLadder)
            {
                isClimbing = true;
            }
            else
            {
                isClimbing = false;
            }

            climbDirection = transform.up * z;

            if (isClimbing)
            {
                playerInput.controller.Move(climbDirection * climbSpeed * Time.deltaTime);
            }
            if (z == -1 && playerInput.playerMovement.isGrounded)
            {
                playerInput.playerMovement.isEnabled = true;
                isOnLadder = false;
                playerInput.playerMovement.customAction = false;

            }

        }
    }

}

