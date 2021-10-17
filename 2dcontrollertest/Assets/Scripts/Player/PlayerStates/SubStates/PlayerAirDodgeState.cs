using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirDodgeState : PlayerAirborneAbilityState
{
    public bool CanAirDodge { get; private set; }

    private bool airDodged;
    private float lastAirDodgeTime;
    private Vector2 airDodgeDirection;
    private Vector2 airDodgeDirectionInput;
    private Vector2 velocity;

    public PlayerAirDodgeState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        CanAirDodge = false;
        player.InputHandler.UseAirDodge();

        // airDodgeDirection = Vector2.right * player.FacingDirection;

        /* airDodgeDirectionInput = player.InputHandler.RawAirDodgeDirectionInput;

        if (airDodgeDirectionInput != Vector2.zero && airDodged == false) {
            if (airDodgeDirectionInput.x == -1 && airDodgeDirectionInput.y == 0) {
                airDodgeDirection.Set(-5f, -1f);
            }
            else if (airDodgeDirectionInput.x == 1 && airDodgeDirectionInput.y == 0) {
                airDodgeDirection.Set(5f, -1f);
            }
            else {
                airDodgeDirection = airDodgeDirectionInput;
            }
            airDodgeDirection.Normalize();

            player.ApplyVelocity(playerData.runSpeed * playerData.airDodgeSpeed * airDodgeDirection);
            velocity = playerData.runSpeed * playerData.airDodgeSpeed * airDodgeDirection;
            airDodged = true;
        } */

        airDodgeDirectionInput = player.InputHandler.RawAirDodgeDirectionInput;

        if (airDodgeDirectionInput != Vector2.zero && !airDodged) {
            
            airDodgeDirection = airDodgeDirectionInput;
            airDodgeDirection.Normalize();

            Debug.Log(airDodgeDirection);

            float angle = Vector2.SignedAngle(Vector2.right, airDodgeDirection);

            core.Movement.ApplyVelocity(playerData.runSpeed * playerData.airDodgeSpeed * airDodgeDirection);
            velocity = playerData.runSpeed * playerData.airDodgeSpeed * airDodgeDirection;
            airDodged = true;
        }
    }

    public override void Exit()
    {
        base.Exit();

        airDodged = false;
        //velocity = Vector2.zero;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isExitingState) return;

        if(isGrounded) {
            stateMachine.ChangeState(player.SpecialLandState);
        }   
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (isExitingState) return;

        if (Time.time >= startTime + playerData.airDodgeTime ) {
            core.Movement.airDrift(ref velocity.x, xInput);
            core.Movement.SetVelocityX(velocity.x);

            if (!isGrounded) {
                core.Movement.SetVelocityZero();
            }

            isAbilityDone = true;
        }
        else {
            velocity.x *= playerData.airDodgeDistanceModifier;
            velocity.y *= playerData.airDodgeDistanceModifier;
            //core.Movement.DisregardGravity(ref velocity.y);
            core.Movement.SetVelocityX(velocity.x);
            core.Movement.SetVelocityY(velocity.y);
            
        }
    }

    public bool CheckIfCanAirDodge() {
        return CanAirDodge && Time.time >= lastAirDodgeTime + playerData.airDodgeTime;
    }

    public void ResetCanAirdodge() {
        CanAirDodge = true;
    }

}
