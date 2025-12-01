using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    public PlayerInput.OnfootActions onFoot;

    private PlayerMovement movement;
    private PlayerLook look;

    [Header("Pause UI")]
    public GameObject pauseMenuUI;      // Assign your pause panel in inspector
    private bool isPaused = false;

    private void Start()
    {
        Time.timeScale = 1.0f;
    }
    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.Onfoot;

        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();
        
        //calls jump function from playerMovement when jump is performed and same for crouch and sprint
        onFoot.Jump.performed += ctx => movement.Jump();
        onFoot.Crouch.performed += ctx => movement.Crouch();
        onFoot.Sprint.started += ctx => movement.SetSprinting(true);
        onFoot.Sprint.canceled += ctx => movement.SetSprinting(false);
        onFoot.Pause.performed += OnPausePerformed;
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        // show UI
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);

        // stop time
        Time.timeScale = 0f;

        // unlock cursor for menu interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;

        // optional: disable player inputs that shouldn't work in pause
        // e.g. movement and look can be ignored by checking isPaused in those scripts
    }

    public void Resume()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }

    private void Crouch_performed(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //tell playerMovement to move using the value of our movement action.
        movement.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }
    private void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }
    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}
