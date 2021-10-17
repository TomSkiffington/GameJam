using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedAbilityState : PlayerState
{

    protected bool isAbilityDone;
    protected bool isGrounded;
    protected bool jumpInputStop;
    protected int xInput;

    public PlayerGroundedAbilityState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = core.CollisionSenses.Grounded;
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        jumpInputStop = player.InputHandler.JumpInputStop;
        xInput = player.InputHandler.NormInputX;

        if (isAbilityDone) {
            if(isGrounded && core.Movement.CurrentVelocity.y >= 0f) {
                stateMachine.ChangeState(player.IdleState);
            } 
            else {
                stateMachine.ChangeState(player.InAirState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
