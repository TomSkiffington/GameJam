using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]

public class Player1 : MonoBehaviour {

    public float runSpeed = 15;
    public int extraJumps = 1;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float maxFullHopHeight = 3.5f;
    public float shortHopHeight = 2;
    public float timeToFullHopApex = .2f;

    public float wallSlideSpeedMax = 4;
    public float wallStickTime = .2f;
    float timeToWallUnstick;

    float gravity;
    float fullHopVelocity;
    float shortHopVelocity;

    float jumpSquatTime = .09f;
    bool inJumpSquat = false;
    float jumpSquatTimer;

    public float accelerationTimeGrounded = .1f;
    public float accelerationTimeAir = .25f;

    Vector3 velocity;
    float velocityXSmoothing;


    private PlayerInputs playerInputs;
    Controller2D controller;

    private void Awake() {
        playerInputs = new PlayerInputs();
    }

    private void OnEnable() {
        playerInputs.Enable ();
    }

    private void OnDisable() {
        playerInputs.Disable ();
    }

    void Start()
    {
        controller = GetComponent<Controller2D> ();

        gravity = -2 * maxFullHopHeight/Mathf.Pow(timeToFullHopApex, 2);
        fullHopVelocity = Mathf.Abs(gravity) * timeToFullHopApex;
        shortHopVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * shortHopHeight);
        
    }
    
    void Update()
    {    
        Vector2 directionalInput = playerInputs.OnGround.Move.ReadValue<Vector2> ();
        int wallDirx = (controller.collisions.left)?-1:1;

        float targetVelocityX = directionalInput.x * runSpeed;
        velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAir);
        velocity.y += gravity * Time.deltaTime;

        bool wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0) {

                velocity.x = 0;
                velocityXSmoothing = 0;

                if (directionalInput.x != wallDirx && directionalInput.x != 0) {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else {
                timeToWallUnstick = wallStickTime;
            }
        }

        if (playerInputs.OnGround.Jump.triggered) {
            if(wallSliding) {
                if (wallDirx == directionalInput.x) {  //jumping up wall
                    velocity.x = -wallDirx * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                else if (directionalInput.x == 0) {    //jumping off wall
                    velocity.x = -wallDirx * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else {  //leaping off wall
                    velocity.x = -wallDirx * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
        if(controller.collisions.below && !inJumpSquat) {
                inJumpSquat = true;
                jumpSquatTimer = 0;
            }
        }
        if (inJumpSquat == true) {
            jumpSquatTimer += Time.deltaTime;
            Debug.Log("jumpsquat");
            if (playerInputs.OnGround.Jump.WasReleasedThisFrame() && jumpSquatTimer < jumpSquatTime) {
                velocity.y = shortHopVelocity;
                Debug.Log("shorthop");
                inJumpSquat = false;
            }
            else if (playerInputs.OnGround.Jump.IsPressed() && jumpSquatTimer >= jumpSquatTime) {
                velocity.y = fullHopVelocity;
                Debug.Log("fullhop");
                inJumpSquat = false;
            }
        }




        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }
    }
}
