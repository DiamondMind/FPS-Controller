using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Prototypes.Characters.FPS
{
    public class PlayerClimb : MonoBehaviour
    {
        public bool debugMode;

        [Range(0.1f, 1f), SerializeField] private float checkHeight = 0.2f;
        [Range(0.1f, 1f), SerializeField] private float checkDistance = 0.5f;
        [SerializeField] private string climbTag = "ClimbObject";
        [SerializeField, Range(0.1f, 1f)] private float climbSmooth = 0.25f;
        [Tooltip("Total value is this plus checkHeight")]
        [Range(0.3f, 3f), SerializeField] private float maxClimbHight = 1.5f;


        PlayerInput playerInput;
        RaycastHit hitInfo;
        bool isClimbing;
        float heightToReach;

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
        }

        private void FixedUpdate()
        {
            if((playerInput.interactInput || playerInput.playerMovement.isRunning) && ValidObstacleInFront() && CanClimbObstacle() && !isClimbing)
            {
                ClimbObstacle();
            }

        }

        private bool ValidObstacleInFront()
        {
            Vector3 origin = transform.position + Vector3.up * checkHeight;
            Vector3 direction = transform.TransformDirection(Vector3.forward);

            if (Physics.Raycast(origin, direction, out hitInfo, checkDistance))
            {
                if (hitInfo.collider.CompareTag(climbTag))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CanClimbObstacle()
        {
            if (hitInfo.collider == null)
                return false;

            heightToReach = transform.position.y + (hitInfo.collider.bounds.size.y - hitInfo.point.y) + checkHeight;

            if (heightToReach <= maxClimbHight + checkHeight)
                return true;
            else
                return false;

        }

        private void ClimbObstacle()
        {
            isClimbing = true;
            playerInput.playerMovement.isEnabled = false;
            playerInput.playerMovement.customAction = true;
            playerInput.playerLook.lockSidewaysRotation = true;

            if (playerInput.headBobController.startEnabled)
            {
                playerInput.headBobController.isEnabled = false;
            }

            StartCoroutine(MovePlayerToHeight());
        }
        private IEnumerator MovePlayerToHeight()
        {
            float elapsedTime = 0f;
            Vector3 initialPos = transform.position;
            Vector3 targetPos = transform.position + new Vector3(0f, heightToReach, 0f);

            while (elapsedTime < climbSmooth / 2f)
            {
                float smoothStep = Mathf.SmoothStep(0f, 1f, elapsedTime / (climbSmooth / 2f));  // for smoother interpolation
                Vector3 newPosition = Vector3.Lerp(initialPos, targetPos, smoothStep);

                playerInput.controller.Move(newPosition - transform.position);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPos;
            StartCoroutine(MovePlayerForward());
        }

        private IEnumerator MovePlayerForward()
        {
            float elapsedTime = 0f;
            Vector3 initialPos = playerInput.controller.transform.position;
            Vector3 targetPos = initialPos + transform.forward * (playerInput.controller.radius * 4f);

            while (elapsedTime < climbSmooth / 2f)
            {
                float smoothStep = Mathf.SmoothStep(0f, 1f, elapsedTime / (climbSmooth / 2f));  // for smoother interpolation
                Vector3 newPosition = Vector3.Lerp(initialPos, targetPos, smoothStep);

                playerInput.controller.Move(newPosition - transform.position);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPos;
            FinishClimbing();
        }

        private void FinishClimbing()
        {
            isClimbing = false;
            playerInput.playerMovement.isEnabled = true;
            playerInput.playerMovement.customAction = false;
            playerInput.playerLook.lockSidewaysRotation = false;

            if (playerInput.headBobController.startEnabled)
            {
                playerInput.headBobController.isEnabled = true;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!debugMode)
                return;

            Vector3 origin = transform.position + Vector3.up * checkHeight;
            Vector3 direction = transform.TransformDirection(Vector3.forward);

            Debug.DrawRay(origin, direction, Color.red);
        }
    }
}
