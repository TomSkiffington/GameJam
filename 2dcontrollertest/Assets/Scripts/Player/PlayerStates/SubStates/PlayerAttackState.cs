using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerGroundedAbilityState
{

    private Weapon weapon;

    private int face;
    private float velocityToSet;
    private float velocityX;

    private bool setVelocity;
    private bool shouldCheckFlip;
    private bool isAirborne;

    private Vector2 hitlagPosition;

    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        setVelocity = false;
        velocityX = core.Movement.CurrentVelocity.x;
        face = core.Movement.FacingDirection;
        
        weapon.EnterWeapon();
    }

    public override void Exit()
    {
        base.Exit();

        player.InputHandler.UseDash();

        weapon.ExitWeapon();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (shouldCheckFlip) {
        core.Movement.CheckIfShouldFlip(xInput);
        }

        if (setVelocity) {
            core.Movement.SetVelocityX(velocityToSet * face);
        }

        if (!isGrounded) {
            if (!isAirborne) isAirborne = !isAirborne;
        }
        else {
            if (isAirborne) {   //when landing while attacking
                isAirborne = false;
                player.StateMachine.ChangeState(player.LandState);  //make attacklandstate?
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (isGrounded) {
            core.Movement.reduceByTraction(ref velocityX, false);
            core.Movement.SetVelocityX(velocityX);
        }
        else if (!isGrounded) {
            core.Movement.airDrift(ref velocityX, xInput);
            core.Movement.SetVelocityX(velocityX);
        }
    }

    public void SetWeapon(Weapon weapon) {
        this.weapon = weapon;
        weapon.InitializeWeapon(this);
    }

    public WeaponAttackDetails[] GetEquippedWeaponAttacks() {
        int itemID = player.playerEquipment.Container.Items[1].item.id; //finds id of weapon in weapon equipment slot
        return player.playerEquipment.database.Items[itemID].data.attackDetails; //finds item in database with id, then returns weapon details
    }

    public BaseAnimations GetEquippedWeaponBaseAnims() {
        int itemID = player.playerEquipment.Container.Items[1].item.id;
        return player.playerEquipment.database.Items[itemID].data.baseAnims;
    }

    public WeaponAnimations GetEquippedWeaponAnims() {
        int itemID = player.playerEquipment.Container.Items[1].item.id;
        return player.playerEquipment.database.Items[itemID].data.weaponAnims;
    }

    public void SetPlayerVelocity(float velocity) {
        core.Movement.SetVelocityX(velocity * face);

        velocityToSet = velocity;
        setVelocity = true;
    }

    public Vector2 DetermineAttackDirection() {
        Vector2 direction = Vector2.zero;

        if (player.InputHandler.AttackInputDirection[(int)AttackDirection.up]) {
            direction.Set(0,1);
            return direction;
        }
        else if (player.InputHandler.AttackInputDirection[(int)AttackDirection.down]) {
            direction.Set(0,-1);
            return direction;
        }
        else if (player.InputHandler.AttackInputDirection[(int)AttackDirection.left]) {
            direction.Set(-1,0);
            return direction;
        }
        else if (player.InputHandler.AttackInputDirection[(int)AttackDirection.right]) {
            direction.Set(1,0);
            return direction;
        }
        else {
            return direction;
        }
    }

    public void HoldHitlagPosition () {

        hitlagPosition = player.transform.position;
        player.transform.position = hitlagPosition;

        core.Movement.SetVelocityZero();
    }

    public bool CheckIfGrounded() {
        return core.CollisionSenses.Grounded;
    }

    public bool CheckIfInJumpSquat() {
        return player.JumpSquatState.CheckIfInJumpSquat();
    }

    public void SetFlipCheck(bool value) {
        shouldCheckFlip = value;
    }

    #region Animation Triggers

    public override void AnimationFinishTrigger() {
        base.AnimationFinishTrigger();

        isAbilityDone = true;
    }

    #endregion

}
