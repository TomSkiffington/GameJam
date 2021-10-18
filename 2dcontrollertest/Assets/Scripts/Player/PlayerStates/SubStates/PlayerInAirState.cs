using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    private Vector2 velocity;

    //Inputs
    private int xInput;
    private int yInput;
    private bool jumpInput;
    private bool wallGrabInput;
    private bool jumpInputStop;
    private bool airDodgeInput;

    //Checks
    private bool fastFalled;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool oldIsTouchingWall; //touched wall in previous frame
    private bool isTouchingWallBack;
    private bool oldIsTouchingWallBack;
    private bool isTouchingLedge;

    private bool coyoteTime;
    private bool wallJumpCoyoteTime;
    private bool isJumping;

    private float startWallJumpCoyoteTime;

    public PlayerInAirState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        oldIsTouchingWall = isTouchingWall;
        oldIsTouchingWallBack = isTouchingWallBack;

        isGrounded = core.CollisionSenses.Grounded;
        isTouchingWall = core.CollisionSenses.CheckIfTouchingWall();
        isTouchingWallBack = core.CollisionSenses.CheckIfTouchingWallBack();
        isTouchingLedge = core.CollisionSenses.Ledge;

        if (isTouchingWall && !isTouchingLedge) {
            player.LedgeClimbState.SetDetectedPosition(player.transform.position);
        }

        if (!wallJumpCoyoteTime && !isTouchingWall && !isTouchingWallBack && (oldIsTouchingWall || oldIsTouchingWallBack)) {
            StartWallJumpCoyoteTime();
        }
    }

    public override void Enter()
    {
        base.Enter();

        velocity.x = core.Movement.CurrentVelocity.x;
    }

    public override void Exit()
    {
        base.Exit();

        oldIsTouchingWall = false;
        isTouchingWall = false;
        oldIsTouchingWallBack = false;
        isTouchingWallBack = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CheckCoyoteTime();
        CheckWallJumpCoyoteTime();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        jumpInput = player.InputHandler.JumpInput;
        jumpInputStop = player.InputHandler.JumpInputStop;
        wallGrabInput = player.InputHandler.WallGrabInput;
        airDodgeInput = player.InputHandler.AirDodgeInput;

        if (isGrounded && core.Movement.CurrentVelocity.y < 0.01f) {
            stateMachine.ChangeState(player.LandState);
        }
        else if (isTouchingWall && !isTouchingLedge && !isGrounded) {
            //stateMachine.ChangeState(player.LedgeClimbState);
        }
        else if (airDodgeInput && player.AirDodgeState.CheckIfCanAirDodge()) {
            stateMachine.ChangeState(player.AirDodgeState);
        }
        else if (isTouchingWall) {
                velocity.x = 0;
        }
        else if (jumpInput && (isTouchingWall || wallJumpCoyoteTime)) {
            //StopWallJumpCoyoteTime();
            //isTouchingWall = core.CollisionSenses.CheckIfTouchingWall();
            //player.WallJumpState.DetermineWallJumpDirection(isTouchingWall);
            //stateMachine.ChangeState(player.WallJumpState);
        }
        else if (jumpInput && player.JumpState.CanJump() && !isJumping) {
            //coyoteTime = false;
            //player.JumpState.DecreaseAmountOfJumpsLeft();
            //stateMachine.ChangeState(player.JumpState);
        }
        else if (wallGrabInput && isTouchingWall && xInput == core.Movement.FacingDirection && isTouchingLedge) {
            //stateMachine.ChangeState(player.WallGrabState);
        }
        else if (isTouchingWall && xInput == core.Movement.FacingDirection && core.Movement.CurrentVelocity.y <= 0) {
            //stateMachine.ChangeState(player.WallSlideState);
        }
        else if (yInput == -1 && core.Movement.CurrentVelocity.y < 0 && !fastFalled) {
            core.Movement.SetVelocityY(-playerData.fastFallV);
            fastFalled = true;
        }
        else {

            if (coyoteTime) {
                core.Movement.CheckIfShouldFlip(xInput);
            }

            player.Anim.SetFloat("yVelocity", core.Movement.CurrentVelocity.y);
            player.Anim.SetFloat("xVelocity", Mathf.Abs(core.Movement.CurrentVelocity.x));
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (isTouchingWall || isTouchingWallBack) {
                velocity.x = 0;
        }
            
        core.Movement.airDrift(ref velocity.x, xInput);
        core.Movement.SetVelocityX(velocity.x);
    }

    private void CheckCoyoteTime() {
        if (coyoteTime && Time.time > startTime + playerData.coyoteTime) {
            isJumping = false;
            coyoteTime = false;
            player.JumpState.DecreaseAmountOfJumpsLeft();
        }
    }

    public void StartCoyoteTime() {
        coyoteTime = true;
    }

    private void CheckWallJumpCoyoteTime() {
        if (wallJumpCoyoteTime && Time.time > startWallJumpCoyoteTime + playerData.coyoteTime) {
            coyoteTime = false;
            player.JumpState.DecreaseAmountOfJumpsLeft();
        }
    }

    public void StartWallJumpCoyoteTime() {
        wallJumpCoyoteTime = true;
        startWallJumpCoyoteTime = Time.time;
    }

    public void StopWallJumpCoyoteTime() {
        wallJumpCoyoteTime = false;
    }

    public void SetIsJumping() {
        isJumping = true;
    }

    public void ResetFastFall() {
        fastFalled = false;
    }
}
