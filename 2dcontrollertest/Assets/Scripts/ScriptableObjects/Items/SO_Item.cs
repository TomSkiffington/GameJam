using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
    Food,
    Weapon,
    Shield,
    Helmet,
    BodyArmour,
    Gloves,
    Boots,
    Accessory,
    Default
}

public enum Attributes {
    strength,
    spirit,
    agility
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Item")]
public class SO_Item : ScriptableObject
{
    public Sprite ui_Display;
    public bool stackable;
    public ItemType type;
    [TextArea(15,20)]
    public string description;

    public Item data = new Item();

    public Item CreateItem() {
        Item newItem = new Item(this);
        return newItem;
    }
}

[System.Serializable]
public class Item 
{
    public string name;
    public int id = -1;

    public ItemMod[] mods;
    public WeaponAttackDetails[] attackDetails;
    public BaseAnimations baseAnims;
    public WeaponAnimations weaponAnims;

    //public WeaponAttackDetails[] AttackDetails { get => attackDetails; private set => attackDetails = value; }

    public Item() {
        name = "";
        id = -1;
    }

    public Item(SO_Item item) {
        name = item.name;
        id = item.data.id;

        attackDetails = new WeaponAttackDetails[item.data.attackDetails.Length];
        baseAnims = new BaseAnimations();
        weaponAnims = new WeaponAnimations();

        mods = new ItemMod[item.data.mods.Length];

        for (int i = 0; i < mods.Length; i++) {
            mods[i] = new ItemMod(item.data.mods[i].min, item.data.mods[i].max);
            mods[i].attribute = item.data.mods[i].attribute;
        }
    }

    public Item(SO_Item item, ItemMod[] _mods) {
        name = item.name;
        id = item.data.id;
        mods = _mods;
    }
}

[System.Serializable]
public class ItemMod
{
    public Attributes attribute;

    public int value = 0;   //unrolled mods have a value of 0
    public int min;
    public int max;


    public ItemMod(int _min, int _max) {
        min = _min;
        max = _max;

        GenerateValue();

    }

    public void GenerateValue() {
        value = UnityEngine.Random.Range(min, max);
    }
}

[System.Serializable]
public struct BaseAnimations
{
    public AnimationClip fTilt;
    public AnimationClip dTilt;
    public AnimationClip uTilt;
    public AnimationClip uAir;
    public AnimationClip fAir;
    //public AnimationClip dAir;
}

[System.Serializable]
public struct WeaponAnimations
{
    public AnimationClip fTilt;
    public AnimationClip dTilt;
    public AnimationClip uTilt;
    public AnimationClip uAir;
    public AnimationClip fAir;
    //public AnimationClip dAir;
}

[System.Serializable]
public struct WeaponAttackDetails
{
    public string attackName;
    public float movementSpeed;
    public float damageAmount;
    public float baseKnockBack;
    public float knockBackScaling;
    public Vector2 knockBackAngle;
}