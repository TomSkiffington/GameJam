using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpSquatState : PlayerGroundedAbilityState
{
    private float jumpSquatStartTime;
    private bool shouldShortHop;
    private bool inJumpSquat;

    public PlayerJumpSquatState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();

        jumpSquatStartTime = startTime;
        inJumpSquat = true;
    }

    public override void Exit()
    {
        base.Exit();

        inJumpSquat = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isExitingState) return;

        if (Time.time >= jumpSquatStartTime + playerData.jumpSquatTime) {
            shouldShortHop = false;
            stateMachine.ChangeState(player.JumpState);
        }
        else if (Time.time < jumpSquatStartTime + playerData.jumpSquatTime && jumpInputStop && isGrounded) {
            shouldShortHop = true;
            stateMachine.ChangeState(player.JumpState);
        }  
    }

    public bool ShouldShortHop() {
        return shouldShortHop;
    }

    public bool CheckIfInJumpSquat() {
        return inJumpSquat;
    }
}
