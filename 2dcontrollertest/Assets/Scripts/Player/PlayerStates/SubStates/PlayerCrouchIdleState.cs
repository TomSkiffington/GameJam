using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchIdleState : PlayerGroundedState
{

    private float velocityX;

    public PlayerCrouchIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetColliderHeight(playerData.crouchColliderHeight);
        
        velocityX = core.Movement.CurrentVelocity.x;
    }

    public override void Exit()
    {
        base.Exit();

        player.SetColliderHeight(playerData.standColliderHeight);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isExitingState) return;

        /* if(!isGrounded) {
            stateMachine.ChangeState(player.CrouchIdleState);
        } */

        if (yInput != -1 && !isTouchingCeiling) {
            stateMachine.ChangeState(player.IdleState);
        }
        else if (xInput != 0) {
            stateMachine.ChangeState(player.CrouchWalkState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        core.Movement.reduceByTraction(ref velocityX, true);
        core.Movement.SetVelocityX(velocityX);
    }

}
