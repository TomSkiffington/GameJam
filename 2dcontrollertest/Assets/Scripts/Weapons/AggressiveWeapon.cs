using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AggressiveWeapon : Weapon
{

    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected AudioClip swingSound;
    //protected SO_AggressiveWeaponData aggressiveWeaponData;

    private List<IDamageable> detectedDamageables = new List<IDamageable>();

    private int timer;
    private int hitlag;

    protected WeaponAttackDetails details;

    public WeaponAnimations weaponAnims;
    public BaseAnimations baseAnims;
    

    protected override void Awake()
    {
        base.Awake();
    }

    private void FixedUpdate() {
        timer++;

        if (timer < hitlag && hitlag != 0) {
            state.HoldHitlagPosition();
        }
        else {
            baseAnimator.speed = 1;
            weaponAnimator.speed = 1;
        }
    }

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();
        
        CheckMeleeAttack();

        if (detectedDamageables.Count > 0 && hitlag == 0) {
            hitlag = (int)(details.damageAmount * 0.333f) + 4;
            baseAnimator.speed = 0;
            weaponAnimator.speed = 0;
        }
        else {
            hitlag = 0;
        }

        timer = 0;
    }

    public override void AnimationActionTriggerEnd()
    {
        base.AnimationActionTriggerEnd();
    }

    private void CheckMeleeAttack() {
        //details = weaponData.AttackDetails[attackType];

        weaponAnims = state.GetEquippedWeaponAnims();
        baseAnims = state.GetEquippedWeaponBaseAnims();

        details = state.GetEquippedWeaponAttacks()[attackType];

        if (weaponAudioSource.clip == null) {
            weaponAudioSource.clip = swingSound;
            weaponAudioSource.Play();
        }

        if (detectedDamageables.Count > 0) {
            weaponAudioSource.clip = hitSound;
            weaponAudioSource.Play();
        }

        foreach(IDamageable item in detectedDamageables) {
            item.Damage(details.damageAmount, details.baseKnockBack, details.knockBackScaling, details.knockBackAngle);

        }
    }

    public void AddToDetected(Collider2D collision) {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        bool noDuplicates = detectedDamageables.Distinct().Count() == detectedDamageables.Count();

        if (damageable != null && noDuplicates) {
            detectedDamageables.Add(damageable);
        }
    }

    public void RemoveFromDetected(Collider2D collision) {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null) {
            detectedDamageables.Remove(damageable);
        }
    }
}
