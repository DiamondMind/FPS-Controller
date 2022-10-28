using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Prototypes.FPSPlayer 
{
    public class dPlayerInput : MonoBehaviour
    {

        public float mouseInputX { get; private set; }
        public float mouseInputY { get; private set; }
        public float horizontalInput { get; private set; }
        public float verticalInput { get; private set; }
        public bool jumpImput { get; private set; }
        public bool crouchInput { get; private set; }
        public bool sprintInput { get; private set; }

        
        public void Update()
        {
            // Gather inputs variables
            mouseInputX = Input.GetAxis("Mouse X");
            mouseInputY = Input.GetAxis("Mouse Y");

            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            jumpImput = Input.GetButtonDown("Jump");
            crouchInput = Input.GetButton("Crouch");
            sprintInput = Input.GetButton("Sprint");
        }
    }
}
