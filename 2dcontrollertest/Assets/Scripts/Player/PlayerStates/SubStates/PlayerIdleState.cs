using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    
    private float velocityX;
    private Vector2[] previousStickInputs;
    
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
        
    }

    public override void Enter() {
        base.Enter();

        
        
        velocityX = core.Movement.CurrentVelocity.x;
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        if (isExitingState) return;

        // if (dashInputRight && isGrounded && velocityX != 0) {
        //     player.DashState.SetDashDirection(1);
        //     core.Movement.SetVelocityZero();
        //     stateMachine.ChangeState(player.DashState);
        // }
        // else if (dashInputLeft && isGrounded && velocityX != 0) {
        //     player.DashState.SetDashDirection(-1);
        //     core.Movement.SetVelocityZero();
        //     stateMachine.ChangeState(player.DashState);
        // } 

        previousStickInputs = GetPreviousFramesXStickValues();

        if (xInput != 0 && (previousStickInputs[0].x * player.Core.Movement.FacingDirection) >= 0.7f && (previousStickInputs[1].x * player.Core.Movement.FacingDirection) <= 0.3f) {
            player.DashState.SetDashDirection(player.Core.Movement.FacingDirection);
            core.Movement.SetVelocityZero();

            stateMachine.ChangeState(player.DashState);
        }

        if (xInput != 0 && isGrounded && !isDashing) {
            stateMachine.ChangeState(player.RunState);
        }
        else if (yInput == -1) {
            stateMachine.ChangeState(player.CrouchIdleState);
        }

        
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();



        core.Movement.reduceByTraction(ref velocityX, false);
        core.Movement.SetVelocityX(velocityX);
    }
}
