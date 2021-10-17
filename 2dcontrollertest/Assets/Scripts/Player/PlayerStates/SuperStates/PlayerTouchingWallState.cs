using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouchingWallState : PlayerState
{

    protected bool isTouchingWall;
    protected bool isGrounded;
    protected bool wallGrabInput;
    protected bool jumpInput;
    protected bool isTouchingLedgeTop;
    protected bool isTouchingLedgeMiddle;
    protected int xInput;
    protected int yInput;

    public PlayerTouchingWallState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTouchingWall = core.CollisionSenses.CheckIfTouchingWall();
        isGrounded = core.CollisionSenses.Grounded;
        isTouchingLedgeTop = core.CollisionSenses.Ledge;

        if(isTouchingWall && !isTouchingLedgeTop) {
            player.LedgeClimbState.SetDetectedPosition(player.transform.position);
        }

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        wallGrabInput = player.InputHandler.WallGrabInput;
        jumpInput = player.InputHandler.JumpInput;

        if (jumpInput && !isGrounded) {
            player.WallJumpState.DetermineWallJumpDirection(isTouchingWall);
            stateMachine.ChangeState(player.WallJumpState);
        }
        else if (isGrounded && !wallGrabInput) {
            stateMachine.ChangeState(player.IdleState);
        }
        else if (!isTouchingWall || (xInput != core.Movement.FacingDirection && !wallGrabInput)) {
            stateMachine.ChangeState(player.InAirState);
        }
        else if (isTouchingLedgeMiddle && !isTouchingLedgeTop) {
            stateMachine.ChangeState(player.LedgeClimbState);
        }
    }
}
