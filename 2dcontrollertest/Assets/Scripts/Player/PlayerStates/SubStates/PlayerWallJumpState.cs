using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerGroundedAbilityState
{
    private int wallJumpDirection;
    private bool isTouchingWall;
    private float velocityX;

    public PlayerWallJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter() {
        base.Enter();

        player.InputHandler.UseJumpInput();
        //player.JumpState.ResetAmountOfJumpsLeft();      //can double jump after wall jumping

        velocityX = playerData.wallJumpVelocity;
        core.Movement.SetVelocity(playerData.wallJumpVelocity, playerData.wallJumpAngle, wallJumpDirection);
        core.Movement.CheckIfShouldFlip(wallJumpDirection);

        player.JumpState.DecreaseAmountOfJumpsLeft();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
        if (isExitingState) return;


        player.Anim.SetFloat("yVelocity", core.Movement.CurrentVelocity.y);
        player.Anim.SetFloat("xVelocity", Mathf.Abs(core.Movement.CurrentVelocity.x));

        if(Time.time >= startTime + playerData.wallJumpTime) {
            isAbilityDone = true;
        }
    }

    public void DetermineWallJumpDirection(bool isTouchingWall) {
        if (isTouchingWall) {
            wallJumpDirection = -core.Movement.FacingDirection;
        }
        else {
            wallJumpDirection = core.Movement.FacingDirection;
        }
    }
}
