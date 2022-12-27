using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DiamondMind.Characters.FirstPerson
{
    public class dMouseLook : MonoBehaviour
    {
        [SerializeField] private dPlayerInput _playerInput;

        [SerializeField] private Transform player;
        [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private float maxYRotation = 90f; 

        float mouseX;
        float mouseY;
        float xRotation = 0f;
               
        void FixedUpdate()
        {
            mouseX = _playerInput.mouseInputX * mouseSensitivity * Time.deltaTime;
            mouseY = _playerInput.mouseInputY * mouseSensitivity * Time.deltaTime;
           
            // Rotate player along the x axis
            player.transform.Rotate(Vector3.up * mouseX);
            // decrease x rotation by mouseY every frame
            xRotation -= mouseY;    
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);    

            xRotation = Mathf.Clamp(xRotation, -maxYRotation, maxYRotation);   
        }
    }
}
