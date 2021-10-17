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
    public Vector2 RawPrimaryAttackDirectionInput { get; private set; }
    public Vector2 AirDodgeDirectionInput { get; private set; }

    public Vector2 MousePosition { get; private set; }

    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }

    public bool WalkInput { get; private set; }
    public bool WalkInputStop { get; private set; }
    public bool DashInputLeft { get; private set; }
    public bool DashInputRight { get; private set; }
    public bool Dashing { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpInputStop { get; private set; }
    public bool WallGrabInput { get; private set; }
    public bool AirDodgeInput { get; private set; }
    public bool AirDodgeInputStop { get; private set; }

    public bool ShowInventory { get; private set; }

    public bool[] AttackInputs { get; private set; }
    public bool[] AttackInputDirection { get; private set; }

    [SerializeField]
    private float jumpHoldTime = 0.2f;

    private float jumpInputStartTime;
    private float airDodgeInputStartTime;

    //private float prevInput;

    private void Start() {
        playerInput = GetComponent<PlayerInput>();

        int atkCount = Enum.GetValues(typeof(CombatInputs)).Length;
        AttackInputs = new bool[atkCount];

        AttackInputDirection = new bool[4];

        cam = Camera.main;
    }

    private void Update() {
        CheckJumpInputHoldTime();
        CheckAirDodgeInputHoldTime();
    }

    public void OnPrimaryAttackInput(InputAction.CallbackContext context) {
        if (context.started) {
            AttackInputs[(int)CombatInputs.primary] = true; //casted as int so the number assigned to primary is returned not the enum called CombatInputs
        }

        if (context.canceled) {
            AttackInputs[(int)CombatInputs.primary] = false;
        }

    }

    public void OnPrimaryAttackDirectionInput(InputAction.CallbackContext context) {

        RawPrimaryAttackDirectionInput = context.ReadValue<Vector2>();

        if (playerInput.currentControlScheme == "Keyboard") {
            RawPrimaryAttackDirectionInput = cam.ScreenToWorldPoint((Vector3)RawPrimaryAttackDirectionInput) - transform.position;
        }

        float xDirection = Mathf.Abs(RawPrimaryAttackDirectionInput.x);
        float yDirection = Mathf.Abs(RawPrimaryAttackDirectionInput.y);

        if (yDirection > xDirection && RawPrimaryAttackDirectionInput.y >= 0) {
            AttackInputDirection[(int)AttackDirection.up] = true;

            AttackInputDirection[(int)AttackDirection.down] = false;
            AttackInputDirection[(int)AttackDirection.left] = false;
            AttackInputDirection[(int)AttackDirection.right] = false;
        }
        else if (yDirection > xDirection && RawPrimaryAttackDirectionInput.y < 0) {
            AttackInputDirection[(int)AttackDirection.down] = true;

            AttackInputDirection[(int)AttackDirection.up] = false;
            AttackInputDirection[(int)AttackDirection.left] = false;
            AttackInputDirection[(int)AttackDirection.right] = false;
        }
        else if (yDirection < xDirection && RawPrimaryAttackDirectionInput.x >= 0) {
            AttackInputDirection[(int)AttackDirection.right] = true;

            AttackInputDirection[(int)AttackDirection.up] = false;
            AttackInputDirection[(int)AttackDirection.down] = false;
            AttackInputDirection[(int)AttackDirection.left] = false;
        }
        else if (yDirection < xDirection && RawPrimaryAttackDirectionInput.x < 0) {
            AttackInputDirection[(int)AttackDirection.left] = true;
            
            AttackInputDirection[(int)AttackDirection.up] = false;
            AttackInputDirection[(int)AttackDirection.down] = false;
            AttackInputDirection[(int)AttackDirection.right] = false;
        }
    }

    public void OnSecondaryAttackInput(InputAction.CallbackContext context) {
        if (context.started) {
            AttackInputs[(int)CombatInputs.secondary] = true;
        }

        if (context.canceled) {
            AttackInputs[(int)CombatInputs.secondary] = false;
        }
        
    }

    public void OnMoveInput(InputAction.CallbackContext context) {
        RawMovementInput = context.ReadValue<Vector2>();

        NormInputX = Mathf.RoundToInt(RawMovementInput.x);
        NormInputY = Mathf.RoundToInt(RawMovementInput.y);
    }

    public void OnMouseMove(InputAction.CallbackContext context) {
        MousePosition = cam.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    public void OnDashInputLeft(InputAction.CallbackContext context) {

        Dashing = true;

        // float xValue = context.ReadValue<float>();

        // Debug.Log(xValue);

        if (!context.performed) {
            Dashing = false;
            //DashInputLeft = false;
            return;
        }

        if (context.interaction is MultiTapInteraction) {
            DashInputLeft = true;
        }

        //prevInput = xValue;
    }

    public void OnDashInputRight(InputAction.CallbackContext context) {

        Dashing = true;

        if (!context.performed) {
            Dashing = false;
            //DashInputLeft = false;
            return;
        }

        if (context.interaction is MultiTapInteraction) {
            DashInputRight = true;
        }
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

    public void OnInventoryInput(InputAction.CallbackContext context) {
        if(context.started) {
            ShowInventory = !ShowInventory;
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

        if (playerInput.currentControlScheme == "Keyboard") {
            MousePosition = cam.ScreenToWorldPoint(context.ReadValue<Vector2>());
            RawAirDodgeDirectionInput = cam.ScreenToWorldPoint((Vector3)RawAirDodgeDirectionInput) - transform.position;
        }
    }

    public void UseAirDodge() {
        AirDodgeInput = false;
    }

    public void UseJumpInput() {
        JumpInput = false;
    }

    public void UseDash() {
        DashInputRight = false;
        DashInputLeft = false;
    }

    public void DashDone() {
        Dashing = false;
    }

    private void CheckJumpInputHoldTime() {
        if (Time.time >= jumpInputStartTime + jumpHoldTime)
        {
            JumpInput = false;
        }
    }

    private void CheckAirDodgeInputHoldTime() {
        if (Time.time >= airDodgeInputStartTime + jumpHoldTime)
        {
            AirDodgeInput = false;
        }
    }
}

public enum CombatInputs {
    primary,
    secondary
}

public enum AttackDirection {
    up,
    down,
    left,
    right
}
