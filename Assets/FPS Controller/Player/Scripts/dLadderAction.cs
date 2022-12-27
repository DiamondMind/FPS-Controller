using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Characters.FirstPerson
{
    public class dLadderAction : MonoBehaviour
    {
        dPlayerMovement _playerMovement;
        dPlayerInput _playerInput;
        dHeadBobController _headBobController;
        CharacterController _controller;

        [SerializeField] public bool isOnLadder { get; private set;}
        public float climbSpeed = 3f;
        public string ladderTag = "Ladder";
        public string ladderEndTag = "ladderEndPoint";


        bool isClimbing;
        Vector3 climbDirection;

        void Start()
        {
            _controller = GetComponent<CharacterController>();
            _playerMovement = GetComponent<dPlayerMovement>();
            _playerInput = GetComponent<dPlayerInput>();
            _headBobController = GetComponent<dHeadBobController>();

            isOnLadder = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == ladderTag)
            {
                isOnLadder = true;
                _playerMovement.isEnabled = false;

                if (_headBobController.startEnabled)
                {
                    _headBobController.isEnabled = false;
                }
            }

            if(other.gameObject.tag == ladderEndTag)
            {
                _playerMovement.isEnabled = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == ladderTag)
            {
                isOnLadder = false;
                _playerMovement.isEnabled = true;

                if (_headBobController.startEnabled)
                {
                    _headBobController.isEnabled = true;
                }
            }
        }

        private void FixedUpdate()
        {
            LadderMovement();
        }

        void LadderMovement()
        {
            float z = _playerInput.verticalInput;

            if (z != 0 && isOnLadder)
            {
                isClimbing = true;
            }
            else
            {
                isClimbing = false;
            }

            climbDirection = transform.up * z;

            if (isClimbing == true)
            {
                _controller.Move(climbDirection * climbSpeed * Time.deltaTime);
                //transform.position += Vector3.up * _playerInput.verticalInput * ladderSpeed * Time.deltaTime;
            }
            if (z == -1 && _controller.isGrounded)
            {
                _playerMovement.isEnabled = true;
                isOnLadder = false;
            }

        }
    }
}
