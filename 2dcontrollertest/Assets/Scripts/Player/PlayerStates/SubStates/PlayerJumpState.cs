using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerGroundedAbilityState
{

    //bool inJumpSquat = false;

    private int amountOfJumpsLeft;
    private float hopVelocity;

    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
        amountOfJumpsLeft = playerData.amountOfJumps;
    }

    public override void Enter()
    {
        base.Enter();

        //player.CheckIfShouldFlip(xInput);

        player.InputHandler.UseJumpInput();

        if (player.JumpSquatState.ShouldShortHop() && isGrounded) {
            hopVelocity = playerData.shortHopIniV;
        }
        else {
            hopVelocity = playerData.fullHopIniV;
        }
        
        core.Movement.SetVelocityY(hopVelocity);

        isAbilityDone = true;
        amountOfJumpsLeft--;
        player.InAirState.SetIsJumping();
    }

    public bool CanJump() {
        if (amountOfJumpsLeft > 0) {
            return true;
        }
        else {
            return false;
        }
    }

    public void ResetAmountOfJumpsLeft() {
        amountOfJumpsLeft = playerData.amountOfJumps;
    }

    public void DecreaseAmountOfJumpsLeft() {
        amountOfJumpsLeft--;
    }
}
