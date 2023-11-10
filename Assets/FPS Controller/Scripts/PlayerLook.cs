using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Prototypes.Characters.FPS
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float maxYRotation = 90f;
        [SerializeField] private float rotationSmoothTime = 0.12f;
        public bool lockSidewaysRotation;

        PlayerInput playerInput;

        Vector2 rotation = Vector2.zero;
        Vector2 currentRotation;
        Vector2 rotationVelocity;

        private void Start()
        {
            playerInput = FindObjectOfType<PlayerInput>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Initialize currentRotation with the player's initial rotation
            rotation = new Vector2(player.localRotation.eulerAngles.y, 0f);
            currentRotation = rotation;
        }

        private void Update()
        {
            MouseInput();
        }

        private void MouseInput()
        {
            rotation.x += playerInput.mouseInputX * mouseSensitivity;
            rotation.y -= playerInput.mouseInputY * mouseSensitivity;



            rotation.y = Mathf.Clamp(rotation.y, -maxYRotation, maxYRotation);

            currentRotation = Vector2.SmoothDamp(currentRotation, rotation, ref rotationVelocity, rotationSmoothTime);
            if (!lockSidewaysRotation)
            {
                player.localRotation = Quaternion.Euler(0f, currentRotation.x, 0f);
            }
            transform.localRotation = Quaternion.Euler(currentRotation.y, 0f, 0f);
        }

    }
}
