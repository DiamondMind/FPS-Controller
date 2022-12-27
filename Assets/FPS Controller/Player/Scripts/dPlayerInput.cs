using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Characters.FirstPerson
{
    public class dPlayerInput : MonoBehaviour
    {

        public float mouseInputX { get; private set; }
        public float mouseInputY { get; private set; }
        public float horizontalInput { get; private set; }
        public float verticalInput { get; private set; }
        public bool jumpInput { get; private set; }
        public bool crouchInput { get; private set; }
        public bool sprintInput { get; private set; }
        public bool interactInput { get; private set; }

        [Header("Lock Cursor")]
        [SerializeField] private bool lockCursor = true;
        [Header("Keybinds")]
        [SerializeField] private KeyCode JumpInput = KeyCode.Space;
        [SerializeField] private KeyCode CrouchInput = KeyCode.LeftAlt;
        [SerializeField] private KeyCode SprintInput = KeyCode.Tab;
        [SerializeField] private KeyCode InteractInput = KeyCode.E;

        void Awake()
        {
            //lock the cursor
            if (lockCursor == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        public void Update()
        {
            // Gather inputs variables
            mouseInputX = Input.GetAxis("Mouse X");
            mouseInputY = Input.GetAxis("Mouse Y");

            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            jumpInput = Input.GetKeyDown(JumpInput);
            crouchInput = Input.GetKeyDown(CrouchInput);
            sprintInput = Input.GetKey(SprintInput);
            interactInput = Input.GetKeyDown(InteractInput);
        }
    }
}
