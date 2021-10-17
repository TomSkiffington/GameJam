using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    // Inputs
    protected Vector2 rawMoveInput;
    protected int xInput;
    protected int yInput;
    protected bool dashInputLeft;
    protected bool dashInputRight;
    protected bool walkInput;
    protected bool walkInputStopHold;
    protected bool jumpInput;

    protected Vector2[] rawStickInputBuffer = new Vector2[3];
    protected Queue<Vector2> rawStickInputOld = new Queue<Vector2>(2);

    // Checks
    protected bool isGrounded;
    protected bool isDashing;
    private bool isTouchingWall;
    private bool wallGrabInput;
    private bool isTouchingLedge;
    protected bool isTouchingCeiling;
    
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();

        isTouchingWall = core.CollisionSenses.CheckIfTouchingWall();
        isGrounded = core.CollisionSenses.Grounded;
        isTouchingLedge = core.CollisionSenses.Ledge;
        isTouchingCeiling = core.CollisionSenses.Ceiling;
        isDashing = player.InputHandler.Dashing;
    }
    public override void Enter() {
        base.Enter();

        player.JumpState.ResetAmountOfJumpsLeft();
        player.AirDodgeState.ResetCanAirdodge();
        player.InAirState.ResetFastFall();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        rawMoveInput = player.InputHandler.RawMovementInput;
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        dashInputLeft = player.InputHandler.DashInputLeft;
        dashInputRight = player.InputHandler.DashInputRight;
        jumpInput = player.InputHandler.JumpInput;
        wallGrabInput = player.InputHandler.WallGrabInput;
        walkInput = player.InputHandler.WalkInput;
        walkInputStopHold = player.InputHandler.WalkInputStop;


        if (player.InputHandler.AttackInputs[(int)CombatInputs.primary] && player.playerEquipment.Container.Items[1].item.id != -1) {
            stateMachine.ChangeState(player.PrimaryAttackState);
        }
        else if (player.InputHandler.AttackInputs[(int)CombatInputs.secondary]) {
            stateMachine.ChangeState(player.SecondaryAttackState);
        }
        else if (jumpInput && player.JumpState.CanJump() && !isTouchingCeiling) {
            stateMachine.ChangeState(player.JumpSquatState);
        }
        else if (!isGrounded) {
            player.InAirState.StartCoyoteTime();
            stateMachine.ChangeState(player.InAirState);
        }
        else if (isTouchingWall && wallGrabInput && isTouchingLedge) {
            stateMachine.ChangeState(player.WallGrabState);
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();

        if (rawStickInputOld.Count == 3) {
            rawStickInputOld.Dequeue();
        }

        while (rawMoveInput.x != 0 && rawStickInputOld.Count <= 2) {
            rawStickInputOld.Enqueue(rawMoveInput);
        }
    }

    protected Vector2[] GetPreviousFramesXStickValues() {
        rawStickInputOld.CopyTo(rawStickInputBuffer, 0);

        if (rawStickInputBuffer[0].x != 0) 
        Debug.Log(rawStickInputBuffer[0].x + " " + rawStickInputBuffer[1].x + " " + rawStickInputBuffer[2].x);

        return rawStickInputBuffer;
    }
}
