using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerInputHandler : MonoBehaviour
{

    private PlayerInput playerInput;
    private Camera cam;

    public Vector2 RawMovementInput { get; private set; }
    public Vector2 RawAirDodgeDirectionInput { get; private set; }
    //public Vector2 AirDodgeDirectionInput { get; private set; }

    public Vector2 MousePosition { get; private set; }

    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }

    public bool WalkInput { get; private set; }
    public bool WalkInputStop { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpInputStop { get; private set; }
    public bool WallGrabInput { get; private set; }
    public bool AirDodgeInput { get; private set; }
    public bool AirDodgeInputStop { get; private set; }

    [SerializeField]
    private float jumpHoldTime = 0.2f;

    private float jumpInputStartTime;
    private float airDodgeInputStartTime;

    //private float prevInput;

    private void Start() {
        playerInput = GetComponent<PlayerInput>();

        cam = Camera.main;
    }

    private void Update() {
        CheckJumpInputHoldTime();
        CheckAirDodgeInputHoldTime();
    }

    public void OnMoveInput(InputAction.CallbackContext context) {
        RawMovementInput = context.ReadValue<Vector2>();

        NormInputX = Mathf.RoundToInt(RawMovementInput.x);
        NormInputY = Mathf.RoundToInt(RawMovementInput.y);
    }

    public void OnMouseMove(InputAction.CallbackContext context) {
        MousePosition = cam.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    public void OnJumpInput(InputAction.CallbackContext context) {
        if (context.started) {
            JumpInput = true;
            JumpInputStop = false;
            jumpInputStartTime = Time.time;
        }

        if(context.canceled) {
            JumpInputStop = true;
        }
    }

    public void OnWallGrabInput(InputAction.CallbackContext context) {
        if (context.started) {
            WallGrabInput = true;
        }
        if (context.canceled) {
            WallGrabInput = false;
        }
    }

    public void OnAirDodgeInput(InputAction.CallbackContext context) {
        if(context.started) {
            AirDodgeInput = true;
            AirDodgeInputStop = false;
            airDodgeInputStartTime = Time.time;
        }
        if (context.canceled) {
            AirDodgeInputStop = true;
        }
    }

    public void OnWalkInput(InputAction.CallbackContext context) {
        if(context.started) {
            WalkInput = true;
            WalkInputStop = false;
        }
        if (context.canceled) {
            WalkInputStop = true;
        }
    }

    public void OnAirDodgeDirectionInput(InputAction.CallbackContext context) {
        RawAirDodgeDirectionInput = context.ReadValue<Vector2>();

        //if (playerInput.currentControlScheme == "Keyboard") {
            MousePosition = cam.ScreenToWorldPoint(context.ReadValue<Vector2>());
            RawAirDodgeDirectionInput = cam.ScreenToWorldPoint((Vector3)RawAirDodgeDirectionInput) - transform.position;
        //}
    }

    public void UseAirDodge() {
        AirDodgeInput = false;
    }

    public void UseJumpInput() {
        JumpInput = false;
    }

    private void CheckJumpInputHoldTime() {
        if (Time.time >= jumpInputStartTime + jumpHoldTime)
        {
            JumpInput = false;
        }
    }

    private void CheckAirDodgeInputHoldTime() {
        if (Time.time >= airDodgeInputStartTime + .1f)
        {
            AirDodgeInput = false;
        }
    }
}
