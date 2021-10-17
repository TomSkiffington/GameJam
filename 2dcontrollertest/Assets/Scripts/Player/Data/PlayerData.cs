using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Move State")]
    public float runSpeed = 12f;
    public float walkSpeed = 6f;
    public float traction = .02f;
    public float turnAroundMomentumModifier = .8f;

    public float iniDashSpeed = 12f;
    public float maxDashSpeed = 14f;
    public float iniDashTime = .12f;
    //public float maxDashTime = .35f;
    public float dAccA = 0.1f;  //dash acceleration A
    public float dAccB = 0.5f;  //dash acceleration B

    [Header("Crouch State")]
    public float crouchWalkSpeed = 6f;
    public float crouchColliderHeight = 0.8f;
    public float standColliderHeight = 1.6f;

    [Header("Jump State")]
    public float jumpSquatTime = .0833333334f;
    public float fullHopIniV = 24f;
    public float shortHopIniV = 17f;
    public int amountOfJumps = 1;

    [Header("Wall Jump State")]
    public float wallJumpTime = 0.15f;
    public float wallJumpVelocity = 22f;
    public Vector2 wallJumpAngle = new Vector2(1,2);

    [Header("In Air State")]
    public float aerialMaxVelX = 5f;
    public float airFriction = .01f;
    public float airMobilityA = .06f;
    public float airMobilityB = .02f;
    public float coyoteTime = .2f;
    public float fastFallV = 13;
    public float gravity = -83;

    [Header("Air Dodge State")]
    public float airDodgeTime = .35f;
    public float airDodgeSpeed = 2f;
    public float airDodgeDistanceModifier = 0.9f;
    //public float distBetweenAfterImages = .5f;

    [Header("Wall Slide State")]
    public float wallSlideVelocity = 4f;

    [Header("Wall Climb State")]
    public float wallClimbVelocity = 3f;

    [Header("Ledge Climb State")]
    public Vector2 startOffset;
    public Vector2 stopOffset;
}
