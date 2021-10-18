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

        if (isTouchingWall) {
                velocityX = 0;
        }
        
        velocityX = core.Movement.CurrentVelocity.x;
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        if (isExitingState) return;

        if (xInput != 0 && isGrounded && !brokenLegs) {
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
