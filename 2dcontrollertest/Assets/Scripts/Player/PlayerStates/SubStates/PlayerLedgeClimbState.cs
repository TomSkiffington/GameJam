using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerState
{
    private Vector2 detectedPosition;   //player position as soon as a ledge is detected
    private Vector2 cornerPosition;     //corner between wall and ground
    private Vector2 startPosition;
    private Vector2 stopPosition;
    private Vector2 workspace;

    private bool isHanging;
    private bool isClimbing;
    private bool isTouchingCeiling;

    private int xInput;
    private int yInput;
    private bool jumpInput;

    public PlayerLedgeClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        player.Anim.SetBool("climbLedge", false);
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        isHanging = true;
    }

    public override void Enter()
    {
        base.Enter();

        core.Movement.SetVelocityZero();
        player.transform.position = detectedPosition;
        cornerPosition = DetermineCornerPosition();

        startPosition.Set(cornerPosition.x - (core.Movement.FacingDirection * playerData.startOffset.x), cornerPosition.y - playerData.startOffset.y);
        stopPosition.Set(cornerPosition.x + (core.Movement.FacingDirection * playerData.stopOffset.x), cornerPosition.y + playerData.stopOffset.y);

        player.transform.position = startPosition;
    }

    public override void Exit()
    {
        base.Exit();

        isHanging = false;

        if(isClimbing) {
            player.transform.position = stopPosition;
            isClimbing = false;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isExitingState) return;

        if(isAnimationFinished) {
            if (isTouchingCeiling) {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
            else {
                stateMachine.ChangeState(player.IdleState);
            }
        } 
        else {
            xInput = player.InputHandler.NormInputX;
            yInput = player.InputHandler.NormInputY;
            jumpInput = player.InputHandler.JumpInput;

            core.Movement.SetVelocityZero();
            player.transform.position = startPosition;

            if (xInput == core.Movement.FacingDirection && isHanging && !isClimbing) {
                CheckForSpace();
                isClimbing = true;
                player.Anim.SetBool("climbLedge", true);
            }
            else if (yInput == -1 && isHanging && !isClimbing) {
                stateMachine.ChangeState(player.InAirState);
            }
        }  
    }

    public void SetDetectedPosition(Vector2 pos) {
        detectedPosition = pos;
    }

    private void CheckForSpace() {
        isTouchingCeiling = Physics2D.Raycast(cornerPosition + (Vector2.up * 0.015f) + (Vector2.right * core.Movement.FacingDirection * 0.015f), Vector2.up, playerData.standColliderHeight * 1.3f, core.CollisionSenses.CollisionMask);
        player.Anim.SetBool("isTouchingCeiling", isTouchingCeiling);

        Debug.DrawRay(cornerPosition + (Vector2.up * 0.015f) + (Vector2.right * core.Movement.FacingDirection * 0.015f), Vector2.up, Color.magenta, 3);
    }

    private Vector2 DetermineCornerPosition() {
        RaycastHit2D xHit = Physics2D.Raycast(core.CollisionSenses.WallCheck.position, Vector2.right * core.Movement.FacingDirection, core.CollisionSenses.WallCheckDistance, core.CollisionSenses.CollisionMask);
        float xDist = xHit.distance;    //distance from raycast origin to ledge
        workspace.Set((xDist + 0.015f) * core.Movement.FacingDirection, 0);

        RaycastHit2D yHit = Physics2D.Raycast(core.CollisionSenses.LedgeCheck.position + (Vector3)workspace, Vector2.down, core.CollisionSenses.LedgeCheck.position.y - core.CollisionSenses.WallCheck.position.y + 0.015f, core.CollisionSenses.CollisionMask);    //position is ledgeCheck pos + xDist
        float yDist = yHit.distance;

        workspace.Set(core.CollisionSenses.WallCheck.position.x + (xDist * core.Movement.FacingDirection), core.CollisionSenses.LedgeCheck.position.y - yDist);   //coordinate of ledge corner
        Debug.DrawRay((Vector3)workspace, Vector2.down, Color.green, 5);
        return workspace;
    }

}
