using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpecialLandState : PlayerGroundedState
{
    private float velocityX;
    private float landTime = 0.16667f;

   

    private bool isCancelled = true;



    public PlayerSpecialLandState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        //player.InputHandler.UseDash();
        isCancelled = true;
        player.CreateWaveDashDust();
        velocityX = core.Movement.CurrentVelocity.x;
    }

    public override void Exit()
    {
        if(isCancelled){
            if(playerData.traction > 0){
                playerData.traction = playerData.traction * -2f;
            }
            else if(playerData.traction >= -.6f){
                playerData.traction = playerData.traction * 2f;
            }
            
            
        }

        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isExitingState) return;

        if (xInput != 0 && Time.time >= startTime + landTime) {
            //player.CheckIfShouldFlip();
            //core.Movement.SetVelocityZero();
            stateMachine.ChangeState(player.RunState);
           
        }
        else if (isAnimationFinished) {

            stateMachine.ChangeState(player.IdleState);
            isCancelled = false;
            playerData.traction = 0.2f;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        core.Movement.reduceByTraction(ref velocityX, false);
        core.Movement.SetVelocityX(velocityX);
    }
}
