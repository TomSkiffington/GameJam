using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerGroundedState
{
    private float velocityX;
    private float oldVelocityX;
    
    public PlayerRunState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
        
    }

    public override void Enter() {
        base.Enter();

        if (Mathf.Abs(velocityX) >= playerData.runSpeed) {
            velocityX = playerData.runSpeed * xInput;
        }
        else {
            velocityX = core.Movement.CurrentVelocity.x;
        }
        

        /* if(Mathf.Sign(oldVelocityX) != xInput) {
            velocityX *= playerData.turnAroundMomentumModifier;
        } */
    }

    public override void Exit() {
        base.Exit();

        oldVelocityX = velocityX;
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
        if (isExitingState) return;

        core.Movement.CheckIfShouldFlip(xInput);

        if(xInput == 0) {
            stateMachine.ChangeState(player.IdleState);
        }
        else if(dashInputRight && isGrounded && !isDashing) {
            player.DashState.SetDashDirection(1);
            stateMachine.ChangeState(player.DashState);
            velocityX = 0;
        }
        else if(dashInputLeft && isGrounded && !isDashing) {
            player.DashState.SetDashDirection(-1);
            stateMachine.ChangeState(player.DashState);
            velocityX = 0;
        }
        else if (yInput == -1) {
            stateMachine.ChangeState(player.CrouchWalkState);
        } 
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();

        float tempMax = xInput * playerData.runSpeed;

        float tempAcc = ((playerData.maxDashSpeed * Mathf.Sign(xInput)) - velocityX) * (1 / (playerData.maxDashSpeed * 2.5f)) * (playerData.dAccA + playerData.dAccB);

        if ((tempMax > 0 && velocityX > tempMax) || (tempMax < 0 && velocityX < tempMax)) {
            velocityX = tempMax;
        }
        else if(0 < Mathf.Abs(velocityX) && Mathf.Abs(velocityX) < playerData.walkSpeed) {
            velocityX = playerData.walkSpeed * xInput;
        }
        else if (walkInput && !walkInputStopHold && isGrounded) {
            velocityX = playerData.walkSpeed * xInput;
        }
        else {
            if (xInput == 0) {
                core.Movement.reduceByTraction(ref velocityX, false);
            }
        }
        velocityX += tempAcc;

        core.Movement.SetVelocityX(velocityX);
    }
}
