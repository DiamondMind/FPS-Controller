using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Prototypes.Characters.FPS
{ 
    public class FirstPersonController : MonoBehaviour
    {

        [Header("---------- Stamina ----------")]
        public bool useStamina = true;
        [SerializeField] private float runStamina = 5f;
        [SerializeField] private float jumpStamina = 15f;
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float currentStamina;
        [SerializeField] private float recoveryDelay = 5f;
        [SerializeField] private float recoveryAmount = 5f;

        [Header("---------- SFX ----------")]
        [SerializeField] private AudioSource audioSource;
        [Range(0f, 1f)] public float volume = 0.5f;

        [Header("---------- Footsteps ----------")]
        public bool useFootsteps = true;
        [SerializeField] private float defaultStepMultiplier = 0.5f;
        [SerializeField] private float crouchStepMultiplier = 1.5f;
        [SerializeField] private float runStepMultiplier = 0.5f;
        [SerializeField] private AudioClip changeStanceSfx;
        [SerializeField] private AudioClip jumpSfx;
        [SerializeField] private AudioClip[] defaultSfx;
        [SerializeField] private AudioClip[] woodSfx;
        [SerializeField] private AudioClip[] waterSfx;
        [SerializeField] private AudioClip[] grassSfx;
        [SerializeField] private AudioClip[] metalSfx;

        public bool canRun { get; private set; }
        public bool canJump { get; private set; }

        FirstPersonMovement playerMovement;

        float stepInterval;
        float timer = 0f;
        RaycastHit hitInfo;

        private void Start()
        {
            playerMovement = GetComponent<FirstPersonMovement>();
            audioSource.volume = volume;
            currentStamina = maxStamina;
            canRun = true;
            canJump = true;
        }

        private void Update()
        {
            if (useFootsteps)
            {
                HandleFootSteps();
            }

            if (useStamina)
            {
                HandleStamina();
            }

            stepInterval = playerMovement.isCrouching ? defaultStepMultiplier * crouchStepMultiplier : playerMovement.isRunning ? defaultStepMultiplier * runStepMultiplier : defaultStepMultiplier;

        }

        public void ReduceStamina(float reductionAmount)
        {
            if (useStamina)
            {
                currentStamina -= reductionAmount;
            }
        }

        private void HandleStamina()
        {
            if (playerMovement.isRunning)
            {
                currentStamina -= runStamina * Time.deltaTime;
            }

            if (currentStamina >= jumpStamina)
            {
                canJump = true;
            }
            else
            {
                canJump = false;
            }

            if (currentStamina < maxStamina)
            {
                StartCoroutine(RegenerateStamina());
            }
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

            canRun = currentStamina >= runStamina;
        }


        private IEnumerator RegenerateStamina()
        {
            if (Time.time > playerMovement.jumpDelay)
            {
                yield return new WaitForSeconds(recoveryDelay);
                currentStamina += recoveryAmount * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            }
        }

        private void HandleFootSteps()
        {
            if (!playerMovement.isGrounded || !playerMovement.isInMotion || !playerMovement.isEnabled)
                return;

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 5f))
                {
                    PlayFootstepSound();
                    timer = stepInterval;
                }
            }

        }

        private void PlayFootstepSound()
        {
            if (hitInfo.collider == null)
                return;

            switch (hitInfo.collider.tag)
            {
                case "Footsteps/Wood":
                    audioSource.PlayOneShot(woodSfx[Random.Range(0, woodSfx.Length - 1)]);
                    break;
                case "Footsteps/Metal":
                    audioSource.PlayOneShot(metalSfx[Random.Range(0, metalSfx.Length - 1)]);
                    break;
                case "Footsteps/Grass":
                    audioSource.PlayOneShot(grassSfx[Random.Range(0, grassSfx.Length - 1)]);
                    break;
                case "Footsteps/Water":
                    audioSource.PlayOneShot(waterSfx[Random.Range(0, waterSfx.Length - 1)]);
                    break;
                default:
                    audioSource.PlayOneShot(defaultSfx[Random.Range(0, defaultSfx.Length - 1)]);
                    break;
            }
        }

        public void JumpAction()
        {
            audioSource.PlayOneShot(jumpSfx);
            ReduceStamina(jumpStamina);
        }

        public void PlayLandSfx()
        {
            PlayFootstepSound();
        }

        public void PlayStanceChangeSfx()
        {
            audioSource.PlayOneShot(changeStanceSfx);
        }

    }

}

