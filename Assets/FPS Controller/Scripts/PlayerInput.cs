using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Prototypes.Characters.FPS
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private bool lockCursor = true;
        [Header("---------- Keybinds ----------")]
        [SerializeField] private KeyCode JumpInput = KeyCode.Space;
        [SerializeField] private KeyCode CrouchInput = KeyCode.LeftAlt;
        [SerializeField] private KeyCode SprintInput = KeyCode.Tab;
        [SerializeField] private KeyCode InteractInput = KeyCode.E;

        public float mouseInputX { get; private set; }
        public float mouseInputY { get; private set; }
        public float horizontalInput { get; private set; }
        public float verticalInput { get; private set; }
        public bool jumpInput { get; private set; }
        public bool crouchInput { get; private set; }
        public bool sprintInput { get; private set; }
        public bool interactInput { get; private set; }


        // storing all relevant components here to prevent using GetComponent() multiple times 
        public FirstPersonMovement playerMovement { get; private set; }
        public HeadBobController headBobController { get; private set; }
        public CharacterController controller { get; private set; }
        public PlayerLook playerLook { get; private set; }
        public FirstPersonController playerController { get; private set; }

        private void Awake()
        {
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            controller = GetComponent<CharacterController>();
            playerMovement = GetComponent<FirstPersonMovement>();
            headBobController = GetComponent<HeadBobController>();
            playerController = GetComponent<FirstPersonController>();
            playerLook = FindObjectOfType<PlayerLook>();
        }

        public void Update()
        {
            // Gather mouse inputs
            mouseInputX = Input.GetAxis("Mouse X");
            mouseInputY = Input.GetAxis("Mouse Y");

            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            jumpInput = Input.GetKeyDown(JumpInput);
            crouchInput = Input.GetKeyDown(CrouchInput);
            interactInput = Input.GetKeyDown(InteractInput);
            sprintInput = Input.GetKey(SprintInput);

        }
    }

}
