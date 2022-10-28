using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DiamondMind.Prototypes.FPSPlayer
{
    public class dMouseLook : MonoBehaviour
    {
        [SerializeField] private dPlayerInput _playerInput;
        [SerializeField] private Transform player;
        [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private float maxYRotation = 90f; 
        [SerializeField] private bool lockCursor = true;
        float xRotation = 0f;
       
        void Awake()
        {
             //lock the cursor
            if(lockCursor == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
            } 
        }
        void Update()
        {
            // Gather mouse imput and store in a float variable
            float mouseX = _playerInput.mouseInputX * mouseSensitivity * Time.deltaTime;
            float mouseY = _playerInput.mouseInputY * mouseSensitivity * Time.deltaTime;
           
            // Rotate player along the x axis
            player.transform.Rotate(Vector3.up * mouseX);
            // decrease x rotation by mouseY every frame
            xRotation -= mouseY;    
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);    // this is done instead of transform.rotate so we can clamp the rotation
            // clamp xRotation to -90, 90
            xRotation = Mathf.Clamp(xRotation, -maxYRotation, maxYRotation);   
        }
    }
}
