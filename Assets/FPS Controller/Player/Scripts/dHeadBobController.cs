using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Characters.FirstPerson
{
    public class dHeadBobController : MonoBehaviour
    {
        dPlayerMovement _playerMovement;
        [SerializeField] Transform camHolder;
        [SerializeField] public bool isEnabled = true;
        [SerializeField] private float walkBobSpeed = 10f;
        [SerializeField] [Range(0.01f, 0.1f)] private float walkMagnitude = 0.05f;
        [SerializeField] private float crouchBobSpeed = 5f;
        [SerializeField] [Range(0.01f, 0.1f)] private float crouchMagnitude = 0.025f;
        [SerializeField] private float sprintBobSpeed = 15f;
        [SerializeField] [Range(0.05f, 0.5f)] private float SprintMagnitude = 0.1f;

        float startYPos;
        float timer;
        public bool startEnabled { get; private set; }

        void Awake()
        {
            startEnabled = isEnabled;   // to know if game started with it enabled

            _playerMovement = GetComponent<dPlayerMovement>();
            startYPos = camHolder.localPosition.y;
        }
        void Update()
        { 
            if(isEnabled)
            {
                DoHeadBob();
            }
        }

        void DoHeadBob()
        {
           
            if (_playerMovement.isGrounded && _playerMovement.moveDirection.magnitude > 0)
            {
                timer += Time.deltaTime * (_playerMovement.isCrouching ? crouchBobSpeed : _playerMovement.isSprinting ? sprintBobSpeed : walkBobSpeed);
                camHolder.localPosition = new Vector3(camHolder.localPosition.x, startYPos + Mathf.Sin(timer) * (_playerMovement.isCrouching ? crouchMagnitude : _playerMovement.isSprinting ? SprintMagnitude : walkMagnitude), camHolder.transform.localPosition.z);
            }
        }
    }
}
