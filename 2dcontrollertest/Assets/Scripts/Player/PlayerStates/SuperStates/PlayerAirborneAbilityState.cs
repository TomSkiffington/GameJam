using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirborneAbilityState : PlayerState
{

    protected bool isAbilityDone;
    protected bool isGrounded;
    protected int xInput;

    public PlayerAirborneAbilityState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
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

        xInput = player.InputHandler.NormInputX;

        if (isAbilityDone) {
            if(isGrounded && core.Movement.CurrentVelocity.y > 0.01f) {
                stateMachine.ChangeState(player.SpecialLandState);
            } 
            else {
                stateMachine.ChangeState(player.InAirState);
            }
        }
    }
}
