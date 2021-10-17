using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BUG: Need to attack first to swap the clips. Find a better place to call OverrideClips

public class SwapWeaponAnim : MonoBehaviour
{
    public WeaponAnimations weaponAnims_;

    protected AggressiveWeapon equippedWeapon;
    protected AnimationClip[] details;

    protected Animator animator;
    protected AnimatorOverrideController animatorOverrideController;

    protected AnimationClipOverrides clipOverrides;
    
    public void OnEnable()
    {
        equippedWeapon = this.GetComponentInParent<AggressiveWeapon>();

        weaponAnims_ = equippedWeapon.weaponAnims;

        animator = GetComponent<Animator>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);

        OverrideClips();
    }

    public void OverrideClips() {
        clipOverrides["fTilt"] = weaponAnims_.fTilt;
        clipOverrides["dTilt"] = weaponAnims_.dTilt;
        clipOverrides["uTilt"] = weaponAnims_.uTilt;
        clipOverrides["uAir"] = weaponAnims_.uAir;
        clipOverrides["fAir"] = weaponAnims_.fAir;

        animatorOverrideController.ApplyOverrides(clipOverrides);
    }
}