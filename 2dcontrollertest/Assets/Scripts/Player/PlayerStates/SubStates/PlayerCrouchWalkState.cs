using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchWalkState : PlayerGroundedState
{

    private float velocityXSmoothing;
    private float velocityX;

    public PlayerCrouchWalkState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        velocityX = core.Movement.CurrentVelocity.x;
        player.SetColliderHeight(playerData.crouchColliderHeight);
    }

    public override void Exit()
    {
        base.Exit();

        velocityXSmoothing = 0;
        player.SetColliderHeight(playerData.standColliderHeight);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isExitingState) return;

        float targetVelocityX = xInput * playerData.crouchWalkSpeed;
        core.Movement.CheckIfShouldFlip(xInput);
        velocityX = Mathf.SmoothDamp (velocityX, targetVelocityX, ref velocityXSmoothing, .01f);
        core.Movement.SetVelocityX(velocityX);

        if(xInput == 0) {
            stateMachine.ChangeState(player.CrouchIdleState);
        }
        else if (yInput != -1 && !isTouchingCeiling && !brokenLegs) {
            stateMachine.ChangeState(player.RunState);
        }
    }

}
