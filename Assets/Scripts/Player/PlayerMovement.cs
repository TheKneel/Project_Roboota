using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool lerpCrouch;
    private bool isSprinting;
    public bool IsGrounded => isGrounded;
    public float GetVerticalVelocity() => playerVelocity.y;

    public float gravity = -9.8f;
    public float speed = 1f;
    public float jumpHeight = 0.5f;
    public float crouchTimer;

    private float originalSpeed;
    private PlayerInput input;

    [Header("Footstep Audio")]
    [SerializeField] AudioSource walkingSound;
    public float movementThreshold = 0.01f;   // how much input counts as "moving"
    private Vector2 lastMoveInput;

    [Header("Sprint Settings")]
    public float walkSpeed = 1f;
    public float sprintSpeed = 5f;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        originalSpeed = speed;
        input = new PlayerInput();

        // Ensure audio source is configured
        if (walkingSound != null)
        {
            walkingSound.playOnAwake = false;
            walkingSound.loop = true;
            walkingSound.pitch = 1f; // default for walking
        }
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;

        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1f;
            p *= p;

            if (isCrouching)
                controller.height = Mathf.Lerp(controller.height, 1, p);
            else
                controller.height = Mathf.Lerp(controller.height, 2, p);

            if (p > 1)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }

        HandleFootstepLoop();
    }

    //receive inputs from our InputManager.cs script and apply it to character controller
    public void ProcessMove(Vector2 input)
    {
        lastMoveInput = input; //stores movement for audio

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        // apply speed (this uses the 'speed' field like before)
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;

        //keeps playerVelocity values to -2
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    public void Crouch()
    {
        isCrouching = !isCrouching;
        crouchTimer = 0f;
        lerpCrouch = true;
    }

    // This method is called by your InputManager when sprint starts/stops
    public void SetSprinting(bool isPressed)
    {
        isSprinting = isPressed;

        // keep speed field behavior similar to what you had before
        if (isPressed)
        {
            speed = sprintSpeed; // sprint speed
        }
        else
        {
            speed = walkSpeed; // normal walk speed
        }
    }

    public float GetCurrentSpeed()
    {
        return speed;
    }
    public Vector3 GetVelocity()
    {
        return controller.velocity;
    }

    // ---------------- Footstep handling ----------------
    private void HandleFootstepLoop()
    {
        if (walkingSound == null) return;

        bool isMoving = lastMoveInput.magnitude > movementThreshold;
        bool shouldPlay = isGrounded && isMoving && !isCrouching; // don't play when in air or crouched (optional)

        if (shouldPlay)
        {
            // set pitch depending on sprint state
            float targetPitch = isSprinting ? 2.0f : 1.0f; // double speed when sprinting
            if (!walkingSound.isPlaying)
            {
                walkingSound.pitch = targetPitch;
                walkingSound.Play();
            }
            else
            {
                // if already playing but sprint state changed, update pitch immediately
                if (Mathf.Abs(walkingSound.pitch - targetPitch) > 0.01f)
                    walkingSound.pitch = targetPitch;
            }
        }
        else
        {
            if (walkingSound.isPlaying)
                walkingSound.Stop();
        }
    }
}
