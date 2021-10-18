using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : CoreComponent
{

public Controller2D Controller { get; private set; }
public PlayerInputHandler InputHandler { get; private set; }
public Vector2 CurrentVelocity { get; private set; }
public int FacingDirection { get; private set; }

[SerializeField]
private PlayerData playerData;

private Transform playerTransform;

private Vector2 workspace;
private Vector2 velocityToApply;

protected override void Awake() {
    base.Awake();

    Controller = GetComponentInParent<Controller2D>();
    InputHandler = GetComponentInParent<PlayerInputHandler>();

    FacingDirection = 1;
    playerTransform = transform.root;
}

public void LogicUpdate() {

    CurrentVelocity = velocityToApply;
    
    ApplyVelocity(velocityToApply);

    if (Controller.collisions.above || Controller.collisions.below) {
        velocityToApply.y = 0;
    }
}

public void PhysicsUpdate() {

    if (!Controller.collisions.below || InputHandler.NormInputY < 0 || Controller.collisions.slopeAngle > 45) {
        velocityToApply.y -= playerData.gravity;   //should replace by using ApplyGravity function in states where needed... maybe
    }
}

#region  Set Functions

    public void SetVelocityZero() {
        velocityToApply = Vector2.zero;
    }

    public void SetVelocity(float velocity, Vector2 angle, int direction) {
        angle.Normalize();
        workspace.Set(angle.x * velocity * direction, angle.y * velocity);
        velocityToApply = workspace;
    }

    public void SetVelocity(float velocity, Vector2 direction) {
        workspace = direction * velocity;
        velocityToApply = workspace;
    }

    public void SetVelocityX(float velocityX) {
        
        workspace.Set(velocityX, CurrentVelocity.y);
        velocityToApply.x = workspace.x;
    }

    public void SetVelocityY(float velocityY) {
        
        workspace.Set(CurrentVelocity.x, velocityY);
        velocityToApply.y = workspace.y;
    }

    public void ApplyVelocity(Vector2 velocity) {
        Controller.Move(velocity * Time.deltaTime, InputHandler.RawMovementInput);
    }

    public void DisregardGravity(ref float velocityY) {
        velocityY -= playerData.gravity * Time.deltaTime * 0.05f;
    }

    /* public void ApplyGravity(ref float velocityY) {
        velocityY += playerData.gravity;
    } */

    public void reduceByTraction(ref float velocityX, bool applyDouble) {
        if (velocityX > 0) {
            if (applyDouble && velocityX > playerData.crouchWalkSpeed) {
                velocityX -= playerData.traction * 2;
            }
            else {
                velocityX -= playerData.traction;
            }
            if (velocityX < 0) {
                velocityX = 0;
            }
        } else {
            if (applyDouble && velocityX < playerData.crouchWalkSpeed) {
                velocityX += playerData.traction * 2;
            }
            else {
                velocityX += playerData.traction;
            }
            if (velocityX > 0) {
                velocityX = 0;
            }
        }
    }

    public void airDrift (ref float velocityX, int xInput) {
        float tempMax = playerData.aerialMaxVelX * xInput;

        if ((tempMax < 0 && velocityX < tempMax) || (tempMax > 0 && velocityX > tempMax)) {
            if (velocityX > 0) {
                velocityX -= playerData.airFriction;
                if (velocityX < 0) {
                    velocityX = 0;
                }
            }
            else {
                velocityX += playerData.airFriction;
                if (velocityX > 0) {
                    velocityX = 0;
                }
            }
        }
        else if (Mathf.Abs(xInput) > 0.3 && (tempMax < 0 && velocityX > tempMax) || (tempMax > 0 && velocityX < tempMax)) {
            velocityX += ((float)(playerData.airMobilityA * xInput) + (Mathf.Sign(xInput) * playerData.airMobilityB));
        }

        if (Mathf.Abs(xInput) < 0.3) {
            if (velocityX > 0) {
                velocityX -= playerData.airFriction;
                if (velocityX < 0) {
                    velocityX = 0;
                }
            }
            else {
                velocityX += playerData.airFriction;
                if (velocityX > 0) {
                    velocityX = 0;
                }
            }
        }
    }

    public void CheckIfShouldFlip(int xInput) {
        if (xInput != 0 && (xInput != FacingDirection)) {
            Flip();
        }
    }

    public void CheckIfShouldFlipAirdodge(float airDodgeDirectionX) {
        int airDodgeFaceDir;

        if (airDodgeDirectionX >= 0) {
            airDodgeFaceDir = 1;
        }
        else {
            airDodgeFaceDir = -1;
        }

        if (airDodgeFaceDir != 0 && (airDodgeFaceDir != FacingDirection)) {
            Flip();
        }
    }

    public void Flip() {
        
        if (FacingDirection > 0) {
            playerTransform.localScale = new Vector3(-1,1,1);
        }
        else {
            playerTransform.localScale = new Vector3(1,1,1);
        }
        
        FacingDirection *= -1;
    }

    #endregion
}
