using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerGroundedState
{
    
    private float dashTimer;
    private float velocityX;
    private int timer;
    private int dashDirection;
    private int oldDashDirection;
    //private float minTimeBetweenDashes = 0.5f;
    
    public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
        
    }

    public override void DoChecks() {
        base.DoChecks();
    }
    public override void Enter() {
        base.Enter();

        player.CreateDust();

        if (core.Movement.FacingDirection != dashDirection) {
            core.Movement.Flip();
        }

        
        player.InputHandler.UseDash();

        dashTimer = 0;
        timer = 0;

        velocityX = playerData.iniDashSpeed * dashDirection;
        //player.CheckIfShouldFlip();
        //velocityX = player.CurrentVelocity.x;
    }

    public override void Exit() {
        base.Exit();

        player.InputHandler.DashDone();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
        if (isExitingState) return;

        dashTimer += Time.deltaTime;

        
        if(xInput != 0 && isAnimationFinished && dashTimer >= playerData.iniDashTime) {
            stateMachine.ChangeState(player.RunState);
        }
        else if(player.JumpSquatState.CheckIfInJumpSquat()) {
            stateMachine.ChangeState(player.JumpSquatState);
        }
        else if(xInput == 0 && isAnimationFinished && dashTimer >= playerData.iniDashTime) {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();

        timer++;

        if (timer == 2) {
            velocityX = playerData.iniDashSpeed * dashDirection;
            if (Mathf.Abs(velocityX) > playerData.maxDashSpeed) {
                velocityX = playerData.maxDashSpeed * xInput;
            }
        }
        if (timer > 1) {
            if (Mathf.Abs(xInput) == 0) {
                core.Movement.reduceByTraction(ref velocityX, false);
            }
            else {
                float tempMax = xInput * playerData.iniDashSpeed;
                float tempAcc = xInput * playerData.dAccA;

                velocityX += tempAcc;

                if ((tempMax > 0 && velocityX > tempMax) || (tempMax < 0 && velocityX < tempMax)) {
                    core.Movement.reduceByTraction(ref velocityX, false);
                    if ((tempMax > 0 && velocityX < tempMax) || (tempMax < 0 && velocityX > tempMax)) {
                        velocityX = tempMax;
                    }
                }
                else {
                    velocityX += tempAcc;
                    if ((tempMax > 0 && velocityX > tempMax) || (tempMax < 0 && velocityX < tempMax)) {
                        velocityX = tempMax;
                    }

                }
            }
        }
        core.Movement.SetVelocityX(velocityX);
    }

    public void SetDashDirection(int direction) {
        dashDirection = direction;
    }
}
