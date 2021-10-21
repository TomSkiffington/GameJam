using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSenses : CoreComponent
{

    #region  Check Transforms

    public Transform GroundCheck { get => groundCheck; private set => groundCheck = value; }
    public Transform LedgeCheck { get => ledgeCheck; private set => ledgeCheck = value; }
    public Transform WallCheck { get => wallCheck; private set => wallCheck = value; }
    public Transform CeilingCheck { get => ceilingCheck; private set => ceilingCheck = value; }

    public float GroundCheckRadius { get => groundCheckRadius; set => groundCheckRadius = value; }
    public float WallCheckDistance { get => wallCheckDistance; set => wallCheckDistance = value; }
    public LayerMask CollisionMask { get => collisionMask; set => collisionMask = value; }

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ceilingCheck;

    [SerializeField] private float groundCheckRadius;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private float wallCheckDistance;

    [SerializeField] private LayerMask collisionMask;

    #endregion

    #region  Check Functions

    public bool Grounded {
        //get => Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionMask);
        get => Physics2D.OverlapCapsule(groundCheck.position, groundCheckSize, CapsuleDirection2D.Horizontal, 0, collisionMask);
    }

    public bool Ceiling {
        get => Physics2D.OverlapCircle(ceilingCheck.position, .25f, collisionMask);
    }

    public bool Ledge {
        get => Physics2D.Raycast(ledgeCheck.position, Vector2.right * core.Movement.FacingDirection, wallCheckDistance, collisionMask);
    }

    public bool CheckIfTouchingWall() {
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, Vector2.right * core.Movement.FacingDirection, wallCheckDistance, collisionMask);

        if (hit && hit.collider.tag != "Through") {     //for drop through platforms
            return hit;                                 //dont know how to turn this function into a property
        }
        else {
            return false;
        }
    }

    public bool CheckIfTouchingWallBack() {
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, Vector2.right * -core.Movement.FacingDirection, wallCheckDistance, collisionMask);

        if (hit && hit.collider.tag != "Through") {
            return hit;
        }
        else {
            return false;
        }
    }

    #endregion
}
