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
    protected bool walkInput;
    protected bool walkInputStopHold;
    protected bool jumpInput;

    // Checks
    protected bool isGrounded;
    private bool isTouchingWall;
    private bool wallGrabInput;
    private bool isTouchingLedge;
    protected bool isTouchingCeiling;
    protected bool brokenLegs = true;  //cant walk
    
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();

        isTouchingWall = core.CollisionSenses.CheckIfTouchingWall();
        isGrounded = core.CollisionSenses.Grounded;
        isTouchingLedge = core.CollisionSenses.Ledge;
        isTouchingCeiling = core.CollisionSenses.Ceiling;
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
        jumpInput = player.InputHandler.JumpInput;
        wallGrabInput = player.InputHandler.WallGrabInput;
        walkInput = player.InputHandler.WalkInput;
        walkInputStopHold = player.InputHandler.WalkInputStop;

        if (jumpInput && player.JumpState.CanJump() && !isTouchingCeiling) {
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
    }
}
