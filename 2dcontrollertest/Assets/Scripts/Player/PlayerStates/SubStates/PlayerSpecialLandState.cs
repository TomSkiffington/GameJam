using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpecialLandState : PlayerGroundedState
{
    private float velocityX;
    private float landTime = 0.16667f;
    private bool cancelledAnim = true;
    private float speedMulti = 1;

    public PlayerSpecialLandState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //player.InputHandler.UseDash();

        player.CreateWaveDashDust();

        //playerData.traction = .1f;
        velocityX = core.Movement.CurrentVelocity.x * speedMulti;
    }

    public override void Exit()
    {
        base.Exit();

        if (cancelledAnim) {
            speedMulti += .1f;
        }

        //playerData.traction = .8f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isExitingState) return;

        if (Time.time == startTime + landTime) {
            cancelledAnim = true;
            stateMachine.ChangeState(player.IdleState);
        }
        else if (isAnimationFinished) {

            cancelledAnim = false;
            speedMulti = 1;

            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (isTouchingWall) {
                velocityX = 0;
        }

        core.Movement.reduceByTraction(ref velocityX, false);
        core.Movement.SetVelocityX(velocityX);
    }
}
