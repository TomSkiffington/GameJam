using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BUG: Need to attack first to swap the clips. Find a better place to call OverrideClips

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) {}

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}

public class SwapBaseAnim : MonoBehaviour
{
    public BaseAnimations baseAnims_;

    protected AggressiveWeapon equippedWeapon;
    protected AnimationClip[] details;

    protected Animator animator;
    protected AnimatorOverrideController animatorOverrideController;

    protected AnimationClipOverrides clipOverrides;
    
    public void OnEnable()
    {
        equippedWeapon = this.GetComponentInParent<AggressiveWeapon>();

        baseAnims_ = equippedWeapon.baseAnims;

        animator = GetComponent<Animator>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);

        OverrideClips();
    }

    public void OverrideClips() {
        clipOverrides["fTilt"] = baseAnims_.fTilt;
        clipOverrides["dTilt"] = baseAnims_.dTilt;
        clipOverrides["uTilt"] = baseAnims_.uTilt;
        clipOverrides["uAir"] = baseAnims_.uAir;
        clipOverrides["fAir"] = baseAnims_.fAir;

        animatorOverrideController.ApplyOverrides(clipOverrides);
    }
}