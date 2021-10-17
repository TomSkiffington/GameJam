using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]

public class Player : MonoBehaviour
{

    #if UNITY_EDITOR
        void OnGUI() 
        {
            //if (!debugMode) return;
            string state = StateMachine.CurrentState.ToString();
            GUI.Label(new Rect(0, 0, 1000, 100), state);
        }
    #endif

    #region State Variables
    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerJumpSquatState JumpSquatState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerSpecialLandState SpecialLandState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallGrabState WallGrabState { get; private set; }
    public PlayerWallClimbState WallClimbState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerLedgeClimbState LedgeClimbState { get; private set; }
    public PlayerCrouchIdleState CrouchIdleState { get; private set; }
    public PlayerCrouchWalkState CrouchWalkState { get; private set; }
    public PlayerAirDodgeState AirDodgeState { get; private set; }
    public PlayerAttackState PrimaryAttackState { get; private set; }
    public PlayerAttackState SecondaryAttackState { get; private set; }

    [SerializeField]
    private PlayerData playerData;
    #endregion

    #region Components
    public Core Core { get; private set; }
    public Animator Anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Controller2D controller { get; private set; }
    public BoxCollider2D MovementCollider { get; private set; }
    #endregion

    #region Other Variables
    Vector2 workspace;
    #endregion
    
    #region Unity Callback Functions
    private void Awake() {

        Core = GetComponentInChildren<Core>();

        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState (this, StateMachine, playerData, "idle");
        DashState = new PlayerDashState (this, StateMachine, playerData, "dash");
        RunState = new PlayerRunState (this, StateMachine, playerData, "run");
        JumpState = new PlayerJumpState (this, StateMachine, playerData, "inAir");
        JumpSquatState = new PlayerJumpSquatState (this, StateMachine, playerData, "inJumpSquat");
        InAirState = new PlayerInAirState (this, StateMachine, playerData, "inAir");
        LandState = new PlayerLandState (this, StateMachine, playerData, "land");
        SpecialLandState = new PlayerSpecialLandState (this, StateMachine, playerData, "land");
        WallSlideState = new PlayerWallSlideState (this, StateMachine, playerData, "wallSlide");
        WallGrabState = new PlayerWallGrabState (this, StateMachine, playerData, "wallGrab");
        WallClimbState = new PlayerWallClimbState (this, StateMachine, playerData, "wallClimb");
        WallJumpState = new PlayerWallJumpState (this, StateMachine, playerData, "inAir");
        LedgeClimbState = new PlayerLedgeClimbState (this, StateMachine, playerData, "ledgeClimbState");
        CrouchIdleState = new PlayerCrouchIdleState (this, StateMachine, playerData, "crouchIdle");
        CrouchWalkState = new PlayerCrouchWalkState (this, StateMachine, playerData, "crouchWalk");
        AirDodgeState = new PlayerAirDodgeState (this, StateMachine, playerData, "airDodge");
        PrimaryAttackState = new PlayerAttackState (this, StateMachine, playerData, "attack");
        SecondaryAttackState = new PlayerAttackState (this, StateMachine, playerData, "attack");
    }

    private void Start() {
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        controller = GetComponent<Controller2D>();
        MovementCollider = GetComponent<BoxCollider2D>();

        PrimaryAttackState.SetWeapon(EquippedWeapon);   //weapon in equipment slot 1

        //SecondaryAttackState.SetWeapon(Inventory.weapons[(int)CombatInputs.primary]);

        StateMachine.Initialize(IdleState);
    }

    private void Update() {
        Core.LogicUpdate();
        StateMachine.CurrentState.LogicUpdate();

        if (InputHandler.ShowInventory) {
            InventoryCanvas.SetActive(true);
        }
        else {
            InventoryCanvas.SetActive(false);
        }
    }

    private void FixedUpdate() {
        Core.PhysicsUpdate();
        StateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

    #region Particles

    public ParticleSystem dust;
    public ParticleSystemRenderer dustRenderer;

    public Material dashDust;
    public Material wavedashDust;

    public void CreateDust() {

        dustRenderer.material = dashDust;

        if (Core.Movement.FacingDirection == 1) {
            dustRenderer.flip = Vector3.zero;
        }
        else {
            dustRenderer.flip = Vector3.right;
        }

        dust.Play();
    }

    public void CreateWaveDashDust() {

        dustRenderer.material = wavedashDust;

        dust.Play();
    }

    #endregion

    #region Inventory and Items

    public SO_Inventory playerInventory;
    public SO_Inventory playerEquipment;
    public Weapon EquippedWeapon;
    [SerializeField] public GameObject InventoryCanvas;

    private void OnTriggerEnter2D(Collider2D other) {
        var groundItem = other.GetComponent<GroundItem>();

        if (groundItem) {

            Item _item;

            //no mods or unrolled mods (mod value = 0)
            if (groundItem.mods.Length == 0 || groundItem.mods[0].value == 0) {
                _item = new Item(groundItem.item);
            }
            //mods already rolled
            else {
                _item = new Item(groundItem.item, groundItem.mods);
            }   
            
            if (playerInventory.AddItem(_item, groundItem.stackSize)) {    //only destroys the ground item if it's able to add to inventory
                Destroy(other.gameObject);
            }
        }
    }

    private void OnApplicationQuit() {
        playerInventory.Container.Clear();
        playerEquipment.Container.Clear();
    }

    #endregion

    #region  Other Functions

    public void SetColliderHeight(float height) {
        Vector2 center = MovementCollider.offset;
        workspace.Set(MovementCollider.size.x, height);

        center.y += (height - MovementCollider.size.y) / 2;

        MovementCollider.size = workspace;
        MovementCollider.offset = center;

        controller.UpdateRaycastOrigins();
        controller.CalculateRaySpacing();
    }

    private void AnimationTrigger() {
        StateMachine.CurrentState.AnimationTrigger();
    }

    private void AnimationFinishTrigger() {
        StateMachine.CurrentState.AnimationFinishTrigger();
    }

    #endregion
}
