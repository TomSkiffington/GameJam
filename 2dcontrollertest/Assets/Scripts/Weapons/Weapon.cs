using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //[SerializeField] protected SO_Item weaponData;

    protected Animator baseAnimator;
    protected Animator weaponAnimator;

    //protected Animation baseAnimation;
    //protected Animation weaponAnimation;

    protected AudioSource weaponAudioSource;

    protected PlayerAttackState state;

    protected bool isGrounded;
    protected bool inJumpSquat;

    //protected int attackCounter;
    protected int attackType;
    protected Vector2 attackDirection;

    protected virtual void Awake() {
        baseAnimator = transform.Find("Base").GetComponent<Animator>();
        weaponAnimator = transform.Find("Weapon").GetComponent<Animator>();
        weaponAudioSource = GetComponent<AudioSource>();

        gameObject.SetActive(false);
    }

    public virtual void EnterWeapon() {
        gameObject.SetActive(true);

        //weaponData = state.GetEquippedWeapon();

        attackDirection = state.DetermineAttackDirection();
        //Debug.Log(attackDirection);

        // if (attackCounter >= weaponData.amountOfAttacks) {
        //     attackCounter = 0;
        // }

        isGrounded = state.CheckIfGrounded();

        attackType = DetermineAttackType();

        

        baseAnimator.SetBool("attack", true);
        weaponAnimator.SetBool("attack", true);

        //baseAnimator.SetInteger("attackCounter", attackCounter);
        //weaponAnimator.SetInteger("attackCounter", attackCounter);

        baseAnimator.SetInteger("attackType", attackType);
        weaponAnimator.SetInteger("attackType", attackType);
    }

    public virtual void ExitWeapon() {
        baseAnimator.SetBool("attack", false);
        weaponAnimator.SetBool("attack", false);

        weaponAudioSource.clip = null;

        //attackCounter++;

        gameObject.SetActive(false);
    }

    public void InitializeWeapon(PlayerAttackState state) {
        this.state = state;
    }

    private int DetermineAttackType() {
        if (attackDirection == Vector2.up) {
            if (isGrounded && !inJumpSquat) {
                return 0;
            }
            else {
                return 3;
            }
        }
        else if (attackDirection == Vector2.down) {
            if (isGrounded && !inJumpSquat) {
                return 1;   
            }
            else {
                return 1;
            }
        }
        else if (attackDirection == Vector2.left) {
            if (!isGrounded || inJumpSquat) {
                return 4;
            }
            else {
                return 2;
            }
        }
        else if (attackDirection == Vector2.right) {
            if (!isGrounded || inJumpSquat) {
                return 4;
            }
            else {
                return 2;
            }
        }
        else {
            return 5;
        }
    }

    #region Animation Triggers

    public virtual void AnimationFinishTrigger() {
        state.AnimationFinishTrigger();
    }

    public virtual void AnimationStartMovementTrigger() {
        //state.SetPlayerVelocity(weaponData.movementSpeed[attackCounter]);
    }

    public virtual void AnimationStopMovementTrigger() {
        //state.SetPlayerVelocity(0f);
    }

    public virtual void AnimationTurnOffFlipTrigger() {
        state.SetFlipCheck(false);
    }

    public virtual void AnimationTurnOnFlipTrigger() {
        state.SetFlipCheck(true);
    }

    public virtual void AnimationActionTrigger() {

    }

    public virtual void AnimationActionTriggerEnd() {

    }

    #endregion
}
